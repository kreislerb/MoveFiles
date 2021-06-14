using MoveFiles.Controllers;
using System;
using System.Drawing;
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


              

            controller = new MainPageController();
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
                ProcesStarted = false;
            }
            else
            {
                controller.Start();
                ProcesStarted = true;
            }

           
           

        }

       
    }
}
