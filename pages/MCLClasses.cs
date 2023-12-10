using Avalonia.Media.Imaging;
using Avalonia.Platform;
using Avalonia;
using Microsoft.VisualBasic;
using MinecraftLaunch.Modules.Models.Auth;
using MinecraftLaunch.Modules.Models.Launch;
using MinecraftLaunch.Modules.Utilities;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.Versioning;
using System.Text.RegularExpressions;
using System.Web;

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
        public static int CurrentCoreToolkitIndex = 0;
        public static List<GameCoreUtil> GameCoreToolkits = new List<GameCoreUtil>()
        {
            new()
        }; 
        public static home Home = new();
        public static down Down = new down();
        public static manage Manage = new manage();
        public static Bitmap UriToBitmap(string uri)
        {
            Bitmap bitmap = null;
            bitmap = new Bitmap(AssetLoader.Open(new Uri(uri)));
            if (bitmap == null)
            {
                bitmap = new Bitmap(uri);
            }
            return bitmap;
        }
        public class SettingsFile
        {
            public List<Account> accounts { get; set; }
            public int maxMem { get; set; }
            public int verComboIndex { get; set; }
            public int javaComboIndex { get; set; }
            public int accountComboIndex { get; set; }

        }
        public static string ExtractAndDecodeYggdrasilUrl(string str)
        {
            var match = Regex.Match(str, "https.*");
            if (match.Success)
            {
                return HttpUtility.UrlDecode(match.Value);
            }
            throw new ArgumentException();
        }

        public static bool IsSpecialChar(string str)
        {
            Regex regExp = new Regex("[ \\[ \\] \\^ \\-_*×――(^)$%~!＠@＃#$…&%￥—+=<>《》!！??？:：•`·、。，；,.;/\'\"{}（）‘’“”-]");
            if (regExp.IsMatch(str))
            {
                return true;
            }
            return false;
        }
        
    }
}
