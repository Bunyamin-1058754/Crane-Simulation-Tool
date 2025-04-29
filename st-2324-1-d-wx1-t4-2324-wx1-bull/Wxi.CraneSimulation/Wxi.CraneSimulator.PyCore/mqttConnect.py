import paho.mqtt.client as paho
from paho import mqtt

client = paho.Client(
    client_id="",
    userdata=None,
    protocol=paho.MQTTv5,
)

def connect_mqtt():
    def on_connect(client, userdata, flags, rc):
        if rc == 0:
            print("Connected to MQTT Broker!")
        else:
            print("Failed to connect, return code %d\n", rc)
    # client.username_pw_set(username, password)
    client.username_pw_set("CraneSimulation", "HowestRAC2023")
    client.tls_set(tls_version=mqtt.client.ssl.PROTOCOL_TLS)
    client.on_connect = on_connect
    client.connect("cab4dd0695d54eea90873d4b3493516d.s2.eu.hivemq.cloud", 8883)
    return client

def disconnect_mqtt():
    client.disconnect()