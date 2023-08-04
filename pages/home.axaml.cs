using Avalonia;
using Avalonia.Controls;
using Avalonia.Media.Imaging;
using MinecraftLaunch.Modules.Models.Auth;
using MinecraftLaunch.Modules.Models.Launch;
using MinecraftLaunch.Modules.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
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
            #region 事件绑定
            verCombo.SelectionChanged += VerCombo_SelectionChanged;
            launchBtn.Click += LaunchBtn_Click;
            #endregion
            List<showAccount> showAccounts = new();
            accounts.ForEach(account =>
            {
                showAccounts.Add(new(account));
            });
            accountCombo.Items = showAccounts;
            verCombo.Items = GameCoreToolkits[CurrentCoreToolkitIndex].GetGameCores();
            JavaInfo fakeJava = new()
            {
                JavaPath = "自动选择合适的Java",
            };
            List<JavaInfo> javaList = new()
            {
                fakeJava
            };
            var javas = JavaUtil.GetJavas();
            foreach (var java in javas)
            {
                javaList.Add(java);
            }
            javaCombo.Items = javaList;
        }
        private void LaunchBtn_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            Window data = new()
            {
                Width = 300,
                Height = 500,
                Title = "启动日志——MCLX Multi-Platform Version",
            };
            TextBox dataText = new()
            {
                IsReadOnly = true,
                FontFamily = versionText.FontFamily,
            };
        }
        private void VerCombo_SelectionChanged(object? sender, SelectionChangedEventArgs e)
        {
            versionText.Content = verCombo.Text;
        }
    }
}
