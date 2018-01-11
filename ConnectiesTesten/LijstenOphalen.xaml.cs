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
using System.Collections.ObjectModel;
using System.Collections.Specialized;

namespace ConnectiesTesten
{
    /// <summary>
    /// Interaction logic for LijstenOphalen.xaml
    /// </summary>
    public partial class LijstenOphalen : Window
    {
        private CollectionViewSource plantInfoViewSource;
        public ObservableCollection<PlantInfo> planten = new ObservableCollection<PlantInfo>();
        public List<PlantInfo> Gewijzigd = new List<PlantInfo>();
        public LijstenOphalen()
        {
            InitializeComponent();
            
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            soortComboBox.ItemsSource = PlantenDBManager.GetSoorten();
            soortComboBox.SelectedIndex = 0;
            GoUpdate();                        
        }
        private void GoUpdate()
        {
            plantInfoViewSource = ((CollectionViewSource)(this.FindResource("plantInfoViewSource")));
            // Load data by setting the CollectionViewSource.Source property:
            // plantInfoViewSource.Source = [generic data source]
            var manager = new PlantenDBManager();
            planten = manager.GetPantInfoSoort(soortComboBox.SelectedItem.ToString());
            plantInfoViewSource.Source = planten;
        }

        private void soortComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            WijzigingenOpslaan();
            GoUpdate();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            WijzigingenOpslaan();
        }
        private void WijzigingenOpslaan()
        {
            List<PlantInfo> resultaat = new List<PlantInfo>();
                    foreach (var plant in planten)
                    {
                        if ((plant.Changed == true)&&(plant.PlantNr!=0))
                        {
                            Gewijzigd.Add(plant);
                            plant.Changed = false;
                        }
                    }
            if ( Gewijzigd.Count > 0)
            {
                if (MessageBox.Show($"Gewijzigde planten van soort '{Gewijzigd[0].Soort}' opslaan", "Opslaan", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                {                    
                    if (Gewijzigd.Count != 0)
                    {
                        var manager = new PlantenDBManager();
                        resultaat = manager.SchrijfWijzigingenWeg(Gewijzigd);
                        if (resultaat.Count != 0)
                        {
                            StringBuilder boodschap = new StringBuilder();
                            boodschap.Append("Niet Doorgevoerd : \n");
                            foreach (var plant in resultaat)
                                boodschap.Append($"{plant.PlantNaam} : {plant.Kleur} : {plant.Kostprijs}");
                            MessageBox.Show(boodschap.ToString());
                        }
                    }
                    Gewijzigd.Clear();
                }
            }
        }
        private void TestenOpFouten_PreviewMouseDown(object sender,EventArgs e)
        {
            bool foutGevonden = false;
            foreach(var c in gridPlantinfo.Children)
                if(Validation.GetHasError((DependencyObject)c))
                {
                    foutGevonden = true;
                }
            if (foutGevonden)
            {
                if(e is MouseButtonEventArgs)
                    ((MouseButtonEventArgs)e).Handled = true;
                if (e is KeyEventArgs)
                    if (((KeyEventArgs)e).Key == Key.Enter || ((KeyEventArgs)e).Key == Key.Tab)
                        ((KeyEventArgs)e).Handled = true;
            }
        }
    }
}
