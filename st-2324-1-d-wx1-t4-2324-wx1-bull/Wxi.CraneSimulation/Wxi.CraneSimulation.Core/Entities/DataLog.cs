using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wxi.CraneSimulation.Core.Entities
{
    public class DataLog
    {
        public Guid Id { get; set; }
        public string X { get; set; }
        public string Y { get; set; }
        public string Name { get; set; }
        public DateTime Date { get; set; }
        public bool IsError { get; set; }

        public override string ToString()
        {
            return $"[{Date}] | X: {X}, Y: {Y}";

        }
    }
}
