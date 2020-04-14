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
using System.Xml;

namespace senior_project
{
    /// <summary>
    /// Interaction logic for askForFilename.xaml
    /// </summary>
    public partial class askForFilename : Window
    {

        private XmlDocument _xml;
        private string path;
        private MainWindow main = new MainWindow();

        public askForFilename()
        {
            InitializeComponent();
            DataContext = this;

            nameBox.Text = ".xml";
            nameBox.SelectAll();
            nameBox.Focus();
        }

        private void SubmitButton(object sender, RoutedEventArgs e)
        {
            
            string file = nameBox.Text;

            //checks if the name already has .xml in it or not and add accordingly
            if (!file.EndsWith(".xml"))
                file += ".xml";

            path = Directory.GetCurrentDirectory();
            path = path.Substring(0, path.Length - 9);
            file = path + file;
            var f = File.Create(file);
            f.Close();
            using (StreamWriter sw = File.AppendText(file))
            {
                sw.WriteLine("<?xml version=\"1.0\" encoding=\"utf-8\"?>\n");
                sw.WriteLine("<Test_Procedure rev=\"\">\n");
                sw.WriteLine("<Procedure_Heading>\n");
                sw.WriteLine("<System />\n"
                                + "<Description />\n"
                                + "<Name />\n"
                                + "<Date />\n"
                                + "<Time/>\n"
                                + "<Software_Load_Version />\n"
                                + "<Program_Phase />\n"
                                + "<Classification />\n"
                                + "<Signature />\n"
                                + "<Start_Time />\n"
                                + "<Stop_Time />\n"
                                + "</Procedure_Heading >\n");
                sw.WriteLine("<Sections>\n");
                sw.WriteLine("<Section id=\"1\">\n");
                sw.WriteLine("<Heading> 1.0 IOS DORT TESTS </Heading>\n");
                sw.WriteLine("<Description> short description </Description>\n");
                sw.WriteLine("<Test_Step id=\"1\">\n" +
                                "<Station></Station>\n" + 
                                "<Control_Action></Control_Action>\n" + 
                                "<Expected_Result></Expected_Result>\n" +
                                "<Result result=\"\"></Result>\n" + 
                                "<Station></Station>\n" +
                                "<Station_Redline></Station_Redline>\n" +
                                "<Control_Action></Control_Action>\n" +
                                "<Control_Action_Redline></Control_Action_Redline>\n" +
                                "<Expected_Result></Expected_Result>\n" +
                                "<Expected_Result_Redline></Expected_Result_Redline>\n" +
                                "<Result result=\"\"></Result>\n" +
                                "<Comments/></Test_Step>\n");
                sw.WriteLine("</Section>\n");
                sw.WriteLine("</Sections>\n");
                sw.WriteLine("</Test_Procedure>");

            }

            TestAdmin admin = new TestAdmin(main, file);
            admin.Show();
        }

        private void CancelButton(object sender, RoutedEventArgs e)
        {
            main.Show();
            this.Close();
        }

        public string Answer
        {
            get { return nameBox.Text; }
        }
    }
}