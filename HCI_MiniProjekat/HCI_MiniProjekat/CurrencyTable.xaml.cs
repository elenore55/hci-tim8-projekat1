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

        public CurrencyTable(List<TableRow> rows)
        {
            //Rows = ((MainWindow)Application.Current.MainWindow).Rows;
            Rows = rows;
            InitializeComponent();
            DataContext = this;
            Currency = "Konvertibilna marka";

            
        }
    }
}
