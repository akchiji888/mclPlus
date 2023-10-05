using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Logging;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using Avalonia.Threading;
using mclPlus.controls;
using MinecraftLaunch.Modules.Installer;
using MinecraftLaunch.Modules.Models.Install;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using static mclPlus.pages.MCLClasses;

namespace mclPlus.pages
{
    partial class down : UserControl
    {
        GameCoresEntity coresList = new();
        public down()
        {
            #region ��ʼ��
            InitializeComponent();
            SetMCList();
            ZhengShi.Initialized += (c, x) =>
            {
                ZhengShi.IsChecked = true;
            };
            if (OperatingSystem.IsWindows() == false)
            {
                JavaInstall.IsEnabled = false;
            }
            #endregion
            #region �¼�&��
            ZhengShi.IsCheckedChanged += (c, x) =>
            {
                if (ZhengShi.IsChecked == true)
                {
                    SetMCList();
                }
            };
            KuaiZhao.IsCheckedChanged += (c, x) =>
            {
                if (KuaiZhao.IsChecked == true)
                {
                    var verList_KuaiZhao = coresList.Cores.Where(x => x.Type == "snapshot" && (x.ReleaseTime.Month != 4 && x.ReleaseTime.Day != 1)).ToList();
                    verListBox.ItemsSource = JieXiVerList(verList_KuaiZhao);
                }
            };
            YuanGu.IsCheckedChanged += (c, x) =>
            {
                if (YuanGu.IsChecked == true)
                {
                    var verList_YuanGu = coresList.Cores.Where(x => x.Type == "old_alpha" || x.Type == "old_beta").ToList();
                    verListBox.ItemsSource = JieXiVerList(verList_YuanGu);
                }
            };
            YuRenJie.IsCheckedChanged += (c, x) =>
            {
                if (YuRenJie.IsChecked == true)
                {
                    var verList_Yurenjie = coresList.Cores.Where(x => x.ReleaseTime.Month == 4 && x.ReleaseTime.Day == 1).ToList();
                    verListBox.ItemsSource = JieXiVerList(verList_Yurenjie);
                }
            };
            verListBox.Tapped += VerListBox_Tapped;
            #endregion
        }

        private async void VerListBox_Tapped(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            if (Avalonia.Application.Current.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                if (verListBox.SelectedItem != null)
                {
                    MCVersionItem item = verListBox.SelectedItem as MCVersionItem;
                    var mainFrame = (desktop.MainWindow as MainWindow).mainFrame;
                    downLoader loader = new();
                    var mcVer = item.verName.Text;
                    loader.VerID.Text = mcVer;
                    mainFrame.Navigate(typeof(downLoader));
                    mainFrame.Content = loader;
                    loader.Start.IsEnabled = false;
                    loader.DownLoadBorder.IsEnabled = false;
                    loader.InstallBar.IsIndeterminate = true;
                    loader.Log.Text = "�����С���";
                    #region Init loader
                    ForgeInstallEntity fakeForge = new()
                    {
                        ForgeVersion = "δѡ��"
                    };
                    FabricInstallBuild fakeFabric = new()
                    {
                        Loader = new()
                        {
                            Version = "δѡ��"
                        }
                    };
                    OptiFineInstallEntity fakeOptiFine = new()
                    {
                        FileName = "δѡ��"
                    };
                    QuiltInstallBuild fakeQuilt = new()
                    {
                        Loader = new()
                        {
                            Version = "δѡ��"
                        }
                    };
                    NeoForgeInstallEntity fakeNeoForge = new()
                    {
                        NeoForgeVersion = "δѡ��"
                    };
                    var forge = (await ForgeInstaller.GetForgeBuildsOfVersionAsync(mcVer)).ToList();
                    var fabric = (await FabricInstaller.GetFabricBuildsByVersionAsync(mcVer)).ToList();
                    var optifine = (await OptiFineInstaller.GetOptiFineBuildsFromMcVersionAsync(mcVer)).ToList();
                    var quilt = (await QuiltInstaller.GetQuiltBuildsByVersionAsync(mcVer)).ToList();
                    List<NeoForgeInstallEntity> neo;
                    await Task.Run(() =>
                    {
                        neo = NeoForgeInstaller.GetNeoForgesOfVersionAsync(mcVer).ToList();
                        neo.Insert(0, fakeNeoForge);
                        Dispatcher.UIThread.Invoke(() =>
                        {
                            loader.verNeo.ItemsSource = neo;
                        });
                    });
                    forge.Insert(0, fakeForge);
                    fabric.Insert(0, fakeFabric);
                    optifine.Insert(0, fakeOptiFine);
                    quilt.Insert(0, fakeQuilt);                    
                    loader.verForge.ItemsSource = forge;
                    loader.verFabric.ItemsSource = fabric;
                    loader.verOpt.ItemsSource = optifine;
                    loader.verQuilt.ItemsSource = quilt;                    
                    loader.verForge.SelectedIndex = 0;
                    loader.verQuilt.SelectedIndex = 0;
                    loader.verOpt.SelectedIndex = 0;
                    loader.verFabric.SelectedIndex = 0;
                    loader.verNeo.SelectedIndex = 0;
                    loader.InitFabricAPI(mcVer);
                    #endregion
                    loader.DownLoadBorder.IsEnabled = true;
                    loader.InstallBar.IsIndeterminate = false;
                    loader.Log.Text = "";
                    loader.Start.IsEnabled = true;
                }
            }
        }
        private async void SetMCList()
        {
            coresList = await GameCoreInstaller.GetGameCoresAsync();
            var verList_release = coresList.Cores.Where(x => x.Type == "release").ToList();
            verListBox.ItemsSource = null;
            verListBox.ItemsSource = JieXiVerList(verList_release);
        }
        private List<controls.MCVersionItem> JieXiVerList(List<GameCoreEmtity>? gameList)
        {
            List<controls.MCVersionItem> ItemsSource = new();
            foreach (var game in gameList)
            {
                controls.MCVersionItem item = new();
                item.verName.Text = game.Id;
                item.verTime.Text = game.ReleaseTime.ToString("g");
                switch (game.Type)
                {
                    case "release":
                        item.verType.Text = "��ʽ��";
                        item.verIcon.Source = UriToBitmap("resm:mclPlus.assets.normal.png");
                        break;

                    case "snapshot":
                        if (game.ReleaseTime.Month == 4 && game.ReleaseTime.Day == 1)
                        {
                            item.verType.Text = "���˽ڰ�";
                            item.verIcon.Source = UriToBitmap("resm:mclPlus.assets.yrj.png");
                        }
                        else
                        {
                            item.verType.Text = "���հ�";
                            item.verIcon.Source = UriToBitmap("resm:mclPlus.assets.snapshot.jpg");
                        }
                        break;
                    case "old_alpha":
                        item.verType.Text = "Զ��Alpha��";
                        item.verIcon.Source = UriToBitmap("resm:mclPlus.assets.old_alpha.png");
                        break;
                    case "old_beta":
                        item.verType.Text = "Զ��Beta��";
                        item.verIcon.Source = UriToBitmap("resm:mclPlus.assets.old_beta.png");
                        break;
                }
                ItemsSource.Add(item);
            }
            return ItemsSource;
        }
    }
}
