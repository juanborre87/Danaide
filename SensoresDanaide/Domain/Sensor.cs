using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SensoresDanaide.Domain
{
    public sealed class Sensor
    {
        public int Index { get; init; }
        public Guid Id { get; init; }
        public bool IsActive { get; init; }
        public string Zone { get; init; } = string.Empty;
        public decimal Value { get; init; }
    }
}
