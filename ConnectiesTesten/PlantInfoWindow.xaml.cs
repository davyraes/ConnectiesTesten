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
using System.Configuration;

namespace ConnectiesTesten
{
    /// <summary>
    /// Interaction logic for PlanInfoWindow.xaml
    /// </summary>
    public partial class PlantInfoWindow : Window
    {
        public PlantInfoWindow()
        {
            InitializeComponent();
        }

        private void buttonOpzoeken_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var manager = new PlantenDBManager();
                int plantnr;
                int.TryParse(textboxPlantNr.Text, out plantnr);
                var info = manager.VraagPlantInfoOp(plantnr);
                labelNaam.Content = info.PlantNaam;
                labelSoort.Content = info.Soort;
                LabelLeverancier.Content = info.Leverancier;
                labelKleur.Content = info.Kleur;
                labelKostprijs.Content = $"€ {info.Kostprijs.ToString()}";
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }
    }
}
