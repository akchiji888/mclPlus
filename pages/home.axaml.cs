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
            #region 事件绑定
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
                JavaPath = "自动选择合适的Java",
            };
            List<string> javaList = new()
            {
                "自动选择合适的Java"
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
            launchData.Text = "正在进行启动前检查……";
            if (verCombo.SelectedIndex != -1)
            {
                LaunchConfig lc = new()
                {
                    Account = Account.Default,
                    LauncherName = "MCLX Multi-Platform Version"
                };
                launchData.Text = "正在检查Java可用性……";
                if (JavaUtil.GetJavas().Count() > 0)
                {
                    if (javaCombo.SelectedIndex == 0)
                    {
                        launchData.Text = "正在选择合适的Java……";
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
                                CloseButtonText = "好的",
                                Content = new TextBlock()
                                {
                                    Text = "未找到合适的Java！",
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
                        launchData.Text = "正在拉起启动进程……";
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
                                        CloseButtonText = "好的",
                                        Content = new TextBlock()
                                        {
                                            Text = "启动成功！",
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
                                        CloseButtonText = "好的",
                                        Content = new TextBlock()
                                        {
                                            Text = "启动失败！发生的错误：" + result.Exception.Message,
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
                        CloseButtonText = "好的",
                        Content = new TextBlock()
                        {
                            FontFamily = launchData.FontFamily,
                            Text = "未安装可用的Java！",
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
                    CloseButtonText = "好的",
                    Content = new TextBlock()
                    {
                        FontFamily = launchData.FontFamily,
                        Text = "未选择游戏版本！",
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
