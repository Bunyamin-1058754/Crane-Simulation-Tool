from container import Container
import pickle
from mqttConnect import client, paho, connect_mqtt
import time
import json


def on_publish(client, userdata, mid):
    print(f"{mid}")


connect_mqtt()
client.on_publish = on_publish


def send_methods():
    klass = Container()
    methode = json.dumps(klass.get_coordinates())

    mid = client.publish(topic="methods", payload=methode, qos=0)
    return mid


send_methods()


time.sleep(2)
client.loop_start()
