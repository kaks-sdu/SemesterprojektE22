FROM python:3.8

RUN apt-get update && apt-get install -y ffmpeg

WORKDIR /app

COPY . .

RUN apt-get install sasl2-bin libsasl2-2 libsasl2-dev libsasl2-modules -y
RUN pip install --trusted-host pypi.python.org -r requirements.txt

EXPOSE 3000

CMD [ "python3", "app.py"]