using Avalonia.Controls;
using MinecraftLaunch.Modules.Models.Auth;
using MinecraftLaunch.Modules.Models.Launch;
using MinecraftLaunch.Modules.Toolkits;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using static mclPlus.pages.MCLClasses;

namespace mclPlus.pages
{
    public partial class home : UserControl
    {
        public home()
        {
            InitializeComponent();
            List<showAccount> showAccounts = new();
            accounts.ForEach(account =>
            {
                showAccounts.Add(new(account));
            });
            accountCombo.Items = showAccounts;
            verCombo.Items = GameCoreToolkits[CurrentCoreToolkitIndex].GetGameCores();
            verCombo.SelectionChanged += VerCombo_SelectionChanged;
            List<JavaInfo> javaList = new();
            if(OperatingSystem.IsWindows() == true)
            {
                JavaInfo fakeJava = new()
                {
                    JavaPath = "自动选择合适的Java",
                };
                javaList.Add(fakeJava);
                foreach(var t in JavaToolkit_FindJavasOnly.GetJavas())
                {
                    javaList.Add(t);
                }
            }
            else
            {
                var javas = WL_JavaInfo.FindJava();
                foreach (var java in javas)
                {
                    javaList.Add(JavaToolkit.GetJavaInfo(java.Path));
                }
            }
            javaCombo.Items = javaList;
        }

        private void VerCombo_SelectionChanged(object? sender, SelectionChangedEventArgs e)
        {
            versionText.Content = verCombo.Text;
        }
    }
}
