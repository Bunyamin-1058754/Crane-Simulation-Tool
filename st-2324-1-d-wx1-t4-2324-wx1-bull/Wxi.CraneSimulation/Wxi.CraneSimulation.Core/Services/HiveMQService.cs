using HiveMQtt.Client;
using HiveMQtt.Client.Events;
using HiveMQtt.Client.Options;
using HiveMQtt.MQTT5.ReasonCodes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wxi.CraneSimulation.Core.Events.Args;

namespace Wxi.CraneSimulation.Core.Services
{
    public delegate void MessageReceivedHandler(object sender, MessageReceivedArgs e);

    public class HiveMQService
    {
        public event MessageReceivedHandler MessageReceived;

        private readonly HiveMQClient _client = new HiveMQClient();
        public async Task<bool> ConnectToServer()
        {
            _client.Options = new HiveMQClientOptions()
            {
                Host = "cab4dd0695d54eea90873d4b3493516d.s2.eu.hivemq.cloud",
                Port = 8883,
                UseTLS = true,
                UserName = "CraneSimulation",
                Password = "HowestRAC2023",
            };

            var result = await _client.ConnectAsync().ConfigureAwait(false);
            if (result.ReasonCode == ConnAckReasonCode.Success)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        public async Task<bool> DisconnectFromServer()
        {
            return await _client.DisconnectAsync();
        }

        public async Task PublishToServer(string topic, string message)
        {
            await _client.PublishAsync(topic, message).ConfigureAwait(false);
        }

        public async Task SubscribeToTopic(string topic)
        {
            await _client.SubscribeAsync(topic).ConfigureAwait(false);

        }

        public void StartListening()
        {
            _client.OnMessageReceived += Client_OnMessageReceived;
        }

        public void StopListening()
        {
            _client.OnMessageReceived -= Client_OnMessageReceived;
        }

        public void Client_OnMessageReceived(object? sender, OnMessageReceivedEventArgs e)
        {
            MessageReceived.Invoke(this, new MessageReceivedArgs(e.PublishMessage.Topic, e.PublishMessage.PayloadAsString));
        }

    }
}
