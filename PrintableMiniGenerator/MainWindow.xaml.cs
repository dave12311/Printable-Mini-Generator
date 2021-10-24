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
            List<MiniListItem> miniListItems = new();
            miniListItems.Add(new MiniListItem { Count = 2 });
            //MiniList.ItemsSource = miniListItems;
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
