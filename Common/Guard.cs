namespace LargeFileSorter;

public static class Guard
{
    public static void FileGuard(string path)
    {
        if (!File.Exists(path))
        {
            throw new FileLoadException($"File not found: {path}");
        }
    }
    
    public static void DirectoryGuard(string path)
    {
        if (!Directory.Exists(path))
        {
            throw new DirectoryNotFoundException($"Directory not found: {path}");
        }
    }
}