using Avalonia;
using Avalonia.Controls;
using Avalonia.Media.Imaging;
using Avalonia.Threading;
using MinecraftLaunch.Launch;
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
            #region �¼���
            verCombo.SelectionChanged += VerCombo_SelectionChanged;
            launchBtn.Click += LaunchBtn_Click;
            #endregion
            List<showAccount> showAccounts = new();
            accounts.ForEach(account =>
            {
                showAccounts.Add(new(account));
            });
            accountCombo.Items = showAccounts;
            var gamecore = new GameCoreUtil();
            var gameCores = gamecore.GetGameCores().ToList();
            List<string> temp = new();
            foreach(var core in gameCores)
            {
                temp.Add(core.Id);
            }
            verCombo.Items = temp;
            temp = null;
            GC.Collect();
            JavaInfo fakeJava = new()
            {
                JavaPath = "�Զ�ѡ����ʵ�Java",
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
                Title = "������־����MCLX Multi-Platform Version",
            };
            TextBox dataText = new()
            {
                IsReadOnly = true,
                FontFamily = versionText.FontFamily,
            };
            data.Content = dataText;
            dataText.Text = ($"[{DateTime.Now}]��ʼ����");
            if(verCombo.SelectedIndex != -1)
            {
                LaunchConfig lc = new()
                {
                    Account = Account.Default,
                    LauncherName = "MCLX Multi-Platform Version"
                };
                if(javaCombo.SelectedIndex == 0)
                {
                    JvmConfig jc = new JvmConfig(JavaUtil.GetCorrectOfGameJava(JavaUtil.GetJavas(), (new GameCoreUtil()).GetGameCore(verCombo.Text)).JavaPath)
                    {
                        MaxMemory = 2048,
                    };
                    lc.JvmConfig = jc;
                }
                else
                {
                    JvmConfig jc = new JvmConfig(javaCombo.Text)
                    {
                        MaxMemory = 2048,
                    };
                    lc.JvmConfig = jc;
                }
                Task.Run(() =>
                {
                    JavaMinecraftLauncher launcher = new(lc, new());
                    launcher.Launch(verCombo.Text, x =>
                    {
                        Dispatcher.UIThread.InvokeAsync(() =>
                        {
                            launchBar.Value = x.Item1;
                            dataText.Text = dataText.Text + "\n" + x.Item2;
                        });
                    });
                });
                
            }
        }
        private void VerCombo_SelectionChanged(object? sender, SelectionChangedEventArgs e)
        {
            versionText.Content = verCombo.SelectedItem as string;
        }
    }
}
