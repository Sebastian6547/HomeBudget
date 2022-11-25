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
using Budget;

namespace HomeBudgetWPF
{
    /// <summary>
    /// Interaction logic for CreateExpenseWindow.xaml
    /// </summary>
    public partial class CreateExpenseWindow : Window,CreateExpenseInterface
    {
        private readonly Presenter presenter;
        public CreateExpenseWindow(Presenter presenter)
        {
            InitializeComponent();
            datePicker.SelectedDate = DateTime.Today;
            this.presenter = presenter;
            this.presenter.CreateExpenseView = this;
            presenter.Populate(this);
            addedSuccess.Visibility = Visibility.Hidden;
        }

        //Event handlers
        private void createCategory_Click(object sender, RoutedEventArgs e)
        {
            CreateCategoryWindow createCategoryWindow = new CreateCategoryWindow(this.presenter);

            createCategoryWindow.ShowDialog();

            presenter.Populate(this);
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            datePicker.SelectedDate = DateTime.Today;
            txtAmount.Text = "";
            txtDescription.Text = "";
            cmbCategories.SelectedItem = null;
            Application.Current.Shutdown();
        }

        private void cancelbtn_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void createbtn_Click(object sender, RoutedEventArgs e)
        {
            presenter.CreateExpense(datePicker.SelectedDate, txtAmount.Text, txtDescription.Text, cmbCategories.SelectedIndex);
        }
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            e.Cancel = !CheckToClose();
            this.Visibility = Visibility.Hidden;
        }
        private bool CheckToClose()
        {
            if (datePicker.SelectedDate == DateTime.Today && txtAmount.Text == ""&& txtDescription.Text==""&&cmbCategories.SelectedItem==null)
                return true;
            MessageBoxResult messageBoxResult = MessageBox.Show("Are you sure you want to close this window?", "Unsaved Changes", MessageBoxButton.YesNo, MessageBoxImage.Warning);
            if (messageBoxResult == MessageBoxResult.Yes)
                return true;
            else
                return false;
        }

        //Interface Methods
        public void ShowSuccess()
        {
            //If create is successful: reset fields:
            datePicker.SelectedDate = DateTime.Today;
            txtAmount.Text = "";
            txtDescription.Text = "";
            cmbCategories.SelectedItem = null;

            addedSuccess.Visibility = Visibility.Visible;
        }


        public void ShowError(string msg)
        {
            MessageBox.Show(msg);
        }

        public void BindCategories(List<Category> categories)
        {
            cmbCategories.ItemsSource = categories;
        }
    }
}
