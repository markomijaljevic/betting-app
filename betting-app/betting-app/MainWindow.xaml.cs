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
        private int _bonus;

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
                        Odds.Text = ( ( Double.Parse(Odds.Text) ) / ( Double.Parse(odds) / 10 ) ).ToString();
                        Win.Text = ((Double.Parse(Odds.Text)) * (Double.Parse(Payment.Text))).ToString();
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
            WalletWindow newWallet = new WalletWindow(wallet);
            newWallet.ShowDialog();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            if(DataGrid2.Items.Count > 0)
            {
                int bonus;
                bonus = checkForBonus();

                if (bonus > 0)
                {
                    MessageBox.Show("Ostvareni bonus iznosi ==> " + bonus.ToString());
                    Odds.Text = (Double.Parse(Odds.Text) + bonus).ToString();
                    Win.Text = ((Double.Parse(Odds.Text)) * (Double.Parse(Payment.Text))).ToString();
                }

                wallet.money.Text = (Double.Parse(wallet.money.Text) - Double.Parse(Payment.Text)).ToString();
                var datagrid = new DataGrid();

                datagrid.Columns.Add(new DataGridTextColumn()
                {
                    Header = "Utakmica",
                    Width = new DataGridLength(200),
                    FontSize = 12,
                    Binding = new Binding("match")
                });

                datagrid.Columns.Add(new DataGridTextColumn()
                {
                    Header = "Početak",
                    Width = new DataGridLength(100),
                    FontSize = 12,
                    Binding = new Binding("startTime")
                });

                datagrid.Columns.Add(new DataGridTextColumn()
                {
                    Header = "Tečaj",
                    Width = new DataGridLength(100),
                    FontSize = 12,
                    Binding = new Binding("odds")
                });

                datagrid.Columns.Add(new DataGridTextColumn()
                {
                    Header = "Odigrano",
                    Width = new DataGridLength(100),
                    FontSize = 12,
                    Binding = new Binding("header")
                });

                foreach (game item in DataGrid2.Items)
                {
                    datagrid.Items.Add(item);
                }
                wallet.stackPanel.Children.Add(datagrid);

                TextBlock payment = new TextBlock();
                payment.Text = "Uplata = " + Payment.Text;
                wallet.stackPanel.Children.Add(payment);

                TextBlock odds = new TextBlock();
                odds.Text = "Ukupan Tečaj = " + Odds.Text;
                wallet.stackPanel.Children.Add(odds);

                TextBlock win = new TextBlock();
                win.Text = "Ukupan dobitak = " + Win.Text + " Kn";
                wallet.stackPanel.Children.Add(win);

                wallet.stackPanel.Children.Add(new Separator());

                MessageBox.Show("Listić je uspješno uplaćen!");
                DataGrid2.Items.Clear();
                Odds.Text = "1";
                Win.Text = "1";
                Payment.Text = "5";
                return;
            }

            MessageBox.Show("Odigraj te bar jednu utakmicu!");
        }

        private int checkForBonus()
        {
            int bonusCounter = 0;
            int bonus = 0;
            int i = 0;
            int[] bonusCount = new int[context.Categories.Count()];

            foreach (Categories cat in context.Categories)
            {
                foreach (game item in DataGrid2.Items)
                {
                    if (cat.Id == item.category)
                    {
                        bonusCounter++;
                    }
                }
                bonusCount[i] = bonusCounter;
                bonusCounter = 0;
                i++;
            }
            i = 0;
            foreach (int count in bonusCount)
            {
                if(count > 0)
                {
                    i++;
                    if (count / 3 > 0)
                    {
                        bonus += 5 * (count / 3);
                    }
                }
            }

            if (i == context.Categories.Count())
                bonus += 10;
            

            return bonus;
        }



    }//end of class
}//end of namespace


