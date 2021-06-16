using MoveFiles.Controllers;
using System;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Media;

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
            LoadConfigs();

            controller = new MainPageController();
            this.DataContext = controller;
        }

       



        bool ProcesStarted = false;
        private MainPageController controller;

        

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

            controller.UpdateInputsUser(tbRegex.Text, tbFolderOrigin.Text, tbFolderDestination.Text, checktime);
            
            if (ProcesStarted)
            {
                controller.Stop();
                btnStart.Content = "START";
                var converter = new System.Windows.Media.BrushConverter();
                var brush = (Brush)converter.ConvertFromString("#FFFF7600");
                btnStart.Background = brush;
                ProcesStarted = false;
            }
            else
            {
                controller.Start();
                btnStart.Content = "STOP";
                var converter = new System.Windows.Media.BrushConverter();
                var brush = (Brush)converter.ConvertFromString("#DDFF0000");
                btnStart.Background = brush;
                ProcesStarted = true;
            }

           
           

        }

        private void Window_Closing(object sender, CancelEventArgs e)
        {
            SaveConfigs();
        }

        public void SaveConfigs()
        {
            Properties.Settings.Default.regex = tbRegex.Text;
            Properties.Settings.Default.check_period = tbCheckFolderPeriod.Text;
            Properties.Settings.Default.origin = tbFolderOrigin.Text;
            Properties.Settings.Default.destination = tbFolderDestination.Text;

            Properties.Settings.Default.Save();
        }


        public void LoadConfigs()
        {
            tbRegex.Text = Properties.Settings.Default.regex;
            tbCheckFolderPeriod.Text = Properties.Settings.Default.check_period;
            tbFolderOrigin.Text = Properties.Settings.Default.origin;
            tbFolderDestination.Text = Properties.Settings.Default.destination;
        }

    }
}
