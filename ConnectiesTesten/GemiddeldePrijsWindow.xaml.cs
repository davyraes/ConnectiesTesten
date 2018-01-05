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
    /// Interaction logic for GemiddeldePrijsWindow.xaml
    /// </summary>
    public partial class GemiddeldePrijsWindow : Window
    {
        public GemiddeldePrijsWindow()
        {
            InitializeComponent();
        }

        private void buttonBereken_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var dbmanager = new PlantenDBManager();
                decimal bedrag = dbmanager.BerekenGemiddeldePrijsPerSoort(textboxPrijs.Text);
                if(bedrag!=0)
                    labalStatus.Content = "Gemiddelde prijs : € " + bedrag;
            }
            catch(Exception ex)
            {
                labalStatus.Content = ex.Message;
            }
        }
    }
}
