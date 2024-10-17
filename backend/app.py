from fastapi import FastAPI
import uvicorn

app = FastAPI()

@app.get("/")
async def hello_world():
    return "Hello, World!"

if __name__ == "__main__":
    uvicorn.run("__main__:app", host="localhost", port=8000, reload=True)
