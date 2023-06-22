using MinecraftLaunch.Modules.Models.Auth;
using MinecraftLaunch.Modules.Models.Download;
using MinecraftLaunch.Modules.Toolkits;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        public static List<GameCoreToolkit> GameCoreToolkits = new List<GameCoreToolkit>()
        {
            new()
        };
        public static List<Account> accounts = new List<Account>();
        public static home Home = new();
        public static down Down = new down();
    }
}
