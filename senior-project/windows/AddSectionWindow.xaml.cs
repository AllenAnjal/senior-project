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
using senior_project;

namespace senior_project
{
    /// <summary>
    /// Interaction logic for AddSectionWindow.xaml
    /// </summary>
    public partial class AddSectionWindow : Window
    {
        TestProcedure toAdd = new TestProcedure();
        TestAdmin newInst;
        String defaultHeading = "Enter Heading Here";
        String defaultDescription = "Enter Section Description Here";
        public AddSectionWindow(ref TestProcedure xmlTestProcedure, TestAdmin inst)
        {
            newInst = inst;
            toAdd = xmlTestProcedure;
            InitializeComponent();
            initilizeTextBoxes();
        }
        private void AddSection(string Heading, string Description)
        {
            TestProcedureSection SectionToAdd = new TestProcedureSection();
            SectionToAdd.id = toAdd.Sections.Count + 1; //Do not modify
            SectionToAdd.Heading = Heading;
            SectionToAdd.Description = Description;

            List<TestProcedureSectionTest_Step> EmptyTestSteps = new List<TestProcedureSectionTest_Step>(); //Creates Empty List of Test Steps
            SectionToAdd.Test_Step = EmptyTestSteps;

            toAdd.Sections.Add(SectionToAdd); //Appends the new section to existing Sections
        }

        private void clickAction()
        {
            if (String.IsNullOrWhiteSpace(SectionHeadingBox.Text) || String.IsNullOrWhiteSpace(SectionDescriptionBox.Text))
            {
                MessageBox.Show("All text fields must be entered", "Missing Text Field");
            }

            else
            {
                AddSection(SectionHeadingBox.Text, SectionDescriptionBox.Text);
                XmlVerification.writeXmltoFile(toAdd, "tmp.xml");
                //newInst.populateTreeView();
                initilizeTextBoxes();
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                clickAction();
            }
            catch(Exception ex)
            {
                MessageBox.Show($"Exception Handled: {ex.Message}");
            }

        }

        #region Got Focus
        private void SectionHeadingBox_GotFocus(object sender, RoutedEventArgs e)
        {
            XmlVerification.textBoxClear(ref SectionHeadingBox, defaultHeading);
        }

        private void SectionDescriptionBox_GotFocus(object sender, RoutedEventArgs e)
        {
            XmlVerification.textBoxClear(ref SectionDescriptionBox, defaultDescription);
        }
        #endregion

        private void initilizeTextBoxes()
        {
            SectionHeadingBox.Text = defaultHeading;
            SectionDescriptionBox.Text = defaultDescription;
        }
    }
}
