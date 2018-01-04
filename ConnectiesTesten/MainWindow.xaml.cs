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
using System.Windows.Navigation;
using System.Windows.Shapes;
using ClassLibrary1;

namespace ConnectiesTesten
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void buttonToevoegen_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Leverancier nieuw = new Leverancier()
                {
                    Naam = textboxNaam.Text,
                    Adres = textboxAdres.Text,
                    Postcode = textboxPostcode.Text,
                    Woonplaats = textboxPlaats.Text
                };
                var dbmanager = new leveranciersDBManager();
                dbmanager.LeverancierToevoegen(nieuw);
                labelStatus.Content = "nieuwe leverancier is toegevoegd";
                textboxNaam.Text = string.Empty;
                textboxAdres.Text = string.Empty;
                textboxPostcode.Text = string.Empty;
                textboxPlaats.Text = string.Empty;
            }
            catch(Exception ex)
            {
                labelStatus.Content = ex.Message;
            }
        }

        private void buttonKorting_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var dbmanager = new leveranciersDBManager();
                labelStatus.Content = dbmanager.Eindejaarskorting() + " prijzen aangepast";
            }
            catch(Exception ex)
            {
                labelStatus.Content = ex.Message;
            }
        }
    }
}
