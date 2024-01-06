using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Logging;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using Avalonia.Threading;
using mclPlus.controls;
using MinecraftLaunch.Classes.Models.Launch;
using MinecraftLaunch.Classes.Models.Install;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using static mclPlus.pages.MCLClasses;
using MinecraftLaunch.Components.Resolver;
using MinecraftLaunch.Classes.Interfaces;
using MinecraftLaunch.Classes.Models.Game;
using MinecraftLaunch.Components.Installer;

namespace mclPlus.pages
{
    partial class down : UserControl
    {
        IEnumerable<VersionManifestEntry> coresList;
        public down()
        {
            #region 初始化
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
            #region 事件&绑定
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
                    var verList_KuaiZhao = coresList.Where(x => x.Type == "snapshot" && (x.ReleaseTime.Month != 4 && x.ReleaseTime.Day != 1)).ToList();
                    verListBox.ItemsSource = JieXiVerList(verList_KuaiZhao);
                }
            };
            YuanGu.IsCheckedChanged += (c, x) =>
            {
                if (YuanGu.IsChecked == true)
                {
                    var verList_YuanGu = coresList.Where(x => x.Type == "old_alpha" || x.Type == "old_beta").ToList();
                    verListBox.ItemsSource = JieXiVerList(verList_YuanGu);
                }
            };
            YuRenJie.IsCheckedChanged += (c, x) =>
            {
                if (YuRenJie.IsChecked == true)
                {
                    var verList_Yurenjie = coresList.Where(x => x.ReleaseTime.Month == 4 && x.ReleaseTime.Day == 1).ToList();
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
                    loader.Log.Text = "加载中……";
                    #region Init loader
                    ForgeInstallEntry fakeForge = new()
                    {
                        ForgeVersion = "未选择"
                    };
                    FabricBuildEntry fakeFabric = new()
                    {
                        Loader = new()
                        {
                            Version = "未选择"
                        }
                    };
                    QuiltBuildEntry fakeQuilt = new()
                    {
                        Loader = new()
                        {
                            Version = "未选择"
                        }
                    };
                    var forge = (await ForgeInstaller.EnumerableFromVersionAsync(mcVer)).ToList();
                    var fabric = (await FabricInstaller.EnumerableFromVersionAsync(mcVer)).ToList();
                    var quilt = (await QuiltInstaller.EnumerableFromVersionAsync(mcVer)).ToList();
                    forge.Insert(0, fakeForge);
                    fabric.Insert(0, fakeFabric);
                    quilt.Insert(0, fakeQuilt);                    
                    loader.verForge.ItemsSource = forge;
                    loader.verFabric.ItemsSource = fabric;
                    loader.verOpt.Text = "暂不可用";
                    loader.verOpt.IsEnabled = false;
                    loader.verQuilt.ItemsSource = quilt;                    
                    loader.verForge.SelectedIndex = 0;
                    loader.verQuilt.SelectedIndex = 0;
                    loader.verOpt.SelectedIndex = 0;
                    loader.verFabric.SelectedIndex = 0;
                    loader.verNeo.Text = "暂不可用";
                    loader.verNeo.IsEnabled = false;
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
            coresList = await VanlliaInstaller.EnumerableGameCoreAsync();
            var verList_release = coresList.Where(x => x.Type == "release").ToList();
            verListBox.ItemsSource = null;
            verListBox.ItemsSource = JieXiVerList(verList_release);
        }
        private List<controls.MCVersionItem> JieXiVerList(List<VersionManifestEntry>? gameList)
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
                        item.verType.Text = "正式版";
                        item.verIcon.Source = UriToBitmap("resm:mclPlus.assets.normal.png");
                        break;

                    case "snapshot":
                        if (game.ReleaseTime.Month == 4 && game.ReleaseTime.Day == 1)
                        {
                            item.verType.Text = "愚人节版";
                            item.verIcon.Source = UriToBitmap("resm:mclPlus.assets.yrj.png");
                        }
                        else
                        {
                            item.verType.Text = "快照版";
                            item.verIcon.Source = UriToBitmap("resm:mclPlus.assets.snapshot.jpg");
                        }
                        break;
                    case "old_alpha":
                        item.verType.Text = "远古Alpha版";
                        item.verIcon.Source = UriToBitmap("resm:mclPlus.assets.old_alpha.png");
                        break;
                    case "old_beta":
                        item.verType.Text = "远古Beta版";
                        item.verIcon.Source = UriToBitmap("resm:mclPlus.assets.old_beta.png");
                        break;
                }
                ItemsSource.Add(item);
            }
            return ItemsSource;
        }
    }
}
