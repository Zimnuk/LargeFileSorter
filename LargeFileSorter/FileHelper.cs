using System.Text;
using LargeFileSorter.Models;
using Microsoft.Extensions.Logging;

namespace LargeFileSorter;
public class FileHelper
{
    private readonly Random _random;
    private readonly ILogger _logger;

    public FileHelper(ILogger logger, Random random)
    {
        _logger = logger;
        _random = random;
    }

    public string GenerateTestCase(uint rows)
    {
        var filePath = $"TestCase.txt";
        _logger.LogInformation($"Generating testcase file {filePath}");
        using (StreamWriter outfile = new StreamWriter(filePath))
        {
            for (int i = 0; i < rows; i++)
            {
                var number = _random.Next(int.MaxValue);
                var stingLength = _random.Next(5, 50);
                var row = $"{number}.{GenerateRandomString(stingLength)}";
                outfile.WriteLine(row);
            }
        }
        
        _logger.LogInformation($"testcase file {filePath} generated successfully");
        return filePath;
    } 
    
    public DirectoryInfo? DivideFileToFolder(string path, int rows)
    {
        if (!File.Exists(path))
        {
            _logger.LogError($"File {path} does not exist");
            return null;
        }
        var folderPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "temp");
        if(Directory.Exists(folderPath))
            Directory.Delete(folderPath, true);
        
        var tempFolder = Directory.CreateDirectory(folderPath);
        _logger.LogInformation($"Created folder: {folderPath}");
        _logger.LogInformation($"Start of large file dividing");
        
        var numberOfFiles = 0;
        
        using (var inFile = new StreamReader(path))
        {
            
            while (!inFile.EndOfStream)
            {
                var i = 0;
                var tempFile = Path.Combine(tempFolder.FullName, $"{Guid.NewGuid()}.txt");
                using var outFile = new StreamWriter(tempFile);
                while (i < rows && !inFile.EndOfStream)
                {
                    var row = inFile.ReadLine();
                    outFile.WriteLine(row);
                    i++;
                }

                numberOfFiles++;
            }
        }
        _logger.LogInformation($"Large file divided for {numberOfFiles} files");
        return tempFolder;
    }

    public void SortFiles(DirectoryInfo dir)
    {
        if (!Directory.Exists(dir.FullName))
        {
            _logger.LogError($"Directory {dir} does not exist");
            return;
        }
        
        var files = dir.GetFiles();
        _logger.LogInformation("Sorting files");
        foreach (var file in files)
        {
            using var inFile = new StreamReader(file.FullName);
            var rows = new List<Row>();
            while (!inFile.EndOfStream)
            {
                var split = inFile.ReadLine().Split('.');
                if (!int.TryParse(split[0], out int number))
                {
                    _logger.LogError($"File {file.FullName} is invalid");
                }
                rows.Add(new Row(number, split[1]));
            }
            inFile.Close();
            rows = rows.Order().ToList();
            var text = rows.Aggregate("", (current, next) => current + next + Environment.NewLine);
            File.WriteAllText(file.FullName, text);
        }
    }

    public string MergeFiles(DirectoryInfo dir)
    {
        if (!Directory.Exists(dir.FullName))
        {
            _logger.LogError($"Directory {dir} does not exist");
            return string.Empty;
        }
        var result = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "result.txt");
        
        _logger.LogInformation($"Merging files from {dir.FullName}");
        var queue = new PriorityQueue<Row, Row>();
        var fileReaders = new Dictionary<string, StreamReader>();
        foreach (var file in dir.GetFiles())
        {
            var inFile = new StreamReader(file.FullName);
            var line = inFile.ReadLine();
            if (line != null)
            {
                var row = Row.FromString(line, file.FullName);
                queue.Enqueue(row,row);
            }
            fileReaders.Add(file.FullName, inFile);
        }
        using var resultFile = new StreamWriter(result);
        while (queue.Count > 0)
        {
            var current = queue.Dequeue();
            resultFile.WriteLine(current.ToString());
            var reader = fileReaders[current.FileName];
            if (!reader.EndOfStream)
            {
                var line = reader.ReadLine();
                var row = Row.FromString(line, current.FileName);
                queue.Enqueue(row, row);
            }
            else
            {
                reader.Close();
            }
        }
        _logger.LogInformation($"Finished merging files to {result}");
        return result;

        
    }
    
    private string GenerateRandomString(int length)
    {
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
        StringBuilder result = new StringBuilder(length);

        for (int i = 0; i < length; i++)
        {
            result.Append(chars[_random.Next(chars.Length)]);
        }

        return result.ToString();
    }
    
}