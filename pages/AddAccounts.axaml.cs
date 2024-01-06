using Avalonia.Controls;

namespace mclPlus.pages
{
    public partial class AddAccounts : UserControl
    {
        public AddAccounts()
        {
            InitializeComponent();
            accountCombo.ItemsSource = new string[]{"离线登录","Microsoft登录","第三方外置登录"};
            accountCombo.SelectedIndex = 0;
            okBtn1.Click += OkBtn1_Click;
        }

        private void OkBtn1_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            switch(accountCombo.SelectedIndex)
            {
                case 0:
                    ChangeFrame(MainFrame,OffGrid); break;
                case 1:
                    ChangeFrame(MainFrame,MsGrid); break;
                case 2:
                    ChangeFrame(MainFrame,YggGrid); break;
            }
        }
        /// <summary>
        /// 更换显示的窗口
        /// </summary>
        /// <param name="t">原窗口</param>
        /// <param name="g">要显示的窗口</param>
        void ChangeFrame(Grid t,Grid g)
        {
            t.IsVisible = false;
            g.IsVisible = true;
        }
    }
}
