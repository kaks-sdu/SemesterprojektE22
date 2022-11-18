# SemesterprojektE22

## How to start


Setup Kafka Connect (one time only)

1. Run ```docker compose up``` in the root directory.

This will make a source folder. The source folder is where Flume takes the files from i.e. where to upload files for streaming.

2. Go to ```localhost:8000``` for the kowl website, and go to the kafka connect tab on the left hand side
3. Click on the blue create connector button

   ![](assets/20221115_214407_image.png)
4. Choose kafka-connect-to-hdfs installation target and HdfsSinkConnector as the connector type

   ![](assets/20221115_214558_image.png)
5. Choose a connector name and flush size (it will be overriden anyway so doesn't matter what it is) and scroll all the way to the bottom and click on the blue next step button
6. Override the connector properties so it looks like:

   ```json
   {
       "name": "kafka-to-hdfs",
       "connector.class": "io.confluent.connect.hdfs.HdfsSinkConnector",
       "tasks.max": 3,
       "key.converter": "org.apache.kafka.connect.storage.StringConverter",
       "topics": "events",
       "hdfs.url": "hdfs://namenode:9000/stream/",
       "format.class": "io.confluent.connect.hdfs.string.StringFormat",
       "flush.size": 100
   }

   ```
Flush size can be changed. It indicates how many lines are put into each file in the HDFS. Ideally it should match the block size of the HDFS cluster such that a balance is reached. Set it as a low number for now.

Setup Hive (must be done every time there is a restart):

1. Find the id of the hive container
   1. For Windows: `docker ps | findstr "hive-server"`
   2. For Linux: `docker ps | grep "hive-server"`
2. Enter the hive container with `docker exec -it <container_id or name of hive_server> /bin/bash`
3. Run `hive -f hive_table_setup.hql`


After this, everything should be setup. To run queries on the data that comes through the source folder, you will need to make sure that you specify ```USE DATABASE semester_project```. The table is called ```push_events```.

Data should automatically be published into the table after it is pushed through flume to kafka to HDFS.

## How to test

Upload the json files into the source folder and it should be automatically put into the Kafka topic 'events' and moved to the stream folder of the HDFS

## How to use speech interface 
The speech interface can be found on port 3000. It accepts a `POST` request to the `/transcribe` endpoint with a body parameter called audio_data that accepts audio data.

The settings.json located in the speech-interface folder contains the possible questions that are matched from the audio data:

```json
{
    "model_size": "base",
    "language": "english",
    "semantic_model": "all-MiniLM-L6-v2",
    "questions": [
        "how many push requests are there",
        "what are the top comments"
    ]
}
```
To add new questions simply add them there.

If given audio that transcribes to "what is the top comment?" the response will be:

```json
{
  "transcribed_text": "What is the top comment?",
  "question": "what are the top comments",
  "index": 1,
  "score": 0.8937845230102539
}
```
Showing both the transcribed text from the audio data, the question that is most closely a match, the index (starting at 0), and the similarity between the transcribed text vs. the best question.

## Useful ports
- 3000 - Speech interface
- 8000 - Kowl Web interface
- 9870 - Web HDFS
- 8080 - Spark web interface
- 9000 - Hadoop connect
- 9092 - Kafka
- 7077 - Spark (possible to be removed)
- 8001 - Presto web interface (to check Hive speeds etc.)
- 10000 - Hive