FROM python:3.8

RUN apt-get update && apt-get install -y ffmpeg

WORKDIR /app

COPY . .

RUN pip install --trusted-host pypi.python.org -r requirements.txt

EXPOSE 3000

CMD [ "python3", "app.py"]