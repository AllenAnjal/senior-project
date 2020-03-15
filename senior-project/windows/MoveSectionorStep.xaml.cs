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
using System.Text.RegularExpressions;

namespace senior_project
{
    /// <summary>
    /// Interaction logic for MoveSectionorStep.xaml
    /// </summary>
    public partial class MoveSectionorStep : Window
    {
        TestProcedure xmlProcedure = new TestProcedure();
        TestAdmin newInst;
        public MoveSectionorStep(ref TestProcedure xml, TestAdmin inst)
        {
            InitializeComponent();
            xmlProcedure = xml;
            newInst = inst;
            MoveSect.IsChecked = false;
            MoveStep.IsChecked = false;
        }

        private void NumberValidationTextBox(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }

        private void saveClickAction()
        {

        }

        private void save1_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                saveClickAction();
            }
            catch(Exception ex)
            {
                MessageBox.Show($"Exception Handled: {ex.Message}");
            }
        }

        private void enableTextBoxes()
        {
            FromSectBox.IsEnabled = true;
            ToSectBox.IsEnabled = true;
            FromStepBox.IsEnabled = true;
            ToStepBox.IsEnabled = true;
        }

        private void RadioButton_Checked(object sender, RoutedEventArgs e)
        {
            MoveSect.IsChecked = true;
            MoveStep.IsChecked = false;

            FromSectBox.IsEnabled = true;
            ToSectBox.IsEnabled = true;
            FromStepBox.IsEnabled = false;
            ToStepBox.IsEnabled = false;
        }

        private void RadioButton_Checked_1(object sender, RoutedEventArgs e)
        {
            MoveSect.IsChecked = false;
            MoveStep.IsChecked = true;
            enableTextBoxes();

        }

        public void moveSection(int fromPosition, int toPosition)
        {
            //Adjust for index
            fromPosition--;
            toPosition--;

            List<TestProcedureSection> sectionList = xmlProcedure?.Sections;

            if (sectionList != null)
            {
                if (isInRange(sectionList, fromPosition) && isInRange(sectionList, toPosition))
                {
                    TestProcedureSection temp;

                    temp = sectionList[fromPosition];
                    sectionList[fromPosition] = sectionList[toPosition];
                    sectionList[toPosition] = temp;
                }
                else
                {
                    MessageBox.Show("Invalid Range", "Error");
                }
            }

        }


        public void MoveSection(int fromPosition, int toPosition)
        {
            TestProcedureSection temp = new TestProcedureSection();
            temp = xmlProcedure.Sections[fromPosition - 1];
            temp.id = toPosition;

            xmlProcedure.Sections.RemoveAt(fromPosition - 1);

            for (int i = 0; i < xmlProcedure.Sections.Count; i++)
            {
                if ((xmlProcedure.Sections[i].id > fromPosition) && (xmlProcedure.Sections[i].id <= toPosition))
                {
                    xmlProcedure.Sections[i].id = xmlProcedure.Sections[i].id - 1;
                }
            }
            xmlProcedure.Sections.Insert((toPosition - 1), temp);
        }

        bool isInRange<T>(List<T> x, int n)
        {
            if (n < x.Count && n > -1)
            {
                return true;
            }
            return false;
        }

        public void MoveTestStep(int fromSection, int toSection, int fromStep, int toStep)
        {
            TestProcedureSectionTest_Step temp = new TestProcedureSectionTest_Step();
            temp = xmlProcedure.Sections[fromSection - 1].Test_Step[fromStep - 1];
            temp.id = toStep;

            if (fromSection != toSection)
            {
                RemoveTestStep(fromSection, fromStep);

                for (int i = 0; i < xmlProcedure.Sections[toSection - 1].Test_Step.Count; i++)
                {
                    if ((xmlProcedure.Sections[toSection - 1].Test_Step[i].id >= toStep))
                    {
                        xmlProcedure.Sections[toSection - 1].Test_Step[i].id = xmlProcedure.Sections[toSection - 1].Test_Step[i].id + 1;
                    }
                }
                xmlProcedure.Sections[toSection - 1].Test_Step.Insert((toStep - 1), temp);
            }
            else
            {
                xmlProcedure.Sections[fromSection - 1].Test_Step.RemoveAt(fromStep - 1);

                for (int i = 0; i < xmlProcedure.Sections[toSection - 1].Test_Step.Count; i++)
                {
                    if (fromStep < toStep)
                    {
                        if ((xmlProcedure.Sections[toSection - 1].Test_Step[i].id > fromStep) && (xmlProcedure.Sections[toSection - 1].Test_Step[i].id <= toStep))
                        {
                            xmlProcedure.Sections[toSection - 1].Test_Step[i].id = xmlProcedure.Sections[toSection - 1].Test_Step[i].id - 1;
                        }
                    }
                    else
                    {
                        if ((xmlProcedure.Sections[toSection - 1].Test_Step[i].id < fromStep) && (xmlProcedure.Sections[toSection - 1].Test_Step[i].id >= toStep))
                        {
                            xmlProcedure.Sections[toSection - 1].Test_Step[i].id = xmlProcedure.Sections[toSection - 1].Test_Step[i].id + 1;
                        }
                    }
                }
                xmlProcedure.Sections[toSection - 1].Test_Step.Insert((toStep - 1), temp);
            }
        }


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
    }
}
