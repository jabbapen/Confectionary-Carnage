from http.client import HTTPException
from typing import Optional, Dict, Any
from pydantic import BaseModel
from fastapi import FastAPI
from mangum import Mangum
import uvicorn
import os
import psycopg2
from fastapi.middleware.cors import CORSMiddleware
from psycopg2.extensions import connection

app = FastAPI()

origins = ["http://confectionary-carnage-webgl.s3-website-us-west-1.amazonaws.com"]

app.add_middleware(
    CORSMiddleware,
    allow_origins=origins,  # Or ["*"] to allow all origins
    allow_credentials=True,
    allow_methods=["*"],  # Or specify ["GET", "POST", "OPTIONS"] etc.
    allow_headers=["*"],
)


# Utility classes/models
class LeaderboardModel(BaseModel):
    name: str
    score: int


class LevelModel(BaseModel):
    level_name: str
    author: str
    serialized_level: str


# Utility functions
def get_db_conn() -> connection:
    try:
        return psycopg2.connect(
            host=os.getenv("PG_HOST"),
            port=os.getenv("PG_PORT"),
            user=os.getenv("PG_USER"),
            password=os.getenv("PG_PASSWORD"),
            dbname=os.getenv("PG_DATABASE"),
        )
    except psycopg2.Error as e:
        raise HTTPException(500, f"DB connection error: {e}")


def init_leaderboard(conn: connection) -> None:
    try:
        with conn.cursor() as cur:
            leaderboard_query = """
            CREATE TABLE IF NOT EXISTS leaderboard (
                id SERIAL PRIMARY KEY,
                name VARCHAR(100) NOT NULL,
                score INT NOT NULL
            );
            """
            cur.execute(leaderboard_query)
            conn.commit()
    except Exception as e:
        raise HTTPException(400, f"Failed to create leaderboard table: {e}")


def init_levels(conn: connection) -> None:
    try:
        with conn.cursor() as cur:
            level_query = """
            CREATE TABLE IF NOT EXISTS levels (
                id SERIAL PRIMARY KEY,
                level_name VARCHAR(255) NOT NULL, 
                author VARCHAR(255),
                serialized_level TEXT NOT NULL UNIQUE
            );
            """
            cur.execute(level_query)
            conn.commit()
    except Exception as e:
        raise HTTPException(400, f"Failed to create level table: {e}")


@app.on_event("startup")
def startup() -> None:
    conn = get_db_conn()
    init_leaderboard(conn)
    init_levels(conn)
    conn.close()


@app.get("/")
async def hello_world() -> str:
    return "Confectionary Carnage Backend"


@app.get("/test-postgres")
async def test_postgres() -> Dict[str, Any]:
    conn = get_db_conn()
    try:
        with conn.cursor() as cur:
            cur.execute("SELECT 'Hello, world!!'")
            result = cur.fetchone()
            if result is None:
                raise HTTPException(500, "No result from database")
            return {"statusCode": 200, "body": result[0]}
    except psycopg2.Error:
        raise HTTPException(500, "DB Connection Error")
    except Exception as e:
        raise HTTPException(400, f"Generic error: {e}")
    finally:
        conn.close()


@app.get("/leaderboard")
async def get_leaderboard(limit: Optional[int] = None) -> Dict[str, Any]:
    conn = get_db_conn()
    init_leaderboard(conn)
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
            return {
                "statusCode": 200,
                "message": "Fetched leaderboard successfully",
                "data": leaderboard,
            }
    except psycopg2.Error:
        raise HTTPException(500, "DB Connection Error")
    except Exception as e:
        raise HTTPException(400, f"Generic error: {e}")
    finally:
        conn.close()


@app.post("/leaderboard")
async def add_to_leaderboard(entry: LeaderboardModel) -> Dict[str, Any]:
    conn = get_db_conn()
    init_leaderboard(conn)
    try:
        with conn.cursor() as cur:
            query = """
                INSERT INTO leaderboard (name, score)
                VALUES (%s, %s)
            """
            cur.execute(query, (entry.name, entry.score))
            conn.commit()

            return {
                "statusCode": 200,
                "message": "Entry added to leaderboard table successfully.",
                "data": entry.dict(),
            }
    except psycopg2.Error:
        conn.rollback()
        raise HTTPException(500, "DB Connection Error")
    except Exception as e:
        conn.rollback()
        raise HTTPException(400, f"Generic error: {e}")
    finally:
        conn.close()


@app.get("/levels")
async def get_random_levels(limit: Optional[int] = None) -> Dict[str, Any]:
    conn = get_db_conn()
    init_levels(conn)
    try:
        with conn.cursor() as cur:
            query = """
                SELECT level_name, author, serialized_level
                FROM levels
                ORDER BY RANDOM()
            """
            if limit is not None:
                query += " LIMIT %s"
                cur.execute(query, (limit,))
            else:
                cur.execute(query)
            result = cur.fetchall()
            levels = [
                {"level_name": row[0], "author": row[1], "serialized_level": row[2]}
                for row in result
            ]
            return {
                "statusCode": 200,
                "message": "Fetched random levels successfully",
                "data": levels,
            }
    except psycopg2.Error:
        raise HTTPException(500, "DB Connection Error")
    except Exception as e:
        raise HTTPException(400, f"Generic error: {e}")
    finally:
        conn.close()


@app.post("/levels")
async def add_levels(entry: LevelModel) -> Dict[str, Any]:
    conn = get_db_conn()
    init_levels(conn)
    try:
        with conn.cursor() as cur:
            query = """
                INSERT INTO levels (level_name, author, serialized_level)
                VALUES (%s, %s, %s)
            """
            cur.execute(query, (entry.level_name, entry.author, entry.serialized_level))
            conn.commit()

            return {
                "statusCode": 200,
                "message": "Entry added to levels table successfully.",
                "data": entry.dict(),
            }
    except psycopg2.errors.UniqueViolation:
        conn.rollback()
        raise HTTPException(400, "Level with this name already exists.")
    except psycopg2.Error:
        conn.rollback()
        raise HTTPException(500, "DB Connection Error")
    except Exception as e:
        conn.rollback()
        raise HTTPException(400, f"Generic error: {e}")
    finally:
        conn.close()


handler = Mangum(app)

if __name__ == "__main__":
    uvicorn_app: str = f"{os.path.basename(__file__).removesuffix('.py')}:app"
    uvicorn.run(uvicorn_app, host="0.0.0.0", port=8000, reload=True)
