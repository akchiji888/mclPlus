using Avalonia.Controls;
using System;

namespace mclPlus.pages
{
    partial class manage : UserControl
    {
        public manage()
        {
            InitializeComponent();
            loginMode.ItemsSource = new string[] { "ÀëÏßµÇÂ¼", "MicrosoftµÇÂ¼", "µÚÈý·½ÍâÖÃµÇÂ¼" };
            AddAccount.Click += AddAccount_Click;
        }
        public void AddYggAccountFromUri(string uri)
        {

        }
        private async void AddAccount_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            AddAccount.IsEnabled = false;
            CancelBtn.Click += (c, x) =>
            {
                AddDialog.Hide();
            };
            OKBtn.Click += (c, x) =>
            {
                AddDialog.Hide();
                switch(loginMode.SelectedIndex)
                {
                    case 0://offline
                        
                        break;
                    case 1://microsoft
                        break;
                    case 2://ygg
                        break;
                }
            };
        }
    }
}
