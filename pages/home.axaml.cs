using Avalonia.Controls;
using MinecraftLaunch.Modules.Models.Auth;
using MinecraftLaunch.Modules.Toolkits;
using System.Collections.Generic;
using System.Threading.Tasks;
using static mclPlus.pages.MCLClasses;

namespace mclPlus.pages
{
    public partial class home : UserControl
    {
        public home()
        {
            InitializeComponent();
            List<showAccount> test = new();
            accounts.ForEach(account =>
            {
                test.Add(new(account));
            });
            accountCombo.Items = test;
            verCombo.Items = GameCoreToolkits[CurrentCoreToolkitIndex].GetGameCores();
            verCombo.SelectionChanged += VerCombo_SelectionChanged;
        }

        private void VerCombo_SelectionChanged(object? sender, SelectionChangedEventArgs e)
        {
            versionText.Content = verCombo.Text;
        }
    }
}
