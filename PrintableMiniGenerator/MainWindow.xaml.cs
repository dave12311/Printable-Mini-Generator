using System.Collections.Generic;
using System.Windows;

namespace PrintableMiniGenerator
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private static readonly FifthEToolsParser FEToolsParser = new();
        public MainWindow()
        {
            InitializeComponent();
            DataContext = FEToolsParser;
        }

        private void MonsterNameFilter_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            FEToolsParser.FilterMonsterListAsync(((System.Windows.Controls.TextBox)e.Source).Text);
        }
    }

    public class MiniListItem
    {
        private static uint NextID;
        public uint ID { get; }
        public uint Count { get; set; } = 1;

        public MiniListItem()
        {
            ID = NextID++;
        }
    }
}
