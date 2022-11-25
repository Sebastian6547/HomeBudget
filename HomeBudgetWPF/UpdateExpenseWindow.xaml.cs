using Budget;
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
using System.Windows.Shapes;

namespace HomeBudgetWPF
{
    /// <summary>
    /// Interaction logic for UpdateExpenseWindow.xaml
    /// </summary>
    public partial class UpdateExpenseWindow : Window, UpdateExpenseInterface
    {
        private readonly Presenter presenter;
        private readonly int expenseId;

        private DateTime defaultDate;
        private string defaultDescription;
        private double defaultAmount;
        private int defaultSelectedIndex;
        public UpdateExpenseWindow(Presenter presenter,int expenseId)
        {
            InitializeComponent();
            this.presenter = presenter;
            this.expenseId = expenseId;
            presenter.UpdateExpenseView = this;
            presenter.Populate(this);
            presenter.SetFields(expenseId);
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            e.Cancel = !CheckToClose();
        }
        private bool CheckToClose()
        {
            if (datePicker.SelectedDate == defaultDate && txtAmount.Text == defaultAmount.ToString() && txtDescription.Text == defaultDescription && cmbCategories.SelectedIndex == defaultSelectedIndex)
                return true;
            MessageBoxResult messageBoxResult = MessageBox.Show("Are you sure you want to close this window?", "Unsaved Changes", MessageBoxButton.YesNo, MessageBoxImage.Warning);
            if (messageBoxResult == MessageBoxResult.Yes)
                return true;
            else
                return false;
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            SetDefaultValues();
            Application.Current.Shutdown();
        }

        private void cancelbtn_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void updatebtn_Click(object sender, RoutedEventArgs e)
        {
            presenter.UpdateExpense(expenseId, datePicker.SelectedDate, txtAmount.Text, txtDescription.Text, cmbCategories.SelectedIndex);
        }

        private void deletebtn_Click(object sender, RoutedEventArgs e)
        {
            presenter.DeleteExpense(expenseId);
            SetDefaultValues();
            Close();
        }

        private void createCategory_Click(object sender, RoutedEventArgs e)
        {
            CreateCategoryWindow createCategoryWindow = new CreateCategoryWindow(this.presenter);

            createCategoryWindow.ShowDialog();

            presenter.Populate(this);
        }
        private void SetDefaultValues()
        {
            datePicker.SelectedDate = defaultDate;
            txtAmount.Text = defaultAmount.ToString();
            txtDescription.Text = defaultDescription;
            cmbCategories.SelectedIndex = defaultSelectedIndex;
        }
        //Interface methods
        public void ShowError(string msg)
        {
            MessageBox.Show(msg);
        }

        public void BindCategories(List<Category> categories)
        {
            cmbCategories.ItemsSource = categories;
        }

        public void FillFields(DateTime date, string description, double amount, int categoryIndex)
        {
            defaultDate = date;
            defaultDescription = description;
            defaultAmount = amount;
            defaultSelectedIndex = categoryIndex;

            SetDefaultValues();
        }

        public void Success()
        {
            SetDefaultValues();
            Close();
        }
    }
}
