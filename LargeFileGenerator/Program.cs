// See https://aka.ms/new-console-template for more information

using LargeFileSorter;
using Microsoft.Extensions.Logging;

var loggerFactory = LoggerFactory.Create(builder =>
{
    builder.AddConsole();
    builder.SetMinimumLevel(LogLevel.Error);
});
var logger = loggerFactory.CreateLogger("LargeFileSorter");
logger.LogInformation("Start of program");
var fileHelper = new FileHelper(logger, new Random());
string rows;
long sizeOfFile;
do
{
    Console.Write("Enter number of rows in file: ");
    rows = Console.ReadLine();
} while (!long.TryParse(rows, out sizeOfFile));

var progressBar = new DetailedProgressBar(100, "Generating process", "% File generated");
var filePath = fileHelper.GenerateTestCase(sizeOfFile, progressBar);

Console.WriteLine($"Writing to {filePath}");
Console.ReadKey();