using System;
using System.IO;

class Program
{
    static void Main(string[] args)
    {
        //1
        string currentDirectory = Directory.GetCurrentDirectory();
        ListFilesAndDirectories(currentDirectory);

        //2
        string sourceDirectory = "";
        string targetDirectory = "";
        CopyDirectory(sourceDirectory, targetDirectory);

        //3
        string filePath = "";
        string directoryPath = "";
        DeleteFile(filePath);
        DeleteDirectory(directoryPath);

        //4

        string filePath = "";
        string directoryPath = "";
        GetFileInfo(filePath);
        GetDirectoryInfo(directoryPath);

        //5

        string directoryPath = "";
        int pageSize = 10; 
        FileInfo[] files = new DirectoryInfo(directoryPath).GetFiles();
        DirectoryInfo[] directories = new DirectoryInfo(directoryPath).GetDirectories();

        var items = new object[files.Length + directories.Length];
        Array.Copy(files, items, files.Length);
        Array.Copy(directories, 0, items, files.Length, directories.Length);

        int totalPages = (items.Length + pageSize - 1) / pageSize;

        for (int page = 0; page < totalPages; page++)
        {
            Console.WriteLine($"Страница {page + 1} из {totalPages}");
            for (int i = page * pageSize; i < Math.Min((page + 1) * pageSize, items.Length); i++)
            {
                if (items[i] is FileInfo file)
                {
                    Console.WriteLine($"{file.Name} (файл)");
                }
                else if (items[i] is DirectoryInfo dir)
                {
                    Console.WriteLine($"{dir.Name} (каталог)");
                }
            }
            Console.ReadKey();
        }

        //6
        string directoryPath = "";
        int pageSize = int.Parse(ConfigurationManager.AppSettings["ItemsPerPage"]);

        FileInfo[] files = new DirectoryInfo(directoryPath).GetFiles();
        DirectoryInfo[] directories = new DirectoryInfo(directoryPath).GetDirectories();

        var items = new object[files.Length + directories.Length];
        Array.Copy(files, items, files.Length);
        Array.Copy(directories, 0, items, files.Length, directories.Length);

        int totalPages = (items.Length + pageSize - 1) / pageSize;

        for (int page = 0; page < totalPages; page++)
        {
            Console.WriteLine($"Страница {page + 1} из {totalPages}");
            for (int i = page * pageSize; i < Math.Min((page + 1) * pageSize, items.Length); i++)
            {
                if (items[i] is FileInfo file)
                {
                    Console.WriteLine($"{file.Name} (файл)");
                }
                else if (items[i] is DirectoryInfo dir)
                {
                    Console.WriteLine($"{dir.Name} (каталог)");
                }
            }
            Console.ReadKey();
        }
    }

    //1
    static void ListFilesAndDirectories(string directoryPath)
    {
        try
        {
            string[] directories = Directory.GetDirectories(directoryPath);
            foreach (string direct in directories)
            {
                Console.WriteLine($"Directory: {direct}");
            }

            string[] files = Directory.GetFiles(directoryPath);
            foreach (string file in files)
            {
                Console.WriteLine($"File: {file}");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
        }
    }

    //2

      static void CopyDirectory(string sourceDir, string targetDir)
    {
        if (!Directory.Exists(targetDir))
        {
            Directory.CreateDirectory(targetDir);
        }

        foreach (string file in Directory.GetFiles(sourceDir))
        {
            string targetFile = Path.Combine(targetDir, Path.GetFileName(file));
            File.Copy(file, targetFile);
        }

        foreach (string dir in Directory.GetDirectories(sourceDir))
        {
            string targetSubDir = Path.Combine(targetDir, Path.GetFileName(dir));
            CopyDirectory(dir, targetSubDir);
        }
    }

    //3
    static void DeleteFile(string path)
    {
        if (File.Exists(path))
        {
            File.Delete(path);
            Console.WriteLine($"Файл {path} успешно удалён.");
        }
        else
        {
            Console.WriteLine($"Файл {path} не найден.");
        }
    }

    static void DeleteDirectory(string path)
    {
        if (Directory.Exists(path))
        {
            Directory.Delete(path, true);
            Console.WriteLine($"Каталог {path} успешно удалён.");
        }
        else
        {
            Console.WriteLine($"Каталог {path} не найден.");
        }
    }

    //4
    static void GetFileInfo(string path)
    {
        if (File.Exists(path))
        {
            FileInfo fileInfo = new FileInfo(path);
            Console.WriteLine($"Имя файла: {fileInfo.Name}");
            Console.WriteLine($"Размер файла: {fileInfo.Length} байт");
            Console.WriteLine($"Атрибуты файла: {fileInfo.Attributes}");
        }
        else
        {
            Console.WriteLine($"Файл {path} не найден.");
        }
    }

    static void GetDirectoryInfo(string path)
    {
        if (Directory.Exists(path))
        {
            DirectoryInfo directoryInfo = new DirectoryInfo(path);
            Console.WriteLine($"Имя каталога: {directoryInfo.Name}");
            Console.WriteLine($"Атрибуты каталога: {directoryInfo.Attributes}");
            long directorySize = 0;
            foreach (FileInfo file in directoryInfo.GetFiles())
            {
                directorySize += file.Length;
            }
            Console.WriteLine($"Размер каталога: {directorySize} байт");
        }
        else
        {
            Console.WriteLine($"Каталог {path} не найден.");
        }
    }
}
