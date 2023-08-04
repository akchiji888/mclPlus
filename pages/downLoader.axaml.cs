using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using MinecraftLaunch.Modules.Models.Download;
using MinecraftLaunch.Modules.Utils;
using System.Collections.Generic;
using System.Linq;
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
            Back.Click += Back_Click;
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
                #endregion
            }
            else
            {
                installType = InstallType.Vanilla;
                #region 启用其他
                verForge.SelectedIndex = 0; verForge.IsEnabled = true;
                verFabric.SelectedIndex = 0; verFabric.IsEnabled = true;
                verOpt.SelectedIndex = 0; verOpt.IsEnabled = true;
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
                #endregion
            }
        }

        private enum InstallType
        {
            Vanilla,
            Optfine,
            Forge,
            Fabric,
            Quilt
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
            verFabricAPI.Items = files;
        }
    }
}
