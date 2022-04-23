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
using System.Threading;
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
using System.ComponentModel;

namespace HCI_MiniProjekat
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        public List<string> FromCurrecies { get; set; } = new List<string>();
        public string ToCurrency { get; set; } = "";
        public List<string> Currencies { get; set; }
        public string minutes;
        public SplashScreenWindow viewer = new SplashScreenWindow();

        public event PropertyChangedEventHandler PropertyChanged;

        private List<ISeries> _series = new List<ISeries>();
        public List<ISeries> Series
        {
            get
            {
                return _series;
            }
            set
            {
                _series = value;
                OnPropertyChanged(new PropertyChangedEventArgs("Series"));
            }
        }

        private List<ISeries> _seriesLine = new List<ISeries>();
        public List<ISeries> SeriesLine
        {
            get
            {
                return _seriesLine;
            }
            set
            {
                _seriesLine = value;
                OnPropertyChanged(new PropertyChangedEventArgs("SeriesLine"));
            }
        }

        public MainWindow()
        {
            LoadCurrencies();
            InitializeComponent();
            element.Visibility = Visibility.Collapsed;
            TimeInterval.Visibility = Visibility.Collapsed;
            tb.ItemsSource = Currencies;
            tb2.ItemsSource = Currencies;
            ToDate.IsEnabled = false;
        }

        public List<Axis> XAxes { get; set; }
            = new List<Axis>
            {
                new Axis
                {
                    Name = "Date",
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
                    Name = "Value",
                    TextSize = 8,
                    LabelsPaint = new SolidColorPaint(SKColors.Black),
                    SeparatorsPaint = new SolidColorPaint(SKColors.LightSlateGray) { StrokeThickness = 2 }
                }
            };

        public List<Axis> XAxesLine { get; set; }
            = new List<Axis>
            {
                new Axis
                {
                    Name = "Date",
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
                    Name = "Value",
                    TextSize = 8,
                    LabelsPaint = new SolidColorPaint(SKColors.Black),
                    SeparatorsPaint = new SolidColorPaint(SKColors.LightSlateGray) { StrokeThickness = 2 }
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
           /** using (TextFieldParser parser = new TextFieldParser("..\\..\\digital_currency_list.csv"))
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
            }**/
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
                MessageBox.Show("You did not fill in the required information!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            else
            {
                if (FromDate.SelectedDate.GetValueOrDefault().CompareTo(ToDate.SelectedDate.GetValueOrDefault()) > 0)
                {
                    MessageBox.Show("Dates must be chosen chronologically!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                else
                {
                    SeriesLine.Clear();
                    Series.Clear();
                    minutes = TimeInterval.Text;

                    Thread viewerThread = new Thread(delegate ()
                    {
                        viewer = new SplashScreenWindow();
                        viewer.Show();
                        System.Windows.Threading.Dispatcher.Run();
                    });

                    viewerThread.SetApartmentState(ApartmentState.STA);
                    viewerThread.Start();
                    List<string> errors = new List<string>();
                    try
                    {
                        DisplayChart(errors);
                    }
                    catch
                    {
                        System.Windows.Threading.Dispatcher.FromThread(viewerThread).InvokeShutdown();
                        MessageBox.Show("Allowed API call frequency is 5 calls per minute and 500 calls per day", "Call limit exceeded", MessageBoxButton.OK, MessageBoxImage.Warning);
                    }

                    DataContext = this;
                    System.Windows.Threading.Dispatcher.FromThread(viewerThread).InvokeShutdown();
                    if (errors.Count > 0)
                    {
                        string currencies = string.Join(", ", errors);
                        MessageBox.Show($"API provided no data for currencies: {currencies}", "Unavailable data", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                }
            }
        }

        public void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            PropertyChanged?.Invoke(this, e);
        }

        private void DisplayChart(List<string> errors)
        {
            List<ISeries> tempSeries = new List<ISeries>();
            List<ISeries> tempSeriesLine = new List<ISeries>();

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
                    RouteValueDictionary jsonDataDict = new RouteValueDictionary(json_data);
                    if (!jsonDataDict.ContainsKey(data_key))
                    {
                        foreach (string key in jsonDataDict.Keys)
                        {
                            if (key.StartsWith("Note"))
                            {
                                throw new Exception("Call limit exceeded");
                            }
                        }
                        errors.Add(curr);
                        continue;
                    }

                    dynamic data = json_data[data_key];
                    RouteValueDictionary result = new RouteValueDictionary(data);
                    ChartValues<double> values = new ChartValues<double>();

                    XAxes.ElementAt(0).Labels.Clear();
                    XAxesLine.ElementAt(0).Labels.Clear();

                    foreach (string key in result.Keys)
                    {
                        DateTime oDate = Convert.ToDateTime(key);
                        if (IsInInterval(oDate))
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

                    tempSeries.Add(
                        new ColumnSeries<double>
                        {
                            Values = values,
                            Name = curr,
                            GroupPadding = 10

                        }
                    );

                    tempSeriesLine.Add(
                        new LineSeries<double>
                        {
                            Values = values,
                            Name = curr,
                            GeometrySize = 10,
                            Fill = null
                        }
                    );
                }
            }

            Series = tempSeries;
            SeriesLine = tempSeriesLine;
        }

        private string FormURL(string from)
        {
            string fn = $"FX_{Intertval.Text.ToUpper()}";
            string to = ToCurrency.Substring(0, 3);
            if (fn != "FX_INTRADAY")
                return $"https://www.alphavantage.co/query?function={fn}&from_symbol={from}&to_symbol={to}&apikey=UWN3B1CJC7X8TNGE";
            return $"https://www.alphavantage.co/query?function={fn}&from_symbol={from}&to_symbol={to}&interval={TimeInterval.Text}&apikey=UWN3B1CJC7X8TNGE";
        }

        private bool IsInInterval(DateTime date)
        {
            if (FromDate.SelectedDate == null && ToDate.SelectedDate == null) return true;
            if (FromDate.SelectedDate == null)
            {
                return DateTime.Compare(date, ToDate.SelectedDate.Value) <= 0;
            }
            if (ToDate.SelectedDate == null)
            {
                return DateTime.Compare(date, FromDate.SelectedDate.Value) >= 0;
            }
            return DateTime.Compare(date, ToDate.SelectedDate.Value) <= 0 && DateTime.Compare(date, FromDate.SelectedDate.Value) >= 0;
        }

        private void CartesianChart_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            MessageBox.Show("Double clicked!");
        }

        private void CartesianChartLine_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            MessageBox.Show("Double clicked line");
        }

        private void Window_Closing(object sender, CancelEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void FromDate_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            ToDate.IsEnabled = true;
        }
    }
}
