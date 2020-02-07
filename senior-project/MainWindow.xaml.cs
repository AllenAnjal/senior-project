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
        TestProcedure xmlProcedure = new TestProcedure();
        

        #endregion

        #region Context Menu Buttons
        /// <summary>
        /// Conext Menu from Right Click has Help, About, Exit buttons with corresponding action
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void start_btn(object sender, RoutedEventArgs e)
        {
            xmlProcedure = XmlVerification.loadXml();
            if (xmlProcedure != null)
            {
                this.Hide();
                Tester tester = new Tester(xmlProcedure);
                tester.Show();
            }
            else
            {
                MessageBox.Show("No valid XML has been selected", "Error");
            }
        }

        private void edit_btn(object sender, RoutedEventArgs e)
        {
            xmlProcedure = XmlVerification.loadXml();
            if (xmlProcedure != null)
            {
                this.Hide();
                TestAdmin admin = new TestAdmin(xmlProcedure, false);
                admin.Show();
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
                ask.Show();
                //TestAdmin admin = new TestAdmin(xmlProcedure, true);
                //admin.Show();
            }
            else
            {
                MessageBox.Show("No valid XML has been selected", "Error");
            }
        }
        #endregion

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
        #endregion


    }


}
