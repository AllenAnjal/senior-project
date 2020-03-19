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
using System.Xml;
using System.IO;
using Microsoft.Win32;

namespace senior_project
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        #region Initialization

        public MainWindow()
        {
            InitializeComponent();
        }

        private TestProcedure xmlProcedure = new TestProcedure();

        #endregion Initialization

        #region Context Menu Buttons

        /// <summary>
        /// Conext Menu from Right Click has Help, About, Exit buttons with corresponding action
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void start_btn(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "XML files|*.xml";

            if ((bool)openFileDialog.ShowDialog())
            {
                this.Hide();
                Tester tester = new Tester(this, openFileDialog.FileName);

                tester.Show();
            }
            else
            {
                MessageBox.Show("No valid XML has been selected", "Error");
            }

            return;
            //xmlProcedure = XmlVerification.loadXml();
   
        }

        private void edit_btn(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "XML files|*.xml";

            if ((bool)openFileDialog.ShowDialog())
            {
                this.Hide();
                TestAdmin admin = new TestAdmin(this, openFileDialog.FileName);

                admin.Show();
               // this.Close();
            }
            else
            {
                MessageBox.Show("No valid XML has been selected", "Error");
            }

        }

        private void create_btn(object sender, RoutedEventArgs e)
        {
            askForFilename ask = new askForFilename();

            //xmlProcedure = XmlVerification.loadXml();
            if (xmlProcedure != null)
            {
                this.Hide();
                ask.ShowDialog();
                //TestAdmin admin = new TestAdmin(xmlProcedure, true);
                //admin.Show();
            }
            else
            {
                MessageBox.Show("No valid XML has been selected", "Error");
            }
        }

        #endregion Context Menu Buttons

        #region Application Shutdown

        /// <summary>
        /// This terminates any remaining sub-processes/threads
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Application.Current.Shutdown();
        }

        #endregion Application Shutdown
    }
}