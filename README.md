# SemesterprojektE22

## How to start
Run ```docker compose up``` in the root directory.

This should make a source and stream-in folder. The stream-in folder contains the hadoop DFS files as a volume. The source folder is where Flume takes the files from.

## How to test
Upload the json files into the source folder and it should be automatically put into the Kafka topic 'events'