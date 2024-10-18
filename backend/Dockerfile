FROM public.ecr.aws/lambda/python:3.12
ENV PYTHONUNBUFFERED=1

COPY . .

RUN pip install poetry

RUN poetry config virtualenvs.create false \
    && poetry config installer.parallel false

RUN poetry install --no-dev

CMD [ "app.handler" ]
