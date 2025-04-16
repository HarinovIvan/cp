using System;

    public class Config
    {
        public int PageSize { get; set; } = 20;
        public string DefaultDirectory { get; set; } = ".";
        public bool ShowHiddenFiles { get; set; } = false;
        public int HistorySize { get; set; } = 50;
    }

    class Program
    {
        private static Config _config;  //поля
        private static string _currentDirectory;
        private static List<string> _commandHistory = new List<string>();
        private static int _historyIndex = 0;

        static void Main(string[] args)
        {
            LoadConfig();
            _currentDirectory = _config.DefaultDirectory;
            
            Console.WriteLine("File Manager");
            Console.WriteLine($"Current directory: {_currentDirectory}");
            Console.WriteLine("Type 'help' for available commands");

            LoadState();
            MainLoop();
            SaveState();
        }

        static void LoadConfig()
        {
            try
            {
                string configFile = "config.json";
                if (File.Exists(configFile))
                {
                    string json = File.ReadAllText(configFile);
                    _config = JsonSerializer.Deserialize<Config>(json);
                }
                else
                {
                    _config = new Config();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading config: {ex.Message}");
                _config = new Config();
            }
        }

        static void MainLoop()
        {
            while (true)
            {
                Console.Write("> ");
                string input = Console.ReadLine();

                if (string.IsNullOrWhiteSpace(input))
                    continue;

                ProcessCommand(input);
            }
        }

        static void ProcessCommand(string command)
        {
            try 
            {
                if (!string.IsNullOrWhiteSpace(command))
                {
                    _commandHistory.Add(command);
                    if (_commandHistory.Count > _config.HistorySize)
                    {
                        _commandHistory.RemoveAt(0);
                    }
                    _historyIndex = _commandHistory.Count;
                }
                
                string[] parts = command.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                if (parts.Length == 0) return;

                switch (parts[0].ToLower())
                {
                    case "ls":
                        if (parts.Length > 1 && int.TryParse(parts[1], out int page))
                        {
                            ListDirectory(page);
                        }
                        else
                        {
                            ListDirectory();
                        }
                        break;
                {
                    case "ls":
                        ListDirectory();
                        break;
                    case "cd":
                        ChangeDirectory(parts.Length > 1 ? parts[1] : "");
                        break;
                    case "exit":
                        Environment.Exit(0);
                        break;
                    case "help":
                        ShowHelp();
                        break;
                    case "cp":
                        CopyFileOrDirectory(parts);
                        break;
                    case "rm":
                        DeleteFileOrDirectory(parts);
                        break;
                    case "info":
                        ShowFileInfo(parts);
                        break;
                    default:
                        Console.WriteLine($"Unknown command: {parts[0]}");
                        break;
                }}
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }

        static void ListDirectory(int page = 1)
        {
            try
            {
                var files = Directory.GetFiles(_currentDirectory);
                var dirs = Directory.GetDirectories(_currentDirectory);
                int totalItems = files.Length + dirs.Length;
                int totalPages = (int)Math.Ceiling(totalItems / (double)_config.PageSize);

                Console.WriteLine($"\nDirectory listing (Page {page} of {totalPages}):");
                Console.WriteLine("Directories:");
                
                // директории
                int startIndex = (page - 1) * _config.PageSize;
                int endIndex = Math.Min(startIndex + _config.PageSize, dirs.Length);
                
                for (int i = startIndex; i < endIndex; i++)
                {
                    Console.WriteLine($"  {Path.GetFileName(dirs[i])}");
                }

                // файлы
                Console.WriteLine("\nFiles:");
                startIndex = Math.Max(0, (page - 1) * _config.PageSize - dirs.Length);
                endIndex = Math.Min(startIndex + _config.PageSize, files.Length);
                
                for (int i = startIndex; i < endIndex; i++)
                {
                    var info = new FileInfo(files[i]);
                    Console.WriteLine($"  {Path.GetFileName(files[i])} ({info.Length} bytes)");
                }

                Console.WriteLine($"\nUse 'ls [page]' to view other pages (1-{totalPages})");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error listing directory: {ex.Message}");
                LogError(ex);
            }
        }

        static void ChangeDirectory(string path)
        {
            try
            {
                if (string.IsNullOrEmpty(path))
                {
                    Console.WriteLine("Current directory: " + _currentDirectory);
                    return;
                }

                string newPath = Path.Combine(_currentDirectory, path);
                if (Directory.Exists(newPath))
                {
                    _currentDirectory = Path.GetFullPath(newPath);
                    Console.WriteLine($"Changed to directory: {_currentDirectory}");
                }
                else
                {
                    Console.WriteLine($"Directory not found: {newPath}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error changing directory: {ex.Message}");
            }
        }

        static void ShowHelp()  //доступные команды
        {
            Console.WriteLine("\nДоступные команды:");
            Console.WriteLine("ls [page] - Список содержимого каталога (с разбивкой по страницам)");
            Console.WriteLine("cd [path] - Изменить каталог");
            Console.WriteLine("cp [source] [dest] - Скопировать файл/каталог");
            Console.WriteLine("rm [path] - удалить файл/каталог");
            Console.WriteLine("info [path] - Показывать информацию о файле/каталоге");
            Console.WriteLine("exit - Выйдите из программы");
            Console.WriteLine("help - Покажите эту справку");
            Console.WriteLine("\nИспользуйте клавиши со стрелками ↑/↓ для навигации по истории команд");
            Console.WriteLine($"Текущий размер страницы: {_config.PageSize} предметы\n");
        }

        static void LogError(Exception ex)  //функция которая будет обрабатывать ошибки
        {
            try
            {
                string errorsDir = Path.Combine(_currentDirectory, "errors");
                if (!Directory.Exists(errorsDir))
                {
                    Directory.CreateDirectory(errorsDir);
                }

                string errorFile = Path.Combine(errorsDir, $"error_{DateTime.Now:yyyyMMdd_HHmmss}.txt");
                File.WriteAllText(errorFile, $"{DateTime.Now}\n{ex}\n\nStack Trace:\n{ex.StackTrace}");
            }
            catch
            {
                
            }
        }

        static void SaveState()
        {
            try
            {
                var state = new
                {
                    CurrentDirectory = _currentDirectory,
                    CommandHistory = _commandHistory
                };
                string json = JsonSerializer.Serialize(state);
                File.WriteAllText("state.json", json);
            }
            catch
            {
                
            }
        }

        static void LoadState()
        {
            try
            {
                if (File.Exists("state.json"))
                {
                    string json = File.ReadAllText("state.json");
                    var state = JsonSerializer.Deserialize<Dictionary<string, object>>(json);
                    _currentDirectory = state["CurrentDirectory"].ToString();
                    
                    var history = JsonSerializer.Deserialize<List<string>>(state["CommandHistory"].ToString());
                    _commandHistory = history;
                }
            }
            catch
            {
               
            }
        }

        static void CopyFileOrDirectory(string[] parts)
        {
            if (parts.Length < 3)
            {
                Console.WriteLine("Usage: cp [source] [destination]");
                return;
            }

            try
            {
                string source = Path.Combine(_currentDirectory, parts[1]);
                string dest = Path.Combine(_currentDirectory, parts[2]);

                if (Directory.Exists(source))
                {
                    Directory.CreateDirectory(dest);
                    foreach (string dirPath in Directory.GetDirectories(source, "*", SearchOption.AllDirectories))
                    {
                        Directory.CreateDirectory(dirPath.Replace(source, dest));
                    }
                    foreach (string filePath in Directory.GetFiles(source, "*.*", SearchOption.AllDirectories))
                    {
                        File.Copy(filePath, filePath.Replace(source, dest), true);
                    }
                    Console.WriteLine($"Copied directory: {parts[1]} -> {parts[2]}");
                }
                else if (File.Exists(source))
                {
                    File.Copy(source, dest, true);
                    Console.WriteLine($"Copied file: {parts[1]} -> {parts[2]}");
                }
                else
                {
                    Console.WriteLine($"Source not found: {parts[1]}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error copying: {ex.Message}");
            }
        }

        static void DeleteFileOrDirectory(string[] parts)
        {
            if (parts.Length < 2)
            {
                Console.WriteLine("Usage: rm [path]");
                return;
            }

            try
            {
                string path = Path.Combine(_currentDirectory, parts[1]);
                
                if (Directory.Exists(path))
                {
                    Directory.Delete(path, true);
                    Console.WriteLine($"Deleted directory: {parts[1]}");
                }
                else if (File.Exists(path))
                {
                    File.Delete(path);
                    Console.WriteLine($"Deleted file: {parts[1]}");
                }
                else
                {
                    Console.WriteLine($"Path not found: {parts[1]}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error deleting: {ex.Message}");
            }
        }

        static void ShowFileInfo(string[] parts)
        {
            if (parts.Length < 2)
            {
                Console.WriteLine("Usage: info [path]");
                return;
            }

            try
            {
                string path = Path.Combine(_currentDirectory, parts[1]);
                
                if (Directory.Exists(path))
                {
                    var dirInfo = new DirectoryInfo(path);
                    Console.WriteLine($"Directory: {path}");
                    Console.WriteLine($"Created: {dirInfo.CreationTime}");
                    Console.WriteLine($"Last modified: {dirInfo.LastWriteTime}");
                    Console.WriteLine($"Attributes: {dirInfo.Attributes}");
                }
                else if (File.Exists(path))
                {
                    var fileInfo = new FileInfo(path);
                    Console.WriteLine($"File: {path}");
                    Console.WriteLine($"Size: {fileInfo.Length} bytes");
                    Console.WriteLine($"Created: {fileInfo.CreationTime}");
                    Console.WriteLine($"Last modified: {fileInfo.LastWriteTime}");
                    Console.WriteLine($"Attributes: {fileInfo.Attributes}");
                }
                else
                {
                    Console.WriteLine($"Path not found: {parts[1]}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error getting info: {ex.Message}");
            }
        }
    }

