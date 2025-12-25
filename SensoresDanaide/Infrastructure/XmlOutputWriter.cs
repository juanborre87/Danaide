using SensoresDanaide.Application;
using SensoresDanaide.Domain;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace SensoresDanaide.Infrastructure
{
    public sealed class XmlOutputWriter : IOutputWriter
    {
        public async Task WriteAsync(
            IAsyncEnumerable<Sensor> sensors,
            string outputPath,
            CancellationToken ct)
        {
            await using var stream = File.Create(outputPath);
            using var writer = XmlWriter.Create(stream, new XmlWriterSettings
            {
                Async = true,
                Indent = true
            });

            await writer.WriteStartDocumentAsync();
            await writer.WriteStartElementAsync(null, "Sensors", null);

            await foreach (var s in sensors.WithCancellation(ct))
            {
                await writer.WriteStartElementAsync(null, "Sensor", null);
                await writer.WriteElementStringAsync(null, "Index", null, s.Index.ToString());
                await writer.WriteElementStringAsync(null, "Id", null, s.Id.ToString());
                await writer.WriteElementStringAsync(null, "IsActive", null, s.IsActive.ToString());
                await writer.WriteElementStringAsync(null, "Zone", null, s.Zone);
                await writer.WriteElementStringAsync(
                    null,
                    "Value",
                    null,
                    s.Value.ToString(CultureInfo.InvariantCulture));

                await writer.WriteEndElementAsync();
            }

            await writer.WriteEndElementAsync();
            await writer.WriteEndDocumentAsync();
        }
    }
}
