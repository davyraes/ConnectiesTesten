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
using ClassLibrary1;

namespace ConnectiesTesten
{
    /// <summary>
    /// Interaction logic for LijstenOphalen.xaml
    /// </summary>
    public partial class LijstenOphalen : Window
    {
        public LijstenOphalen()
        {
            InitializeComponent();
            comboSoort.ItemsSource = PlantenDBManager.GetSoorten();
        }

        private void comboSoort_Selected(object sender, RoutedEventArgs e)
        {
            try
            {
                var info = new PlantenDBManager();
                Listboxnaam.ItemsSource = info.GetPantInfoSoort(comboSoort.SelectedValue.ToString());
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}
