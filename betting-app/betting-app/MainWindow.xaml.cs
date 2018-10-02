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
using System.Collections.ObjectModel;
using System.Text.RegularExpressions;

namespace betting_app
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    /// 

    public class game
    {
        public string match { get; set; }
        public string startTime { get; set; }
        public string odds { get; set; }
        public string header { get; set; }
        public int? category { get; set; }
    }


    public partial class MainWindow : Window
    {
        private static readonly Regex _regex = new Regex("[^0-9.-]+");
        private ObservableCollection<table_match> matches;
        private databaseEntities context;
        private WalletWindow wallet;

        public MainWindow()
        {
            InitializeComponent();
            Loaded += fillDataGrid;
            Payment.TextChanged += Payment_TextChanged;
            wallet = new WalletWindow();
        }

        private void fillDataGrid(object sender, RoutedEventArgs e)
        {
            context = new databaseEntities();
            matches = new ObservableCollection<table_match>();

            foreach(table_match match in context.table_match)
            {
                matches.Add(
                    new table_match
                    {
                         home_team = match.home_team + " - " + match.away_tema,
                         star__of_the_match = match.star__of_the_match,
                         home_team_odds = match.home_team_odds,
                         even_odds = match.even_odds,
                         away_team_odds = match.away_team_odds,
                         home_team_or_even_odds = match.home_team_or_even_odds,
                         away_team_or_even_odds = match.away_team_or_even_odds,
                         home_or_away_team_odds = match.home_or_away_team_odds,
                         handicap_odds = match.handicap_odds,
                         category_id = match.category_id
                    });
            }
            DataGrid.ItemsSource = matches;
        }

        private void DataGrid_SelectedCellsChanged(object sender, SelectedCellsChangedEventArgs e)
        {
            int columnIndex = ((DataGrid)sender).CurrentCell.Column.DisplayIndex;

            if (columnIndex > 1 && DataGrid2.Items.Count < 20)
            {
                int index = DataGrid.Items.IndexOf(DataGrid.CurrentItem);
                DataGridRow row = (DataGridRow)DataGrid.ItemContainerGenerator.ContainerFromIndex(index);

                string header = DataGrid.CurrentCell.Column.Header.ToString();
                string odds = (DataGrid.CurrentCell.Column.GetCellContent(row) as TextBlock).Text;
                string startTime = (DataGrid.Columns[1].GetCellContent(row) as TextBlock).Text;
                string match = (DataGrid.Columns[0].GetCellContent(row) as TextBlock).Text;
                int? category = (row.Item as table_match).category_id;

                if (String.IsNullOrEmpty(odds))
                    return;

                if (exists(match, header, odds))
                    return;

                var data = new game
                {
                    match = match,
                    startTime = startTime,
                    odds = odds,
                    header = header,
                    category = category
                };

                DataGrid2.Items.Add(data);
                
                Odds.Text = ( ((Double.Parse(odds)/10) * (Double.Parse(Odds.Text)))).ToString();
                Win.Text = ((Double.Parse(Odds.Text)) * (Double.Parse(Payment.Text))).ToString();
            }
        }

        private void Payment_TextChanged(object sender, TextChangedEventArgs e)
        {
            string text = ((TextBox)sender).Text;

            if (_regex.IsMatch(text))
            {
                MessageBox.Show("Unesite numeričku vrijednost");
                ((TextBox)sender).Clear();
                return;
            }

            if (!(string.IsNullOrWhiteSpace(((TextBox)sender).Text)))
            {
                if (Double.Parse(text) > Double.Parse(wallet.money.Text))
                {
                    MessageBox.Show("Nema te dovoljno novca za uplatu!");
                    ((TextBox)sender).Clear();
                    return;
                }

                Win.Text = (Double.Parse(text) * Double.Parse(Odds.Text)).ToString();
            }
         
        }

        private bool exists(string match,string header, string odds)
        {
            foreach(game item in DataGrid2.Items)
            {
                if (item.match == match)
                {
                    if (item.header == header)
                    {
                        Odds.Text = ( ( Double.Parse(Odds.Text)) / ( Double.Parse(odds) / 10 ) ).ToString();
                        Win.Text = (( Double.Parse(Odds.Text) ) * ( Double.Parse(Payment.Text) )).ToString();

                        DataGrid2.Items.Remove(item);
                    }
                    else
                    {
                        Odds.Text = ((Double.Parse(Odds.Text)) / (Double.Parse(item.odds) / 10)).ToString();
                        Odds.Text = ((Double.Parse(Odds.Text)) * (Double.Parse(odds) / 10)).ToString();
                        Win.Text = ((Double.Parse(Odds.Text)) * (Double.Parse(Payment.Text))).ToString();
                        item.odds = odds;
                        item.header = header;
                        DataGrid2.Items.Refresh();
                    }

                    return true;
                }
            }
            return false;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            //WalletWindow wallet = new WalletWindow();
            wallet.ShowDialog();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            wallet.money.Text = (Double.Parse(wallet.money.Text) - Double.Parse(Payment.Text)).ToString();

        }
    }//end of class
}//end of namespace


