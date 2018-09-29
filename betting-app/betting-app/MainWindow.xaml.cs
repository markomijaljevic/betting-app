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
        public Button removeBtn;
    }


    public partial class MainWindow : Window
    {
        private ObservableCollection<table_match> matches;
        private databaseEntities context;

        public MainWindow()
        {
            InitializeComponent();
            Loaded += fillDataGrid;
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
                         handicap_odds = match.handicap_odds
                         
                    });
            }
            DataGrid.ItemsSource = matches;
        }

        private void DataGrid_SelectedCellsChanged(object sender, SelectedCellsChangedEventArgs e)
        {
            int columnIndex = ((DataGrid)sender).CurrentCell.Column.DisplayIndex;

            if (columnIndex > 1)
            {
              
                int index = DataGrid.Items.IndexOf(DataGrid.CurrentItem);
                DataGridRow row = (DataGridRow)DataGrid.ItemContainerGenerator.ContainerFromIndex(index);

                string header = DataGrid.CurrentCell.Column.Header.ToString();
                string odds = (DataGrid.CurrentCell.Column.GetCellContent(row) as TextBlock).Text;
                string startTime = (DataGrid.Columns[1].GetCellContent(row) as TextBlock).Text;
                string match = (DataGrid.Columns[0].GetCellContent(row) as TextBlock).Text;

                var data = new game
                {
                    match = match,
                    startTime = startTime,
                    odds = odds,
                    header = header,
                  
                };

                DataGrid2.Items.Add(data);
                
            }
         
        }
    }//end of class
}//end of namespace


