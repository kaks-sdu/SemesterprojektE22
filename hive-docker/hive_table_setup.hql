ADD JAR /opt/hive/lib/json-serde-1.3.8-jar-with-dependencies.jar;

CREATE DATABASE IF NOT EXISTS semester_project;
use semester_project;

CREATE EXTERNAL TABLE push_events (
  actor struct<avatar_url:string, gravatar_id:string, id:int, login:string, url:string>,
  created_at string,
  id string,
  payload struct<before:string, commits:array<struct<author:struct<email:string, name:string>, is_distinct:boolean, message:string, sha:string, url:string>>, distinct_size:int, head:string, push_id:int, ref:string, size:int>,
  public boolean,
  repo struct<id:int, name:string, url:string>,
  type string)
ROW FORMAT SERDE 'org.openx.data.jsonserde.JsonSerDe'
LOCATION 'hdfs://namenode:9000/stream/topics/events/partition=0';