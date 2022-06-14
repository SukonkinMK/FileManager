using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FileManager.Utils;

namespace FileManager
{
    internal class Program
    {
        const int WINDOW_WIDTH = 120;
        const int WINDOW_HEIGHT = 40;
        private static string currentDir = Directory.GetCurrentDirectory();
        static void Main(string[] args)
        {
            Console.BackgroundColor = ConsoleColor.DarkBlue;
            Console.Title = "FileManager";
            Console.SetBufferSize(WINDOW_WIDTH, WINDOW_HEIGHT);
            Console.SetWindowSize(WINDOW_WIDTH, WINDOW_HEIGHT);
            DrawWindow(0,0,WINDOW_WIDTH, 28);
            DrawWindow(0, 28, WINDOW_WIDTH, 8);
            UpdateConsole();
            Console.ReadKey(true);
        }

        /// <summary>
        /// Отрисовка окна
        /// </summary>
        /// <param name="x">координата Х левого верхнего угла</param>
        /// <param name="y">координата Y левого верхнего угла</param>
        /// <param name="width">ширина окна</param>
        /// <param name="height">высота окна</param>
        static void DrawWindow(int x, int y, int width, int height)
        {
            //header
            Console.SetCursorPosition(x, y);
            Console.Write("\u2554");
            for(int i = 0; i < width - 2; i++)
            {
                Console.Write("\u2550");
            }
            Console.Write("\u2557");
            //body
            Console.SetCursorPosition(x, y + 1);
            for(int j = 0; j < height - 2; j++)
            {
                Console.Write("\u2551");
                for (int i = 0; i < width - 2; i++)
                {
                    Console.Write(" ");
                }
                Console.Write("\u2551");
            }
            //footer
            Console.SetCursorPosition(x, y + height - 1);
            Console.Write("\u255A");
            for (int i = 0; i < width - 2; i++)
            {
                Console.Write("\u2550");
            }
            Console.Write("\u255D");
            Console.SetCursorPosition(x, y);
        }

        /// <summary>
        /// Оюновление ввода с консоли
        /// </summary>
        static void UpdateConsole()
        {
            DrawConsole(GetShortPath(currentDir), 0, 36, WINDOW_WIDTH, 3);
            ProcessEnterCommand(WINDOW_WIDTH);
        }

        /// <summary>
        /// обработка команд с консоли
        /// </summary>
        /// <param name="width">Длинна строки ввода</param>
        static void ProcessEnterCommand(int width)
        {
            (int left, int top) = GetCursorPosition();
            StringBuilder command = new StringBuilder();
            ConsoleKeyInfo keyInfo;
            char key;
            do
            {
                keyInfo = Console.ReadKey();
                key = keyInfo.KeyChar;
                if(keyInfo.Key != ConsoleKey.Enter && keyInfo.Key != ConsoleKey.Backspace && keyInfo.Key != ConsoleKey.UpArrow)
                {
                    command.Append(key);
                }
                (int currentLeft, int currentTop) = GetCursorPosition();
                if(currentLeft == width - 2)
                {
                    Console.SetCursorPosition(currentLeft - 1, currentTop);
                    Console.Write(" ");
                    Console.SetCursorPosition(currentLeft - 1, currentTop);
                }
                if (keyInfo.Key == ConsoleKey.Backspace)
                {
                    if(command.Length > 0)
                    {
                        command.Remove(command.Length - 1, 1);
                    }
                    if (currentLeft >= left)
                    {
                        Console.SetCursorPosition(currentLeft, top);
                        Console.Write(" ");
                        Console.SetCursorPosition(currentLeft, top);

                    }
                    else
                    {
                        command.Clear(); 
                        Console.SetCursorPosition(left, top);
                    }
                }
            }
            while(keyInfo.Key != ConsoleKey.Enter);
            ParseCommandString(command.ToString());
        }

        static void ParseCommandString(string command)
        {
            string[] commandParams = command.ToLower().Split(' ');
            if(commandParams.Length > 0)
            {
                switch (commandParams[0])
                {
                    case "cd":
                        if (commandParams.Length > 1)
                            if (Directory.Exists(commandParams[1]))
                            {
                                currentDir = commandParams[1];
                            }
                        break;
                    case "ls":
                        if (commandParams.Length > 1 && Directory.Exists(commandParams[1]))
                        {
                            if (commandParams.Length > 3 && commandParams[2] == "-p" && int.TryParse(commandParams[3], out int n))
                            {
                                DrawTree(new DirectoryInfo(commandParams[1]), n);
                            }
                            else
                            {
                                DrawTree(new DirectoryInfo(commandParams[1]), 1);
                            }
                        }
                        break;
                    case "cp":
                        if(commandParams.Length > 1)
                        {
                            if(commandParams.Length > 2 && Directory.Exists(commandParams[1]))
                            {
                                CopyDirectory(commandParams[1], commandParams[2], true);
                            }
                            else if (commandParams.Length > 2 && File.Exists(commandParams[1]))
                            {
                                File.Copy(commandParams[1], commandParams[2]);
                            }
                        }
                        break;
                    case "rm":
                        if (commandParams.Length > 1)
                        {
                            if (Directory.Exists(commandParams[1]))
                            {
                                Directory.Delete(commandParams[1], true);
                            }
                            else if (File.Exists(commandParams[1]))
                            {
                                File.Delete(commandParams[1]);
                            }
                        }
                        break;
                    case "file":
                        if (commandParams.Length > 1)
                        {
                            if (File.Exists(commandParams[1]))
                            {
                                FileInfo file = new FileInfo(commandParams[1]);
                                UpdateInfoWindow(file);
                            }
                        }
                        break;
                    case "dir":
                        if (commandParams.Length > 1)
                        {
                            if (Directory.Exists(commandParams[1]))
                            {
                                DirectoryInfo dir = new DirectoryInfo(commandParams[1]);
                                UpdateInfoWindow(dir);
                            }
                        }
                        break;
                }
            }
            UpdateConsole();
        }

        /// <summary>
        /// Вывод информации о файле на экран
        /// </summary>
        /// <param name="file">Целевой файл</param>
        static void UpdateInfoWindow(FileInfo file)
        {
            DrawWindow(0, 28, WINDOW_WIDTH, 8);
            Console.SetCursorPosition(1, 29);
            int top = Console.CursorTop;
            Console.WriteLine($"Тип файла: {file.Extension}");
            Console.SetCursorPosition(1, top + 1);
            Console.WriteLine($"Размер файла: {file.Length} bytes");
            Console.SetCursorPosition(1, top + 2);
            Console.WriteLine($"Расположение: {file.Directory}");
            Console.SetCursorPosition(1, top + 3);
            Console.WriteLine($"Создан: {file.CreationTime.ToString("dd MMMM yyyy г., HH:mm:ss")}");
            Console.SetCursorPosition(1, top + 4);
            Console.WriteLine($"Изменен: {file.LastWriteTime.ToString("dd MMMM yyyy г., HH:mm:ss")}");
            Console.SetCursorPosition(1, top + 5);
            Console.WriteLine($"Доступен только для чтения: {file.IsReadOnly}");
        }

        /// <summary>
        /// Вывод информации о директории
        /// </summary>
        /// <param name="dir">Целевая директория</param>
        static void UpdateInfoWindow(DirectoryInfo dir)
        {
            DrawWindow(0, 28, WINDOW_WIDTH, 8);
            Console.SetCursorPosition(1, 29);
            int top = Console.CursorTop;
            Console.WriteLine($"Тип: папка с файлами");
            Console.SetCursorPosition(1, top + 1);
            Console.WriteLine($"Размер папки: {DirSize(dir)} bytes");
            Console.SetCursorPosition(1, top + 2);
            Console.WriteLine($"Расположена на диске: {dir.Root}");
            Console.SetCursorPosition(1, top + 3);
            Console.WriteLine($"Созданa: {dir.CreationTime.ToString("dd MMMM yyyy г., HH:mm:ss")}");
            Console.SetCursorPosition(1, top + 4);
            Console.WriteLine($"Измененa: {dir.LastWriteTime.ToString("dd MMMM yyyy г., HH:mm:ss")}");
            Console.SetCursorPosition(1, top + 5);
            Console.WriteLine($"Содержит: {SubDirsQuantuty(dir)} папок, {SubFilesQuantuty(dir)} файлов");
        }

        /// <summary>
        /// Подсчет общего числа вложенных папок
        /// </summary>
        /// <param name="d">Целевой каталог</param>
        /// <returns>Число вложенных в целевой каталог папок</returns>
        static int SubDirsQuantuty(DirectoryInfo d)
        {
            int quantity = 0;
            DirectoryInfo[] dirs = d.GetDirectories();
            quantity += dirs.Length;
            foreach(DirectoryInfo dir in dirs)
            {
                quantity += SubDirsQuantuty(dir);
            }
            return quantity;
        }

        /// <summary>
        /// Подсчет общего числа вложенных файлов
        /// </summary>
        /// <param name="d">Целевой каталог</param>
        /// <returns>Число вложенных в целевой каталог файлов</returns>
        static int SubFilesQuantuty(DirectoryInfo d)
        {
            int quantity = 0;
            FileInfo[] files = d.GetFiles();
            quantity += files.Length;
            DirectoryInfo[] dirs = d.GetDirectories();
            foreach (DirectoryInfo dir in dirs)
            {
                quantity += SubFilesQuantuty(dir);
            }
            return quantity;
        }

        /// <summary>
        /// Подсчет размера каталога на диске
        /// </summary>
        /// <param name="d">Целевой каталог</param>
        /// <returns>Размер каталога на диске в байтах</returns>
        static long DirSize(DirectoryInfo d)
        {
            long size = 0;
            FileInfo[] files = d.GetFiles();
            foreach(FileInfo file in files)
            {
                size += file.Length;
            }
            DirectoryInfo[] dirs = d.GetDirectories();
            foreach (DirectoryInfo dir in dirs)
            {
                size += DirSize(dir);
            }
                return size;
        }

        /// <summary>
        /// Копирование каталога
        /// </summary>
        /// <param name="sourseDir">Копируемый каталог</param>
        /// <param name="targetDir">Скопированый каталог</param>
        /// <param name="recursive">скопировать все вложенные каталоги</param>
        static void CopyDirectory(string sourseDir, string targetDir, bool recursive)
        {
            DirectoryInfo dir = new DirectoryInfo(sourseDir);
            DirectoryInfo[] dirs = dir.GetDirectories();
            Directory.CreateDirectory(targetDir);
            foreach (FileInfo file in dir.GetFiles())
            {
                string targetFilePath = Path.Combine(targetDir, file.Name);
                file.CopyTo(targetFilePath);
            }
            if (recursive)
            {
                foreach (DirectoryInfo subDir in dirs)
                {
                    string newTargetDir = Path.Combine(targetDir, subDir.Name);
                    CopyDirectory(subDir.FullName, newTargetDir, true);
                }
            }
        }

        /// <summary>
        /// Определение текущей позиции курсора
        /// </summary>
        /// <returns>Кортеж координат курсора left и top</returns>
        static (int left, int top) GetCursorPosition()
        {
            return (Console.CursorLeft, Console.CursorTop);
        }

        /// <summary>
        /// Получение сокращенного пути к текущей директории
        /// </summary>
        /// <param name="path">полный путь к текущей директории</param>
        /// <returns>сокращенный путь к текущей директории</returns>
        static string GetShortPath(string path)
        {
            StringBuilder shortPathName = new StringBuilder((int)API.MAX_PATH);
            API.GetShortPathName(path, shortPathName, API.MAX_PATH);
            return shortPathName.ToString();
        }

        /// <summary>
        /// Отрисовка консоли
        /// </summary>
        /// <param name="dir">Имя текущей директории</param>
        /// <param name="x">Координата Х верхнего левого угла окна консоли</param>
        /// <param name="y">Координата Y верхнего левого угла окна консоли</param>
        /// <param name="width">Ширина окна консоли</param>
        /// <param name="height">Высота окна консоли</param>
        static void DrawConsole(string dir, int x, int y, int width, int height)
        {
            DrawWindow(x, y, width, height);
            Console.SetCursorPosition(x + 1, y + height / 2);
            Console.Write($"{dir}>");
        }

        /// <summary>
        /// Отрисовка дерева каталога
        /// </summary>
        /// <param name="dir">Директория</param>
        /// <param name="page">Страница</param>
        static void DrawTree(DirectoryInfo dir, int page)
        {
            StringBuilder tree = new StringBuilder();
            GetTree(tree, dir, "", true);
            DrawWindow(0, 0, WINDOW_WIDTH, 28);
            (int currentLeft, int currentTop) = GetCursorPosition();
            int pageLines = 26;
            string[] lines = tree.ToString().Split('\n');
            int pageTotal = (lines.Length + pageLines - 1) / pageLines;
            if (page > pageTotal)
            {
                page = pageTotal;
            }
            for (int i = (page - 1) * pageLines, counter = 0; i < page*pageLines; i++, counter++)
            {
                if(lines.Length - 1 > i)
                {
                    if(currentLeft + 1 + lines[i].Length < WINDOW_WIDTH)
                    {
                        Console.SetCursorPosition(currentLeft + 1, currentTop + 1 + counter);
                    }
                    else
                    {
                        Console.SetCursorPosition(currentLeft + 1 - (WINDOW_WIDTH - lines[i].Length - currentLeft - 1), currentTop + 1 + counter);
                    }
                    Console.WriteLine(lines[i]);
                }
            }
            //отрисовка footera
            string footer = $"╣ {page} of {pageTotal} ╠";
            Console.SetCursorPosition(WINDOW_WIDTH / 2 - footer.Length / 2, 27);
            Console.Write(footer);  
        }

        /// <summary>
        /// Создание строки с деревом каталогов
        /// </summary>
        /// <param name="tree">Строка для хранения дерева</param>
        /// <param name="dir">директрория, для которой строится дерево</param>
        /// <param name="indent">отступ</param>
        /// <param name="lastDirectory">является ли директория последней</param>
        static void GetTree(StringBuilder tree, DirectoryInfo dir, string indent, bool lastDirectory)
        {
            tree.Append(indent);
            if (lastDirectory)
            {
                tree.Append("└─");
                indent += "  ";
            }
            else
            {
                tree.Append("├─");
                indent += "│ ";
            }
            tree.Append($"{dir.Name}\n");
            FileInfo[] files = dir.GetFiles();
            for(int i = 0; i < files.Length; i++)
            {
                if (i == files.Length - 1)
                {
                    tree.Append($"{indent}└─{files[i].Name}\n");
                }
                else
                {
                    tree.Append($"{indent}├─{files[i].Name}\n");
                }
            }
            DirectoryInfo[] subDirs = dir.GetDirectories();
            for (int i = 0; i < subDirs.Length; i++)
            {
                GetTree(tree, subDirs[i], indent, i == subDirs.Length - 1);
            }
        }
    }
}
