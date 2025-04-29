using MQTTnet;
using MQTTnet.Client;
using System;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
public class Movement : MonoBehaviour
{
    private readonly IMqttClient mqttClient;
    private Vector3 nextPosition;
    private bool holdingContainer;
    private Vector3 spreaderPosition;
    public Transform cabinGroup;
    public Transform spreader;
    public Transform container;
    // Start is called before the first frame update
    void Start()
    {
        ConnectToServer();
        nextPosition = cabinGroup.localPosition;
        spreaderPosition = spreader.localPosition;
    }

    // Update is called once per frame
    void Update()
    {
        cabinGroup.localPosition = nextPosition;
        nextPosition = cabinGroup.localPosition;

        spreader.localPosition = spreaderPosition;
        spreaderPosition = spreader.localPosition;

        if (holdingContainer)
        {
            container.SetParent(spreader);
        }
        else
        {
            container.parent = null;
        }
    }

    private async void ConnectToServer()
    {
        var factory = new MqttFactory();
        var mqttClient = factory.CreateMqttClient();

        var options = new MqttClientOptionsBuilder()
            .WithTls()
            .WithTcpServer("cab4dd0695d54eea90873d4b3493516d.s2.eu.hivemq.cloud", 8883)
            .WithCredentials("CraneSimulation", "HowestRAC2023")
            .Build();

        var result = await mqttClient.ConnectAsync(options);

        if (result.ResultCode == MqttClientConnectResultCode.Success)
        {
            Debug.Log("Connect succeeded");
        }
        else
        {
            Debug.Log("Connect failed");
        }

        await mqttClient.SubscribeAsync("simulation/crane/movement");
        mqttClient.ApplicationMessageReceivedAsync += MqttClient_ApplicationMessageReceivedAsync;
    }

    private Task MqttClient_ApplicationMessageReceivedAsync(MqttApplicationMessageReceivedEventArgs arg)
    {
        string message = Encoding.UTF8.GetString(arg.ApplicationMessage.PayloadSegment);
        switch (message)
        {
            case "Forward":
                nextPosition = new Vector3(nextPosition.x - (float)0.3, nextPosition.y, nextPosition.z);
                break;
            case "Backward":
                nextPosition = new Vector3(nextPosition.x + (float)0.3, nextPosition.y, nextPosition.z);
                break;
            case "Up":
                spreaderPosition = new Vector3(spreaderPosition.x, spreaderPosition.y + (float)0.1, spreaderPosition.z);
                break;
            case "Down":
                if (Math.Round(spreaderPosition.y) == -17)
                {
                    spreaderPosition.y = (float)-16.8;
                }
                else
                {
                    spreaderPosition = new Vector3(spreaderPosition.x, spreaderPosition.y - (float)0.1, spreaderPosition.z);
                }
                break;
            case "Grab":
                holdingContainer = !holdingContainer;
                break;
        }
        return Task.CompletedTask;
    }
}