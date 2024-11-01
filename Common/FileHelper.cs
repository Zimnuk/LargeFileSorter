﻿using System.Diagnostics;
using System.Text;
using Common;
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

    public string GenerateTestCase(long rows, DetailedProgressBar progressBar)
    {
        var filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, $"TestCase.txt");
        _logger.LogInformation($"Generating testcase file {filePath}");
        var j = 0;
        var percentValue = rows / 100;
        using (StreamWriter outfile = new StreamWriter(filePath))
        {
            for (int i = 0; i < rows; i++)
            {
                var number = _random.Next(int.MaxValue);
                var stingLength = _random.Next(5, 50);
                var row = $"{number}.{GenerateRandomString(stingLength)}";
                outfile.WriteLine(row);
                if (j++ == percentValue)
                {
                    progressBar.Update("In progress");
                    j = 0;
                }
            }
        }
        
        _logger.LogInformation($"testcase file {filePath} generated successfully");
        if(progressBar.ProgressBar.Value < progressBar.ProgressBar.Maximum)
            progressBar.Update("In progress");
        progressBar.Dispose();
        return filePath;
    } 
    
    public DirectoryInfo? DivideFileToFolder(string path)
    {
        Guard.FileGuard(path);
        
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
                while (i < Constants.TempFileRows && !inFile.EndOfStream)
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

    public void SortFiles(DirectoryInfo dir, DetailedProgressBar detailedProgressBar)
    {
        Guard.DirectoryGuard(dir.FullName);
        
        var files = dir.GetFiles();
        _logger.LogInformation("Sorting files");
        foreach (var file in files)
        {
            SortFile(file);
            detailedProgressBar.Update(file.Name);
        }
        detailedProgressBar.Dispose();
    }

    private void SortFile(FileInfo file)
    {
        using var inFile = new StreamReader(file.FullName);
        var rows = new Row[Constants.TempFileRows];
        var i = 0;
        while (!inFile.EndOfStream)
        {
            var split = inFile.ReadLine().Split('.');
            if (!int.TryParse(split[0], out int number))
            {
                _logger.LogError($"File {file.FullName} is invalid");
            }
            rows[i++] = new Row(number, split[1]);
        }
        inFile.Close();
        Array.Sort(rows);
        using (StreamWriter outfile = new StreamWriter(file.FullName))
        {
            foreach (var row in rows)
            {
                outfile.WriteLine(row.ToString());
            }
        }
    }
    
    public string MergeFiles(DirectoryInfo dir, DetailedProgressBar detailedProgressBar)
    {
        Guard.DirectoryGuard(dir.FullName);
        
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
        var i = 0;
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

            if (++i == Constants.TempFileRows)
            {
                detailedProgressBar.Update("In progress");
                i = 0;
            }
            
        }
        if(detailedProgressBar.ProgressBar.Value < detailedProgressBar.ProgressBar.Maximum)
            detailedProgressBar.Update("In progress");
        
        detailedProgressBar.Dispose();
        _logger.LogInformation($"Finished merging files to {result}");
        dir.Delete(true);
        return result;

        
    }
    
    private string GenerateRandomString(int length)
    {
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789 ";
        StringBuilder result = new StringBuilder(length);

        for (int i = 0; i < length; i++)
        {
            result.Append(chars[_random.Next(chars.Length)]);
        }

        return result.ToString();
    }
    
}