from fastapi import FastAPI
from mangum import Mangum
import uvicorn
import os
import psycopg2


app = FastAPI()


@app.get("/")
async def hello_world():
    return "Hey does my pipeline automatically update my change?"


@app.get("/test-postgres")
async def test_postgres():
    conn = psycopg2.connect(
        host=os.getenv("PG_HOST"),
        port=os.getenv("PG_PORT"),
        user=os.getenv("PG_USER"),
        password=os.getenv("PG_PASSWORD"),
        dbname=os.getenv("PG_DATABASE"),
    )
    cur = conn.cursor()
    cur.execute("SELECT 'Hello, world!!'")
    result = cur.fetchone()
    cur.close()
    conn.close()

    return {"statusCode": 200, "body": result[0]}


handler = Mangum(app, lifespan="off")

if __name__ == "__main__":
    uvicorn_app = f"{os.path.basename(__file__).removesuffix('.py')}:app"
    uvicorn.run(uvicorn_app, host="0.0.0.0", port=8000, reload=True)
