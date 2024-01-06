using Avalonia.Controls;
using static mclPlus.pages.MCLClasses;
using System;
using DialogHostAvalonia;
using Avalonia;
using mclPlus.controls;

namespace mclPlus.pages
{
    partial class manage : UserControl
    {
        
        public manage()
        {
            InitializeComponent();
            AddAccount.Click += AddAccount_Click;
        }

        private async void AddAccount_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            var result = await DialogHost.Show(new AddAccounts(), "account");
        }

        public void AddYggAccountFromUri(string uri)
        {

        }
    }
}
