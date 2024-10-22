using System.Diagnostics;
using LargeFileSorter;
using Microsoft.Extensions.Logging;

var loggerFactory = LoggerFactory.Create(builder =>
{
    builder.AddConsole();
    builder.SetMinimumLevel(LogLevel.Trace);
});
var logger = loggerFactory.CreateLogger("LargeFileSorter");
logger.LogInformation("Start of program");
var stopwatch = Stopwatch.StartNew();
var fileHelper = new FileHelper(logger, new Random());
var testCaseFile = fileHelper.GenerateTestCase(3 * 1000 * 1000 );
stopwatch.Stop();
logger.LogInformation($"file generated in {stopwatch.Elapsed}");
stopwatch.Start();
var directoryInfo = fileHelper.DivideFileToFolder(testCaseFile, 1000);
stopwatch.Stop();
logger.LogInformation($"file divided in {stopwatch.Elapsed}");
stopwatch.Start();
fileHelper.SortFiles(directoryInfo);
stopwatch.Stop();
logger.LogInformation($"files sorted in {stopwatch.Elapsed}");
stopwatch.Start();
fileHelper.MergeFiles(directoryInfo);
stopwatch.Stop();
logger.LogInformation($"file sorted in {stopwatch.Elapsed}");