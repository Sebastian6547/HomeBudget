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
using System.Windows.Shapes;
using Budget;


namespace HomeBudgetWPF
{
    /// <summary>
    /// Interaction logic for CreateCategoryWindow.xaml
    /// </summary>
    public partial class CreateCategoryWindow : Window, CreateCategoryInterface
    {
        Presenter presenter;
        public CreateCategoryWindow(Presenter presenter)
        {
            InitializeComponent();
            this.presenter = presenter;
            cmbCategoryTypes.ItemsSource = Enum.GetValues(typeof(Category.CategoryType)).Cast<Category.CategoryType>().ToList();
            this.presenter.CreateCategoryView = this;
            addedSuccess.Visibility = Visibility.Hidden;
        }

        //Event handlers
        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            txtDescription.Text = "";
            cmbCategoryTypes.SelectedItem = null;
            Application.Current.Shutdown();
        }

        private void cancelbtn_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
        private bool CheckToClose()
        {
            if (txtDescription.Text == ""&&cmbCategoryTypes.SelectedItem==null)
                return true;
            MessageBoxResult messageBoxResult = MessageBox.Show("Are you sure you want to close this window?", "Unsaved Changes", MessageBoxButton.YesNo, MessageBoxImage.Warning);
            if (messageBoxResult == MessageBoxResult.Yes)
                return true;
            else
                return false;
        }
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            e.Cancel = !CheckToClose();
            this.Visibility = Visibility.Hidden;
        }

        private void createbtn_Click(object sender, RoutedEventArgs e)
        {
            if (cmbCategoryTypes.SelectedItem == null)
                MessageBox.Show("All fields need to be filled.");
            else
            {
                presenter.CreateCategory(txtDescription.Text, (Category.CategoryType)cmbCategoryTypes.SelectedItem);
            }
        }

        //Interface methods
        public void ShowSuccess()
        {
            //clear form when successful
            txtDescription.Text = "";
            cmbCategoryTypes.SelectedItem = null;

            //show success
            addedSuccess.Visibility = Visibility.Visible;
        }

        public void ShowError(string msg)
        {
            MessageBox.Show(msg);
        }

    }
}
