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
using Budget;
using System.Data;
using System.ComponentModel;

namespace HomeBudgetWPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, MainWindowInterface
    {
        private readonly Presenter presenter;
        private CreateExpenseWindow createExpenseWindow;
        private CreateCategoryWindow createCategoryWindow;
        public MainWindow(Presenter givenPresenter)
        {
            InitializeComponent();
            this.presenter = givenPresenter;
            this.presenter.MainWindowView = this;

            presenter.DataView = myDataGrid;
            myDataGrid.presenter = this.presenter;

            myDataChart.Visibility = Visibility.Hidden;

            currentFile.Text += this.presenter.GetCurrentFile();

            createExpenseWindow = new CreateExpenseWindow(this.presenter);
            createCategoryWindow = new CreateCategoryWindow(this.presenter);    
            
            presenter.PopulateCategories(this);
            presenter.PopulateDataView(null, null, false, 0, false, false);
            ContextChanged();
        }

        private void createCategory_Click(object sender, RoutedEventArgs e)
        {
            createCategoryWindow = new CreateCategoryWindow(this.presenter);
            createCategoryWindow.ShowDialog();
            presenter.PopulateCategories(this);
        }

        private void createExpense_Click(object sender, RoutedEventArgs e)
        {
            createExpenseWindow = new CreateExpenseWindow(this.presenter);
            createExpenseWindow.ShowDialog();
            presenter.PopulateCategories(this);
            ContextChanged();
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
            //this.Close();
        }
        public void BindCategories(List<Category> categories)
        {
            cmbCategories.ItemsSource = categories;
        }

        public void ContextChanged()
        {
            DateTime? startDate = datePickerStart.SelectedDate;
            DateTime? endDate = datePickerEnd.SelectedDate;
            bool filterFlag = ctgCheckbox.IsChecked == null ? false : ctgCheckbox.IsChecked == true;
            Category currentCategory = cmbCategories.SelectedItem as Category;
            int categoryId;
            bool isByCategory = (bool) byCategory.IsChecked;
            bool isByMonth = (bool)byMonth.IsChecked;
            bool gridChecked = (bool)Grid.IsChecked;
            bool chartChecked = (bool)Chart.IsChecked;

            if (currentCategory == null)
                categoryId = 1;
            else
                categoryId = currentCategory.Id;

                
            if (chartChecked)
            {
                presenter.DataView = myDataChart;
                myDataChart.presenter = presenter;
                myDataChart.Visibility = Visibility.Visible;
                myDataGrid.Visibility = Visibility.Hidden;
            }
            else
            {
                presenter.DataView = myDataGrid;
                myDataGrid.presenter = presenter;
                myDataGrid.Visibility = Visibility.Visible;
                myDataChart.Visibility = Visibility.Hidden;
            }
            presenter.PopulateDataView(startDate, endDate, filterFlag, categoryId, isByCategory, isByMonth);
        }

        private void datePickerStart_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            ContextChanged();
        }

        private void cmbCategories_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ContextChanged();
        }

        private void Checkbox_Click(object sender, RoutedEventArgs e)
        {
            ContextChanged();
        }

        private void expenseDataGrid_ContextMenuOpening(object sender, ContextMenuEventArgs e)
        {
            if (!(byCategory.IsChecked != true && byMonth.IsChecked != true))
                e.Handled = true;
        }

    }
}
