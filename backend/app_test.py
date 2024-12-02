import psycopg2
import pytest
from fastapi.testclient import TestClient
from app import app
import os


# Set environment variables for test database
@pytest.fixture(autouse=True)
def setup_test_env():
    os.environ["PG_HOST"] = "localhost"
    os.environ["PG_PORT"] = "5433"
    os.environ["PG_USER"] = "test_user"
    os.environ["PG_PASSWORD"] = "test_password"
    os.environ["PG_DATABASE"] = "test_db"


@pytest.fixture(scope="session", autouse=True)
def cleanup_database():
    # Connect to the database
    conn = psycopg2.connect(
        host="localhost",
        port="5433",
        user="test_user",
        password="test_password",
        dbname="test_db"
    )

    try:
        with conn.cursor() as cur:
            # Drop existing tables if they exist
            cur.execute("""
                DROP TABLE IF EXISTS leaderboard;
                DROP TABLE IF EXISTS levels;
            """)
            conn.commit()
    finally:
        conn.close()

@pytest.fixture
def client():
    """Fixture to initialize TestClient for FastAPI."""
    with TestClient(app) as test_client:
        yield test_client


def test_hello_world(client):
    response = client.get("/")
    assert response.status_code == 200
    assert response.json() == "Hey does my pipeline automatically update my change?"


def test_test_postgres(client):
    response = client.get("/test-postgres")
    assert response.status_code == 200
    assert response.json() == {"statusCode": 200, "body": "Hello, world!!"}


def test_get_leaderboard_empty(client):
    response = client.get("/leaderboard")
    assert response.status_code == 200
    assert "data" in response.json()
    assert response.json()["statusCode"] == 200
    assert response.json()["data"] == []
    assert "message" in response.json()


def test_add_to_leaderboard(client):
    payload = {"name": "test_user", "score": 100}
    response = client.post("/leaderboard", json=payload)
    assert response.status_code == 200
    assert response.json()["statusCode"] == 200
    assert response.json()["data"] == payload
    assert "message" in response.json()


def test_get_leaderboard_with_entry(client):
    payload = {"name": "test_user", "score": 100}
    client.post("/leaderboard", json=payload)

    response = client.get("/leaderboard")
    assert response.status_code == 200
    assert response.json()["statusCode"] == 200
    data = response.json()["data"]
    assert len(data) > 0
    assert data[0]["name"] == payload["name"]
    assert data[0]["score"] == payload["score"]


def test_get_leaderboard_with_limit(client):
    entries = [
        {"name": f"user_{i}", "score": i * 100}
        for i in range(5)
    ]
    for entry in entries:
        client.post("/leaderboard", json=entry)

    limit = 3
    response = client.get(f"/leaderboard?limit={limit}")
    assert response.status_code == 200
    assert len(response.json()["data"]) <= limit


def test_get_levels_empty(client):
    response = client.get("/levels")
    assert response.status_code == 200
    assert response.json()["statusCode"] == 200
    assert response.json()["data"] == []
    assert "message" in response.json()


def test_add_levels(client):
    payload = {
        "level_name": "test_level",
        "author": "test_author",
        "serialized_level": "test_serialized_data"
    }
    response = client.post("/levels", json=payload)
    assert response.status_code == 200
    assert response.json()["statusCode"] == 200
    assert response.json()["data"] == payload
    assert "message" in response.json()


def test_get_levels_with_entry(client):
    payload = {
        "level_name": "test_level",
        "author": "test_author",
        "serialized_level": "test_serialized_data"
    }
    client.post("/levels", json=payload)

    response = client.get("/levels")
    assert response.status_code == 200
    assert response.json()["statusCode"] == 200
    data = response.json()["data"]
    assert len(data) > 0
    assert data[0]["level_name"] == payload["level_name"]
    assert data[0]["author"] == payload["author"]
    assert data[0]["serialized_level"] == payload["serialized_level"]


def test_get_levels_with_limit(client):
    entries = [
        {
            "level_name": f"level_{i}",
            "author": f"author_{i}",
            "serialized_level": f"data_{i}"
        }
        for i in range(5)
    ]
    for entry in entries:
        client.post("/levels", json=entry)

    limit = 3
    response = client.get(f"/levels?limit={limit}")
    assert response.status_code == 200
    assert len(response.json()["data"]) <= limit