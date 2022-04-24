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

namespace HCI_MiniProjekat
{
    /// <summary>
    /// Interaction logic for CurrencyTable.xaml
    /// </summary>
    public partial class CurrencyTable : Window
    {
        public List<TableRow> Rows { get; set; }
        public string Currency { get; set; }
        public List<string> FromCurrenciesSymbols { get; set; }
        public string CurrencyTitle { get; set; }

        public CurrencyTable(List<TableRow> rows, List<string> fromCurreciesSymbols, string title)
        {
            Rows = rows;
            FromCurrenciesSymbols = fromCurreciesSymbols;
            InitializeComponent();
            DataContext = this;
            CurrencyTitle = title;
            
        }

        private void DisplayTableButton_Click(object sender, RoutedEventArgs e)
        {
        
        }

    }
}
