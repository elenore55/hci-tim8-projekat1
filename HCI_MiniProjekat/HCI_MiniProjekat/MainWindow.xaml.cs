using Microsoft.VisualBasic.FileIO;
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

namespace HCI_MiniProjekat
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public List<string> FromCurrecies { get; set; } = new List<string>();
        public string ToCurrency { get; set; } = "";
        public List<string> Currencies { get; set; }
        public MainWindow()
        {
            LoadCurrencies();
            InitializeComponent();
            element.Visibility = Visibility.Collapsed;
            tb.ItemsSource = Currencies;
            tb2.ItemsSource = Currencies;
        }

        private void LoadCurrencies()
        {
            List<string> cur = new List<string>();
            using (TextFieldParser parser = new TextFieldParser("..\\..\\..\\physical_currency_list.csv"))
            {
                parser.TextFieldType = FieldType.Delimited;
                parser.SetDelimiters(",");
                parser.ReadLine();
                while (!parser.EndOfData)
                {
                    string[] fields = parser.ReadFields();
                    string c = $"{fields[0]} - {fields[1]}";
                    cur.Add(c);
                }
            }
            using (TextFieldParser parser = new TextFieldParser("..\\..\\..\\digital_currency_list.csv"))
            {
                parser.TextFieldType = FieldType.Delimited;
                parser.SetDelimiters(",");
                parser.ReadLine();
                while (!parser.EndOfData)
                {
                    string[] fields = parser.ReadFields();
                    string c = $"{fields[0]} - {fields[1]}";
                    cur.Add(c);
                }
            }
            Currencies = cur;
        }

        private void tb_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return)
            {
                string selected = tb.Text;
                selected = selected.Trim();
                if (!Currencies.Contains(selected))
                {
                    return;
                }
                if (FromCurrecies.Contains(selected))
                {
                    return;
                }
                FromCurrecies.Add(selected);
                First_List.Items.Refresh();
                tb.Text = "";
            }
        }

        private void tb2_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return)
            {
                string selected = tb2.Text;
                selected = selected.Trim();
                if (!Currencies.Contains(selected))
                {
                    return;
                }
                ToCurrency = selected;
                element.Visibility = 0;
                element.Content = ToCurrency.Split(" ")[0];
                element.ToolTip = ToCurrency;
                //Second.Items.Refresh();
                tb2.Text = "";
            }
        }

        private void Chip_DeleteClick(object sender, RoutedEventArgs e)
        {
            string selected = sender.ToString()[31..];
            foreach (string s in FromCurrecies)
            {
                if (s.StartsWith(selected))
                {
                    FromCurrecies.Remove(s);
                    break;
                }
            }
            First_List.Items.Refresh();
        }

        private void element_DeleteClick(object sender, RoutedEventArgs e)
        {
            ToCurrency = "";
            element.Visibility = Visibility.Collapsed;
        }

        private void Fetch_Click(object sender, RoutedEventArgs e)
        {
            if (FromCurrecies.Count == 0 || ToCurrency == "" || FromDate.SelectedDate == null || ToDate == null || Type.SelectedItem == null)
            {
                MessageBox.Show("Niste Izabarali sve opcije!");
            }
            MessageBox.Show("Izabarali ste sve opcije!");
        }
    }
}
