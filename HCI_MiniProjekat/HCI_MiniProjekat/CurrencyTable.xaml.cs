using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace HCI_MiniProjekat
{
    /// <summary>
    /// Interaction logic for CurrencyTable.xaml
    /// </summary>
    public partial class CurrencyTable : Window
    {
        public List<TableRow> Rows2 { get; set; }
        public ObservableCollection<TableRow> Rows { get; set; }
        public string Currency { get; set; }
        public List<string> FromCurrenciesSymbols2 { get; set; }

        public ObservableCollection<string> FromCurrenciesSymbols { get; set; }
        public string CurrencyTitle { get; set; }
        public int selectedIndex { get; set; }

        public CurrencyTable() 
        {
            Rows = new ObservableCollection<TableRow>();
            FromCurrenciesSymbols = new ObservableCollection<string>();
            InitializeComponent();
        }
        public CurrencyTable(List<TableRow> rows, List<string> fromCurreciesSymbols, string title)
        {
            Rows = new ObservableCollection<TableRow>();
            foreach (var v in rows)
            {
                Rows.Add(v);
            }
            FromCurrenciesSymbols = new ObservableCollection<string>();
            foreach (var v in fromCurreciesSymbols)
            {
                FromCurrenciesSymbols.Add(v);
            }
            InitializeComponent();
            DataContext = this;
            CurrencyTitle = title;
            
        }

        private void DisplayTableButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                int n = DisplayTableCB.SelectedIndex;
                selectedIndex = n;
                ((MainWindow)this.Owner).ShowTable(n);
            }
            catch
            {
                SplashScreenWindow viewer = new SplashScreenWindow();
                Thread viewerThread = new Thread(delegate ()
                {
                    viewer = new SplashScreenWindow();
                    viewer.Show();
                    System.Windows.Threading.Dispatcher.Run();
                });
                //System.Windows.Threading.Dispatcher.FromThread(viewerThread).InvokeShutdown();
                MessageBox.Show("Allowed API call frequency is 5 calls per minute and 500 calls per day", "Call limit exceeded", MessageBoxButton.OK, MessageBoxImage.Warning);
            }


        }

        private void ListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }
        protected override void OnClosing(CancelEventArgs e)
        {
            this.Visibility = Visibility.Hidden;
            e.Cancel = true;
        }
    }
}
