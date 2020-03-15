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

namespace senior_project
{
    /// <summary>
    /// Interaction logic for askForFilename.xaml
    /// </summary>
    public partial class askForFilename : Window
    {
        public string MyValue { get; set; }
        MainWindow main = new MainWindow();
        TestProcedure xmlProcedure = new TestProcedure();
        
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
            this.DialogResult = true;

            string file = nameBox.Text;
            file += ".xml";
            createFile(file);
           // xmlProcedure = XmlVerification.loadXml(file);
            TestAdmin admin = new TestAdmin(main, file);
            admin.Show();

        }
        private void createFile(string path)
        {
            try
            {
                // Create the file, or overwrite if the file exists.
                using (FileStream fs = File.Create(path))
                {
                    byte[] info = new UTF8Encoding(true).GetBytes("This is some text in the file.");
                    // Add some information to the file.
                    fs.Write(info, 0, info.Length);
                }

                // Open the stream and read it back.
                using (StreamReader sr = File.OpenText(path))
                {
                    string s = "";
                    while ((s = sr.ReadLine()) != null)
                    {
                        Console.WriteLine(s);
                    }
                }
            }

            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
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
