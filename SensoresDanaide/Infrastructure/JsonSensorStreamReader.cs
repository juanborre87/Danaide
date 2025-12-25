using SensoresDanaide.Domain;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace SensoresDanaide.Infrastructure
{
    public sealed class JsonSensorStreamReader
    {
        public async IAsyncEnumerable<Sensor> ReadAsync(
            string path,
            [System.Runtime.CompilerServices.EnumeratorCancellation] CancellationToken ct = default)
        {
            if (!File.Exists(path))
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("No se encontró el archivo de entrada:");
                Console.WriteLine($"   {path}");
                Console.ResetColor();

                yield break;
            }

            await using var stream = File.OpenRead(path);

            await foreach (var element in JsonSerializer.DeserializeAsyncEnumerable<JsonElement>(
                stream,
                cancellationToken: ct))
            {
                if (element.ValueKind != JsonValueKind.Object)
                    continue;

                yield return new Sensor
                {
                    Index = element.GetProperty("index").GetInt32(),
                    Id = element.GetProperty("id").GetGuid(),
                    IsActive = element.GetProperty("isActive").GetBoolean(),
                    Zone = element.GetProperty("zone").GetString()!,
                    Value = decimal.Parse(
                        element.GetProperty("value").GetString()!,
                        CultureInfo.InvariantCulture)
                };
            }
        }
    }
}
