using Avalonia;
using Avalonia.Controls;
using FluentAvalonia.UI.Controls;
using mclPlus.pages;
using MinecraftLaunch.Modules.Models.Download;
using static mclPlus.pages.MCLClasses;

namespace mclPlus
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            mainView.SelectionChanged += MainView_SelectionChanged;
            APIManager.Current.Host = APIManager.Mcbbs.Host;
            AppTitle.PointerPressed += AppTitle_PointerPressed;
            closeBtn.Click += (c, x) =>
            {
                this.Close();
            };
            minBtn.Click += (c, x) =>
            {
                this.WindowState = WindowState.Minimized;
            };
            maxBtn.Click += (c, x) =>
            {
                maxBtn.IsVisible = false;
                this.WindowState = WindowState.Maximized;
            };
            restoreBtn.Click += (c, x) =>
            {
                maxBtn.IsVisible = true;
                this.WindowState = WindowState.Normal;
            };
        }

        private void MainView_SelectionChanged(object? sender, FluentAvalonia.UI.Controls.NavigationViewSelectionChangedEventArgs e)
        {
            var selectedItem = (NavigationViewItem)e.SelectedItem;
            switch (selectedItem.Tag)
            {
                case "Home":
                    mainFrame.Navigate(typeof(home));
                    mainFrame.Content = Home;
                    break;
                case "Down":
                    mainFrame.Navigate(typeof (down));
                    mainFrame.Content = Down;
                    break;
            }
        }
        private void AppTitle_PointerPressed(object? sender, Avalonia.Input.PointerPressedEventArgs e)
        {
            BeginMoveDrag(e);
        }
    }
}
