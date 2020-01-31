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
using System.Xml;
using System.Xml.Schema;
using System.IO;
using Microsoft.Win32;
using System.Reflection;
using System.Xml.Serialization;


namespace senior_project
{
    /// <summary>
    /// Interaction logic for TestAdmin.xaml
    /// </summary>
    public partial class TestAdmin : Window
    {



        #region Initialization
        TestProcedure xmlProcedure = new TestProcedure();

        public TestAdmin()
        {
            InitializeComponent();
        }


        /// <summary>
        /// Test Admin Constructor, creates the Xml object from the provided Xml File path location or if NewTp, then creates a new Test Procedure
        /// </summary>
        /// <param name="xmlFilePath"></param>
        public TestAdmin(TestProcedure newProcedure, bool createNew)
        {
            InitializeComponent();
            xmlProcedure = newProcedure;
            if (createNew)
            {
                CreateNewTP();
            }

            populateTreeView();
        }
        #endregion

        #region Create New Test Procdure
        /// <summary>
        /// Creates a new Test Procedure XML object and initializes all elements to empty
        /// </summary>
        private void CreateNewTP()
        {
            //Create Procedure Heading Node
            TestProcedureProcedure_Heading Procedure_Heading = new TestProcedureProcedure_Heading();

            Procedure_Heading.Date = " ";
            Procedure_Heading.Description = " ";
            Procedure_Heading.Load_Version = " ";
            Procedure_Heading.Name = " ";
            Procedure_Heading.Organization = " ";
            Procedure_Heading.Time = " ";
            Procedure_Heading.Signature = ""; 

            //Create Empty Sections List
            List<TestProcedureSection> Sections = new List<TestProcedureSection>();

            //Create Empty Redline Text, Steps, Section Lists
            List<TestProcedureRedlinesListRedlineText> textList = new List<TestProcedureRedlinesListRedlineText>();
            List<TestProcedureRedlinesListRedlineStep> stepsList = new List<TestProcedureRedlinesListRedlineStep>();
            List<TestProcedureRedlinesListRedlineSection> sectionList = new List<TestProcedureRedlinesListRedlineSection>();

            //Add Empty Redline Text, Steps, Section lists to Redline Node
            TestProcedureRedlinesList RedlinesList = new TestProcedureRedlinesList();

            RedlinesList.SectionList = sectionList;
            RedlinesList.StepsList = stepsList;
            RedlinesList.TextList = textList;

            //Add all Empty Nodes to Test Procedure Root Node
            xmlProcedure.Procedure_Heading = Procedure_Heading;
            xmlProcedure.Sections = Sections;
            xmlProcedure.RedlinesList = RedlinesList;
        }
        #endregion

        #region De-Serialize XML string
        /// <summary>
        /// Turns an XML string file into an XML object that can then be modified
        /// </summary>
        /// <param name="xmlFilePath"> the file path where the xml lists </param>
        /// <returns>XmlData a test procedure object</returns>
        private TestProcedure DeserializeXML(string xmlFilePath)
        {
            XmlSerializer deserializer = new XmlSerializer(typeof(TestProcedure));
            TextReader reader = new StreamReader(xmlFilePath);
            object obj = deserializer.Deserialize(reader);
            TestProcedure XmlData = (TestProcedure) obj;
            reader.Close();

            return XmlData;
        }
        #endregion

        #region Serialize XML
        /// <summary>
        /// Takes an XML object and serializes to string and saves to location of XmlSavePath
        /// </summary>
        /// <param name="XmlData"></param>
        /// <param name="XmlSavePath"></param>
        public void SerializeXML(string XmlSavePath)
        {
            XmlSerializerNamespaces ns = new XmlSerializerNamespaces();
            ns.Add("", ""); //Gets rid of the serializer xlms tags

            XmlSerializer xs = new XmlSerializer(typeof(TestProcedure));
            TextWriter txtWriter = new StreamWriter(XmlSavePath);
            xs.Serialize(txtWriter, xmlProcedure, ns);
            txtWriter.Close();
        }
        #endregion
        

        #region Remove Section
        /// <summary>
        /// Removes the Section from the Test Procedure
        /// </summary>
        /// <param name="SectionToRemove"></param>
        public void RemoveSection(int SectionToRemove)
        {
            xmlProcedure.Sections.RemoveAt(SectionToRemove - 1);

            //Need to Give New Id Values to sections since a section was removed
            for (int i = 0; i < xmlProcedure.Sections.Count; i++)
            {
                if (xmlProcedure.Sections[i].id > SectionToRemove)
                {
                    xmlProcedure.Sections[i].id = (xmlProcedure.Sections[i].id) - 1;
                }
            }
        }
        #endregion

        #region Remove Test Step
        /// <summary>
        /// Removes a Test Step from a specified Test Section
        /// </summary>
        /// <param name="FromSection"></param>
        /// <param name="SteptoRemove"></param>
        public void RemoveTestStep(int FromSection, int SteptoRemove)
        {
            for (int i = 0; i < xmlProcedure.Sections[FromSection - 1].Test_Step.Count; i++)
            {
                if (xmlProcedure.Sections[FromSection - 1].Test_Step[i].id > SteptoRemove)
                {
                    xmlProcedure.Sections[FromSection - 1].Test_Step[i].id = xmlProcedure.Sections[FromSection - 1].Test_Step[i].id - 1;
                }
            }
            xmlProcedure.Sections[FromSection - 1].Test_Step.RemoveAt(SteptoRemove - 1);
        }
        #endregion


        #region Populate Tree
        public void populateTreeView()
        {
            XmlVerification.xmltoTreeView(xmlProcedure, ref treeView1);
        }
        #endregion

        #region Tree Event handling
        private void TreeView1_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            updateTextBoxes();
        }
        #endregion

        #region Validation
        // Display any validation errors.
        private static void ValidationCallBack(object sender, ValidationEventArgs e)
        {
            Console.WriteLine($"Validation Error:\n   {e.Message}\n");
        }
        #endregion


        //Action of a button press
        #region Actions
        private void addSectionAction()
        {
            AddSectionWindow newWindow = new AddSectionWindow(ref xmlProcedure, this);
            newWindow.Show();
        }

        private void removeAction()
        {
            int stepID = 1;
            int sectID = 1;

            TreeViewItem item = (TreeViewItem)treeView1.SelectedItem;

            if (item?.Tag is TestProcedureSectionTest_Step)
            {
                TestProcedureSectionTest_Step step = (TestProcedureSectionTest_Step)item.Tag;
                stepID = step.id;

                TreeViewItem parentItem = item?.Parent as TreeViewItem;
                TestProcedureSection section = (TestProcedureSection)parentItem.Tag;
                sectID = section.id;
            }

            RemoveTestStep(sectID, stepID);
            populateTreeView();
            XmlVerification.writeXmltoFile(xmlProcedure, "tmpAdmin.xml");
        }

        private void addStepAction()
        {
            int currentSectionID = 1; //Prob need to find a better default

            TreeViewItem item = (TreeViewItem)treeView1.SelectedItem;
            if (item?.Tag is TestProcedureSection)
            {
                TestProcedureSection section = (TestProcedureSection)item.Tag;
                currentSectionID = section.id;
            }
            else if (item?.Tag is TestProcedureSectionTest_Step)
            {
                TreeViewItem parentItem = item?.Parent as TreeViewItem;
                TestProcedureSection section = (TestProcedureSection)parentItem.Tag;
                currentSectionID = section.id;
            }

            AddTestStepWindow newTestStepWindow = new AddTestStepWindow(ref xmlProcedure, this, currentSectionID);
            newTestStepWindow.Show();
        }

        private void removeSectionAction()
        {
            int currentSectionID = 1; //Prob need to find a better default

            TreeViewItem item = (TreeViewItem)treeView1.SelectedItem;
            if (item?.Tag is TestProcedureSection)
            {
                TestProcedureSection section = (TestProcedureSection)item.Tag;
                currentSectionID = section.id;
            }
            else if (item?.Tag is TestProcedureSectionTest_Step)
            {
                TreeViewItem parentItem = item?.Parent as TreeViewItem;
                TestProcedureSection section = (TestProcedureSection)parentItem.Tag;
                currentSectionID = section.id;
            }

            RemoveSection(currentSectionID);
            populateTreeView();
            XmlVerification.writeXmltoFile(xmlProcedure, "tmpAdmin.xml");
        }

        private void moveSectionAction()
        {
            MoveSectionorStep moveSectorStep = new MoveSectionorStep(ref xmlProcedure, this);
            moveSectorStep.Show();
        }

        private void saveXmlAction()
        {
            SaveFileDialog saveFile = new SaveFileDialog();

            saveFile.Filter = "XML Documents (.xml)|*.xml";
            saveFile.FileName = "XMLSave";

            Nullable<bool> result = saveFile.ShowDialog();

            if (result == true)
            {
                Console.WriteLine(saveFile.FileName);
            }
            try
            {
                if(xmlProcedure.Procedure_Heading.Revision != null)
                {
                    int x = Int32.Parse(xmlProcedure.Procedure_Heading.Revision.ToString());
                    xmlProcedure.Procedure_Heading.Revision = (x + 1).ToString();
                }
                else
                {
                    xmlProcedure.Procedure_Heading.Revision = "1";
                }


            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}");
            }
            XmlVerification.writeXmltoFile(xmlProcedure, saveFile.FileName);
        }
        #endregion

        //Try a button press and catch exceptions
        #region Buttons
        private void Btn_add_section_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                addSectionAction();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Exception Handled: {ex.Message}");
            }
        }

        private void Btn_remove_step_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                removeAction();
            }
            catch(Exception ex)
            {
                MessageBox.Show($"Exception Handled: {ex.Message}");
            }
        }

        private void Btn_add_step_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                addStepAction();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Exception Handled: {ex.Message}");
            }
        }

        private void Btn_remove_section_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                removeSectionAction();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Exception Handled: {ex.Message}");
            }
        }

        private void Move_section_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                moveSectionAction();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Exception Handled: {ex.Message}");
            }
        }

        private void SaveXmlButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                saveXmlAction();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Exception Handled: {ex.Message}");
            }
        }
        #endregion





        #region Refresh Text Boxes
        private void updateTextBoxes()
        {
            TreeViewItem item = (TreeViewItem) treeView1.SelectedItem;
            if (item?.Tag is TestProcedureSectionTest_Step)
            {
                TestProcedureSectionTest_Step step = (TestProcedureSectionTest_Step) item.Tag;

                stepBox.Text = step.id.ToString();
                stationBox.Text = step.Station.ToString();
                controlActionBox.Text = step.Control_Action.ToString();
                expectedResultBox.Text = step.Expected_Result.ToString();
            }
        }
        #endregion

        #region Context Menu
        /// <summary>
        /// Right Click Menu Exit Button causes window to close
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
        #endregion




        #region Window Close
        /// <summary>
        /// Launches Main Window Launcher when Test Admin window is closed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_Closed(object sender, EventArgs e)
        {
            MainWindow t = new MainWindow();
            t.Show();
        }
        #endregion

        #region Change Heading
        /// <summary>
        /// Changes the Heading for a Test Procedure
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void EditHeading_Click(object sender, RoutedEventArgs e)
        {
           
        }
        #endregion

        #region Change Description
        /// <summary>
        /// Changes the Description for a Test Procedure
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void EditDescription_Click(object sender, RoutedEventArgs e)
        {

        }







        #endregion

        private void StepBox_KeyDown(object sender, KeyEventArgs e)
        {

        }

        private void StationBox_KeyDown(object sender, KeyEventArgs e)
        {
            TreeViewItem item = (TreeViewItem)treeView1.SelectedItem;

            if (e.Key == Key.Enter && item.Tag is TestProcedureSectionTest_Step)
            {
                TestProcedureSectionTest_Step step = ((TestProcedureSectionTest_Step)item.Tag);
                step.Station = stationBox.Text;
                XmlVerification.writeXmltoFile(xmlProcedure, "tmpAdmin.xml");
            }
        }

        private void ControlActionBox_KeyDown(object sender, KeyEventArgs e)
        {
            TreeViewItem item = (TreeViewItem)treeView1.SelectedItem;

            if (e.Key == Key.Enter && item.Tag is TestProcedureSectionTest_Step)
            {
                TestProcedureSectionTest_Step step = ((TestProcedureSectionTest_Step)item.Tag);
                step.Control_Action = controlActionBox.Text;
                XmlVerification.writeXmltoFile(xmlProcedure, "tmpAdmin.xml");

            }
        }

        private void ExpectedResultBox_KeyDown(object sender, KeyEventArgs e)
        {
            TreeViewItem item = (TreeViewItem)treeView1.SelectedItem;

            if (e.Key == Key.Enter && item.Tag is TestProcedureSectionTest_Step)
            {
                TestProcedureSectionTest_Step step = ((TestProcedureSectionTest_Step)item.Tag);
                step.Expected_Result = expectedResultBox.Text;
                XmlVerification.writeXmltoFile(xmlProcedure, "tmpAdmin.xml");

            }
        }

    }
}