using System.Diagnostics;
using Common;
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
Console.Write("Enter path to file: ");
var filePath = Console.ReadLine();

var stopwatch = Stopwatch.StartNew();
Console.WriteLine("Dividing large file for smaller files...");
var directoryInfo = fileHelper.DivideFileToFolder(filePath);

Console.WriteLine("Sorting temporary files...");
var sortBar = new DetailedProgressBar(directoryInfo.GetFiles().Length, "Sorting file ", "Temporary files sorted");
fileHelper.SortFiles(directoryInfo, sortBar);

Console.WriteLine("Merging temporary files...");
var mergeBar = new DetailedProgressBar(directoryInfo.GetFiles().Length, "Merge process...", "Merge Finished");
var resultFilePath = fileHelper.MergeFiles(directoryInfo, mergeBar);

Console.WriteLine($"Merge finished: {resultFilePath}");
stopwatch.Stop();
Console.WriteLine($"Time taken: {stopwatch.Elapsed}");
Console.ReadKey();