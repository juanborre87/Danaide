using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SensoresDanaide.Application
{

    public sealed class AppConfig
    {
        public InputConfig Input { get; init; } = new();
        public OutputConfig Output { get; init; } = new();
    }

    public sealed class InputConfig
    {
        public string JsonPath { get; init; } = string.Empty;
    }

    public sealed class OutputConfig
    {
        public string CsvPath { get; init; } = string.Empty;
        public string XmlPath { get; init; } = string.Empty;
        public string SummaryPath { get; init; } = string.Empty;
    }
}
