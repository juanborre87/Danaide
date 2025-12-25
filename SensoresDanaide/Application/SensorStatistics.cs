using SensoresDanaide.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SensoresDanaide.Application
{
    public sealed class SensorStatistics
    {
        private decimal _totalValue;
        private int _count;

        private decimal _maxValue = decimal.MinValue;
        private Guid _maxValueSensorId;

        private readonly Dictionary<string, (decimal sum, int count)> _byZone = new();
        private readonly Dictionary<string, int> _activeByZone = new();

        public void Process(Sensor sensor)
        {
            _totalValue += sensor.Value;
            _count++;

            if (sensor.Value > _maxValue)
            {
                _maxValue = sensor.Value;
                _maxValueSensorId = sensor.Id;
            }

            if (!_byZone.ContainsKey(sensor.Zone))
                _byZone[sensor.Zone] = (0m, 0);

            var (sum, count) = _byZone[sensor.Zone];
            _byZone[sensor.Zone] = (sum + sensor.Value, count + 1);

            if (sensor.IsActive)
            {
                _activeByZone[sensor.Zone] =
                    _activeByZone.TryGetValue(sensor.Zone, out var c) ? c + 1 : 1;
            }
        }

        public object BuildResult()
        {
            return new
            {
                maxValueSensorId = _count > 0 ? _maxValueSensorId : (Guid?)null,
                averageValue = _count > 0 ? _totalValue / _count : 0m,

                averageValueByZone = _byZone.ToDictionary(
                    k => k.Key,
                    v => v.Value.count > 0
                        ? v.Value.sum / v.Value.count
                        : 0m
                ),

                activeSensorsByZone = _activeByZone
            };
        }
    }
}
