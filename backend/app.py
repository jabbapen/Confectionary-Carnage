from fastapi import FastAPI
from mangum import Mangum
import uvicorn
import os


app = FastAPI()

@app.get("/")
async def hello_world():
    return "Hello, World?"

handler = Mangum(app, lifespan="off")

if __name__ == "__main__":
    uvicorn_app = f"{os.path.basename(__file__).removesuffix('.py')}:app"
    uvicorn.run(uvicorn_app, host="0.0.0.0", port=8000, reload=True)
