using LiveChartsCore;
using LiveChartsCore.SkiaSharpView.Painting;
using SkiaSharp;
using System;
using Microsoft.VisualBasic.FileIO;
using System.Web.Script.Serialization;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using LiveCharts;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

using System.Windows.Navigation;
using System.Windows.Shapes;
using LiveChartsCore.SkiaSharpView;
using Microsoft.AspNetCore.Routing;

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
        public string minutes;

        public List<ISeries> Series { get; set; } = new List<ISeries>();
        public List<ISeries> SeriesLine { get; set; } = new List<ISeries>();

        public MainWindow()
        {
            LoadCurrencies();
            InitializeComponent();
            element.Visibility = Visibility.Collapsed;
            TimeInterval.Visibility = Visibility.Collapsed;
            tb.ItemsSource = Currencies;
            tb2.ItemsSource = Currencies;
        }

        public List<Axis> XAxes { get; set; }
            = new List<Axis>
            {
                new Axis
                {
                    Name = "My X Axis",
                    NamePaint = new SolidColorPaint(SKColors.Black),
                    Labels = new List<string>(),
                    LabelsPaint = new SolidColorPaint(SKColors.Blue),
                    TextSize = 8,
                    SeparatorsPaint = new SolidColorPaint(SKColors.LightSlateGray) { StrokeThickness = 2 }
                }
            };

        public List<Axis> YAxes { get; set; }
            = new List<Axis>
            {
                new Axis
                {
                    Name = "My Y Axis",
                    TextSize = 8,
                    LabelsPaint = new SolidColorPaint(SKColors.Black),
                    // Labeler = Labelers.Default
                }
            };

        public List<Axis> XAxesLine { get; set; }
            = new List<Axis>
            {
                new Axis
                {
                    Name = "My X Axis",
                    NamePaint = new SolidColorPaint(SKColors.Black),
                    Labels = new List<string>(),
                    LabelsPaint = new SolidColorPaint(SKColors.Blue),
                    TextSize = 8,
                    SeparatorsPaint = new SolidColorPaint(SKColors.LightSlateGray) { StrokeThickness = 2 }
                }
            };

        public List<Axis> YAxesLine { get; set; }
            = new List<Axis>
            {
                new Axis
                {
                    Name = "My Y Axis",
                    TextSize = 8,
                    LabelsPaint = new SolidColorPaint(SKColors.Black),
                    // Labeler = Labelers.Default
                }
            };

        private void LoadCurrencies()
        {
            List<string> cur = new List<string>();
            using (TextFieldParser parser = new TextFieldParser("..\\..\\physical_currency_list.csv"))
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
            using (TextFieldParser parser = new TextFieldParser("..\\..\\digital_currency_list.csv"))
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
                element.Content = ToCurrency.Split(' ')[0];
                element.ToolTip = ToCurrency;
                //Second.Items.Refresh();
                tb2.Text = "";
            }
        }

        private void Chip_DeleteClick(object sender, RoutedEventArgs e)
        {
            string selected = sender.ToString().Substring(sender.ToString().Length - 3); // [31..]
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

        private void Intertval_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (Intertval.SelectedIndex == 0)
            {
                TimeInterval.Visibility = Visibility.Visible;
            }
            else
            {
                TimeInterval.Visibility = Visibility.Collapsed;
            }
        }

        private void Fetch_Click(object sender, RoutedEventArgs e)
        {
            if (FromCurrecies.Count == 0 || ToCurrency == "" || FromDate.SelectedDate == null || ToDate == null || Type.SelectedItem == null || Intertval.SelectedItem == null)
            {
                MessageBox.Show("Niste Izabarali sve opcije!");
            }
            else
            {
                if (FromDate.SelectedDate.GetValueOrDefault().CompareTo(ToDate.SelectedDate.GetValueOrDefault()) > 0)
                {
                    MessageBox.Show("Datumi moraju biti izabrani hronoloski!");
                }
                MessageBox.Show("Izabarali ste sve opcije!");
                DataContext = this;
                minutes = TimeInterval.Text;
                DisplayChart();
            }
        }

        private void DisplayChart()
        {
            DateTime startDate = FromDate.SelectedDate.GetValueOrDefault();
            DateTime endDate = ToDate.SelectedDate.GetValueOrDefault();

            foreach (string curr in FromCurrecies)
            {
                string queryURL = FormURL(curr.Substring(0, 3));
                Uri queryUri = new Uri(queryURL);

                using (WebClient client = new WebClient())
                {
                    JavaScriptSerializer js = new JavaScriptSerializer();
                    dynamic json_data = js.Deserialize(client.DownloadString(queryUri), typeof(object));
                    string data_key;
                    if (Intertval.Text != "Intraday") data_key = $"Time Series FX ({Intertval.Text})";
                    else data_key = $"Time Series FX ({minutes})";
                    dynamic data = json_data[data_key];
                    RouteValueDictionary result = new RouteValueDictionary(data);
                    ChartValues<double> values = new ChartValues<double>();

                    foreach (var key in result.Keys)
                    {
                        DateTime oDate = Convert.ToDateTime(key);
                        if (DateTime.Compare(oDate, startDate) >= 0 && DateTime.Compare(oDate, endDate) <= 0)
                        {
                            XAxes.ElementAt(0).Labels.Add(key);
                            XAxesLine.ElementAt(0).Labels.Add(key);
                            object obj = new object();
                            result.TryGetValue(key, out obj);
                            RouteValueDictionary d = new RouteValueDictionary(obj);
                            d.TryGetValue(Type.Text.ToLower(), out obj);
                            double num = Convert.ToDouble(obj);
                            values.Add(num);
                        }
                    }

                    Series.Add(
                        new ColumnSeries<double>
                        {
                            Values = values,
                            Name = curr,
                            GroupPadding = 10
                        }
                    );

                    SeriesLine.Add(
                        new LineSeries<double>
                        {
                            Values = values,
                            Stroke = new SolidColorPaint(SKColors.Red) { StrokeThickness = 2 },
                            Name = curr,
                            GeometrySize = 10,
                            Fill = null
                        }
                    );
                }
            }
        }

        private string FormURL(string from)
        {
            string fn = $"FX_{Intertval.Text.ToUpper()}";
            string to = ToCurrency.Substring(0, 3);
            if (fn != "FX_INTRADAY")
                return $"https://www.alphavantage.co/query?function={fn}&from_symbol={from}&to_symbol={to}&apikey=UWN3B1CJC7X8TNGE";
            return $"https://www.alphavantage.co/query?function={fn}&from_symbol={from}&to_symbol={to}&interval={TimeInterval.Text}&apikey=UWN3B1CJC7X8TNGE";
        }

        private void CartesianChart_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            MessageBox.Show("Double clicked!");
        }
    }
}
