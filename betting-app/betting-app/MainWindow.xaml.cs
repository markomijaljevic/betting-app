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

    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            Loaded += MainWindow_Loaded;
        }

        private void MainWindow_Loaded(object sender, EventArgs e)
        {
            databaseEntities context = new databaseEntities();
            ObservableCollection<table_match> match = new ObservableCollection<table_match>();
            foreach (table_match m in context.table_match)
            {
                match.Add(new table_match
                {

                    Id = m.Id,
                    home_team = m.home_team + " - " + m.away_tema,
                    star__of_the_match = m.star__of_the_match,
                    home_team_odds = m.home_team_odds,
                    even_odds = m.even_odds,
                    away_team_odds = m.away_team_odds,
                    home_team_or_even_odds = m.home_team_or_even_odds,
                    away_team_or_even_odds = m.away_team_or_even_odds,
                    home_or_away_team_odds = m.home_or_away_team_odds,
                    handicap_odds = m.handicap_odds

                });
            }

            dataGrid.ItemsSource = match;
        }

    }//end of class
}//end of namespace


