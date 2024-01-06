using Avalonia.Controls;

namespace mclPlus.pages
{
    public partial class AddAccounts : UserControl
    {
        public AddAccounts()
        {
            InitializeComponent();
            accountCombo.ItemsSource = new string[]{"���ߵ�¼","Microsoft��¼","���������õ�¼"};
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
        /// ������ʾ�Ĵ���
        /// </summary>
        /// <param name="t">ԭ����</param>
        /// <param name="g">Ҫ��ʾ�Ĵ���</param>
        void ChangeFrame(Grid t,Grid g)
        {
            t.IsVisible = false;
            g.IsVisible = true;
        }
    }
}
