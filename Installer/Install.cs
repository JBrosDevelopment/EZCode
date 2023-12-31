﻿using System.IO.Compression;
using System.Net;

namespace Installer
{
    public class Option
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public bool Checked { get; set; }

        public Option(string name, int id, bool check)
        {
            Id = id;
            Name = name;
            Checked = check;
        }
    }
    public static class Install
    {
        static bool done;
        public static string appdataDir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), "EZCode");
        public static string tempDirectory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Windows), "Temp", "EZCode");
        public static string filepath = "C:\\Program Files (x86)\\EZCode\\";
        public static string githubRepoUrl = "https://github.com/JBrosDevelopment/EZCode";
        static string _MainText;
        static int percentage;
        static string MainText
        {
            get => _MainText;
            set
            {
                _MainText = value;
            }
        }
        public static void DownloadMain()
        {
            CreateDirs();
            Option[] options = Options();
            try
            {
                string type = "";
                string realTag = $"{EZCode.EzCode.Version}";
                string releaseTag = type != "" ? $"v{realTag}_{type.ToLower()}" : $"v{realTag}";
                string releaseTitle = type != "" ? $"v{realTag}_{type}" : $"v{realTag}";

                foreach (var option in options)
                {
                    if (!option.Checked) continue;
                    switch (option.Id)
                    {
                        case 0: // Install main
                            installcore();
                            installer();
                            ezcodeplayer();
                            break;
                        case 1: // Install SLN
                            slnbuilder();
                            break;
                        case 2: // Install IDE
                            ez_ide();
                            break;
                    }
                }

                void installcore() // Install Source Start
                {
                    Working($"Installing Core From {githubRepoUrl}.git... This may take a second.");

                    Directory.CreateDirectory(tempDirectory);

                    string installFile = releaseTitle + ".zip";

                    string downloadUrl = $"{githubRepoUrl}/archive/refs/tags/{releaseTag}.zip";

                    WebInstaller(installFile, downloadUrl, tempDirectory, true);

                    string[] d = Directory.GetDirectories(Path.Combine(tempDirectory, $"EZCode-{(type != "" ? $"{realTag}_{type.ToLower()}" : realTag)}"));
                    for (int i = 0; i < d.Length; i++)
                    {
                        DirectoryInfo info = new DirectoryInfo(d[i]);
                        string t_decompress = Path.Combine(appdataDir, info.Name); 
                        if (info.Name.ToLower() == "ezcode" || info.Name.ToLower() == "ezcode-winforms" || info.Name.ToLower() == "packages")
                        {
                            if (Directory.Exists(t_decompress))
                                Directory.Delete(t_decompress, true);
                            Directory.Move(d[i], t_decompress);
                        }
                    }
                    Directory.Delete(tempDirectory, true);
                }

                void installer() // Installer
                {
                    Working($"\n\nInstalling Installer From {githubRepoUrl}.git... This may take a second.");

                    Directory.CreateDirectory(tempDirectory);

                    string installFile = $"Installer_v{realTag}.zip";

                    string downloadUrl = $"{githubRepoUrl}/releases/download/{releaseTag}/{installFile}";

                    string decompressDirectory = Path.Combine(filepath, $"Installer {releaseTitle}");

                    WebInstaller($"{installFile}.zip", downloadUrl, tempDirectory, true);

                    string[] d = Directory.GetFiles(Path.Combine(tempDirectory, $"Installer"));
                    Directory.CreateDirectory(decompressDirectory);
                    for (int i = 0; i < d.Length; i++)
                    {
                        FileInfo info = new FileInfo(d[i]);
                        string t_decompress = Path.Combine(decompressDirectory, info.Name);
                        File.Move(d[i], t_decompress, true);
                    }
                    Directory.Delete(tempDirectory, true);

                    Program.CreateShortcut("EZ IDE", Path.Combine(decompressDirectory, "EZ_IDE.exe"));
                    Program.CreateStartMenuShortcut("EZCode", "EZ IDE", Path.Combine(decompressDirectory, "EZ_IDE.exe"));
                }

                void ezcodeplayer() // Install EZCode Main
                {
                    Working($"\n\nInstalling Player From {githubRepoUrl}.git... This may take a second.");

                    Directory.CreateDirectory(tempDirectory);

                    string installFile = $"EZCode_Player_v{realTag}.zip";

                    string downloadUrl = $"{githubRepoUrl}/releases/download/{releaseTag}/{installFile}";

                    string decompressDirectory = Path.Combine(filepath, $"EZCode-Player {releaseTitle}");

                    WebInstaller($"{installFile}.zip", downloadUrl, tempDirectory, true);

                    string[] d = Directory.GetFiles(Path.Combine(tempDirectory, $"EZCode_Player"));
                    Directory.CreateDirectory(decompressDirectory);
                    for (int i = 0; i < d.Length; i++)
                    {
                        FileInfo info = new FileInfo(d[i]);
                        string t_decompress = Path.Combine(decompressDirectory, info.Name);
                        File.Move(d[i], t_decompress, true);
                    }
                    Directory.Delete(tempDirectory, true);

                    Program.CreateShortcut("EZCode Player", Path.Combine(decompressDirectory, "EZCodePlayer.exe"));
                    Program.CreateStartMenuShortcut("EZCode", "EZCode Player", Path.Combine(decompressDirectory, "EZCodePlayer.exe"));
                    Program.SetUpFile(Path.Combine(decompressDirectory, "EZCodePlayer.exe"), Path.Combine(appdataDir, "EZCode", "EZCode_Logo.ico"));
                }

                void slnbuilder() // Install Sln Builder
                {
                    Working($"\n\nInstalling SLN Builder From {githubRepoUrl}.git... This may take a second.");

                    Directory.CreateDirectory(tempDirectory);

                    string installFile = $"Sln_Builder_v{realTag}.zip";

                    string downloadUrl = $"{githubRepoUrl}/releases/download/{releaseTag}/{installFile}";

                    string decompressDirectory = Path.Combine(filepath, $"SLN_Builder {releaseTitle}");

                    WebInstaller($"{installFile}.zip", downloadUrl, tempDirectory, true);

                    string[] d = Directory.GetFiles(Path.Combine(tempDirectory, $"Sln_Builder"));
                    Directory.CreateDirectory(decompressDirectory);
                    for (int i = 0; i < d.Length; i++)
                    {
                        FileInfo info = new FileInfo(d[i]);
                        string t_decompress = Path.Combine(decompressDirectory, info.Name);
                        File.Move(d[i], t_decompress, true);
                    }
                    Directory.Delete(tempDirectory, true);

                    Program.CreateShortcut("SLN Builder", Path.Combine(decompressDirectory, "EZ_SLN_Builder.exe"));
                    Program.CreateStartMenuShortcut("EZCode", "SLN Builder", Path.Combine(decompressDirectory, "EZ_SLN_Builder.exe"));
                }

                void ez_ide() // Install IDE
                {
                    Working($"\n\nInstalling IDE From {githubRepoUrl}.git... This may take a second.");

                    Directory.CreateDirectory(tempDirectory);

                    string installFile = $"EZ_IDE_v{realTag}.zip";

                    string downloadUrl = $"{githubRepoUrl}/releases/download/{releaseTag}/{installFile}";

                    string decompressDirectory = Path.Combine(filepath, $"EZ_IDE {releaseTitle}");

                    WebInstaller($"{installFile}.zip", downloadUrl, tempDirectory, true);

                    string[] d = Directory.GetFiles(Path.Combine(tempDirectory, $"EZ_IDE"));
                    Directory.CreateDirectory(decompressDirectory);
                    for (int i = 0; i < d.Length; i++)
                    {
                        FileInfo info = new FileInfo(d[i]);
                        string t_decompress = Path.Combine(decompressDirectory, info.Name);
                        File.Move(d[i], t_decompress, true);
                    }
                    Directory.Delete(tempDirectory, true);

                    Program.CreateShortcut("EZ IDE", Path.Combine(decompressDirectory, "EZ_IDE.exe"));
                    Program.CreateStartMenuShortcut("EZCode", "EZ IDE", Path.Combine(decompressDirectory, "EZ_IDE.exe"));
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error during installation: {ex.Message}");
            }
        }
        static Option[] Options()
        {
            Console.ResetColor();
            string txt = "Choose what to Install (Press space to check or uncheck an option, press enter to submit)";
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.WriteLine(txt);
            Console.ResetColor();

            List<Option> options = new List<Option>()
            {
                new Option("EZCode Core and Player", 0, true),
                new Option("EZ_IDE (Development platform for EZCode)", 2, true),
                new Option("EZCode SLN (Microsoft Visual Studio Project) Builder", 1, false),
            };

            int index = 0;

            ConsoleKey key;

            do
            {
                Console.Clear();

                Console.ForegroundColor = ConsoleColor.Blue;
                Console.WriteLine(txt);
                Console.ResetColor();

                for (int i = 0; i < options.Count; i++)
                {
                    if (i == index)
                    {
                        Console.ForegroundColor = ConsoleColor.Cyan;
                    }

                    Console.WriteLine($"{(options[i].Checked ? "[X]" : "[ ]")} {options[i].Name}");

                    Console.ResetColor();
                }

                key = Console.ReadKey().Key;

                switch (key)
                {
                    case ConsoleKey.UpArrow:
                        index = index == 0 ? options.Count - 1 : index - 1;
                        break;
                    case ConsoleKey.DownArrow:
                        index = index == options.Count - 1 ? 0 : index + 1;
                        break;
                    case ConsoleKey.Spacebar:
                        bool o = options[index].Checked;
                        for (int i = 0; i < options.Count; i++)
                        {
                            options[i].Checked = options[i].Id == 0 ? true : options[i].Checked;
                        }
                        options[index].Checked = options[index].Id == 0 ? true : !o;
                        break;
                    case ConsoleKey.Enter:
                        break;
                    default:
                        Console.ForegroundColor = ConsoleColor.DarkRed;
                        Console.WriteLine(" is invalid input. Please use the arrow keys, spacebar, or enter.");
                        Console.ResetColor();
                        Console.ReadKey();
                        break;
                }

            } while (key != ConsoleKey.Enter);

            return options.ToArray();
        }

        public static void CreateDirs()
        {
            Directory.CreateDirectory(tempDirectory);
            Directory.CreateDirectory(filepath);
            Directory.CreateDirectory(appdataDir);
        }

        static void WebInstaller(string installFile, string downloadUrl, string tempDirectory, bool Decompress = false, string words = "\nInstallation completed successfully.")
        {
            try
            {
                using (WebClient webClient = new WebClient())
                {
                    string downloadFilePath = Path.Combine(tempDirectory, installFile);

                    webClient.DownloadProgressChanged += (object sender, DownloadProgressChangedEventArgs e) => percentage = e.ProgressPercentage;
                    webClient.DownloadFile(downloadUrl, downloadFilePath);

                    if (Decompress)
                    {
                        ZipFile.ExtractToDirectory(downloadFilePath, tempDirectory, true);

                        File.Delete(downloadFilePath);
                    }

                    done = true;
                }
                Console.Clear();
                Console.Write(MainText);
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine($"\n{words}");
                Console.ResetColor();
            }
            catch (Exception ex)
            {
                done = true;
                Console.ForegroundColor = ConsoleColor.DarkRed;
                Console.WriteLine("\n\n" + ex.Message);
            }
        }

        static async Task Working(string text)
        {
            MainText += text;
            done = false;
            int tick = 0;
            while (!done)
            {
                Console.Clear();
                Console.Write(MainText);
                Console.ForegroundColor = ConsoleColor.Cyan;
                switch (tick)
                {
                    case 0:
                        Console.Write("/");
                        tick++;
                        break;
                    case 1:
                        Console.Write("-");
                        tick++;
                        break;
                    case 2:
                        Console.Write(@"\");
                        tick = 0;
                        break;
                }
                /*Console.Write($" {percentage}% ");
                Console.ForegroundColor = ConsoleColor.Red;
                Console.Write("|");
                Console.ForegroundColor = ConsoleColor.Blue;
                string per = "";
                int dashes = percentage / 5;
                for (int i = 0; i < 20; i++)
                {
                    per += i < dashes ? "-" : " ";
                }
                Console.Write(per);
                Console.ForegroundColor = ConsoleColor.Red;
                Console.Write("|");*/
                Console.ResetColor();
                await Task.Delay(250);
            }
        }
    }
}
