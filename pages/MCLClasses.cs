using Microsoft.VisualBasic;
using MinecraftLaunch.Modules.Models.Auth;
using MinecraftLaunch.Modules.Models.Launch;
using MinecraftLaunch.Modules.Toolkits;
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
        /// # Java，包含 Path 和 Version 属性
        /// </summary>
        public class Java
        {
            public string Path { get; set; }
            public string Version { get; set; }

            public Java(string path)
            {
                Path = path;
                Version = string.Empty;
            }

            public Java(string path, string version)
            {
                Path = path;
                Version = version;
            }
        }
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
        /// <summary>
        /// # JavaInfo 用于获得Java信息
        /// </summary>
        public class WL_JavaInfo
        {
            /// <summary>
            /// ## 在终端执行 shell arg
            /// </summary>
            /// <param name="shell"></param>
            /// <param name="arg"></param>
            /// <returns></returns>
            private static HashSet<Java> RunProcess(string shell, string arg)
            {
                var startInfo = new ProcessStartInfo(shell, arg)
                {
                    WorkingDirectory = Directory.GetCurrentDirectory(),
                    UseShellExecute = false,
                    RedirectStandardInput = true,
                    RedirectStandardError = true,
                    RedirectStandardOutput = true,
                    CreateNoWindow = true,
                };
                var cmd = new Process
                {
                    StartInfo = startInfo
                };
                cmd.Start();
                if (cmd.HasExited)
                {
                    cmd.Close();
                    return new HashSet<Java>();
                }

                var reader = cmd.StandardOutput.ReadToEnd();
                var javas = new HashSet<Java>();
                foreach (var i in reader.Split('\n'))
                {
                    if (i.Replace("\r", string.Empty) == string.Empty)
                        continue;
                    javas.Add(new Java(i.Replace("\r", string.Empty)));
                }

                cmd.Close();
                return javas;
            }

            /// <summary>
            /// ## 寻找已安装的Java **多平台处理**
            /// </summary>
            /// <returns></returns>
            private static HashSet<Java> SearchJava()
            {
                if (OperatingSystem.IsWindows())
                    return RunProcess("where.exe", "javaw.exe");

                if (OperatingSystem.IsMacOS())
                    return MacJavas();

                if (OperatingSystem.IsLinux())
                    return LinuxJavas();
                //Unix系，仅返回环境变量的java路径
                return RunProcess("which", "java");
            }

            /// <summary>
            /// ## MacOS 的 Java查询
            /// </summary>
            /// <returns></returns>
            private static HashSet<Java> MacJavas()
            {
                const string javaHomePath = "/Library/Java/JavaVirtualMachines";
                var javas = new HashSet<Java>();
                foreach (var i in Directory.GetDirectories(javaHomePath))
                {
                    if (!Directory.Exists(i + "/Contents/Home/bin"))
                        continue;
                    javas.Add(new Java(i + "/Contents/Home/bin/java"));
                }
                return javas;
            }

            /// <summary>
            /// ## Linux 的 Java查询 **处理不同的发行版**
            /// </summary>
            /// <returns></returns>
            private static HashSet<Java> LinuxJavas()
            {
                const string binPath = "/usr/bin/";
                var jvmDirectory = string.Empty;
                if (Directory.Exists("/usr/lib/jvm"))
                    jvmDirectory = "/usr/lib/jvm";
                else if (Directory.Exists("/usr/lib64/jvm"))
                    jvmDirectory = "/usr/lib64/jvm";

                if (jvmDirectory == string.Empty)
                    return new HashSet<Java>();
                //Arch系 包管理器为 pacman
                if (File.Exists(binPath + "pacman"))
                    return ArchJavas(Directory.GetDirectories(jvmDirectory));
                //Debian系 包管理器为 apt
                if (File.Exists(binPath + "apt"))
                    return DebianJavas(Directory.GetDirectories(jvmDirectory));
                //Red hat系 包管理器为 yum/dnf
                if (File.Exists(binPath + "yum") || File.Exists(binPath + "dnf"))
                    return RedHatJavas(Directory.GetDirectories(jvmDirectory));
                //opensuse系 包管理器为 zypper
                if (File.Exists(binPath + "zypper"))
                    return OpensuseJavas(Directory.GetDirectories(jvmDirectory));
                //其他系，难精确查找，仅返回环境变量中的java路径
                return RunProcess("which", "java");
            }

            /// <summary>
            /// ### Arch系 Linux 的Java查询
            /// </summary>
            /// <param name="javaRootPaths"></param>
            /// <returns></returns>
            private static HashSet<Java> ArchJavas(string[] javaRootPaths)
            {
                var javas = new HashSet<Java>();
                foreach (var i in javaRootPaths)
                {
                    if (i.Contains("default"))
                        continue;
                    if (Directory.Exists(i + "/bin"))
                        javas.Add(new Java(i + "/bin/java"));
                }
                return javas;
            }

            /// <summary>
            /// ### Debian系 Linux 的Java查询
            /// </summary>
            /// <param name="javaRootPaths"></param>
            /// <returns></returns>
            private static HashSet<Java> DebianJavas(string[] javaRootPaths)
            {
                var javas = new HashSet<Java>();
                foreach (var i in javaRootPaths)
                {
                    if (!Directory.Exists(i + "/bin"))
                        continue;
                    javas.Add(new Java(i + "/bin/java"));
                }

                return javas;
            }

            /// <summary>
            /// ### 红帽系 Linux 的Java查询
            /// </summary>
            /// <param name="javaRootPaths"></param>
            /// <returns></returns>
            private static HashSet<Java> RedHatJavas(string[] javaRootPaths)
            {
                var javas = new HashSet<Java>();
                foreach (var i in javaRootPaths)
                {
                    if (i.Contains("jre"))
                        continue;
                    if (Directory.Exists(i + "/bin"))
                        javas.Add(new Java(i + "/bin/java"));
                }
                return javas;
            }

            /// <summary>
            /// ### OpenSuse Linux 的Java查询
            /// </summary>
            /// <param name="javaRootPaths"></param>
            /// <returns></returns>
            private static HashSet<Java> OpensuseJavas(string[] javaRootPaths)
            {
                var javas = new HashSet<Java>();
                foreach (var i in javaRootPaths)
                {
                    if (i.Contains("jre"))
                        continue;
                    if (Directory.Exists(i + "/bin"))
                        javas.Add(new Java(i + "/bin/java"));
                }
                return javas;
            }

            /// <summary>
            /// ## 通过读 java 根目录下的 release 文件来获得版本信息
            /// </summary>
            /// <param name="path"></param>
            /// <returns></returns>
            private static string FindJavaVersion(string path)
            {
                if (!File.Exists(Path.GetFullPath(path)))
                    //不存在release文件时，使用 `java -version`命令查看版本
                    return FindJavaVersionInCmd(path);
                try
                {
                    var sr = new StreamReader(Path.GetFullPath(path));
                    return ReadRelease(sr, path);
                }
                catch (Exception)
                {
                    return "Cannot Open Release File!";
                }
            }

            /// <summary>
            /// ## 通过 java -version 来获得版本信息
            /// 注意！如果用 `cmd.StandardOutput.ReadToEnd()` 则会返回空字符串
            /// 因此！应该使用 `cmd.StandardError.ReadToEnd()`
            /// 奇怪的特性
            /// </summary>
            /// <param name="path"></param>
            /// <returns></returns>
            private static string FindJavaVersionInCmd(string path)
            {
                string javaPath;
                if (OperatingSystem.IsWindows())
                    javaPath = path.Replace("release", @"bin\java.exe");
                else
                    javaPath = path.Replace("release", "bin/java");
                var cmd = RunJavaVersionCommand(javaPath);
                cmd.Start();
                var versionText = cmd.StandardError.ReadToEnd();
                cmd.Close();
                //使用正则表达式来提取 “” 中的内容，并且删除双引号
                return Regex.Match(versionText, "[\"].*?[\"]").ToString().Replace("\"", string.Empty);
            }

            /// <summary>
            /// 运行 java -version 命令并返回结果
            /// </summary>
            /// <param name="javaPath"></param>
            /// <returns></returns>
            private static Process RunJavaVersionCommand(string javaPath)
            {
                var startInfo = new ProcessStartInfo(javaPath, "-version")
                {
                    WorkingDirectory = Directory.GetCurrentDirectory(),
                    UseShellExecute = false,
                    RedirectStandardInput = true,
                    RedirectStandardError = true,
                    RedirectStandardOutput = true,
                    CreateNoWindow = true,
                };
                return new Process
                {
                    StartInfo = startInfo
                };
            }

            /// <summary>
            /// ## 读取 Release 文件
            /// 若读取错误则使用java -version命令读取
            /// </summary>
            /// <param name="sr"></param>
            /// <param name="path"></param>
            /// <returns></returns>
            private static string ReadRelease(StreamReader sr, string path)
            {
                while (!sr.EndOfStream)
                {
                    var versionText = sr.ReadLine();
                    if (versionText == null)
                    {
                        sr.Close();
                        return "Unknow Version";
                    }

                    if (versionText.Contains("JAVA_VERSION="))
                    {
                        sr.Close();
                        return versionText.Replace("JAVA_VERSION=", string.Empty).Replace("\"", string.Empty);
                    }
                }
                //读取错误时
                sr.Close();
                return FindJavaVersionInCmd(path);
            }

            /// <summary>
            /// ## 寻找并设置好现有 java 的版本
            /// </summary>
            /// <param name="javas"></param>
            /// <returns></returns>
            private static HashSet<Java> SetJavaVersion(HashSet<Java> javas)
            {
                var res = new HashSet<Java>();
                foreach (var i in javas)
                {
                    string javaReleasePath;
                    if (OperatingSystem.IsWindows())
                        javaReleasePath = i.Path.Replace(@"bin\javaw.exe", "release");
                    else
                        javaReleasePath = i.Path.Replace("bin/java", "release");
                    res.Add(new Java(i.Path, FindJavaVersion(javaReleasePath)));
                }

                return res;
            }

            /// <summary>
            /// # 寻找 Java及其版本
            /// </summary>
            /// <returns></returns>
            public static HashSet<Java> FindJava()
            {
                return SetJavaVersion(SearchJava());
            }
        }
        public static int CurrentCoreToolkitIndex = 0;
        public static List<GameCoreToolkit> GameCoreToolkits = new List<GameCoreToolkit>()
        {
            new()
        };
        public static List<Account> accounts = new List<Account>();
        public static home Home = new();
        public static down Down = new down();
    }
}
