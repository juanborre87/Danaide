using SensoresDanaide.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SensoresDanaide.Application
{
    public interface IOutputWriter
    {
        Task WriteAsync(
            IAsyncEnumerable<Sensor> sensors,
            string outputPath,
            CancellationToken ct);
    }
}
