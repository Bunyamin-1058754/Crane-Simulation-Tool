using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wxi.CraneSimulation.Core.Events.Args
{
    public class MessageReceivedArgs : EventArgs
    {
        public string Topic { get; set; }
        public string Message { get; set; }

        public MessageReceivedArgs(string topic, string message)
        {
            Topic = topic;
            Message = message;
        }
    }
}
