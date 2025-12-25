// See https://aka.ms/new-console-template for more information
using Microsoft.Extensions.Configuration;
using SensoresDanaide.Application;
using SensoresDanaide.Domain;
using SensoresDanaide.Infrastructure;
using System.Text.Json;
using System.Threading.Channels;

static void ShowMenu()
{
    Console.WriteLine();
    Console.WriteLine("===== SENSOR PROCESSOR =====");
    Console.WriteLine("1 - Iniciar procesamiento");
    Console.WriteLine("0 - Salir");
    Console.Write("Seleccione una opción: ");
}

static (bool csv, bool xml) ShowFormatMenu()
{
    Console.WriteLine();
    Console.WriteLine("Seleccione formatos de salida:");
    Console.WriteLine("1 - CSV");
    Console.WriteLine("2 - XML");
    Console.WriteLine("3 - CSV y XML");
    Console.Write("Opción: ");

    return Console.ReadLine() switch
    {
        "1" => (true, false),
        "2" => (false, true),
        "3" => (true, true),
        _ => (false, false)
    };
}

/* ================================
   CARGA DE CONFIGURACIÓN
   ================================ */

var config = new ConfigurationBuilder()
    .SetBasePath(AppContext.BaseDirectory)
    .AddJsonFile("appsettings.json", optional: false)
    .Build();

var appConfig = new AppConfig();
config.Bind(appConfig);

/* ================================
   MENÚ PRINCIPAL
   ================================ */

while (true)
{
    ShowMenu();
    var option = Console.ReadLine();

    if (option == "0")
        break;

    if (option != "1")
        continue;

    var (useCsv, useXml) = ShowFormatMenu();

    if (!useCsv && !useXml)
    {
        Console.WriteLine("Opción inválida");
        continue;
    }

    Console.WriteLine("Procesando archivo...");

    await RunProcess(appConfig, useCsv, useXml);

    Console.WriteLine("Proceso finalizado");
}

/* ================================
   MÉTODO PRINCIPAL DE PROCESO
   ================================ */

static async Task RunProcess(AppConfig cfg, bool useCsv, bool useXml)
{
    var reader = new JsonSensorStreamReader();
    var statistics = new SensorStatistics();

    var cts = new CancellationTokenSource();
    var tasks = new List<Task>();

    Channel<Sensor>? csvChannel = null;
    Channel<Sensor>? xmlChannel = null;

    if (useCsv)
    {
        csvChannel = Channel.CreateUnbounded<Sensor>();
        var csvWriter = new CsvOutputWriter();

        tasks.Add(csvWriter.WriteAsync(
            csvChannel.Reader.ReadAllAsync(cts.Token),
            cfg.Output.CsvPath,
            cts.Token));
    }

    if (useXml)
    {
        xmlChannel = Channel.CreateUnbounded<Sensor>();
        var xmlWriter = new XmlOutputWriter();

        tasks.Add(xmlWriter.WriteAsync(
            xmlChannel.Reader.ReadAllAsync(cts.Token),
            cfg.Output.XmlPath,
            cts.Token));
    }

    await foreach (var sensor in reader.ReadAsync(cfg.Input.JsonPath, cts.Token))
    {
        statistics.Process(sensor);

        if (useCsv)
            await csvChannel!.Writer.WriteAsync(sensor);

        if (useXml)
            await xmlChannel!.Writer.WriteAsync(sensor);
    }

    csvChannel?.Writer.Complete();
    xmlChannel?.Writer.Complete();

    await Task.WhenAll(tasks);

    var summaryJson = JsonSerializer.Serialize(
        statistics.BuildResult(),
        new JsonSerializerOptions { WriteIndented = true });

    await File.WriteAllTextAsync(cfg.Output.SummaryPath, summaryJson);
}
