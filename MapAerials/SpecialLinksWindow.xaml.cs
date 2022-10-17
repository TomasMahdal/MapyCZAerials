using MapAerials.Structures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace MapAerials
{
    /// <summary>
    /// Interaction logic for SpecialLinksWindow.xaml
    /// </summary>
    public partial class SpecialLinksWindow : Window
    {
        public SpecialLinksWindow(MainViewModel mv)
        {
            InitializeComponent();
            DataContext = mv.WServer.SpecialLinks;
        }

        /// <summary>
        /// Click on copy button next to link
        /// </summary>
        private void btnCopy_Click(object sender, RoutedEventArgs e)
        {
            for (var vis = sender as Visual; vis != null; vis = VisualTreeHelper.GetParent(vis) as Visual)
            {
                if (vis is DataGridRow)
                {
                    var row = (DataGridRow)vis;
                    var item = (SpecialLink)row.Item;
                    Clipboard.SetText(item.Url);
                    break;
                }
            }
        }
    }
}
