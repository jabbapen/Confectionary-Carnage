from http.client import HTTPException
from pydantic import BaseModel
from fastapi import FastAPI
from mangum import Mangum
import uvicorn
import os
import psycopg2

app = FastAPI()

# utility classes/methods
class LeaderboardModel(BaseModel):
    name: str
    score: int

# added to minimize code bloat
def get_db_conn():
    try: 
        return psycopg2.connect(
            host=os.getenv("PG_HOST"),
            port=os.getenv("PG_PORT"),
            user=os.getenv("PG_USER"),
            password=os.getenv("PG_PASSWORD"),
            dbname=os.getenv("PG_DATABASE"),
        )
    except psycopg2.Error as e:
        raise HTTPException(500, "DB conn error")

def init_leaderboard(conn): 
    try:
        with conn.cursor() as cur:
            leaderboard_query = """
            CREATE TABLE IF NOT EXISTS leaderboard (
                id SERIAL PRIMARY KEY,
                name varchar(100) NOT NULL,
                score int NOT NULL
            );
            """
            cur.execute(leaderboard_query)
            conn.commit() 
    except Exception: 
        raise HTTPException(400, "Failed to create leaderboard table")

@app.on_event("startup")
def startup():
    conn = get_db_conn()
    init_leaderboard(conn)
    conn.close()

@app.get("/")
async def hello_world():
    return "Hey does my pipeline automatically update my change?"

@app.get("/test-postgres")
async def test_postgres():
    conn = get_db_conn()
    cur = conn.cursor()
    cur.execute("SELECT 'Hello, world!!'")
    result = cur.fetchone()
    cur.close()
    conn.close()

    return {"statusCode": 200, "body": result[0]}

# return first limit entries in leaderboard 
@app.get("/leaderboard")
async def get_leaderboard(limit=None):
    conn = get_db_conn()
    try:
        with conn.cursor() as cur:
            query = """
                SELECT name, score 
                FROM leaderboard
                ORDER BY score DESC
            """
            if limit is not None:
                query += " LIMIT %s"
                cur.execute(query, (limit,))
            else:
                cur.execute(query)
            result = cur.fetchall()
            leaderboard = [{"name": row[0], "score": row[1]} for row in result]  
            return {"statusCode": 200, "message": "Fetched leaderboard successfully", "data": leaderboard}
    except psycopg2.Error:
        raise HTTPException(500, "DB Connection Error")
    except Exception: 
        raise HTTPException(400, "Generic error")
    finally:
        if conn is not None:
            conn.close()

# add item to leaderboard
@app.post("/leaderboard")
async def add_to_leaderboard(entry: LeaderboardModel):
    conn = get_db_conn() 
    try:
        with conn.cursor() as cur:
            query = """
                INSERT INTO leaderboard (name, score)
                VALUES (%s, %s)
            """
            cur.execute(query, (entry.name, entry.score))
            conn.commit()

            return {"statusCode": 200, "message": "Entry added to leaderboard table successfully.", "data": entry}
    except psycopg2.Error:
        conn.rollback()
        raise HTTPException(500, "DB Connection Error")
    except Exception: 
        conn.rollback()
        raise HTTPException(400, "Generic error")
    finally:
        if conn is not None:
            conn.close()


handler = Mangum(app, lifespan="off")

if __name__ == "__main__":
    uvicorn_app = f"{os.path.basename(__file__).removesuffix('.py')}:app"
    uvicorn.run(uvicorn_app, host="0.0.0.0", port=8000, reload=True)
