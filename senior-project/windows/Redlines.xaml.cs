using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
    /// Interaction logic for Redlines.xaml
    /// </summary>
    public partial class Redlines : Window
    {
        #region Initilization 
        TestProcedure tmp = new TestProcedure();
        TestAdmin testAdmin;
        string redlineTypeLoaded;

        public Redlines(ref TestProcedure newProcedure, bool createNewTP)
        {
            InitializeComponent();
            tmp = newProcedure;
            testAdmin = new TestAdmin(tmp, false); //creates the test admin object with the pre-existing test procedure
        }
        #endregion

        #region Window Close / Exit Button
        /// <summary>
        /// Exits the current instance of the window for Redlines
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Exit_button_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        /// <summary>
        /// Upon the Window Closing saves to the temporary file and then calls Test Admin Window
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_Closed(object sender, EventArgs e)
        {
            //MainWindow t = new MainWindow();
            //t.Show();
            XmlVerification.writeXmltoFile(tmp, "tmp.xml");
            this.Close();
            TestAdmin t = new TestAdmin(tmp, false);
            t.Show();
        }
        #endregion

        #region Populate Red Line Data
        /// <summary>
        /// Populates the redlines window with the current redline under consideration
        /// </summary>
        private void PopulateRedLineData()
        {
            redlineTypeLoaded = ""; //clears any previously loaded information

            if (tmp.RedlinesList.TextList != null && tmp.RedlinesList.TextList.Count > 0) //Text redlines handled first
            {
                FromSection.Visibility = Visibility.Visible;
                LblFromSection.Visibility = Visibility.Visible;

                ToSection.Visibility = Visibility.Hidden;
                LblToSection.Visibility = Visibility.Hidden;

                FromStep.Visibility = Visibility.Visible;
                LblFromStep.Visibility = Visibility.Visible;

                ToStep.Visibility = Visibility.Hidden;
                LblToStep.Visibility = Visibility.Hidden;

                if (tmp.RedlinesList.TextList.ElementAt(0).ActionToTake.ToString() == "ControlAction")
                {
                    ActionToTake.Content = "Change Control Action";
                    CurrentTextBlock.Text = tmp.Sections[Int32.Parse(tmp.RedlinesList.TextList[0].FromSection) - 1].Test_Step[Int32.Parse(tmp.RedlinesList.TextList[0].FromStep) - 1].Control_Action;
                }
                else if (tmp.RedlinesList.TextList.ElementAt(0).ActionToTake.ToString() == "Station")
                {
                    ActionToTake.Content = "Change Station";
                    CurrentTextBlock.Text = tmp.Sections[Int32.Parse(tmp.RedlinesList.TextList[0].FromSection) - 1].Test_Step[Int32.Parse(tmp.RedlinesList.TextList[0].FromStep) - 1].Station;
                }
                else if (tmp.RedlinesList.TextList.ElementAt(0).ActionToTake.ToString() == "ExpectedResult")
                {
                    ActionToTake.Content = "Change Expected Result";
                    CurrentTextBlock.Text = tmp.Sections[Int32.Parse(tmp.RedlinesList.TextList[0].FromSection) - 1].Test_Step[Int32.Parse(tmp.RedlinesList.TextList[0].FromStep) - 1].Expected_Result;
                }
                else if (tmp.RedlinesList.TextList.ElementAt(0).ActionToTake.ToString() == "SectionHeading")
                {
                    ActionToTake.Content = "Change Section Heading";
                    CurrentTextBlock.Text = tmp.Sections[Int32.Parse(tmp.RedlinesList.TextList[0].FromSection) - 1].Heading;
                }
                else if (tmp.RedlinesList.TextList.ElementAt(0).ActionToTake.ToString() == "SectionDescription")
                {
                    ActionToTake.Content = "Change Section Description";
                    CurrentTextBlock.Text = tmp.Sections[Int32.Parse(tmp.RedlinesList.TextList[0].FromSection) - 1].Description;
                }

                FromSection.Content = tmp.RedlinesList.TextList.ElementAt(0).FromSection.ToString();
                FromStep.Content = tmp.RedlinesList.TextList.ElementAt(0).FromStep.ToString();
                SuggestedTextBlock.Text = tmp.RedlinesList.TextList.ElementAt(0).Text.ToString();

                redlineTypeLoaded = "text"; 

            }
            else if (tmp.RedlinesList.StepsList != null && tmp.RedlinesList.StepsList.Count > 0) //Steps
            {
                SuggestedTextBlock.Text = "";
                SuggestedTextBlock.Visibility = Visibility.Hidden;
                CurrentTextBlock.Text = "";
                CurrentTextBlock.Visibility = Visibility.Hidden;

                FromSection.Visibility = Visibility.Visible;
                LblFromSection.Visibility = Visibility.Visible;

                ToSection.Visibility = Visibility.Hidden;
                LblToSection.Visibility = Visibility.Hidden;

                FromStep.Visibility = Visibility.Visible;
                LblFromStep.Visibility = Visibility.Visible;

                ToStep.Visibility = Visibility.Hidden;
                LblToStep.Visibility = Visibility.Hidden;

                if (tmp.RedlinesList.StepsList.ElementAt(0).ActionToTake == "MoveStep")
                {
                    ToStep.Visibility = Visibility.Visible;
                    LblToStep.Visibility = Visibility.Visible;

                    ActionToTake.Content = "Move Step";
                    ToStep.Content = tmp.RedlinesList.StepsList.ElementAt(0).ToStep.ToString();
                }
                else if (tmp.RedlinesList.StepsList.ElementAt(0).ActionToTake == "RemoveStep")
                {
                    FromStep.Visibility = Visibility.Visible;
                    LblFromStep.Visibility = Visibility.Visible;

                    ActionToTake.Content = "Remove Step";
                    ToStep.Content = ""; 
                }

                FromSection.Content = tmp.RedlinesList.StepsList.ElementAt(0).FromSection.ToString();
                FromStep.Content = tmp.RedlinesList.StepsList.ElementAt(0).FromStep.ToString();
                

                redlineTypeLoaded = "step";

            }
            else if (tmp.RedlinesList.SectionList != null && tmp.RedlinesList.SectionList.Count > 0) //Sections
            {
                SuggestedTextBlock.Text = "";
                SuggestedTextBlock.Visibility = Visibility.Hidden;
                CurrentTextBlock.Text = "";
                CurrentTextBlock.Visibility = Visibility.Hidden;

                StepBorder.Visibility = groupBoxNew.Visibility = groupBoxOld.Visibility = Visibility.Hidden;
                
                FromSection.Visibility = Visibility.Visible;
                LblFromSection.Visibility = Visibility.Visible;

                ToSection.Visibility = Visibility.Hidden;
                LblToSection.Visibility = Visibility.Hidden;

                FromStep.Visibility = Visibility.Hidden;
                LblFromStep.Visibility = Visibility.Hidden;

                ToStep.Visibility = Visibility.Hidden;
                LblToStep.Visibility = Visibility.Hidden;

                if (tmp.RedlinesList.SectionList.ElementAt(0).ActionToTake == "MoveSection")
                {
                    ToSection.Visibility = Visibility.Visible;
                    LblToSection.Visibility = Visibility.Visible;

                    ActionToTake.Content = "Move Section";
                    ToSection.Content = tmp.RedlinesList.SectionList.ElementAt(0).ToSection.ToString();
                }

                else if (tmp.RedlinesList.SectionList.ElementAt(0).ActionToTake == "RemoveSection")
                {
                    ActionToTake.Content = "Remove Section";
                    ToSection.Content = "";
                }
                    

                FromSection.Content = tmp.RedlinesList.SectionList.ElementAt(0).FromSection.ToString();
                

                redlineTypeLoaded = "section";
            }

        }
        #endregion

        #region Red Line Text Fields
        /// <summary>
        /// Redline text change handler
        /// </summary>
        private void RedLineText()
        {
            
                ChangeText(tmp.RedlinesList.TextList.ElementAt(0).Text.ToString(), tmp.RedlinesList.TextList.ElementAt(0).ActionToTake.ToString(), Convert.ToInt32(tmp.RedlinesList.TextList.ElementAt(0).FromSection), Convert.ToInt32(tmp.RedlinesList.TextList.ElementAt(0).FromStep));

                RemoveRedLine("ChangeText");

                XmlVerification.writeXmltoFile(tmp, "tmp.xml");

        }
        #endregion

        #region Red Line Steps
        /// <summary>
        /// Redline test step change handler
        /// </summary>
        private void RedLineStep()
        {
            SuggestedTextBlock.Text = "";
            SuggestedTextBlock.Visibility = Visibility.Hidden;
            CurrentTextBlock.Text = "";
            CurrentTextBlock.Visibility = Visibility.Hidden;

            int fromSect, toSect, fromTestStep, toTestStep;

            fromSect = Int32.Parse(tmp.RedlinesList.StepsList.ElementAt(0).FromSection);
            fromTestStep = Int32.Parse(tmp.RedlinesList.StepsList.ElementAt(0).FromStep);

            if (tmp.RedlinesList.StepsList.ElementAt(0).ActionToTake == "MoveStep")
            {

                toSect = Int32.Parse(tmp.RedlinesList.StepsList.ElementAt(0).ToSection);
                toTestStep = Int32.Parse(tmp.RedlinesList.StepsList.ElementAt(0).ToStep);

                MoveTestStep(fromSect, toSect, fromTestStep, toTestStep);
            }
            else if (tmp.RedlinesList.StepsList.ElementAt(0).ActionToTake == "RemoveStep")
            {
                RemoveTestStep(fromSect, fromTestStep);
            }

            RemoveRedLine("ChangeStep");

            XmlVerification.writeXmltoFile(tmp, "tmp.xml");
            
        }
        #endregion

        #region Red Line Sections
        /// <summary>
        /// Redline section change handler
        /// </summary>
        /// <param name="id"></param>
        private void RedLineSection()
        {
            if (tmp.RedlinesList.SectionList != null && tmp.RedlinesList.SectionList.Count > 0) //Sections
            {
                SuggestedTextBlock.Text = "";
                SuggestedTextBlock.Visibility = Visibility.Hidden;
                CurrentTextBlock.Text = "";
                CurrentTextBlock.Visibility = Visibility.Hidden;

                int fromSect, toSect;

                fromSect = Int32.Parse(tmp.RedlinesList.SectionList.ElementAt(0).FromSection);
                

                if (tmp.RedlinesList.SectionList.ElementAt(0).ActionToTake == "MoveSection")
                {
                    toSect = Int32.Parse(tmp.RedlinesList.SectionList.ElementAt(0).ToSection);
                    MoveSection(fromSect, toSect);
                }
                else if (tmp.RedlinesList.SectionList.ElementAt(0).ActionToTake == "RemoveSection")
                {

                    RemoveSection(fromSect);
                }

                RemoveRedLine("ChangeSection");

                XmlVerification.writeXmltoFile(tmp, "tmp.xml");

            }

            //Closes the REDLINE window once all the redlines have been dealt with
            /*
            if (!tmp.RedlinesList.TextList.Any() && !tmp.RedlinesList.StepsList.Any() && !tmp.RedlinesList.SectionList.Any())
                this.Close();
            */
        }
        #endregion

        #region On Load
        /// <summary>
        /// Loads the first Redline when the Window is first loaded (JUST FILLS IT IN TAKES NO ACTION)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            PopulateRedLineData();
        }
        #endregion


        #region Button Actions
        /// <summary>
        /// Accepts All remaining Proposed Redlines 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// 
        private void acceptAllAction()
        {
            for (int i = 0; i < tmp.RedlinesList.TextList.Count; i++) //Perform all of the text changes to be made
            {
                RedLineText();
            }

            for (int i = 0; i < tmp.RedlinesList.StepsList.Count; i++)
            {
                RedLineStep();
            }

            for (int i = 0; i < tmp.RedlinesList.SectionList.Count; i++)
            {
                RedLineSection();
            }

            //Closes the REDLINE window once all the redlines have been dealt with
            if (!tmp.RedlinesList.TextList.Any() && !tmp.RedlinesList.StepsList.Any() && !tmp.RedlinesList.SectionList.Any())
                this.Close();
        }

        private void acceptClickAction()
        {
            if (redlineTypeLoaded == "text")
                RedLineText(); //handle the redline change to make

            else if (redlineTypeLoaded == "step")
                RedLineStep();

            else if (redlineTypeLoaded == "section")
                RedLineSection();

            XmlVerification.writeXmltoFile(tmp, "tmp.xml");

            PopulateRedLineData(); //always repopulate the redlines window with the next redline under consideration

            //Closes the REDLINE window once all the redlines have been dealt with
            if (!tmp.RedlinesList.TextList.Any() && !tmp.RedlinesList.StepsList.Any() && !tmp.RedlinesList.SectionList.Any())
                this.Close();
        }

        private void rejectAction()
        {
            if (redlineTypeLoaded == "text")
                RemoveRedLine("ChangeText");

            else if (redlineTypeLoaded == "step")
                RemoveRedLine("ChangeStep");

            else if (redlineTypeLoaded == "section")
                RemoveRedLine("ChangeSection");

            XmlVerification.writeXmltoFile(tmp, "tmp.xml");

            //Closes the REDLINE window once all the redlines have been dealt with
            if (!tmp.RedlinesList.TextList.Any() && !tmp.RedlinesList.StepsList.Any() && !tmp.RedlinesList.SectionList.Any())
                this.Close();

            PopulateRedLineData();
        }

        private void rejectAllAction()
        {
            for (int i = 0; i < tmp.RedlinesList.TextList.Count; i++) //Perform all of the text changes to be made
            {
                RemoveRedLine("ChangeText");
            }

            for (int i = 0; i < tmp.RedlinesList.StepsList.Count; i++)
            {
                RemoveRedLine("ChangeStep");
            }

            for (int i = 0; i < tmp.RedlinesList.SectionList.Count; i++)
            {
                RemoveRedLine("ChangeSection");
            }

            XmlVerification.writeXmltoFile(tmp, "tmp.xml");

            //Closes the REDLINE window once all the redlines have been dealt with
            if (!tmp.RedlinesList.TextList.Any() && !tmp.RedlinesList.StepsList.Any() && !tmp.RedlinesList.SectionList.Any())
                this.Close();
        }
        #endregion

        #region Buttons
        private void btn_accept_all_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                acceptAllAction();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Exception Handled: {ex.Message}");
            }

        }

        /// <summary>
        /// Accept the Current Proposed Redline
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_accept_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                acceptClickAction();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Exception Handled: {ex.Message}");
            }
        }

        /// <summary>
        /// Call remove redline to delete a single suggested redline
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_reject_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                rejectAction();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Exception Handled: {ex.Message}");
            }

        }

        /// <summary>
        /// Deletes all proposed changes from the XML document
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_reject_all_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                rejectAllAction();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Exception Handled: {ex.Message}");
            }
        }
        #endregion








        #region Remove Red Line from Red Lines List
        /// <summary>
        /// Removes the first item in the associated redline list
        /// </summary>
        /// <param name="RedLineToDel"></param>
        public void RemoveRedLine(string RedLineToDel)
        {

            //TestProcedure testProcedure = new TestProcedure();
            //tmp = testProcedure;
            if (RedLineToDel == "ChangeText")
            {

                tmp.RedlinesList.TextList.RemoveAt(0);

                //Need to Give New Id Values to sections since a section was removed
                for (int i = 0; i < tmp.RedlinesList.TextList.Count; i++)
                {
                    if (tmp.RedlinesList.TextList[i].id > 1)
                    {
                        tmp.RedlinesList.TextList[i].id = (tmp.RedlinesList.TextList[i].id) - 1;
                    }
                }
            }
            else if (RedLineToDel == "ChangeStep")
            {

                tmp.RedlinesList.StepsList.RemoveAt(0);

                //Need to Give New Id Values to sections since a section was removed
                for (int i = 0; i < tmp.RedlinesList.StepsList.Count; i++)
                {
                    if (tmp.RedlinesList.StepsList[i].id > 1)
                    {
                        tmp.RedlinesList.StepsList[i].id = (tmp.RedlinesList.StepsList[i].id) - 1;
                    }
                }
            }
            else if (RedLineToDel == "ChangeSection")
            {
                tmp.RedlinesList.SectionList.RemoveAt(0);

                //Need to Give New Id Values to sections since a section was removed
                for (int i = 0; i < tmp.RedlinesList.SectionList.Count; i++)
                {
                    if (tmp.RedlinesList.SectionList[i].id > 1)
                    {
                        tmp.RedlinesList.SectionList[i].id = tmp.RedlinesList.SectionList[i].id - 1;
                    }
                }
            }
        }
        #endregion

        #region Change Text of a Test Procedure
        /// <summary>
        /// Based on Action To Take Field perform the necessary action
        /// </summary>
        /// <param name="newText"></param>
        /// <param name="ActiontoTake"></param>
        /// <param name="fromSection"></param>
        /// <param name="fromStep"></param>
        public void ChangeText(string newText, string ActiontoTake, int fromSection, int fromStep)
        {
            if (ActiontoTake == "ControlAction")
                tmp.Sections[fromSection - 1].Test_Step[fromStep - 1].Control_Action = newText;
            else if (ActiontoTake == "Station")
                tmp.Sections[fromSection - 1].Test_Step[fromStep - 1].Station = newText;
            else if (ActiontoTake == "ExpectedResult")
                tmp.Sections[fromSection - 1].Test_Step[fromStep - 1].Expected_Result = newText;
            else if (ActiontoTake == "SectionHeading")
                tmp.Sections[fromSection - 1].Heading = newText;
            else if (ActiontoTake == "SectionDescription")
                tmp.Sections[fromSection - 1].Description = newText;
        }
        #endregion

        #region Move and Remove a Test Step from a Test Procedure
        /// <summary>
        /// Moves a Test Step to a different position either in the same section or to a different section
        /// </summary>
        /// <param name="fromSection"></param>
        /// <param name="toSection"></param>
        /// <param name="fromStep"></param>
        /// <param name="toStep"></param>


        public void MoveTestStep(int fromSection, int toSection, int fromStep, int toStep)
        {
            TestProcedureSectionTest_Step temp = new TestProcedureSectionTest_Step();
            temp = tmp.Sections[fromSection - 1].Test_Step[fromStep - 1];
            temp.id = toStep;

            if (fromSection != toSection)
            {
                RemoveTestStep(fromSection, fromStep);

                for (int i = 0; i < tmp.Sections[toSection - 1].Test_Step.Count; i++)
                {
                    if ((tmp.Sections[toSection - 1].Test_Step[i].id >= toStep))
                    {
                        tmp.Sections[toSection - 1].Test_Step[i].id = tmp.Sections[toSection - 1].Test_Step[i].id + 1;
                    }
                }
                tmp.Sections[toSection - 1].Test_Step.Insert((toStep - 1), temp);
            }
            else
            {
                tmp.Sections[fromSection - 1].Test_Step.RemoveAt(fromStep - 1);

                for (int i = 0; i < tmp.Sections[toSection - 1].Test_Step.Count; i++)
                {
                    if (fromStep < toStep)
                    {
                        if ((tmp.Sections[toSection - 1].Test_Step[i].id > fromStep) && (tmp.Sections[toSection - 1].Test_Step[i].id <= toStep))
                        {
                            tmp.Sections[toSection - 1].Test_Step[i].id = tmp.Sections[toSection - 1].Test_Step[i].id - 1;
                        }
                    }
                    else
                    {
                        if ((tmp.Sections[toSection - 1].Test_Step[i].id < fromStep) && (tmp.Sections[toSection - 1].Test_Step[i].id >= toStep))
                        {
                            tmp.Sections[toSection - 1].Test_Step[i].id = tmp.Sections[toSection - 1].Test_Step[i].id + 1;
                        }
                    }
                }
                tmp.Sections[toSection - 1].Test_Step.Insert((toStep - 1), temp);
            }
        }

        /// <summary>
        /// Removes a Test Step from a specified Test Section
        /// </summary>
        /// <param name="FromSection"></param>
        /// <param name="SteptoRemove"></param>
        public void RemoveTestStep(int FromSection, int SteptoRemove)
        {
            for (int i = 0; i < tmp.Sections[FromSection - 1].Test_Step.Count; i++)
            {
                if (tmp.Sections[FromSection - 1].Test_Step[i].id > SteptoRemove)
                {
                    tmp.Sections[FromSection - 1].Test_Step[i].id = tmp.Sections[FromSection - 1].Test_Step[i].id - 1;
                }
            }
            tmp.Sections[FromSection - 1].Test_Step.RemoveAt(SteptoRemove - 1);
        }
        #endregion

        #region Move and Remove a Section from a Test Procedure
        /// <summary>
        /// Moves a Test Section to the new position and readjusts other section IDs accordingly
        /// </summary>
        /// <param name="fromPosition"></param>
        /// <param name="toPosition"></param>
        public void MoveSection(int fromPosition, int toPosition)
        {
            TestProcedureSection temp = new TestProcedureSection();
            temp = tmp.Sections[fromPosition - 1];
            temp.id = toPosition;

            tmp.Sections.RemoveAt(fromPosition - 1);

            for (int i = 0; i < tmp.Sections.Count; i++)
            {
                if ((tmp.Sections[i].id > fromPosition) && (tmp.Sections[i].id <= toPosition))
                {
                    tmp.Sections[i].id = tmp.Sections[i].id - 1;
                }
            }
            tmp.Sections.Insert((toPosition - 1), temp);
        }

        /// <summary>
        /// Removes the Section from the Test Procedure
        /// </summary>
        /// <param name="SectionToRemove"></param>
        public void RemoveSection(int SectionToRemove)
        {
            tmp.Sections.RemoveAt(SectionToRemove - 1);

            //Need to Give New Id Values to sections since a section was removed
            for (int i = 0; i < tmp.Sections.Count; i++)
            {
                if (tmp.Sections[i].id > SectionToRemove)
                {
                    tmp.Sections[i].id = (tmp.Sections[i].id) - 1;
                }
            }
        }
        #endregion
    }
}