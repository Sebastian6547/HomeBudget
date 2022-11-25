using System;
using System.Collections.Generic;
using System.Linq;
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
using Budget;

namespace HomeBudgetWPF
{
    /// <summary>
    /// Interaction logic for DataGridView.xaml
    /// </summary>
    public partial class DataGridView : UserControl, DataViewInterface
    {
        public Presenter presenter { get; set; }
        private bool disableGridInteraction = false;
        private List<object> _dataSource;

        public List<object> DataSource
        {
            get { return _dataSource; }
            set
            {
                _dataSource = value;
            }
        }
        
        public DataGridView()
        {
            InitializeComponent();
        }

        public void ShowData()
        {
            expenseDataGrid.Columns.Clear();
            expenseDataGrid.ItemsSource = DataSource;
            expenseDataGrid.Columns.Clear();

            DataGridTextColumn date = new DataGridTextColumn();
            DataGridTextColumn category = new DataGridTextColumn();
            DataGridTextColumn description = new DataGridTextColumn();
            DataGridTextColumn amount = new DataGridTextColumn();
            DataGridTextColumn balance = new DataGridTextColumn();

            date.Header = "Date";
            category.Header = "Category";
            description.Header = "Description";
            amount.Header = "Amount";
            balance.Header = "Balance";

            date.Binding = new Binding("Date");
            category.Binding = new Binding("Category");
            description.Binding = new Binding("ShortDescription");
            amount.Binding = new Binding("Amount");
            balance.Binding = new Binding("Balance");

            balance.Binding.StringFormat = "F2";
            Style b = new Style();
            b.Setters.Add(new Setter(TextBlock.TextAlignmentProperty, TextAlignment.Right));
            balance.CellStyle = b;

            amount.Binding.StringFormat = "F2";
            Style a = new Style();
            a.Setters.Add(new Setter(TextBlock.TextAlignmentProperty, TextAlignment.Right));
            amount.CellStyle = a;
            
            expenseDataGrid.Columns.Add(date);
            expenseDataGrid.Columns.Add(category);
            expenseDataGrid.Columns.Add(description);
            expenseDataGrid.Columns.Add(amount);
            expenseDataGrid.Columns.Add(balance);

            disableGridInteraction = false;

            searchLabel.Visibility = Visibility.Visible;
            txtSearchBox.Visibility = Visibility.Visible;
            btnSearch.Visibility = Visibility.Visible;
        }

        public void ShowDataByMonth()
        {
            expenseDataGrid.ItemsSource = DataSource;
            expenseDataGrid.Columns.Clear();

            DataGridTextColumn month = new DataGridTextColumn();
            month.Header = "Month";
            month.Binding = new Binding("Month");

            DataGridTextColumn total = new DataGridTextColumn();
            total.Header = "Total";
            total.Binding = new Binding("Total");


            total.Binding.StringFormat = "F2";
            Style s = new Style();
            s.Setters.Add(new Setter(TextBlock.TextAlignmentProperty, TextAlignment.Right));
            total.CellStyle = s;


            expenseDataGrid.Columns.Add(month);
            expenseDataGrid.Columns.Add(total);

            disableGridInteraction = true;

            searchLabel.Visibility = Visibility.Hidden;
            txtSearchBox.Visibility = Visibility.Hidden;
            btnSearch.Visibility = Visibility.Hidden;
        }

        public void ShowDataByCategory()
        {
            expenseDataGrid.ItemsSource = DataSource;
            expenseDataGrid.Columns.Clear();

            DataGridTextColumn category = new DataGridTextColumn();
            category.Header = "Category";
            category.Binding = new Binding("Category");

            DataGridTextColumn total = new DataGridTextColumn();
            total.Header = "Total";
            total.Binding = new Binding("Total");


            total.Binding.StringFormat = "F2";
            Style s = new Style();
            s.Setters.Add(new Setter(TextBlock.TextAlignmentProperty, TextAlignment.Right));
            total.CellStyle = s;

            expenseDataGrid.Columns.Add(category);
            expenseDataGrid.Columns.Add(total);

            disableGridInteraction = true;

            searchLabel.Visibility = Visibility.Hidden;
            txtSearchBox.Visibility = Visibility.Hidden;
            btnSearch.Visibility = Visibility.Hidden;
        }

        public void ShowDataByMonthByCategory()
        {
            expenseDataGrid.ItemsSource = DataSource;
            expenseDataGrid.Columns.Clear();
            List<string> categories = presenter.GetCategoriesAsString();

            DataGridTextColumn month = new DataGridTextColumn();
            month.Header = "Month";
            month.Binding = new Binding("[Month]");
            expenseDataGrid.Columns.Add(month);


            foreach (string category in categories)
            {
                DataGridTextColumn categoryColumn = new DataGridTextColumn();
                categoryColumn.Header = category;
                categoryColumn.Binding = new Binding($"[{category}]");
                expenseDataGrid.Columns.Add(categoryColumn);
            }

            DataGridTextColumn total = new DataGridTextColumn();
            total.Header = "Total";
            total.Binding = new Binding("[Total]");


            total.Binding = new Binding("Total");
            total.Binding.StringFormat = "F2";
            Style s = new Style();
            s.Setters.Add(new Setter(TextBlock.TextAlignmentProperty, TextAlignment.Right));
            total.CellStyle = s;

            expenseDataGrid.Columns.Add(total);

            disableGridInteraction = true;

            searchLabel.Visibility = Visibility.Hidden;
            txtSearchBox.Visibility = Visibility.Hidden;
            btnSearch.Visibility = Visibility.Hidden;
        }

        private void updateBudgetItem(object sender, RoutedEventArgs e)
        {
            BudgetItem budgetItem = expenseDataGrid.SelectedItem as BudgetItem;
            int selectedIndex = expenseDataGrid.SelectedIndex;
            if (budgetItem != null)
            {
                UpdateExpenseWindow updateExpenseWindow = new UpdateExpenseWindow(presenter, budgetItem.ExpenseID);
                updateExpenseWindow.ShowDialog();
            }
            SelectDataGridItem(selectedIndex);
            if (expenseDataGrid.Items.Count == selectedIndex)
            {
                SelectDataGridItem(expenseDataGrid.Items.Count - 1);
            }
        }

        private void deleteBudgetItem(object sender, RoutedEventArgs e)
        {
            BudgetItem budgetItem = expenseDataGrid.SelectedItem as BudgetItem;
            int selectedIndex = expenseDataGrid.SelectedIndex;
            if (budgetItem != null)
            {
                presenter.DeleteExpense(budgetItem.ExpenseID);
            }
            if(expenseDataGrid.Items.Count == selectedIndex)
            {
                SelectDataGridItem(expenseDataGrid.Items.Count - 1);
            }
            else
            {
                SelectDataGridItem(selectedIndex);
            }

        }
        private void Row_DoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (!disableGridInteraction)
            {
                BudgetItem budgetItem = expenseDataGrid.SelectedItem as BudgetItem;
                int selectedIndex = expenseDataGrid.SelectedIndex;
                if (budgetItem != null)
                {
                    UpdateExpenseWindow updateExpenseWindow = new UpdateExpenseWindow(presenter, budgetItem.ExpenseID);
                    updateExpenseWindow.ShowDialog();
          
                }
                SelectDataGridItem(selectedIndex);
                if (expenseDataGrid.Items.Count == selectedIndex)
                {
                    SelectDataGridItem(expenseDataGrid.Items.Count - 1);
                }
                expenseDataGrid.Focus();
            }
        }

        private void expenseDataGrid_ContextMenuOpening(object sender, ContextMenuEventArgs e)
        {
           if (disableGridInteraction)
                e.Handled = true;
        }
        private void btnSearch_Click(object sender, RoutedEventArgs e)
        {
            int startIndex = expenseDataGrid.SelectedIndex;

            List<BudgetItem> budgetItems = new List<BudgetItem>();
            for (int i = 0; i < expenseDataGrid.Items.Count; i++)
                budgetItems.Add(expenseDataGrid.Items[i] as BudgetItem);

            presenter.UpdateSearchResult(startIndex,budgetItems,txtSearchBox.Text);
            //if (startIndex == -1)
            //    startIndex = 0;
            //if (startIndex == expenseDataGrid.Items.Count - 1)
            //    startIndex = -1;
            //int index = startIndex+1;
            //while (!(index==startIndex))
            //{
            //    BudgetItem budgetItem = expenseDataGrid.Items[index] as BudgetItem;
            //    if (budgetItem.ShortDescription.Contains(txtSearchBox.Text) || budgetItem.Amount.ToString().Contains(txtSearchBox.Text))
            //    {
            //        expenseDataGrid.SelectedIndex = index;
            //        expenseDataGrid.Focus();
            //        expenseDataGrid.ScrollIntoView(expenseDataGrid.SelectedItem);
            //        return;
            //    }
            //    index++;
            //    if (index >= expenseDataGrid.Items.Count)
            //        index = 0;
            //}
            //MessageBox.Show("No Search Results");
        }

        public void SelectDataGridItem(int index)
        {
            expenseDataGrid.SelectedIndex = index;
            expenseDataGrid.Focus();
            expenseDataGrid.ScrollIntoView(expenseDataGrid.SelectedItem);
        }

        public void ShowError(string msg)
        {
            MessageBox.Show(msg);
        }
    }
}
