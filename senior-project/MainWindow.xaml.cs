using System;
using System.Collections.Generic;
using System.Data;
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
using System.Xml.Schema;
using System.IO;
using System.Xml.Serialization;
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
        /// Context Menu from Right Click has Help, About, Exit buttons with corresponding action
        /// Buttons:
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

        /// <summary>
        /// Create blank xml file with template
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void create_btn(object sender, RoutedEventArgs e)
        {
            
            FileNameWindow fileDialog = new FileNameWindow();
            fileDialog.ShowDialog();
            
            //string caption = "XML Creation Tool";
            //string xmlFile = @"..\..\templates\template.xml";
            //string emptyFile = @"..\..\templates\processed_template.xml";
            //string schemaFile = @"..\..\Procedure_Schema.xsd";
            string filename = fileDialog.Answer;
            if (fileDialog.DialogResult == false) return;

            // Create XML Object
            XmlSerializer serializer = new XmlSerializer(typeof(Procedure));
            Procedure p = new Procedure();

            TextWriter writer = new StreamWriter(filename);

            serializer.Serialize(writer, p);
            writer.Close();
            // Serializer XML Object
            // Print XML

            /* GOOD CODE
             * Test 1: PASS
            

            XmlDocument xmlTestDoc = LoadDocumentWithSchemaValidation(xmlFile, schemaFile);

            string message = (xmlTestDoc != null) ? "Test Successful." : "Test failed.";
            MessageBox.Show(
                message,
                caption,
                MessageBoxButton.OK,
                MessageBoxImage.Information);
            */

            /*
            string xsdFileName = @"..\..\templates\Procedure_Schema.xsd";
            string caption = "XML Creation Tool";

            MessageBoxResult result = MessageBox.Show(
                "Create new XML file?",
                caption,
                MessageBoxButton.YesNo,
                MessageBoxImage.Exclamation);
            switch(result)
            {
                case MessageBoxResult.No:
                    return;
            }
            DataSet xsdDataSet = ReadSchemaFromXmlTextReader(xsdFileName);
            //string path = @"C:\Users\Allen G. Anjal\source\repos\senior-project\senior-project\test2.xml";
            string messageBoxText = WriteXmlToFile(xsdDataSet);
            MessageBox.Show(
                messageBoxText,
                caption,
                MessageBoxButton.OK,
                MessageBoxImage.Information);
            */
            /*
            askForFilename ask = new askForFilename();
            
            //xmlProcedure = XmlVerification.loadXml();
            if (xmlProcedure != null)
            {
                this.Hide();
                //ask.Show();
                TestAdmin admin = new TestAdmin(xmlProcedure, true);
                admin.Show();
            }
            else
            {
                MessageBox.Show("No valid XML has been selected", "Error");
            }
            */
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

        private DataSet ReadSchemaFromXmlTextReader(string filename)
        {
            // Create the DataSet to read the schema into.
            DataSet thisDataSet = new DataSet();
            //thisDataSet.Namespace = "http://tempuri.org/ProcedureSchema.xsd";
            thisDataSet.Namespace = "http://www.w3.org/2001/XMLSchema";
            thisDataSet.Prefix = "xs";

            Console.WriteLine("{");
            Console.WriteLine(thisDataSet.Namespace);
            Console.WriteLine(thisDataSet.Prefix);
            Console.WriteLine("}");
            /*
            // Create a FileStream object with the file path and name.
            System.IO.FileStream myFileStream = new System.IO.FileStream
            (filename, System.IO.FileMode.Open);



            // Create a new XmlTextReader object with the FileStream.
            System.Xml.XmlTextReader myXmlTextReader =
            new System.Xml.XmlTextReader(myFileStream);
            */
            // Read the schema into the DataSet and close the reader.
            //thisDataSet.ReadXmlSchema(myXmlTextReader);
            thisDataSet.ReadXmlSchema(filename);
            //myXmlTextReader.Close();
            //myFileStream.Close();

            

            return thisDataSet;
        }

        private string WriteXmlToFile(DataSet thisDataSet)
        {
            if (thisDataSet == null) { return "Test unsuccessful"; }

            // Create a file name to write to.
            string filename = "myXmlDoc.xml";
            // Create the FileStream to write with.
            System.IO.FileStream myFileStream = new System.IO.FileStream
            (filename, System.IO.FileMode.Create);

            XmlWriterSettings settings = new XmlWriterSettings
            {
                Indent = true,
                OmitXmlDeclaration = false
            };

            // Create an XmlTextWriter with the fileStream.
            System.Xml.XmlWriter myXmlWriter = XmlWriter.Create(myFileStream, settings);
            // Write to the file with the WriteXml method.
            thisDataSet.WriteXml(myXmlWriter);
            myXmlWriter.Close();

            
            return myFileStream.Name;
        }

        public XmlDocument LoadDocumentWithSchemaValidation(string xmlFilename, string xsdFilename)
        {
            XmlReader reader;

            XmlReaderSettings settings = new XmlReaderSettings();

            if (xmlFilename == null || xsdFilename == null)
            {
                return null;
            }

            XmlSchema schema = getSchema(xsdFilename);
            if (schema == null)
            {
                return null;
            }

            schema.Write(Console.Out);

            settings.Schemas.Add(schema);

            settings.ValidationEventHandler += settings_ValidationEventHandler;
            settings.ValidationFlags =
                settings.ValidationFlags | XmlSchemaValidationFlags.ReportValidationWarnings;
            settings.ValidationType = ValidationType.Schema;
            try
            {
                reader = XmlReader.Create(xmlFilename, settings);
            }
            catch (System.IO.FileNotFoundException)
            {
                return null;
            }

            XmlDocument doc = new XmlDocument
            {
                PreserveWhitespace = true
            };
            doc.Load(reader);
            reader.Close();

            return doc;
        }
        
        private XmlSchema getSchema(string filename)
        {
            XmlSchemaSet xs = new XmlSchemaSet();
            XmlSchema schema;
            try
            {
                schema = xs.Add("http://tempuri.org/ProcedureSchema.xsd", filename);
            }
            catch (System.IO.FileNotFoundException)
            {
            
                return null;
                
            }
            return schema;
        }
        
        private void validateXML(string schemaFilename, XmlDocument doc)
        {
            if (doc.Schemas.Count == 0)
            {
                // Helper method to retrieve schema.
                XmlSchema schema = getSchema(schemaFilename);
                doc.Schemas.Add(schema);
            }

            // Use an event handler to validate the XML node against the schema.
            doc.Validate(settings_ValidationEventHandler);
        }
        
        void settings_ValidationEventHandler(object sender,
    System.Xml.Schema.ValidationEventArgs e)
        {
            if (e.Severity == XmlSeverityType.Warning)
            {
                MessageBox.Show
                    ("The following validation warning occurred: " + e.Message);
            }
            else if (e.Severity == XmlSeverityType.Error)
            {
                MessageBox.Show
                    ("The following critical validation errors occurred: " + e.Message);
                Type objectType = sender.GetType();
            }
        }

        private void SetValueForNull(DataSet dataSet)
        {
            Console.WriteLine("Set: {0}", dataSet.DataSetName);
            Console.WriteLine("\thas {0} tables", dataSet.Tables.Count);
            foreach(DataTable table in dataSet.Tables)
            {
                Console.WriteLine("Table: {0}", table.TableName);
                foreach (DataRow row in table.Rows)
                {
                    foreach (DataColumn column in table.Columns)
                    {
                        

                        if (column.DataType.ToString() == "System.Int32" || column.DataType.ToString() == "System.Single" || column.DataType.ToString() == "System.Double" || column.DataType.ToString() == "System.Decimal")
                        {
                            if (row[column] == DBNull.Value) row[column] = 0;
                        }
                        else if (column.DataType.ToString() == "System.String")
                        {
                            if (row[column] == DBNull.Value || row[column].ToString().Trim() == "") row[column] = "0";
                        }
                        Console.WriteLine(row[column]);
                        continue;
                    }
                }
            }
        }
    }


}
