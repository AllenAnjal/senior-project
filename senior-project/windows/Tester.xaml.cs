using Microsoft.Win32;
using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using System.Collections.Generic;
using System.Xml;

namespace senior_project
{
    /// <summary>
    /// Interaction logic for Tester.xaml
    /// </summary>
    public partial class Tester : Window
    {
        private MainWindow main;
        private String commentDefault = "Leave a comment";
        private TestProcedure xmlProcedure;
        private bool hasCommented = false;
        private bool redlineClicked = false;
        private DispatcherTimer t;
        private DateTime start;
        private exportWindow export = new exportWindow();
        private commentWindow cmt = new commentWindow();

        //  XML TreeView Implementation with XmlDataProvider and XmlDocument
        private XmlDocument _xml;

        private List<XmlElement> listTestSteps;

        private XmlDataProvider _xmlDataProvider;
        private TreeView _treeView;

        public Tester(MainWindow mw, String xmlFile)
        {
            InitializeComponent();
            main = mw;

            _xml = new XmlDocument();
            //listTestSteps = new List<XmlElement>();

            try
            {
                _xml.Load(xmlFile);
            }
            catch (Exception err)
            {
                MessageBox.Show(err.Message, "Error");
            }

            userInfoPage x = new userInfoPage(xmlProcedure);
            x.ShowDialog();

            t = new DispatcherTimer(new TimeSpan(0, 0, 0, 0, 50), DispatcherPriority.Background, t_Tick, Dispatcher.CurrentDispatcher);

            start = DateTime.Now;
        }

        public Tester(TestProcedure newProcedure)
        {
            InitializeComponent();
            xmlProcedure = newProcedure;

            userInfoPage x = new userInfoPage(xmlProcedure);
            x.ShowDialog();

            //XmlVerification.xmltoTreeView(xmlProcedure, ref treeView1);
            //beginTest();

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
            if (_treeView.SelectedItem == null) return;
            hasCommented = false;
            XmlElement sel = _treeView.SelectedItem as XmlElement;
            if (sel.Name == "Test_Step")
            {
                sel["Pass"].InnerText = "true";
                sel["Fail"].InnerText = "false";
            }

            //writeStep(true);
            //XmlVerification.writeXmltoFile(xmlProcedure, "tmp.xml");
            //forwardStep();
        }

        private void failAction()
        {
            if (_treeView.SelectedItem == null) return;
            hasCommented = false;
            XmlElement sel = _treeView.SelectedItem as XmlElement;
            
            if (sel.Name == "Test_Step")
            {
                sel["Pass"].InnerText = "false";
                sel["Fail"].InnerText = "true";
                cmt.Show();
            }
            //loadComment();
            //hasCommented = false;
            //writeStep(false);
            //XmlVerification.writeXmltoFile(xmlProcedure, "tmp.xml");
            //XmlVerification.exportCsv(xmlProcedure, treeView1);
            //forwardStep();
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
                lblStep.Background = new SolidColorBrush(Color.FromRgb(254, 1, 1));
                lblStation.Background = new SolidColorBrush(Color.FromRgb(254, 1, 1));
                lblControlAction.Background = new SolidColorBrush(Color.FromRgb(254, 1, 1));
                lblExpectedResult.Background = new SolidColorBrush(Color.FromRgb(254, 1, 1));
                borderStep.Background = new SolidColorBrush(Color.FromRgb(254, 1, 1));
                borderStation.Background = new SolidColorBrush(Color.FromRgb(254, 1, 1));
                borderControl.Background = new SolidColorBrush(Color.FromRgb(254, 1, 1));
                borderExp.Background = new SolidColorBrush(Color.FromRgb(254, 1, 1));
            }
            else
            {
                lblStep.Background = new SolidColorBrush(Color.FromRgb(2, 93, 186));
                lblStation.Background = new SolidColorBrush(Color.FromRgb(2, 93, 186));
                lblControlAction.Background = new SolidColorBrush(Color.FromRgb(2, 93, 186));
                lblExpectedResult.Background = new SolidColorBrush(Color.FromRgb(2, 93, 186));
                borderStep.Background = new SolidColorBrush(Color.FromRgb(2, 93, 186));
                borderStation.Background = new SolidColorBrush(Color.FromRgb(2, 93, 186));
                borderControl.Background = new SolidColorBrush(Color.FromRgb(2, 93, 186));
                borderExp.Background = new SolidColorBrush(Color.FromRgb(2, 93, 186));
            }
            //red.ShowDialog();

            //  TESTING FOR TREEVIEW
            //  ftn_Open_File();

            //  TESTING FOR TREEVIEW
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

        #endregion buttons

        #region TreeView

        private void TreeView1_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            string section = String.Empty, step = String.Empty;
            XmlElement pos = _treeView.SelectedItem as XmlElement;
            if (pos != null)
            {
                if (pos.Name == "Test_Step")
                {
                    step = pos.SelectSingleNode("@id").Value;
                    section = pos.ParentNode.SelectSingleNode("@id").Value;
                    lblProcedurePosition.Text = String.Format("Section {0}, Step {1}", section, step);
                }
                else if (pos.Name == "Section")
                {
                    section = pos.SelectSingleNode("@id").Value;
                    lblProcedurePosition.Text = String.Format("Section {0}", section);
                }
            }
            else
            {
                lblProcedurePosition.Text = String.Format("Section {0}, Step {1}", section, step);
            }
        }

        private void TreeView1_PreviewMouseLeftButtonUp_1(object sender, MouseButtonEventArgs e)
        {
        }

        //Initialize the treeView to section 0, step 0

        //Navigate to next step in test procedure

        #endregion TreeView

        #region Test Steps

        /*
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
    }*/

        #endregion Test Steps

        private void updateTextBoxes()
        {
            //_treeView.Items.Refresh();
            //_treeView.UpdateLayout();
            return;
            TreeViewItem item = (TreeViewItem)treeView1.SelectedItem;
            if (item?.Tag is TestProcedureSectionTest_Step)
            {
                TestProcedureSectionTest_Step step = (TestProcedureSectionTest_Step)item.Tag;

                tbStep.Text = step.id.ToString();
                tbStation.Text = step.Station.ToString();
                tbControlAction.Text = step.Control_Action.ToString();
                tbExpectedResult.Text = step.Expected_Result.ToString();
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
                //  TESTING
                if (_treeView.SelectedItem == null) return;
                XmlElement x = _treeView.SelectedItem as XmlElement;
                if (x.Name == "Test_Step")
                {
                    //MessageBox.Show(x["Pass"].InnerText + "\n" + x["Pass"].Value);
                    cmt.Show();
                    MessageBox.Show(String.Format("{0}", getProcedureProgress(x.ParentNode as XmlElement)));
                }

                //  TESTING
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

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            string resourceKeyName = "xmlData";
            string treeViewName = "treeView1";

            var dpd = DependencyPropertyDescriptor.FromProperty(ItemsControl.ItemsSourceProperty, typeof(TreeView));
            if (dpd != null)
            {
                dpd.AddValueChanged(treeView1, ThisIsCalledWhenPropertyIsChanged);
            }

            _xmlDataProvider = FindResource(resourceKeyName) as XmlDataProvider;
            _treeView = FindName(treeViewName) as TreeView;

            _xmlDataProvider.Document = _xml;
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            main.Show();
        }

        private void ThisIsCalledWhenPropertyIsChanged(object sender, EventArgs e)
        {
            //MessageBox.Show("WOAH");
            pbProcedureProgress.Value = getProcedureProgress(null);
            _treeView.Items.Refresh();
            _treeView.UpdateLayout();
        }

        private void ftn_Open_File()
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "XML files|*.xml";

            if ((bool)openFileDialog.ShowDialog())
            {
                _xml = new XmlDocument();

                try
                {
                    _xml.Load(openFileDialog.FileName);
                }
                catch (Exception err)
                {
                    MessageBox.Show(err.Message, "Error");
                }

                _xmlDataProvider.Document = _xml;
            }

            MessageBox.Show(openFileDialog.FileName);
            _treeView.Items.Refresh();
            _treeView.UpdateLayout();
        }

        private double getProcedureProgress(XmlElement searchRoot)
        {
            XmlNodeList nList = _xml.GetElementsByTagName("Test_Step");
            int o = 0;
            foreach (XmlNode n in nList)
            {
                if (XmlConvert.ToBoolean(n["Pass"].InnerText) || XmlConvert.ToBoolean(n["Fail"].InnerText)) o++;
            }
            return (((double)o / nList.Count) * 100);
        }

        private void mnuSave_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "XML files|*.xml";
            if ((bool)saveFileDialog.ShowDialog())
            {
                try
                {
                    _xml.Save(saveFileDialog.FileName);
                }
                catch (Exception err)
                {
                    MessageBox.Show(err.Message, "Error");
                }
            }
        }
    }
}