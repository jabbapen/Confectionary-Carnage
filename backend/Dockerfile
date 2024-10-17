FROM python:3.12-slim

WORKDIR /usr/src/app

COPY . .

RUN pip install poetry

RUN poetry config virtualenvs.create false \
    && poetry config installer.parallel false

RUN poetry install --no-dev

EXPOSE 8000

CMD ["uvicorn", "app:app", "--host", "0.0.0.0", "--port", "8000"]
