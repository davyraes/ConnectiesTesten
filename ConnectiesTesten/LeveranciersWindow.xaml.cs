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
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using ClassLibrary1;

namespace ConnectiesTesten
{
    /// <summary>
    /// Interaction logic for LeveranciersWindow.xaml
    /// </summary>
    public partial class LeveranciersWindow : Window
    {
        private CollectionViewSource leverancierViewSource = new CollectionViewSource();
        private ObservableCollection<Leverancier> leveranciers = new ObservableCollection<Leverancier>();
        public LeveranciersWindow()
        {
           
            InitializeComponent();
            
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            var manager = new leveranciersDBManager();
            leveranciers = manager.LeverancierPerPostcode(string.Empty);
            var postnrs = (from l in leveranciers orderby l.Postcode select l.Postcode.ToString()).Distinct().ToList();
            postnrs.Insert(0, "Alles");
            ComboBoxPostnummer.ItemsSource = postnrs;
            ComboBoxPostnummer.SelectedIndex = 0;
            VulGrid();           
        }
        private void VulGrid()
        {
            leverancierViewSource = ((CollectionViewSource)(this.FindResource("leverancierViewSource")));
            // Load data by setting the CollectionViewSource.Source property:
            // leverancierViewSource.Source = [generic data source]
            var manager = new leveranciersDBManager();
            leveranciers = manager.LeverancierPerPostcode(ComboBoxPostnummer.SelectedValue.ToString());
            leverancierViewSource.Source = leveranciers;
            leveranciers.CollectionChanged += On_Collection_Changed;
        }

        private void ComboBoxPostnummer_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            VulGrid();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            foreach (Leverancier leverancier in leveranciers)
                if (leverancier.Changed == true)
                    GewijzigdeLeveranciers.Add(leverancier);
            if (OudeLeveranciers.Count > 0 || NieuweLeveranciers.Count > 0 || GewijzigdeLeveranciers.Count > 0)
            {
                if (MessageBox.Show("Wilt u alles wegschrijven naar de database ?", "Opslaan", MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.Yes) == MessageBoxResult.Yes)
                {
                    var boodschap = new StringBuilder();
                    var manager = new leveranciersDBManager();
                    var nietaangepast = manager.LeveranciersVerwijderen(OudeLeveranciers);
                    if (nietaangepast.Count > 0)
                    {
                        boodschap.Append("Niet verwijderd : \n");
                        foreach (Leverancier leverancier in nietaangepast)
                            boodschap.Append($"{leverancier.LevNr} : {leverancier.Naam} niet\n");
                    }
                    int aantalVerwijderd = OudeLeveranciers.Count - nietaangepast.Count;
                    nietaangepast.Clear();
                    nietaangepast = manager.LeveranciersToevoegen(NieuweLeveranciers);
                    if (nietaangepast.Count > 0)
                    {
                        boodschap.Append("Niet toegevoegd : \n");
                        foreach (Leverancier leverancier in nietaangepast)
                            boodschap.Append($"{leverancier.LevNr} : {leverancier.Naam} niet\n");
                    }
                    int aantalToegevoegd = NieuweLeveranciers.Count - nietaangepast.Count;
                    nietaangepast.Clear();
                    nietaangepast = manager.LeverancierAanpassen(GewijzigdeLeveranciers);
                    if (nietaangepast.Count > 0)
                    {
                        boodschap.Append("Niet gewijzigd : \n");
                        foreach (Leverancier leverancier in nietaangepast)
                            boodschap.Append($"{leverancier.LevNr} : {leverancier.Naam} niet\n");
                    }
                    int aantalGewijzigd = GewijzigdeLeveranciers.Count - nietaangepast.Count;
                    boodschap.Append("\n\n");
                    boodschap.Append($"{aantalVerwijderd} leveranciers verwijderd in de database\n");
                    boodschap.Append($"{aantalToegevoegd} leveranciers toegevoegd in de database\n");
                    boodschap.Append($"{aantalGewijzigd} leveranciers gewijzigd in de database\n");
                    MessageBox.Show(boodschap.ToString(), "Info", MessageBoxButton.OK);
                }
            }
        }
        public List<Leverancier> OudeLeveranciers = new List<Leverancier>();
        public List<Leverancier> NieuweLeveranciers = new List<Leverancier>();
        public List<Leverancier> GewijzigdeLeveranciers = new List<Leverancier>();
        private void On_Collection_Changed(object sender,NotifyCollectionChangedEventArgs e)
        {
            if (e.OldItems != null)
                foreach (Leverancier leverancier in e.OldItems)
                    OudeLeveranciers.Add(leverancier);
            if (e.NewItems != null)
                foreach (Leverancier leverancier in e.NewItems)
                    NieuweLeveranciers.Add(leverancier);
        }
    }
}
