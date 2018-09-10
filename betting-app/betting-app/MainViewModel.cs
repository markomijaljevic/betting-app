using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using System.Windows.Input;
using System.ComponentModel;
using System.Windows.Controls;

namespace betting_app
{
    class MainViewModel : INotifyPropertyChanged
    {

        private databaseEntities context;
        private ObservableCollection<table_match> match;
        private MainWindow _main;

        public event PropertyChangedEventHandler PropertyChanged;
      //  public event SelectedCellsChangedEventHandler SelectedCellsChanged;

        public MainViewModel() { }

        public MainViewModel(MainWindow main) {

            _main = main;
            context = new databaseEntities();
            match = new ObservableCollection<table_match>();
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
            main.dataGrid.ItemsSource = match;

        }

        protected virtual void OnPropertyChanged( string propertyName )
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs( propertyName));
        }

        public ICommand cellChanged
        {
            get
            {
                return new DelegateCommand(cellClick,canCellClick);
            }
        }

        private bool canCellClick()
        {
            return true;
        }

        public void cellClick()
        {
            
        }
       
    }//end of class
}
