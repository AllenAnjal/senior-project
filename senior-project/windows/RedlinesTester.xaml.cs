using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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
    /// Interaction logic for RedlinesTester.xaml
    /// </summary>
    public partial class RedlinesTester : Window
    {
        TestProcedure tmp;

        bool MoveSec;
        bool MoveStep;
        bool EditControlStationResult;
        bool SectDescription;
        bool RemoveStep;
        bool RemoveSect;

        List<string> redlineOptions = new List<string>();

        public RedlinesTester(TestProcedure test)
        {
            InitializeComponent();
            tmp = test;
            LoadComboBox();
            ActionsList.ItemsSource = redlineOptions;
            ActionsList.SelectedIndex = -1;
            DisenableTextboxes();
        }

        private void NumberValidationTextBox(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }

        private void DisenableTextboxes()
        {
            MoveSec = false;
            MoveStep = false;
            EditControlStationResult = false;
            SectDescription = false;
            RemoveStep = false;
            RemoveSect = false;

            FromSection.IsEnabled = false;
            ToSection.IsEnabled = false;
            FromStep.IsEnabled = false;
            ToStep.IsEnabled = false;
            ProposedText.IsEnabled = false;
        }

        private void EnableMoveSection()
        {
            MoveSec = true;
            MoveStep = false;
            EditControlStationResult = false;
            SectDescription = false;
            RemoveStep = false;
            RemoveSect = false;

            FromSection.IsEnabled = true;
            ToSection.IsEnabled = true;
            FromStep.IsEnabled = false;
            ToStep.IsEnabled = false;
            ProposedText.IsEnabled = false;
        }

        private void EnableMoveTestStep()
        {
            MoveSec = false;
            MoveStep = true;
            EditControlStationResult = false;
            SectDescription = false;
            RemoveStep = false;
            RemoveSect = false;


            FromSection.IsEnabled = true;
            ToSection.IsEnabled = true;
            FromStep.IsEnabled = true;
            ToStep.IsEnabled = true;
            ProposedText.IsEnabled = false;
        }

        private void EnableEditControlActionStationResult()
        {
            MoveSec = false;
            MoveStep = false;
            EditControlStationResult = true;
            SectDescription = false;
            RemoveStep = false;
            RemoveSect = false;

            FromSection.IsEnabled = true;
            ToSection.IsEnabled = false;
            FromStep.IsEnabled = true;
            ToStep.IsEnabled = false;
            ProposedText.IsEnabled = true;
        }
        private void EnableSectionDescription()
        {
            MoveSec = false;
            MoveStep = false;
            EditControlStationResult = false;
            SectDescription = true;
            RemoveStep = false;
            RemoveSect = false;

            FromSection.IsEnabled = true;
            ToSection.IsEnabled = false;
            FromStep.IsEnabled = false;
            ToStep.IsEnabled = false;
            ProposedText.IsEnabled = true;
        }
        private void EnableRemoveTestStep()
        {
            MoveSec = false;
            MoveStep = false;
            EditControlStationResult = false;
            SectDescription = false;
            RemoveStep = true;
            RemoveSect = false;

            FromSection.IsEnabled = true;
            ToSection.IsEnabled = false;
            FromStep.IsEnabled = true;
            ToStep.IsEnabled = false;
            ProposedText.IsEnabled = false;
        }

        private void EnableRemoveSection()
        {
            MoveSec = false;
            MoveStep = false;
            EditControlStationResult = false;
            SectDescription = false;
            RemoveStep = false;
            RemoveSect = true;

            FromSection.IsEnabled = true;
            ToSection.IsEnabled = false;
            FromStep.IsEnabled = false;
            ToStep.IsEnabled = false;
            ProposedText.IsEnabled = false;
        }

        private void LoadComboBox()
        {
            redlineOptions.Add("Move Section");
            redlineOptions.Add("Move Test Step");
            redlineOptions.Add("Edit Control Action");
            redlineOptions.Add("Edit Station");
            redlineOptions.Add("Edit Expected Result");
            redlineOptions.Add("Edit Section Description");
            redlineOptions.Add("Remove Test Step");
            redlineOptions.Add("Remove Section");
        }

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var comboBox = sender as ComboBox;
            string val = comboBox.SelectedItem as string;
            this.Title = "Redlines Proposal: " + val;

            if (val == "Move Section")
            {
                EnableMoveSection();
            }

            else if (val == "Move Test Step")
            {
                EnableMoveTestStep();
            }

            else if (val == "Edit Control Action" || val == "Edit Station" || val == "Edit Expected Result")
            {
                EnableEditControlActionStationResult();
            }

            else if (val == "Edit Section Description")
            {
                EnableSectionDescription();
            }

            else if (val == "Remove Test Step")
            {
                EnableRemoveTestStep();
            }

            else if (val == "Remove Section")
            {
                EnableRemoveSection();
            }

        }


        private void clickAction()
        {
            if (Validation())
            {
                MessageBox.Show("Success! Time to Save!");

                if (MoveSec == true)
                {
                    TestProcedureRedlinesListRedlineSection section = new TestProcedureRedlinesListRedlineSection();
                    section.FromSection = FromSection.Text;
                    section.ToSection = ToSection.Text;
                    section.ActionToTake = "MoveSection";

                    section.id = tmp.RedlinesList.SectionList.Count + 1;
                    tmp.RedlinesList.SectionList.Add(section);
                }
                else if (MoveStep == true)
                {
                    TestProcedureRedlinesListRedlineStep step = new TestProcedureRedlinesListRedlineStep();
                    step.FromSection = FromSection.Text;
                    step.ToSection = ToSection.Text;

                    step.FromStep = FromStep.Text;
                    step.ToStep = ToStep.Text;
                    step.ActionToTake = "MoveStep";
                    step.id = tmp.RedlinesList.StepsList.Count + 1;
                    tmp.RedlinesList.StepsList.Add(step);
                }
                else if (EditControlStationResult == true)
                {
                    TestProcedureRedlinesListRedlineText text = new TestProcedureRedlinesListRedlineText();
                    text.FromSection = FromSection.Text;
                    text.FromStep = FromStep.Text;


                    if (ActionsList.SelectedItem as string == "Edit Control Action")
                    {
                        text.ActionToTake = "ControlAction";
                        text.ControlAction = "true";
                        text.ExpectedResult = "false";
                        text.SectionHeader = "false";
                        text.SectionDescription = "false";

                    }

                    else if (ActionsList.SelectedItem as string == "Edit Station")
                    {
                        text.ActionToTake = "Station";
                        text.ControlAction = "false";
                        text.SectionHeader = "true";
                        text.ExpectedResult = "false";
                        text.SectionDescription = "false";
                    }

                    else if (ActionsList.SelectedItem as string == "Edit Expected Result")
                    {
                        text.ActionToTake = "ExpectedResult";
                        text.ControlAction = "false";
                        text.SectionHeader = "false";
                        text.ExpectedResult = "true";
                        text.SectionDescription = "false";
                    }

                    text.Text = ProposedText.Text;
                    text.id = tmp.RedlinesList.TextList.Count + 1;

                    tmp.RedlinesList.TextList.Add(text);
                }
                else if (SectDescription == true)
                {
                    TestProcedureRedlinesListRedlineText text = new TestProcedureRedlinesListRedlineText();
                    text.FromSection = FromSection.Text;
                    text.ActionToTake = "SectionDescription";
                    text.SectionDescription = "true";
                    text.ControlAction = "false";
                    text.SectionHeader = "false";
                    text.ExpectedResult = "false";
                    text.Text = ProposedText.Text;
                    text.id = tmp.RedlinesList.TextList.Count + 1;

                    tmp.RedlinesList.TextList.Add(text);
                    //text.FromStep = 
                }
                else if (RemoveStep == true)
                {
                    TestProcedureRedlinesListRedlineStep step = new TestProcedureRedlinesListRedlineStep();
                    step.ActionToTake = "RemoveStep";
                    step.FromSection = FromSection.Text;
                    step.FromStep = FromStep.Text;
                    step.id = tmp.RedlinesList.StepsList.Count + 1;

                    tmp.RedlinesList.StepsList.Add(step);
                }
                else if (RemoveSect == true)
                {
                    TestProcedureRedlinesListRedlineSection section = new TestProcedureRedlinesListRedlineSection();
                    section.FromSection = FromSection.Text;
                    section.ActionToTake = "RemoveSection";
                    section.id = tmp.RedlinesList.SectionList.Count + 1;
                    tmp.RedlinesList.SectionList.Add(section);
                }

                XmlVerification.writeXmltoFile(tmp, "tmp.xml");
                this.Close();
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
        private bool FromSectionValidate()
        {
            //Validate if not empty, then if number, then if section exist
            if (FromSection.Text.Length == 0)
            {
                MessageBox.Show("There needs to be input for From Section", "Missing From Section input");
                return false;
            }
            else
            {
                List<TestProcedureSection> sectionList = tmp?.Sections;
                if (isInRange(sectionList, Convert.ToInt32(FromSection.Text) - 1))
                {
                    return true;
                }
                else
                {
                    MessageBox.Show("Input out of range", "Input Error");
                    return false;
                }
                
            }

            
        }

        private bool isInRange<T>(List<T> x, int n)
        {
            if (n < x.Count && n > -1)
            {
                return true;
            }
            return false;
        }

        private bool ToSectionValidate()
        {
            //Validate if not empty, then if number, then if section exist
            if (ToSection.Text.Length == 0)
            {
                MessageBox.Show("There needs to be input for To Section", "Missing To Section input");
                return false;
            }
            else
            {
                List<TestProcedureSection> sectionList = tmp?.Sections;
                if (isInRange(sectionList, Convert.ToInt32(ToSection.Text) - 1))
                {
                    return true;
                }
                else
                {
                    MessageBox.Show("Input out of range", "Input Error");
                    return false;
                }

            }
        }

        private bool FromStepValidate()
        {
            //Validate if not empty, then if number, then if section exist, then if step exist
            if (FromStep.Text.Length == 0)
            {
                MessageBox.Show("There needs to be input for From Step", "Missing From Step Input");
                return false;
            }

            else
            {
                TestProcedureSectionTest_Step temp = new TestProcedureSectionTest_Step();

                try
                {
                    temp = tmp.Sections[Convert.ToInt32(FromSection.Text) - 1].Test_Step[Convert.ToInt32(FromStep.Text) - 1];
                    return true;
                }
                catch (ArgumentOutOfRangeException e)
                {
                    if(e.Source != null)
                    {
                        MessageBox.Show("Error: {0}", e.Source);
                        return false;
                    }
                    /*MessageBox.Show("Input out of bounds", "Error");
                    return false;*/
                }
                //return true;
            }
            return false;
        }

        private bool ToStepValidate()
        {
            //Validate if not empty, then if number, then if section exist, then if step exist
            if (ToStep.Text.Length == 0)
            {
                MessageBox.Show("There needs to be input for To Step", "Missing To Step Input");
                return false;
            }

            else
            {
                TestProcedureSectionTest_Step temp = new TestProcedureSectionTest_Step();

                try
                {
                    temp = tmp.Sections[Convert.ToInt32(ToSection.Text) - 1].Test_Step[Convert.ToInt32(ToStep.Text) - 1];
                    return true;
                }
                catch (ArgumentOutOfRangeException e)
                {
                    if (e.Source != null)
                    {
                        MessageBox.Show("Error: {0}", e.Source);
                        return false;
                    }
                }
            }
            return false;
        }

        private bool ProposedTextValidate()
        {
            //Validate if not empty
            if (ProposedText.Text.Length == 0)
            {
                MessageBox.Show("There needs to be text for Proposed Text", "Missing Proposed Text");
                return false;
            }
            return true;
        }
        private bool Validation()
        {
            if (MoveSec)
            {
                //Check FromSection && ToSection
                return FromSectionValidate() && ToSectionValidate();
            }

            else if (MoveStep)
            {
                //Check FromSection, ToSection, FromStep and ToStep
                return FromSectionValidate() && ToSectionValidate() && FromStepValidate() && ToStepValidate();
            }

            else if (EditControlStationResult)
            {
                //Check FromSection, FromStep, and ProposedText
                return FromSectionValidate() && FromStepValidate() && ProposedTextValidate();
            }

            else if (SectDescription)
            {
                //Check FromSection and ProposedText
                return FromSectionValidate() && ProposedTextValidate();
            }

            else if (RemoveStep)
            {
                //Check FromSection and FromStep
                return FromSectionValidate() && FromStepValidate();
            }

            else if (RemoveSect)
            {
                //Check FromSection
                return FromSectionValidate();
            }
            else if (ActionsList.SelectedIndex == -1)
            {
                MessageBox.Show("Redlines Option must be selected", "Select a Redlines Option");
                return false;
            }
            MessageBox.Show("Complete the form", "Error");
            return false;
        }
    }
}
