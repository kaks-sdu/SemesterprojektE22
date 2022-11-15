FROM confluentinc/cp-server-connect-base:7.3.0

#ADD quickstart-hdfs.properties /usr/share/confluent-hub-components/confluentinc-kafka-connect-hdfs/etc/

RUN   confluent-hub install --no-prompt confluentinc/kafka-connect-hdfs:10.1.14 
   #&& confluent-hub install --no-prompt microsoft/kafka-connect-iothub:0.6 \
   #&& confluent-hub install --no-prompt wepay/kafka-connect-bigquery:1.1.0

# /usr/share/confluent-hub-components/confluentinc-kafka-connect-hdfs/etc/quickstart-hdfs.properties

#ADD quickstart-hdfs.properties /usr/share/confluent-hub-components/confluentinc-kafka-connect-hdfs/etc/

#ENTRYPOINT [ "/etc/confluent/docker/run &"] 


# Exec into hdfs and hdfs dfs -cat /stream/topics/events/partition=0/events+0+0000000000+0000000002.txt  to see the data