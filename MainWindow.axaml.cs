using Avalonia.Controls;
using FluentAvalonia.UI.Controls;

namespace mclPlus
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            mainView.SelectionChanged += MainView_SelectionChanged;
        }

        private void MainView_SelectionChanged(object? sender, FluentAvalonia.UI.Controls.NavigationViewSelectionChangedEventArgs e)
        {
            var selectedItem = (NavigationViewItem)e.SelectedItem;
            //switch
        }
    }
}
