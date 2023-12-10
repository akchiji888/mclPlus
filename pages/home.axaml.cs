using Avalonia;
using Avalonia.Controls;
using Avalonia.Media.Imaging;
using Avalonia.Threading;
using FluentAvalonia.UI.Controls;
using MinecraftLaunch.Launch;
using MinecraftLaunch.Modules.Models.Auth;
using MinecraftLaunch.Modules.Models.Launch;
using MinecraftLaunch.Modules.Utilities;
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
            List<string> javaList = new()
            {
                "�Զ�ѡ����ʵ�Java"
            };
            var javas = JavaUtil.GetJavas();
            foreach (var java in javas)
            {
                javaList.Add(java.JavaPath);
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
                        java = JavaUtil.GetCorrectOfGameJava(JavaUtil.GetJavas(), (new GameCoreUtil()).GetGameCore(verCombo.SelectedItem as string)).JavaPath;
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
                                Content = new TextBlock()
                                {
                                    Text = "δ�ҵ����ʵ�Java��",
                                    FontFamily = launchData.FontFamily,
                                    FontSize = 16
                                },
                                FontFamily = launchData.FontFamily,
                            };
                            await dialog.ShowAsync();
                        }
                    }
                    else
                    {
                        JvmConfig jc = new JvmConfig(javaCombo.SelectedItem as string)
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
                            result = launcher.Launch(verCombo.SelectedItem as string, x =>
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
                                        Content = new TextBlock()
                                        {
                                            Text = "�����ɹ���",
                                            FontFamily = launchData.FontFamily,
                                            FontSize = 16
                                        },
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
                                        Content = new TextBlock()
                                        {
                                            Text = "����ʧ�ܣ������Ĵ���" + result.Exception.Message,
                                            FontFamily = launchData.FontFamily,
                                            FontSize = 16
                                        },
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
                        Content = new TextBlock()
                        {
                            FontFamily = launchData.FontFamily,
                            Text = "δ��װ���õ�Java��",
                            FontSize = 16
                        },
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
                    Content = new TextBlock()
                    {
                        FontFamily = launchData.FontFamily,
                        Text = "δѡ����Ϸ�汾��",
                        FontSize = 16
                    },
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
