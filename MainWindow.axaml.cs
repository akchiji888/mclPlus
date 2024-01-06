using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Threading;
using FluentAvalonia.UI.Controls;
using mclPlus.pages;
using static mclPlus.pages.MCLClasses;

namespace mclPlus
{
    public partial class MainWindow : Window
    {
        
        public MainWindow()
        {
            InitializeComponent();
            DragDrop.SetAllowDrop(this, true);
            mainView.SelectionChanged += MainView_SelectionChanged;
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
            AddHandler(DragDrop.DropEvent, Drop);
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
                case "Manage":
                    mainFrame.Navigate(typeof(manage));
                    mainFrame.Content = Manage;
                    break;
            }
        }
        private void AppTitle_PointerPressed(object? sender, Avalonia.Input.PointerPressedEventArgs e)
        {
            BeginMoveDrag(e);
        }
        private void Drop(object? sender, DragEventArgs e)
        {
            var text = e.Data.GetText();
            if (text.Contains("authlib-injector"))
            {
                var serverURI = ExtractAndDecodeYggdrasilUrl(text);
                
            }
        }
    }
}
