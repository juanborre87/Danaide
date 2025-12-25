using SensoresDanaide.Application;
using SensoresDanaide.Domain;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SensoresDanaide.Infrastructure
{
    public sealed class CsvOutputWriter : IOutputWriter
    {
        public async Task WriteAsync(
            IAsyncEnumerable<Sensor> sensors,
            string outputPath,
            CancellationToken ct)
        {
            await using var writer = new StreamWriter(outputPath, false, Encoding.UTF8);

            await writer.WriteLineAsync("Index,Id,IsActive,Zone,Value");

            await foreach (var s in sensors.WithCancellation(ct))
            {
                var line = string.Join(",",
                    s.Index,
                    s.Id,
                    s.IsActive,
                    s.Zone,
                    s.Value.ToString(CultureInfo.InvariantCulture));

                await writer.WriteLineAsync(line);
            }
        }
    }
}
