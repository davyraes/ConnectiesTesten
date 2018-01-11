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

namespace ConnectiesTesten
{
    /// <summary>
    /// Interaction logic for LeveranciersWindow.xaml
    /// </summary>
    public partial class LeveranciersWindow : Window
    {
        public LeveranciersWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {

            System.Windows.Data.CollectionViewSource leverancierViewSource = ((System.Windows.Data.CollectionViewSource)(this.FindResource("leverancierViewSource")));
            // Load data by setting the CollectionViewSource.Source property:
            // leverancierViewSource.Source = [generic data source]
        }
    }
}
