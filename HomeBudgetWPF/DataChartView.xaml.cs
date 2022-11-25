using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Controls.DataVisualization.Charting;

namespace HomeBudgetWPF
{
    /// <summary>
    /// Interaction logic for DataChartView.xaml
    /// </summary>
    public partial class DataChartView : UserControl, DataViewInterface
    {
        private List<object> _dataSource;
        public Presenter presenter { get; set; }
        private List<string> Categories;
        private enum ChartType
        {
            Standard,
            ByCategory,
            ByMonth,
            ByMonthAndCategory
        }
        private ChartType chartType = ChartType.Standard;

        public List<object> DataSource
        {
            get { return _dataSource; }
            set
            {
                _dataSource = value;
            }
        }
        public DataChartView()
        {
            InitializeComponent();

        }

        public void ShowData()
        {
            chPie.Visibility = Visibility.Hidden;
            txtInvalid.Visibility = Visibility.Visible;
        }

        public void ShowDataByCategory()
        {
            chPie.Visibility = Visibility.Hidden;
            txtInvalid.Visibility = Visibility.Visible;
        }

        public void ShowDataByMonth()
        {
            chPie.Visibility = Visibility.Hidden;
            txtInvalid.Visibility = Visibility.Visible;
        }
        public void DataClear()
        {
            ((PieSeries)chPie.Series[0]).ItemsSource = null;
        }

        public void ShowDataByMonthByCategory()
        {
            txtTitle.Text = "By Month";
            chartType = ChartType.ByMonthAndCategory;
            chPie.Visibility = Visibility.Visible;
            txtInvalid.Visibility = Visibility.Hidden;
            this.Categories = presenter.GetCategoriesAsString(); // save the categories
            drawByMonthPieChart();
        }

        private void drawByMonthPieChart()
        {
            List<string> months = new List<string>();
            foreach (object obj in _dataSource)
            {
                var item = obj as Dictionary<string, object>;
                if (item != null)
                {
                    months.Add(item["Month"].ToString());
                }
            }
            cbMonths.ItemsSource = months;
            cbMonths.SelectedIndex = -1;
            set_MonthCategory_Data();

        }

        private void set_MonthCategory_Data()
        {
            DataClear();

            if (cbMonths.Items.Count == 0) return;

            if (cbMonths.SelectedIndex < 0 || cbMonths.SelectedIndex > cbMonths.Items.Count - 1)
            {
                cbMonths.SelectedIndex = cbMonths.Items.Count - 1;
            }

            string selectedMonth = cbMonths.SelectedItem.ToString();
            var DisplayData = new List<KeyValuePair<string, double>>();
            foreach (object obj in _dataSource)
            {
                var item = obj as Dictionary<string, object>;

                if (item != null && (string)item["Month"] == selectedMonth)
                {
                    foreach (var pair in item)
                    {
                        string category = pair.Key;
                        string value = pair.Value.ToString();
                        if (Categories == null)
                            Categories = presenter.GetCategoriesAsString();
                        // if the key is not a category, skip processing
                        if (!Categories.Contains(category)) continue;
                        // what is the amount of money for this category (item[category]);
                        var amount = 0.0;
                        double.TryParse(value, out amount);
                        // only display expenses (i.e., amount < 0)
                        if (amount > 0)
                        {
                            DisplayData.Add(new KeyValuePair<string, double>(category, -amount));
                        }
                    }

                    break;
                }
            }
            ((PieSeries)chPie.Series[0]).ItemsSource = DisplayData;
        }

        private void cbMonths_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            set_MonthCategory_Data();
        }

        public void ShowError(string msg)
        {
            throw new NotImplementedException();
        }

        public void SelectDataGridItem(int index)
        {
            throw new NotImplementedException();
        }
    }
}
