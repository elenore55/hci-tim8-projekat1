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
        public List<string> FromCurreciesSymbols { get; set; } = new List<string>();
        public string ToCurrency { get; set; } = "";
        public List<string> Currencies { get; set; }
        public string minutes;
        public SplashScreenWindow viewer = new SplashScreenWindow();
        public Dictionary<string, RouteValueDictionary> DataPerCurrency { get; set; } = new Dictionary<string, RouteValueDictionary>();

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
            DisplayTable.Visibility = Visibility.Collapsed;
        }

        public List<Axis> XAxes { get; set; }
            = new List<Axis>
            {
                new Axis
                {
                    Name = "DateTime",
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
                    Name = "DateTime",
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
                FromCurreciesSymbols.Add(selected.Split()[0]);
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
                FromDate.Visibility = Visibility.Hidden;
                ToDate.Visibility = Visibility.Hidden;
                FromTime.Visibility = Visibility.Visible;
                ToTime.Visibility = Visibility.Visible;
            }
            else
            {
                TimeInterval.Visibility = Visibility.Collapsed;
                FromDate.Visibility = Visibility.Visible;
                ToDate.Visibility = Visibility.Visible;
                FromTime.Visibility = Visibility.Hidden;
                ToTime.Visibility = Visibility.Hidden;
            }
        }

        private void Fetch_Click(object sender, RoutedEventArgs e)
        {
            if (FromCurrecies.Count == 0 || ToCurrency == "" || Type.SelectedItem == null || Intertval.SelectedItem == null)
            {
                MessageBox.Show("You did not fill in the required information!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                MarkRed();
            }
            else
            {
                if (Intertval.Text != "Intraday" && !AreDatesChronological())
                {
                    MessageBox.Show("Dates must be chosen chronologically!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                else if (Intertval.Text == "Intraday" && !AreTimesChronological())
                {
                    MessageBox.Show("Times must be chosen chronologically!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
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
                    DisplayTable.Visibility = Visibility.Visible;
                    DisplayTableCB.SelectedIndex = 0;
                }
            }
        }

        private void MarkRed()
        {
            MarkFromCurrencyRed();
            MarkToCurrencyRed();
            MarkTypeRed();
            MarkIntervalRed();
            MarkTimeIntervalRed();
        }

        public void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            PropertyChanged?.Invoke(this, e);
        }

        private void DisplayChart(List<string> errors)
        {
            List<ISeries> tempSeries = new List<ISeries>();
            List<ISeries> tempSeriesLine = new List<ISeries>();

            YAxes.ElementAt(0).Name = $"Value - {Type.Text.Substring(3)}";
            YAxesLine.ElementAt(0).Name = $"Value - {Type.Text.Substring(3)}";
            XAxes.ElementAt(0).Name = $"DateTime - {Intertval.Text}";
            XAxesLine.ElementAt(0).Name = $"DateTime - {Intertval.Text}";

            string to = ToCurrency.Substring(0, 3);

            foreach (string curr in FromCurrecies)
            {
                string curr_key = $"{curr},{to},{Intertval.Text},{TimeInterval.Text}";
                if (!DataPerCurrency.ContainsKey(curr_key))
                {
                    string queryURL = FormURL(curr.Substring(0, 3));
                    Uri queryUri = new Uri(queryURL);

                    using (WebClient client = new WebClient())
                    {
                        JavaScriptSerializer js = new JavaScriptSerializer();
                        dynamic json_data = js.Deserialize(client.DownloadString(queryUri), typeof(object));
                        DataPerCurrency[curr_key] = new RouteValueDictionary(json_data); ;     
                    }
                }

                string data_key;
                if (Intertval.Text != "Intraday") data_key = $"Time Series FX ({Intertval.Text})";
                else data_key = $"Time Series FX ({minutes})";

                RouteValueDictionary jsonDataDict = DataPerCurrency[curr_key];

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

                dynamic data = jsonDataDict[data_key];
                RouteValueDictionary result = new RouteValueDictionary(data);
                ChartValues<double> values = new ChartValues<double>();

                XAxes.ElementAt(0).Labels.Clear();
                XAxesLine.ElementAt(0).Labels.Clear();

                if (Intertval.Text == "Intraday")
                {
                    string k = result.Keys.ElementAt(0);
                    XAxes.ElementAt(0).Name += $" ({k.Substring(0, 10)})";
                    XAxesLine.ElementAt(0).Name += $" ({k.Substring(0, 10)})";
                }

                foreach (string key in result.Keys)
                {
                    DateTime oDate = Convert.ToDateTime(key);
                    if (IsInInterval(oDate))
                    {
                        string label = key;
                        if (Intertval.Text == "Intraday")
                            label = key.Substring(11, 5);
                        XAxes.ElementAt(0).Labels.Insert(0, label);
                        XAxesLine.ElementAt(0).Labels.Insert(0, label);
                        object obj;
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

            Series = tempSeries;
            SeriesLine = tempSeriesLine;
            Thread.Sleep(500);
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
            if (Intertval.Text != "Intraday")
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
            if (FromTime.SelectedTime == null && ToTime.SelectedTime == null) return true;
            string timeStr = date.ToShortTimeString();
            string timeStrFrom;
            string timeStrTo;
            if (FromTime.SelectedTime == null)
            {
                timeStrTo = ToTime.Text;
                if (timeStrTo.Length == 4) timeStrTo = "0" + timeStrTo;
                return timeStr.CompareTo(timeStrTo) <= 0;
            }
            if (ToTime.SelectedTime == null)
            {
                timeStrFrom = FromTime.Text;
                if (timeStrFrom.Length == 4) timeStrFrom = "0" + timeStrFrom;
                return timeStr.CompareTo(timeStrFrom) >= 0;
            }
            timeStrFrom = FromTime.Text;
            if (timeStrFrom.Length == 4) timeStrFrom = "0" + timeStrFrom;
            timeStrTo = ToTime.Text;
            if (timeStrTo.Length == 4) timeStrTo = "0" + timeStrTo;
            return timeStr.CompareTo(timeStrTo) <= 0 && timeStr.CompareTo(timeStrFrom) >= 0;
        }

        private bool AreDatesChronological()
        {
            if (FromDate.SelectedDate != null && ToDate.SelectedDate != null)
            {
                return FromDate.SelectedDate.GetValueOrDefault().CompareTo(ToDate.SelectedDate.GetValueOrDefault()) <= 0;
            }
            return true;
        }

        private bool AreTimesChronological()
        {
            if (FromTime.SelectedTime != null && ToTime.SelectedTime != null)
            {
                return FromTime.SelectedTime.GetValueOrDefault().CompareTo(ToTime.SelectedTime.GetValueOrDefault()) <= 0;
            }
            return true;
        }

        private void CartesianChart_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            
        }

        private void CartesianChartLine_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            
        }

        private void Window_Closing(object sender, CancelEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void DisplayTableButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void tb2_LostFocus(object sender, RoutedEventArgs e)
        {
            MarkToCurrencyRed();
        }

        private void tb2_GotFocus(object sender, RoutedEventArgs e)
        {
            tb2_border.BorderThickness = new Thickness(0, 0, 0, 0);
        }

        private void tb_LostFocus(object sender, RoutedEventArgs e)
        {
            MarkFromCurrencyRed();
        }

        private void tb_GotFocus(object sender, RoutedEventArgs e)
        {
            tb_border.BorderThickness = new Thickness(0, 0, 0, 0);
        }

        private void Type_LostFocus(object sender, RoutedEventArgs e)
        {
            MarkTypeRed();
        }

        private void Type_GotFocus(object sender, RoutedEventArgs e)
        {
            Type.BorderBrush = Brushes.DarkSlateGray;
        }

        private void Intertval_LostFocus(object sender, RoutedEventArgs e)
        {
            MarkIntervalRed();
        }

        private void Intertval_GotFocus(object sender, RoutedEventArgs e)
        {
            Intertval.BorderBrush = Brushes.DarkSlateGray;
        }

        private void TimeInterval_LostFocus(object sender, RoutedEventArgs e)
        {
            MarkTimeIntervalRed();
        }

        private void TimeInterval_GotFocus(object sender, RoutedEventArgs e)
        {
            TimeInterval.BorderBrush = Brushes.DarkSlateGray;
        }

        private void MarkFromCurrencyRed()
        {
            if (FromCurrecies.Count == 0)
            {
                tb_border.BorderThickness = new Thickness(0, 0, 0, 2);
                tb_border.BorderBrush = Brushes.Red;
            }
        }

        private void MarkToCurrencyRed()
        {
            if (ToCurrency == "")
            {
                tb2_border.BorderThickness = new Thickness(0, 0, 0, 2);
                tb2_border.BorderBrush = Brushes.Red;
            }
        }

        private void MarkTypeRed()
        {
            if (Type.SelectedItem == null)
            {
                Type.BorderThickness = new Thickness(0, 0, 0, 1);
                Type.BorderBrush = Brushes.Red;
            }
        }

        private void MarkIntervalRed()
        {
            if (Intertval.SelectedItem == null)
            {
                Intertval.BorderThickness = new Thickness(0, 0, 0, 1);
                Intertval.BorderBrush = Brushes.Red;
            }
        }

        private void MarkTimeIntervalRed()
        {
            if (TimeInterval.SelectedItem == null)
            {
                TimeInterval.BorderThickness = new Thickness(0, 0, 0, 1);
                TimeInterval.BorderBrush = Brushes.Red;
            }
        }
    }
}
