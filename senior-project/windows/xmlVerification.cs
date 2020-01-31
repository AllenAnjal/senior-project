using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Windows;
using Microsoft.Win32;
using System.Xml.Linq;
using System.IO;
using System.Windows.Controls;
using System.Xml.Serialization;


namespace launcher_cae
{
    class XmlVerification
    {

        public static string filePath = "";
        public static string saveFilePath = "";
        public static string saveSafeFilePath = "";

        #region XML Operations
        //Write TestProcedure object to directory of .exe
        public static void writeXmltoFile(TestProcedure xmlProcedure, String filename)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(TestProcedure));
            XmlSerializerNamespaces ns = new XmlSerializerNamespaces();
            ns.Add("", ""); //Gets rid of the serializer xlms tags

            using (StreamWriter writer = File.CreateText(filename))
            {
                try
                {
                    serializer.Serialize(writer, xmlProcedure, ns);
                }
                catch (Exception e)
                {
                    MessageBox.Show(e.Message, "Error");
                }
            }
        }

        //Load XML into TestProcedure object or return null
        public static TestProcedure loadXml()
        {
            OpenFileDialog load_file = new OpenFileDialog();
            XmlSerializer serializer = new XmlSerializer(typeof(TestProcedure));

            load_file.Filter = "XML Files (*.XML) |*.xml";

            if (load_file.ShowDialog() == true)
            {
                using (System.IO.Stream fileStream = load_file.OpenFile())
                {
                    filePath = load_file.FileName;

                    try
                    {
                        return (TestProcedure)serializer.Deserialize(fileStream);
                    }
                    catch (Exception e)
                    {
                        MessageBox.Show(e.Message, "Error");
                    }

                }
            }
            return null;
        }

        //Prompt the user to save the XML to a location
        public static void saveXml()
        {
            SaveFileDialog save_file = new SaveFileDialog();

            save_file.Filter = "XML Files (*.XML) |*.xml | CSV Files (*.CSV) | *.csv";

            if (save_file.ShowDialog() == true)
            {
                using (System.IO.Stream fileStream = save_file.OpenFile())
                {
                    saveFilePath = save_file.FileName;
                    saveSafeFilePath = save_file.SafeFileName;


                }
            }
        }

        //Testprocedure object to treeView hierarchy 
        public static void xmltoTreeView(TestProcedure xmlProcedure, ref TreeView treeView)
        {
            treeView.Items.Clear();
            List<TestProcedureSection> sectionList = xmlProcedure.Sections;

            if (sectionList != null)
            {
                foreach (TestProcedureSection section in sectionList)
                {

                    TreeViewItem newHeader = new TreeViewItem();
                    newHeader.Tag = section;
                    newHeader.Header = section.Heading.ToString();
                    if (section.Test_Step != null)
                    {
                        foreach (TestProcedureSectionTest_Step step in section.Test_Step)
                        {
                            TreeViewItem newChild = new TreeViewItem();
                            newChild.Header = step.id.ToString();
                            newChild.Tag = step;
                            newHeader.Items.Add(newChild);
                        }
                    }
                    treeView.Items.Add(newHeader);
                }
            }
        }
        #endregion

        #region CSV Operations
        //Export data to CSV
        public static void exportCsv(TestProcedure tp, TreeView treeView1)
        {
            TreeViewItem item = (TreeViewItem)treeView1.SelectedItem;
            TreeViewItem parentItem = item?.Parent as TreeViewItem;

            if (!(item?.Tag is TestProcedureSectionTest_Step))
            {
                return;
            }

            TestProcedureSection section = (TestProcedureSection)parentItem.Tag;
            TestProcedureSectionTest_Step step = ((TestProcedureSectionTest_Step)item.Tag);
            TestProcedureProcedure_Heading head = tp.Procedure_Heading;


            String fData = $"{head.Date},{section.id},{step.id},{head.Revision},{head.FileName},{head.Load_Version}," +
                $"{head.Name},{returnSteps(section.id, step.id, 5, tp)},{ step.Comments},{ head.Severity},{ head.System}\n";

            //If there is no file - create file with correct header
            if (!File.Exists("fail.csv"))
            {
                if (item?.Tag != null)
                {
                    using (StreamWriter writer = File.CreateText("fail.csv"))
                    {
                        try
                        {
                            writer.Write("Date,Section,Step,Revision,File Name,Software Load Version,Name,Steps To Reproduce,Failure Description,Severity,System\n" + fData);
                            writer.Close();
                        }
                        catch (Exception e)
                        {
                            MessageBox.Show(e.Message, "Error");
                        }
                    }
                }
            }
            //If file already exists, appened to it 
            else
            {
                try
                {
                    StreamWriter newWriter = File.AppendText("fail.csv");
                    newWriter.Write(fData);
                    newWriter.Close();
                }
                catch (Exception e)
                {
                    MessageBox.Show(e.Message, "Error");
                }

            }
        }

        //Sanitize string for CSV
        private static string ConvertToCsvCell(string value)
        {
            var mustQuote = value.Any(x => x == ',' || x == '\"' || x == '\r' || x == '\n');

            if (!mustQuote)
            {
                return value;
            }

            value = value.Replace("\"", "\"\"");

            return string.Format("\"{0}\"", value);
        }
        #endregion



        //Return n previous steps
        private static String returnSteps(int sectionID, int stepID, int nSteps, TestProcedure tp)
        {
            sectionID--;
            stepID--;

            Stack<String> tmpStack = new Stack<String>();
            String result = "";

            for (int i = sectionID; i >= 0; i--)
            {
                for (int j = stepID; j >= 0 && tmpStack.Count != nSteps; j--)
                {
                    tmpStack.Push("Step #" + (tmpStack.Count) + " " + tp.Sections[i].Test_Step[j].Control_Action.ToString() + "\n\n");
                }
            }

            int n = tmpStack.Count;
            for (int i = 0; i < n; i++)
            {
                result += tmpStack.Pop();
            }
            return ConvertToCsvCell(result);
        }

        //Clear a textbox upon clicking when the data is a default
        public static void textBoxClear(ref TextBox tb, String defaultText)
        {
            if(tb.Text == defaultText)
            {
                tb.Clear();
            }
        }

        //Return the selected step of a treeView
        public static TestProcedureSectionTest_Step getCurrentStep(ref TreeView tv)
        {
            TreeViewItem item = (TreeViewItem)tv.SelectedItem;
            if(item?.Tag is TestProcedureSectionTest_Step)
            {
                return (TestProcedureSectionTest_Step)item?.Tag;
            }
            return null;
        }




















    }





}
