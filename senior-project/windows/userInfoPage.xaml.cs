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
using System.Windows.Shapes;

using System.IO;
using System.Drawing;
using System.Diagnostics;


namespace senior_project
{
    /// <summary>
    /// Interaction logic for userInfoPage.xaml
    /// </summary>
    public partial class userInfoPage : Window
    {
        TestProcedure newProcedure;
        public userInfoPage(string xmlFile)
        {
            InitializeComponent();
            
        }



        //Pull data from the window and store in XML
        private void pullData()
        {
            //Save Data to xml procedure
            /*
            TestProcedureProcedure_Heading head = newProcedure.Procedure_Heading;

            head.Date = DateTime.Now.ToShortDateString();
            head.FileName = System.IO.Path.GetFileName(XmlVerification.filePath);
            head.Name = userNameBox.Text;
            head.System = systemBox.Text;
            head.Organization = orgBox.Text;
            //Close and enable Tester window
            XmlVerification.writeXmltoFile(newProcedure, "tmp.xml");
            */
            this.Close();
        }


        private void Button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                pullData();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}");
            }
        }


    }
}
