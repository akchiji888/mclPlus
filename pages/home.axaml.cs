using Avalonia;
using Avalonia.Controls;
using Avalonia.Media.Imaging;
using Avalonia.Threading;
using FluentAvalonia.UI.Controls;
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
    partial class home : UserControl
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
            accountCombo.ItemsSource = showAccounts;
            var gamecore = new GameCoreUtil();
            var gameCores = gamecore.GetGameCores().ToList();
            List<string> temp = new();
            foreach (var core in gameCores)
            {
                temp.Add(core.Id);
            }
            verCombo.ItemsSource = temp;
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
            javaCombo.ItemsSource = javaList;
            javaCombo.SelectedIndex = 0;
        }
        private async void LaunchBtn_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            MinecraftLaunchResponse result;
            bool canStart = true;
            launchData.Text = "���ڽ�������ǰ��顭��";
            if (verCombo.SelectedIndex != -1)
            {
                LaunchConfig lc = new()
                {
                    Account = Account.Default,
                    LauncherName = "MCLX Multi-Platform Version"
                };
                launchData.Text = "���ڼ��Java�����ԡ���";
                if (JavaUtil.GetJavas().Count() > 0)
                {
                    if (javaCombo.SelectedIndex == 0)
                    {
                        launchData.Text = "����ѡ����ʵ�Java����";
                        string java = "";
                        java = JavaUtil.GetCorrectOfGameJava(JavaUtil.GetJavas(), (new GameCoreUtil()).GetGameCore(verCombo.Text)).JavaPath;
                        if (java != "")
                        {
                            JvmConfig jc = new JvmConfig(java)
                            {
                                MaxMemory = 2048,
                            };
                            lc.JvmConfig = jc;
                        }
                        else
                        {
                            canStart = false;
                            ContentDialog dialog = new()
                            {
                                Title = "MCLX Multi-Platform Version",
                                CloseButtonText = "�õ�",
                                Content = "δ�ҵ����ʵ�Java��",
                                FontFamily = launchData.FontFamily,
                            };
                            await dialog.ShowAsync();
                        }
                    }
                    else
                    {
                        JvmConfig jc = new JvmConfig(javaCombo.Text)
                        {
                            MaxMemory = 2048,
                        };
                        lc.JvmConfig = jc;
                    }
                    if(canStart == true)
                    {
                        launchData.Text = "���������������̡���";
                        await Task.Run(() =>
                        {                            
                            JavaMinecraftLauncher launcher = new(lc, new());
                            result = launcher.Launch(verCombo.Text, x =>
                            {
                                Dispatcher.UIThread.InvokeAsync(() =>
                                {
                                    launchBar.Value = x.Item1;
                                    if(x.Item1 == 9f)
                                    {
                                        launchData.Text = $"{(x.Item1/10f).ToString("P2")} {x.Item2}";
                                    }
                                    else
                                    {
                                        launchData.Text = $"{x.Item1.ToString("P2")} {x.Item2}";
                                    }                                    
                                });
                            });
                            if (result.State is MinecraftLaunch.Modules.Enum.LaunchState.Succeess)
                            {
                                Dispatcher.UIThread.InvokeAsync(() =>
                                {
                                    launchData.Text = "";
                                    ContentDialog dialog = new()
                                    {
                                        Title = "MCLX Multi-Platform Version",
                                        CloseButtonText = "�õ�",
                                        Content = "�����ɹ���",
                                        FontFamily = launchData.FontFamily,
                                    };
                                    dialog.ShowAsync();
                                });
                            }
                            else
                            {
                                Dispatcher.UIThread.InvokeAsync(() =>
                                {
                                    launchData.Text = "";
                                    ContentDialog dialog = new()
                                    {
                                        Title = "MCLX Multi-Platform Version",
                                        CloseButtonText = "�õ�",
                                        Content = "����ʧ�ܣ������Ĵ���" + result.Exception.Message,
                                        FontFamily = launchData.FontFamily,
                                    };
                                    dialog.ShowAsync();
                                });

                            }
                        });
                    }
                }   
                else
                {
                    launchData.Text = "";
                    canStart = false;
                    ContentDialog dialog = new()
                    {
                        Title = "MCLX Multi-Platform Version",
                        CloseButtonText = "�õ�",
                        Content = "δ��װ���õ�Java��",
                        FontFamily = launchData.FontFamily,
                    };
                    await dialog.ShowAsync();
                }
            }
            else
            {
                launchData.Text = "";
                canStart = false;
                ContentDialog dialog = new()
                {
                    Title = "MCLX Multi-Platform Version",
                    CloseButtonText = "�õ�",
                    Content = "δѡ����Ϸ�汾��",
                    FontFamily = launchData.FontFamily,
                };
                await dialog.ShowAsync();
            }
        }
        private void VerCombo_SelectionChanged(object? sender, SelectionChangedEventArgs e)
        {
            versionText.Content = verCombo.SelectedItem as string;
        }
    }
}
