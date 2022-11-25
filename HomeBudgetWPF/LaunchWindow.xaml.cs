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
using Microsoft.Win32;
using Budget;
using System.IO;

namespace HomeBudgetWPF
{
    /// <summary>
    /// Interaction logic for LaunchWindow.xaml
    /// </summary>

    public partial class LaunchWindow : Window, LaunchWindowInterface
    {
        private readonly Presenter presenter;
        public LaunchWindow()
        {
            InitializeComponent();
            presenter = new Presenter(this);
        }

        public string GetExistingFile()
        {
            OpenFileDialog openFile = new OpenFileDialog();

            openFile.RestoreDirectory = true;
     
            
            string fileName = null;
            if (openFile.ShowDialog() == true)
            {
                fileName = openFile.FileName;
            }
            return fileName;
        }
        

        public void OpenMainCloseLaunch()
        {
            Application.Current.MainWindow = new MainWindow(presenter);
            Application.Current.MainWindow.Show();
            this.Close();
        }


        public void ShowError(string message)
        {
            MessageBox.Show(message);
        }

        private void newFile_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.MainWindow = new ProjectDetailsWindow(Window.GetWindow(this));
            Application.Current.MainWindow.Show();
        }

        private void openFile_Click(object sender, RoutedEventArgs e)
        {
            string file = GetExistingFile();
            if (file != null)
                presenter.ProcessExistingFile(file);
        }
    }
}
