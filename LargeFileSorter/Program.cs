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

var directoryInfo = fileHelper.DivideFileToFolder(filePath, 1000);

var sortBar = new DetailedProgressBar(directoryInfo.GetFiles().Length, "Sorting file ", "Temporary files sorted");
fileHelper.SortFiles(directoryInfo, sortBar);

var mergeBar = new DetailedProgressBar(directoryInfo.GetFiles().Length, "Merge process...", "Merge Finished");
fileHelper.MergeFiles(directoryInfo, mergeBar, 1000);

Console.ReadKey();