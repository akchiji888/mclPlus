using Avalonia;
using Avalonia.Controls;
using Avalonia.Media.Imaging;
using Avalonia.Threading;
using FluentAvalonia.UI.Controls;
using MinecraftLaunch.Components.Launcher;
using MinecraftLaunch.Classes.Models.Auth;
using MinecraftLaunch.Classes.Models.Launch;
using MinecraftLaunch.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using static mclPlus.pages.MCLClasses;
using MinecraftLaunch.Components.Resolver;
using MinecraftLaunch.Classes.Interfaces;
using MinecraftLaunch.Classes.Models.Game;
using MinecraftLaunch.Components.Authenticator;
using MinecraftLaunch.Components.Fetcher;
using MinecraftLaunch.Components.Watcher;

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
            IGameResolver gamecore = new GameResolver();
            var gameCores = gamecore.GetGameEntitys().ToList();
            List<string> temp = new();
            foreach (var core in gameCores)
            {
                temp.Add(core.Id);
            }
            verCombo.ItemsSource = temp;
            temp = null;
            GC.Collect();
            JavaEntry fakeJava = new()
            {
                JavaPath = "�Զ�ѡ����ʵ�Java",
            };
            List<string> javaList = new()
            {
                "�Զ�ѡ����ʵ�Java"
            };
            var javas = new JavaFetcher().Fetch();
            foreach (var java in javas)
            {
                javaList.Add(java.JavaPath);
            }
            javaCombo.ItemsSource = javaList;
            javaCombo.SelectedIndex = 0;
        }
        private async void LaunchBtn_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            IGameResolver gameResolver = new GameResolver();
            bool canStart = true;
            launchData.Text = "���ڽ�������ǰ��顭��";
            if (verCombo.SelectedIndex != -1)
            {
                LaunchConfig lc = new()
                {
                    Account = new OfflineAuthenticator("Test").Authenticate(),
                    LauncherName = "MCLX Multi-Platform Version"
                };
                launchData.Text = "���ڼ��Java�����ԡ���";
                if (new JavaFetcher().Fetch().Count() > 0)
                {
                    if (javaCombo.SelectedIndex == 0)
                    {
                        launchData.Text = "����ѡ����ʵ�Java����";
                        string java = "";
                        java = JavaUtil.GetCurrentJava(new JavaFetcher().Fetch(), gameResolver.GetGameEntity(verCombo.SelectedItem as string)).JavaPath;
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
                    if (canStart == true)
                    {
                        launchBar.IsIndeterminate = true;
                        launchData.Text = "���������������̡���";
                        await Task.Run(async () =>
                        {
                            Launcher launcher = new(gameResolver, lc);
                            var watcher = await launcher.LaunchAsync(verCombo.SelectedItem as string);
                            await Dispatcher.UIThread.InvokeAsync(() =>
                            {
                                launchBar.IsIndeterminate = false;
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
