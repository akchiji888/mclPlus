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
        public static int CurrentCoreToolkitIndex = 0;
        public static List<GameCoreUtil> GameCoreToolkits = new List<GameCoreUtil>()
        {
            new()
        };
        public static List<Account> accounts = new List<Account>();
        public static home Home = new();
        public static down Down = new down();
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
    }
}
