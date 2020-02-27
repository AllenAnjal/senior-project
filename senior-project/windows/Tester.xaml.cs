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
using Microsoft.Win32;
using System.Windows.Threading;
using System.Diagnostics;
using System.IO;
using Microsoft.VisualBasic;


namespace senior_project
{
    /// <summary>
    /// Interaction logic for Tester.xaml
    /// </summary>
    public partial class Tester : Window
    {
        String commentDefault = "Leave a comment";
        TestProcedure xmlProcedure;
        bool hasCommented = false;
        bool redlineClicked = false;
        DispatcherTimer t;
        DateTime start;
        exportWindow export = new exportWindow();

        public Tester(TestProcedure newProcedure)
        {
            InitializeComponent();
            xmlProcedure = newProcedure;

            userInfoPage x = new userInfoPage(xmlProcedure);
            x.ShowDialog();

            XmlVerification.xmltoTreeView(xmlProcedure, ref treeView1);
            beginTest();

            t = new DispatcherTimer(new TimeSpan(0, 0, 0, 0, 50), DispatcherPriority.Background, t_Tick, Dispatcher.CurrentDispatcher);
           
            start = DateTime.Now;
        }
        private void t_Tick(object sender, EventArgs e)
        {
            timer.Text = Convert.ToString(DateTime.Now - start);
        }
        #region buttons

        private void passAction()
        {
            hasCommented = false;
            writeStep(true);
            XmlVerification.writeXmltoFile(xmlProcedure, "tmp.xml");
            forwardStep();
        }

        private void failAction()
        {
            loadComment();
            hasCommented = false;
            writeStep(false);
            XmlVerification.writeXmltoFile(xmlProcedure, "tmp.xml");
            XmlVerification.exportCsv(xmlProcedure, treeView1);
            forwardStep();
        }

        //Update XML tmp file, forward step
        private void PassButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                passAction();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}");
            }

        }

        private void FailButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                failAction();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}");
            }
        }

       

        public void writeStep(bool pass)
        {
            TreeViewItem item = (TreeViewItem)treeView1.SelectedItem;

            if (item?.Tag != null && item?.Tag is TestProcedureSectionTest_Step)
            {
                TestProcedureSectionTest_Step step = ((TestProcedureSectionTest_Step)item.Tag);
                if (pass)
                {
                    step.Pass = "True";
                    step.Fail = "False";
                }
                else
                {
                    step.Fail = "True";
                    step.Pass = "False";
                }
            }
        }



        private void RedlineButton_Click(object sender, RoutedEventArgs e)
        {
            RedlinesTester red = new RedlinesTester(xmlProcedure);
            redlineClicked = !redlineClicked;
            if (redlineClicked)
            {
              
                
                step.Background = new SolidColorBrush(Color.FromRgb(254,1,1));
                station.Background = new SolidColorBrush(Color.FromRgb(254, 1, 1));
                control_action.Background = new SolidColorBrush(Color.FromRgb(254, 1, 1));
                expected_result.Background = new SolidColorBrush(Color.FromRgb(254, 1, 1));
            }
            else
            {
                step.Background = new SolidColorBrush(Color.FromRgb(2, 93, 186));
                station.Background = new SolidColorBrush(Color.FromRgb(2, 93, 186));
                control_action.Background = new SolidColorBrush(Color.FromRgb(2, 93, 186));
                expected_result.Background = new SolidColorBrush(Color.FromRgb(2, 93, 186));
            }
            //red.ShowDialog();
        }


        //If there is an image in the XML step, send image to button
        //If no image available, change button to red and image unavailable (or remove button entirely)
        //Change width to 0 to hide. 
     

        private void SaveXmlButton_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog saveFile = new SaveFileDialog();

            saveFile.Filter = "XML Documents (.xml)|*.xml";
            saveFile.FileName = "XMLSave";

            if (saveFile.ShowDialog().GetValueOrDefault())
            {
                Console.WriteLine(saveFile.FileName);
            }

            XmlVerification.writeXmltoFile(xmlProcedure, saveFile.FileName);
        }
        #endregion

        #region TreeView
        private void TreeView1_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            updateTextBoxes();
        }

        private void TreeView1_PreviewMouseLeftButtonUp_1(object sender, MouseButtonEventArgs e)
        {
            updateTextBoxes();
        }
        //Initialize the treeView to section 0, step 0 


        //Navigate to next step in test procedure

        #endregion

        #region Test Steps
        private void forwardStep()
        {
            NextStep();
        }

        private void beginTest()
        {
            TreeViewItem beginSection = (treeView1?.Items[0] as TreeViewItem);
            if (beginSection != null && beginSection.HasItems)
            {
                beginSection.IsExpanded = true;
                (beginSection.Items[0] as TreeViewItem).IsSelected = true;
            }
        }

        private void NextStep()
        {
            TreeViewItem selectedItem = treeView1?.SelectedItem as TreeViewItem;
            TreeViewItem parentItem = selectedItem?.Parent as TreeViewItem;

            //If parent is Section, grab current ids and iterate to the next step or section
            if (selectedItem?.Tag is TestProcedureSectionTest_Step)
            {
                TestProcedureSectionTest_Step currStep = (TestProcedureSectionTest_Step)selectedItem?.Tag;
                TestProcedureSection currSection = (TestProcedureSection)parentItem?.Tag;

                int stepIndex = currStep.id - 1;
                int sectionIndex = currSection.id - 1;

                //Move to next step in same section
                if (stepIndex < currSection.Test_Step.Count - 1)
                {
                    (parentItem.Items[stepIndex + 1] as TreeViewItem).IsSelected = true;
                }
                //Move to first step in next section
                else if (sectionIndex < treeView1.Items.Count - 1)
                {
                    TreeViewItem nextSection = treeView1?.Items[sectionIndex + 1] as TreeViewItem;
                    if (nextSection.HasItems)
                    {
                        nextSection.IsExpanded = true;
                        (nextSection.Items[0] as TreeViewItem).IsSelected = true;
                    }
                }
                //No more steps or sections
                else
                {
                    MessageBox.Show("Procedure is complete!");
                    export.Show();
                    this.Close();
                }
            }
        }
        #endregion


        private void updateTextBoxes()
        {
            TreeViewItem item = (TreeViewItem)treeView1.SelectedItem;
            if (item?.Tag is TestProcedureSectionTest_Step)
            {
                TestProcedureSectionTest_Step step = (TestProcedureSectionTest_Step)item.Tag;

                stepBox.Text = step.id.ToString();
                stationBox.Text = step.Station.ToString();
                controlActionBox.Text = step.Control_Action.ToString();
                expectedResultBox.Text = step.Expected_Result.ToString();

              
            }
        }



        private void Exit_Button(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void CommentButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                loadComment();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}");
            }
        }

        private void loadComment()
        {
            TestProcedureSectionTest_Step step = XmlVerification.getCurrentStep(ref treeView1);
            if (step != null && !hasCommented)
            {
                commentWindow newWindow = new commentWindow(ref step);
                newWindow.ShowDialog();
                if (step.Comments != commentDefault)
                {
                    hasCommented = true;
                    XmlVerification.writeXmltoFile(xmlProcedure, "tmp.xml");
                }
            }
        }
    }
}
