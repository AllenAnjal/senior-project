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

namespace senior_project
{
    /// <summary>
    /// Interaction logic for AddTestStepWindow.xaml
    /// </summary>
    public partial class AddTestStepWindow : Window
    {
        TestProcedure xml;
        TestAdmin newInst;
        int sectionId;

        String defaultStation = "Enter Station Here";
        String defaultCA = "Enter Control Action Here"; 
        String defaultER = "Enter Expected Result Here";

        public AddTestStepWindow(ref TestProcedure xml1, TestAdmin inst, int currSectionId)
        {
            xml = xml1;
            newInst = inst;
            sectionId = currSectionId; 
            InitializeComponent();
            initializeTextBoxes();
        }

        private void initializeTextBoxes()
        {
            StationBox.Text = defaultStation;
            ControlActionBox.Text = defaultCA;
            ExpResultBox.Text = defaultER;
        }


        public void AddTestStep(int SectionToAddTestStep, string Station, string ControlAction, string ExpResult)
        {
            int NumberOfTestSteps = xml.Sections[SectionToAddTestStep - 1].Test_Step.Count; //This needs to be verified before accessing like this - if Sections[SectionToAddTestStep - 1] is empty the program will crash.

            TestProcedureSectionTest_Step StepToAdd = new TestProcedureSectionTest_Step();
            StepToAdd.id = NumberOfTestSteps + 1; //Do not Modify
            StepToAdd.Station = Station;
            StepToAdd.Control_Action = ControlAction;
            StepToAdd.Expected_Result = ExpResult;
            StepToAdd.Pass = "False"; //Initialized Value
            StepToAdd.Fail = "False"; //Initialized Value
            StepToAdd.Comments = ""; //As Test Admin this should be empty

            xml.Sections[SectionToAddTestStep - 1].Test_Step.Add(StepToAdd); //Should we throw expection if Section to add int value is 0? 
        }

        private void saveAction()
        {
            if (String.IsNullOrWhiteSpace(ControlActionBox.Text) || String.IsNullOrWhiteSpace(ExpResultBox.Text) || String.IsNullOrWhiteSpace(StationBox.Text))
            {
                MessageBox.Show("All text fields must be entered", "Missing Text Field");
            }

            else
            {
                AddTestStep(sectionId, StationBox.Text, ControlActionBox.Text, ExpResultBox.Text);
                XmlVerification.writeXmltoFile(xml, "tmp.xml");
               // newInst.populateTreeView();
                initializeTextBoxes();
            }
        }


        private void save1_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                saveAction();
            }
            catch(Exception ex)
            {
                MessageBox.Show($"Exception Handled: {ex.Message}");
            }
        }

        #region TextBox Focus
        private void ControlActionBox_GotFocus(object sender, RoutedEventArgs e)
        {
            XmlVerification.textBoxClear(ref ControlActionBox, defaultCA);

        }
        private void StationBox_GotFocus(object sender, RoutedEventArgs e)
        {
            XmlVerification.textBoxClear(ref StationBox, defaultStation);
        }

        private void ExpResultBox_GotFocus(object sender, RoutedEventArgs e)
        {
            XmlVerification.textBoxClear(ref ExpResultBox, defaultER);
        }
        #endregion



    }
}
