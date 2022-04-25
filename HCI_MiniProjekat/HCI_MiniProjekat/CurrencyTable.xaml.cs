using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
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

namespace HCI_MiniProjekat
{
    /// <summary>
    /// Interaction logic for CurrencyTable.xaml
    /// </summary>
    public partial class CurrencyTable : Window
    {
        public ObservableCollection<TableRow> Rows { get; set; }
        public string Currency { get; set; }
        public ObservableCollection<string> FromCurreciesSymbols { get; set; }
        public string CurrencyTitle { get; set; }
        public int selectedIndex { get; set; }
        public CurrencyTable()
        {
            Rows = new ObservableCollection<TableRow>();
            FromCurreciesSymbols = new ObservableCollection<string>();
            InitializeComponent();
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            this.Visibility = Visibility.Hidden;
            e.Cancel = true;
        }

        private void ListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void DisplayTableButton_Click(object sender, RoutedEventArgs e)
        {
            

        }

        internal void setRows(List<TableRow> rows)
        {
            foreach (var r in rows)
            {
                Rows.Add(r);
            }
        }

        internal void setSymbols(List<string> fromCurreciesSymbols)
        {
            foreach (var v in fromCurreciesSymbols)
            {
                fromCurreciesSymbols.Add(v);
            }
        }
    }
}
