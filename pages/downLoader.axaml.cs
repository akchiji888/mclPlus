using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Threading;
using FluentAvalonia.UI.Controls;
using MinecraftLaunch.Modules.Installer;
using MinecraftLaunch.Modules.Models.Download;
using MinecraftLaunch.Modules.Models.Install;
using MinecraftLaunch.Modules.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using static mclPlus.pages.MCLClasses;

namespace mclPlus.pages
{
    public partial class downLoader : UserControl
    {
        private InstallType installType = InstallType.Vanilla;
        public downLoader()
        {
            InitializeComponent();
            verForge.SelectionChanged += verForge_SelectionChanged;
            verFabric.SelectionChanged += verFabric_SelectionChanged;
            verOpt.SelectionChanged += verOpt_SelectionChanged;
            verQuilt.SelectionChanged += verQuilt_SelectionChanged;
            verNeo.SelectionChanged += VerNeo_SelectionChanged;
            Back.Click += Back_Click;
            Start.Click += Start_Click;
        }
        private void VerNeo_SelectionChanged(object? sender, SelectionChangedEventArgs e)
        {
            if (verNeo.SelectedIndex != 0)
            {
                installType = InstallType.NeoForge;
                #region 禁用其他
                verForge.SelectedIndex = 0; verForge.IsEnabled = false;
                verFabric.SelectedIndex = 0; verFabric.IsEnabled = false;
                verOpt.SelectedIndex = 0; verOpt.IsEnabled = false;
                verFabricAPI.SelectedIndex = -1; verFabricAPI.IsEnabled = false;
                verQuilt.SelectedIndex = 0;verQuilt.IsEnabled = false;
                #endregion
            }
            else
            {
                installType = InstallType.Vanilla;
                #region 启用其他
                verForge.SelectedIndex = 0; verForge.IsEnabled = true;
                verFabric.SelectedIndex = 0; verFabric.IsEnabled = true;
                verOpt.SelectedIndex = 0; verOpt.IsEnabled = true;
                verQuilt.SelectedIndex = 0; verQuilt.IsEnabled = true;
                verFabricAPI.SelectedIndex = -1;
                #endregion
            }
        }

        private async void Start_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            try
            {
                DownLoadBorder.IsEnabled = false;
                bool HasJava = true;
                if (JavaUtil.GetJavas().Count() < 1)
                {
                    HasJava = false;
                }
                bool CanContinue = true;
                Back.IsEnabled = false;
                Start.IsEnabled = false;
                if (Avalonia.Application.Current.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
                {
                    (desktop.MainWindow as MainWindow).mainView.IsEnabled = false;
                }
                var core = new GameCoreUtil();
                var result = new InstallerResponse();
                ContentDialog dialog = new ContentDialog()
                {
                    Title = "MCLX Multi-Platform Version",
                    Content = $"{VerID.Text}安装完成！",
                    CloseButtonText = "好的",
                    FontFamily = verForge.FontFamily,
                };
                switch (installType)
                {
                    case InstallType.Vanilla:
                        var ver = VerID.Text;
                        await Task.Run(async () =>
                        {
                            GameCoreInstaller gci = new(core, ver);
                            gci.ProgressChanged += async (c, x) =>
                            {
                                await Dispatcher.UIThread.InvokeAsync(() =>
                                {
                                    Log.Text = $"{x.ProgressDescription} {x.Progress.ToString("P")}";
                                    InstallBar.Value = x.Progress;
                                });
                            };
                            var result = await gci.InstallAsync();
                        });

                        break;
                    case InstallType.Quilt:
                        var q_build = verQuilt.SelectedItem as QuiltInstallBuild;
                        await Task.Run(async () =>
                        {
                            var qi = new QuiltInstaller(core, q_build);
                            qi.ProgressChanged += (c, x) =>
                            {
                                Dispatcher.UIThread.InvokeAsync(() =>
                                {
                                    Log.Text = $"{x.ProgressDescription} {x.Progress.ToString("P")}";
                                    InstallBar.Value = x.Progress;
                                });
                            };
                            var result = await qi.InstallAsync();
                        });
                        break;
                    case InstallType.Fabric:
                        var f_build = verFabric.SelectedItem as FabricInstallBuild;
                        await Task.Run(async () =>
                        {
                            bool NeedFApi = false;
                            await Dispatcher.UIThread.InvokeAsync(() =>
                            {
                                if (verFabricAPI.SelectedIndex != -1)
                                {
                                    NeedFApi = true;
                                }
                            });
                            var fi = new FabricInstaller(core, f_build);
                            fi.ProgressChanged += (c, x) =>
                            {
                                Dispatcher.UIThread.InvokeAsync(() =>
                                {
                                    Log.Text = $"{x.ProgressDescription} {x.Progress.ToString("P")}";
                                    InstallBar.Value = x.Progress;
                                });
                            };
                            var result = await fi.InstallAsync();
                            if (NeedFApi == true)
                            {
                                ModrinthFileInfo file = new();
                                await Dispatcher.UIThread.InvokeAsync(() =>
                                {
                                    file = verFabricAPI.SelectedItem as ModrinthFileInfo;
                                    InstallBar.IsIndeterminate = true;
                                    Log.Text = "正在下载Fabric API";
                                });
                                Directory.CreateDirectory(result.GameCore.GetModsPath());
                                await HttpUtil.HttpDownloadAsync(file.Url, result.GameCore.GetModsPath());
                            }
                        });
                        dialog.Content = $"{VerID.Text}安装完成！";
                        break;
                    case InstallType.Forge:
                        if (HasJava == true)
                        {
                            var forge_build = verForge.SelectedItem as ForgeInstallEntity;
                            await Task.Run(async () =>
                            {
                                var fi = new ForgeInstaller(core, forge_build, JavaUtil.GetJavas().FirstOrDefault().JavaPath);
                                fi.ProgressChanged += async (c, x) =>
                                {
                                    await Dispatcher.UIThread.InvokeAsync(() =>
                                    {
                                        Log.Text = $"{x.ProgressDescription} {x.Progress.ToString("P")}";
                                        InstallBar.Value = x.Progress;
                                    });
                                };
                                var result = await fi.InstallAsync();
                            });
                        }
                        else
                        {
                            dialog.Content = "安装失败！\n原因：安装Forge需要Java";
                            CanContinue = false;
                            await dialog.ShowAsync();
                        }
                        break;
                    case InstallType.NeoForge:
                        if (HasJava == true)
                        {
                            var neoBuild = verNeo.SelectedItem as NeoForgeInstallEntity;
                            await Task.Run(async () =>
                            {
                                var ni = new NeoForgeInstaller(core, neoBuild, JavaUtil.GetJavas().FirstOrDefault().JavaPath);
                                ni.ProgressChanged += async (c, x) =>
                                {
                                    await Dispatcher.UIThread.InvokeAsync(() =>
                                    {
                                        Log.Text = $"{x.ProgressDescription} {x.Progress.ToString("P")}";
                                        InstallBar.Value = x.Progress;
                                    });
                                };
                                var result = await ni.InstallAsync();
                            });
                        }
                        else
                        {
                            dialog.Content = "安装失败！\n原因：安装NeoForge需要Java";
                            CanContinue = false;
                            await dialog.ShowAsync();
                        }
                        break;
                    case InstallType.Optfine:
                        if (HasJava == true)
                        {
                            var optbuild = verOpt.SelectedItem as OptiFineInstallEntity;
                            await Task.Run(async () =>
                            {
                                var fi = new OptiFineInstaller(core, optbuild, JavaUtil.GetJavas().FirstOrDefault().JavaPath);
                                fi.ProgressChanged += async (c, x) =>
                                {
                                    await Dispatcher.UIThread.InvokeAsync(() =>
                                    {
                                        Log.Text = $"{x.ProgressDescription} {x.Progress.ToString("P")}";
                                        InstallBar.Value = x.Progress;
                                    });
                                };
                                var result = await fi.InstallAsync();
                            });
                        }
                        else
                        {
                            dialog.Content = "安装失败！\n原因：安装Optifine需要Java";
                            CanContinue = false;
                            await dialog.ShowAsync();
                        }
                        break;

                }
                if (CanContinue == true)
                {
                    InstallBar.IsIndeterminate = true;
                    Log.Text = "正在进行资源补全……";
                    await Task.Run(() =>
                    {
                        ResourceInstaller installer = new(result.GameCore);
                        installer.DownloadAsync(null);
                    });
                    InstallBar.IsIndeterminate = false;
                    await dialog.ShowAsync();
                    DownLoadBorder.IsEnabled = true;
                    Back.IsEnabled = true;
                    Start.IsEnabled = true;
                }
                if (Avalonia.Application.Current.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop1)
                {
                    (desktop1.MainWindow as MainWindow).mainView.IsEnabled = true;
                    (desktop1.MainWindow as MainWindow).mainFrame.Navigate(typeof(down));
                    (desktop1.MainWindow as MainWindow).mainFrame.Content = Down;
                }
                var Index = Home.verCombo.SelectedIndex;
                Home.verCombo.ItemsSource = GameCoreToolkits[CurrentCoreToolkitIndex].GetGameCores();
                Home.verCombo.SelectedIndex = Index;
            }
            catch(Exception ex)
            {
                ContentDialog dialog = new ContentDialog()
                {
                    Title = "MCLX Multi-Platform Version",
                    Content = $"安装过程中出现了错误！\n错误信息:{ex.Message}",
                    CloseButtonText = "好的",
                    FontFamily = verForge.FontFamily,
                };
                await dialog.ShowAsync();
                if (Avalonia.Application.Current.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop1)
                {
                    (desktop1.MainWindow as MainWindow).mainView.IsEnabled = true;
                    (desktop1.MainWindow as MainWindow).mainFrame.Navigate(typeof(down));
                    (desktop1.MainWindow as MainWindow).mainFrame.Content = Down;
                }
            }
        }

        private void Back_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            if (Avalonia.Application.Current.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                var mainFrame = (desktop.MainWindow as MainWindow).mainFrame;
                mainFrame.Navigate(typeof(down));
                mainFrame.Content = Down;
            }
        }

        private void verQuilt_SelectionChanged(object? sender, SelectionChangedEventArgs e)
        {
            if (verQuilt.SelectedIndex != 0)
            {
                installType = InstallType.Quilt;
                #region 禁用其他
                verForge.SelectedIndex = 0; verForge.IsEnabled = false;
                verFabric.SelectedIndex = 0; verFabric.IsEnabled = false;
                verOpt.SelectedIndex = 0; verOpt.IsEnabled = false;
                verFabricAPI.SelectedIndex = -1; verFabricAPI.IsEnabled = false;
                verNeo.SelectedIndex = 0; verNeo.IsEnabled = false;
                #endregion
            }
            else
            {
                installType = InstallType.Vanilla;
                #region 启用其他
                verForge.SelectedIndex = 0; verForge.IsEnabled = true;
                verFabric.SelectedIndex = 0; verFabric.IsEnabled = true;
                verOpt.SelectedIndex = 0; verOpt.IsEnabled = true;
                verNeo.SelectedIndex = 0; verNeo.IsEnabled = true;
                verFabricAPI.SelectedIndex = -1;
                #endregion
            }
        }

        private void verOpt_SelectionChanged(object? sender, SelectionChangedEventArgs e)
        {
            if (verOpt.SelectedIndex != 0)
            {
                installType = InstallType.Optfine;
                #region 禁用其他
                verForge.SelectedIndex = 0; verForge.IsEnabled = false;
                verFabric.SelectedIndex = 0; verFabric.IsEnabled = false;
                verQuilt.SelectedIndex = 0; verQuilt.IsEnabled = false;
                verNeo.SelectedIndex = 0; verNeo.IsEnabled = false;
                verFabricAPI.SelectedIndex = -1; verFabricAPI.IsEnabled = false;
                #endregion
            }
            else
            {
                installType = InstallType.Vanilla;
                #region 启用其他
                verForge.SelectedIndex = 0; verForge.IsEnabled = true;
                verFabric.SelectedIndex = 0; verFabric.IsEnabled = true;
                verQuilt.SelectedIndex = 0; verQuilt.IsEnabled = true;
                verNeo.SelectedIndex = 0; verNeo.IsEnabled = true;
                verFabricAPI.SelectedIndex = -1;
                #endregion
            }
        }

        private void verFabric_SelectionChanged(object? sender, SelectionChangedEventArgs e)
        {
            if (verFabric.SelectedIndex != 0)
            {
                installType = InstallType.Fabric;
                #region 禁用其他
                verForge.SelectedIndex = 0; verForge.IsEnabled = false;
                verOpt.SelectedIndex = 0; verOpt.IsEnabled = false;
                verQuilt.SelectedIndex = 0; verQuilt.IsEnabled = false;
                verNeo.SelectedIndex = 0; verNeo.IsEnabled = false;
                #endregion
                verFabricAPI.IsEnabled = true;
            }
            else
            {
                installType = InstallType.Vanilla;
                #region 启用其他
                verForge.SelectedIndex = 0; verForge.IsEnabled = true;
                verOpt.SelectedIndex = 0; verOpt.IsEnabled = true;
                verQuilt.SelectedIndex = 0; verQuilt.IsEnabled = true;
                verNeo.SelectedIndex = 0; verNeo.IsEnabled = true;
                #endregion
                verFabricAPI.IsEnabled = false;
                verFabricAPI.SelectedIndex = -1;
            }
        }

        private void verForge_SelectionChanged(object? sender, SelectionChangedEventArgs e)
        {
            if (verForge.SelectedIndex != 0)
            {
                installType = InstallType.Forge;
                #region 禁用其他
                verFabric.SelectedIndex = 0; verFabric.IsEnabled = false;
                verOpt.SelectedIndex = 0; verOpt.IsEnabled = false;
                verQuilt.SelectedIndex = 0; verQuilt.IsEnabled = false;
                verFabricAPI.SelectedIndex = -1; verFabricAPI.IsEnabled = false;
                verNeo.SelectedIndex = 0; verNeo.IsEnabled = false;
                #endregion
            }
            else
            {
                installType = InstallType.Vanilla;
                #region 启用其他
                verFabric.SelectedIndex = 0; verFabric.IsEnabled = true;
                verOpt.SelectedIndex = 0; verOpt.IsEnabled = true;
                verQuilt.SelectedIndex = 0; verQuilt.IsEnabled = true;
                verFabricAPI.SelectedIndex = -1;
                verNeo.SelectedIndex = 0; verNeo.IsEnabled = true;
                #endregion
            }
        }

        private enum InstallType
        {
            Vanilla,
            Optfine,
            Forge,
            Fabric,
            Quilt,
            NeoForge
        }

        public async void InitFabricAPI(string id)
        {
            List<ModrinthFileInfo> files = new();
            var fabricAPI = await ModrinthUtil.GetProjectInfos("P7dR8mSH");
            fabricAPI.ForEach(x =>
            {
                if (x.GameVersion.First() == id)
                {
                    x.Files.ForEach(f =>
                    {
                        files.Add(f);
                    });
                }
            });
            verFabricAPI.ItemsSource = files;
        }
    }
}
