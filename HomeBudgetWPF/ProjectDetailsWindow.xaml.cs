using Microsoft.Win32;
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
using System.IO;
using System.Windows.Forms;

namespace HomeBudgetWPF
{
    /// <summary>
    /// Interaction logic for ProjectDetailsWindow.xaml
    /// </summary>
    public partial class ProjectDetailsWindow : Window, ProjectDetailsWindowInterface
    {
        private readonly Presenter presenter;
        private readonly Window parentWindow;
        private string folderPath;
        public ProjectDetailsWindow(Window window)
        {
            InitializeComponent();
            presenter = new Presenter(this);
            parentWindow = window;
        }
        private void Open_Folder(object sender, RoutedEventArgs e)
        {
            System.Windows.Controls.Button myButton = sender as System.Windows.Controls.Button;
            FolderBrowserDialog folderBrowser = new FolderBrowserDialog();
            if (folderBrowser.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                folderPath = folderBrowser.SelectedPath;
                buttonText.Text = "Change folder"; 
            }
        }

        private void SubmitDetails_Click(object sender, RoutedEventArgs e)
        {
            if (folderPath != null && inputName.Text.Length != 0 && inputName.Text.IndexOfAny(System.IO.Path.GetInvalidFileNameChars())  < 0)
                presenter.ProcessNewFile(folderPath + "\\" + inputName.Text + ".db");
            else
            {
                MessageBoxResult result = System.Windows.MessageBox.Show("Please enter a valid filename and valid folder", "Invalid input", MessageBoxButton.OK);
            }
        }

        public void OpenMainCloseLaunch()
        {
            System.Windows.Application.Current.MainWindow = new MainWindow(this.presenter);
            System.Windows.Application.Current.MainWindow.Show();
            parentWindow.Close();
            this.Close();

        }
    }
}
