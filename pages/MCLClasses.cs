using Avalonia.Media.Imaging;
using Avalonia.Platform;
using Avalonia;
using Microsoft.VisualBasic;
using MinecraftLaunch.Modules.Models.Auth;
using MinecraftLaunch.Modules.Models.Launch;
using MinecraftLaunch.Modules.Utils;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.Versioning;
using System.Text.RegularExpressions;

namespace mclPlus.pages
{
    internal class MCLClasses
    {
        public class showAccount : Account
        {
            public showAccount(Account account)
            {
                switch (account.Type)
                {
                    case MinecraftLaunch.Modules.Enum.AccountType.Offline:
                        showName = $"{account.Name}-离线账号";
                        break;
                    case MinecraftLaunch.Modules.Enum.AccountType.Microsoft:
                        showName = $"{account.Name}-微软账号";
                        break;
                    case MinecraftLaunch.Modules.Enum.AccountType.Yggdrasil:
                        showName = $"{account.Name}-外置登录账号";
                        break;
                }
            }
            public string showName { get; set; }
        }
        /// <summary>
        /// JavaToolkit的重制版，仅保留了FindJavas和相关函数
        /// </summary>
        public sealed class JavaToolkit_FindJavasOnly
        {
            [SupportedOSPlatform("windows")]
            public static JavaInfo GetJavaInfo(string javaDirectorypath)
            {
                try
                {
                    int? ires = null;
                    string tempinfo = null;
                    string pattern = "java version \"\\s*(?<version>\\S+)\\s*\"";
                    ProcessStartInfo Info = new ProcessStartInfo
                    {
                        Arguments = "-version",
                        FileName = Path.Combine(javaDirectorypath, "java.exe"),
                        UseShellExecute = false,
                        CreateNoWindow = true,
                        RedirectStandardError = true,
                        RedirectStandardOutput = true
                    };
                    using Process Program = new Process
                    {
                        StartInfo = Info
                    };
                    Program.Start();
                    Program.WaitForExit(8000);
                    StreamReader res = Program.StandardError;
                    bool end = false;
                    while (res.Peek() != -1)
                    {
                        string temp = res.ReadLine();
                        if (temp.Contains("java version"))
                        {
                            tempinfo = new Regex(pattern).Match(temp).Groups["version"].Value;
                        }
                        else if (temp.Contains("openjdk version"))
                        {
                            pattern = pattern.Replace("java", "openjdk");
                            tempinfo = new Regex(pattern).Match(temp).Groups["version"].Value;
                        }
                        else if (temp.Contains("64-Bit"))
                        {
                            end = true;
                        }
                    }
                    string[] sres = tempinfo.Split(".");
                    if (sres.Length != 0)
                    {
                        ires = ((int.Parse(sres[0]) == 1) ? new int?(int.Parse(sres[1])) : new int?(int.Parse(sres[0])));
                    }
                    return new JavaInfo
                    {
                        Is64Bit = end,
                        JavaDirectoryPath = (javaDirectorypath.EndsWith('\\') ? javaDirectorypath : (javaDirectorypath + "\\")),
                        JavaSlugVersion = Convert.ToInt32(ires),
                        JavaVersion = tempinfo,
                        JavaPath = Path.Combine(javaDirectorypath, "javaw.exe")
                    };
                }
                catch (Exception)
                {
                    return null;
                }
            }
            [SupportedOSPlatform("windows")]
            public static IEnumerable<JavaInfo> GetJavas()
            {
                try
                {
                    string? environmentVariable = Environment.GetEnvironmentVariable("Path");
                    List<string> JavaPreList = new List<string>();
                    string[] array = Strings.Split(environmentVariable.Replace("\\\\", "\\").Replace("/", "\\"), ";");
                    foreach (string obj in array)
                    {
                        string pie = obj.Trim(" \"".ToCharArray());
                        if (!obj.EndsWith("\\"))
                        {
                            pie += "\\";
                        }
                        if (System.IO.File.Exists(obj + "javaw.exe"))
                        {
                            JavaPreList.Add(pie);
                        }
                    }
                    DriveInfo[] drives = DriveInfo.GetDrives();
                    for (int j = 0; j < drives.Length; j++)
                    {
                        JavaSearchFolder(new DirectoryInfo(drives[j].Name), ref JavaPreList, Source: false);
                    }
                    JavaSearchFolder(new DirectoryInfo(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)), ref JavaPreList, Source: false);
                    JavaSearchFolder(new DirectoryInfo(AppDomain.CurrentDomain.SetupInformation.ApplicationBase), ref JavaPreList, Source: false, IsFullSearch: true);
                    List<string> JavaWithoutReparse = new List<string>();
                    foreach (string Pair2 in JavaPreList)
                    {
                        FileSystemInfo Info = new FileInfo(Pair2.Replace("\\\\", "\\").Replace("/", "\\") + "javaw.exe");
                        do
                        {
                            if (!Info.Attributes.HasFlag(FileAttributes.ReparsePoint))
                            {
                                Info = ((Info is FileInfo) ? ((FileInfo)Info).Directory : ((DirectoryInfo)Info).Parent);
                            }
                        }
                        while (Info != null);
                        JavaWithoutReparse.Add(Pair2);
                    }
                    if (JavaWithoutReparse.Count > 0)
                    {
                        JavaPreList = JavaWithoutReparse;
                    }
                    List<string> JavaWithoutInherit = new List<string>();
                    foreach (string Pair in JavaPreList)
                    {
                        if (!Pair.Contains("javapath_target_"))
                        {
                            JavaWithoutInherit.Add(Pair);
                        }
                    }
                    if (JavaWithoutInherit.Count > 0)
                    {
                        JavaPreList = JavaWithoutInherit;
                    }
                    JavaPreList.Sort((string x, string s) => x.CompareTo(s));
                    foreach (string i in JavaPreList)
                    {
                        JavaInfo res = GetJavaInfo(i);
                        yield return new JavaInfo
                        {
                            Is64Bit = res.Is64Bit,
                            JavaDirectoryPath = i,
                            JavaSlugVersion = res.JavaSlugVersion,
                            JavaVersion = res.JavaVersion,
                            JavaPath = Path.Combine(i, "javaw.exe")
                        };
                    }
                }
                finally
                {
                    GC.Collect();
                }
            }

            private static void JavaSearchFolder(DirectoryInfo OriginalPath, ref List<string> Results, bool Source, bool IsFullSearch = false)
            {
                try
                {
                    if (!OriginalPath.Exists)
                    {
                        return;
                    }
                    string Path = OriginalPath.FullName.Replace("\\\\", "\\");
                    if (!Path.EndsWith("\\"))
                    {
                        Path += "\\";
                    }
                    if (System.IO.File.Exists(Path + "javaw.exe"))
                    {
                        Results.Add(Path);
                    }
                    foreach (DirectoryInfo FolderInfo in OriginalPath.EnumerateDirectories())
                    {
                        if (!FolderInfo.Attributes.HasFlag(FileAttributes.ReparsePoint))
                        {
                            string SearchEntry = GetFolderNameFromPath(FolderInfo.Name).ToLower();
                            if (IsFullSearch || FolderInfo.Parent.Name.ToLower() == "users" || SearchEntry.Contains("java") || SearchEntry.Contains("jdk") || SearchEntry.Contains("env") || SearchEntry.Contains("环境") || SearchEntry.Contains("run") || SearchEntry.Contains("软件") || SearchEntry.Contains("jre") || SearchEntry == "bin" || SearchEntry.Contains("mc") || SearchEntry.Contains("software") || SearchEntry.Contains("cache") || SearchEntry.Contains("temp") || SearchEntry.Contains("corretto") || SearchEntry.Contains("roaming") || SearchEntry.Contains("users") || SearchEntry.Contains("craft") || SearchEntry.Contains("program") || SearchEntry.Contains("世界") || SearchEntry.Contains("net") || SearchEntry.Contains("游戏") || SearchEntry.Contains("oracle") || SearchEntry.Contains("game") || SearchEntry.Contains("file") || SearchEntry.Contains("data") || SearchEntry.Contains("jvm") || SearchEntry.Contains("服务") || SearchEntry.Contains("server") || SearchEntry.Contains("客户") || SearchEntry.Contains("client") || SearchEntry.Contains("整合") || SearchEntry.Contains("应用") || SearchEntry.Contains("运行") || SearchEntry.Contains("前置") || SearchEntry.Contains("mojang") || SearchEntry.Contains("官启") || SearchEntry.Contains("新建文件夹") || SearchEntry.Contains("eclipse") || SearchEntry.Contains("microsoft") || SearchEntry.Contains("hotspot") || SearchEntry.Contains("runtime") || SearchEntry.Contains("x86") || SearchEntry.Contains("x64") || SearchEntry.Contains("forge") || SearchEntry.Contains("原版") || SearchEntry.Contains("optifine") || SearchEntry.Contains("官方") || SearchEntry.Contains("启动") || SearchEntry.Contains("hmcl") || SearchEntry.Contains("mod") || SearchEntry.Contains("高清") || SearchEntry.Contains("download") || SearchEntry.Contains("launch") || SearchEntry.Contains("程序") || SearchEntry.Contains("path") || SearchEntry.Contains("国服") || SearchEntry.Contains("网易") || SearchEntry.Contains("ext") || SearchEntry.Contains("netease") || SearchEntry.Contains("1.") || SearchEntry.Contains("启动"))
                            {
                                JavaSearchFolder(FolderInfo, ref Results, Source);
                            }
                        }
                    }
                }
                catch (UnauthorizedAccessException)
                {
                }
                catch (Exception)
                {
                }
            }

            private static string GetFolderNameFromPath(string FolderPath)
            {
                if (FolderPath.EndsWith(":\\") || FolderPath.EndsWith(":\\\\"))
                {
                    return FolderPath.Substring(0, 1);
                }
                if (FolderPath.EndsWith("\\") || FolderPath.EndsWith("/"))
                {
                    FolderPath = Strings.Left(FolderPath, FolderPath.Length - 1);
                }
                return GetFileNameFromPath(FolderPath);
            }

            private static string GetFileNameFromPath(string FilePath)
            {
                if (FilePath.EndsWith("\\") || FilePath.EndsWith("/"))
                {
                    throw new Exception("不包含文件名：" + FilePath);
                }
                if (!FilePath.Contains("\\") && !FilePath.Contains("/"))
                {
                    return FilePath;
                }
                if (FilePath.Contains("?"))
                {
                    FilePath = Strings.Left(FilePath, FilePath.LastIndexOf("?"));
                }
                return Strings.Mid(FilePath, 0);
            }
        }
        public static int CurrentCoreToolkitIndex = 0;
        public static List<GameCoreUtil> GameCoreToolkits = new List<GameCoreUtil>()
        {
            new()
        };
        public static List<Account> accounts = new List<Account>();
        public static home Home = new();
        public static down Down = new down();
        public static IBitmap UriToBitmap(string uri)
        {
            IBitmap bitmap = null;
            var assets = AvaloniaLocator.Current.GetService<IAssetLoader>();
            bitmap = new Bitmap(assets.Open(new Uri(uri)));
            if (bitmap == null)
            {
                bitmap = new Bitmap(uri);
            }
            return bitmap;
        }
    }
}
