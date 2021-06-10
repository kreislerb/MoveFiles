using MoveFiles.Controllers;
using System;
using System.IO;
using System.Windows;
using System.Windows.Forms;


namespace MoveFiles
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void btnFolderOrigin_Click(object sender, RoutedEventArgs e)
        {
            using (var fbd = new FolderBrowserDialog())
            {
                DialogResult result = fbd.ShowDialog();
               
                
                if (result == System.Windows.Forms.DialogResult.OK && !string.IsNullOrWhiteSpace(fbd.SelectedPath))
                {
                    tbFolderOrigin.Text = fbd.SelectedPath;
                    string[] files = Directory.GetFiles(fbd.SelectedPath);

                }
            }
        }

        private void btnFolderDestination_Click(object sender, RoutedEventArgs e)
        {
            using (var fbd = new FolderBrowserDialog())
            {
                DialogResult result = fbd.ShowDialog();

                if (result == System.Windows.Forms.DialogResult.OK && !string.IsNullOrWhiteSpace(fbd.SelectedPath))
                {
                    tbFolderDestination.Text = fbd.SelectedPath;
                    string[] files = Directory.GetFiles(fbd.SelectedPath);

                }
            }
        }

        private void btnStart_Click(object sender, RoutedEventArgs e)
        {
            long checktime = 0;
            try
            {
                checktime = long.Parse(tbCheckFolderPeriod.Text);
            }
            catch (Exception err)
            {
                checktime = 0;
            }
            
            var controller = new MainPageController(tbRegex.Text, tbFolderOrigin.Text, tbFolderDestination.Text, checktime);
            controller.StartProcess();

        }

       
    }
}
