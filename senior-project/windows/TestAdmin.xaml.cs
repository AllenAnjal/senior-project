using Microsoft.Win32;
using System;
using System.ComponentModel;
using System.Collections;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using System.Collections.Generic;
using System.Diagnostics;
using System.Xml;
using System.IO;

namespace senior_project
{
    /// <summary>
    /// Interaction logic for Tester.xaml
    /// </summary>
    public partial class TestAdmin : Window
    {
        //  Begin Declarations

        private MainWindow main;
        private String commentDefault = "Leave a comment";
        private bool pausePlay = false;
        private bool hasCommented = false;
        private bool redlineClicked = false;
        private DispatcherTimer t;
        private DateTime start;
        private Stopwatch stopWatch;
        private exportWindow export = new exportWindow();
        private commentWindow cmt = new commentWindow();

        //strings to store text from user 
        private string stepTxt;
        private string stationTxt;
        private string controlTxt;
        private string expectedTxt;

        //arraylist to store change
        private ArrayList changes = new ArrayList();

        //  XML TreeView Implementation with XmlDataProvider and XmlDocument
        private XmlDocument _xml;
        private string xmlFile;

        private List<TreeViewItem> tItems;
        private int pos = 0;
        private int count = 0;
        private int tmp = 0;

        private XmlDataProvider _xmlDataProvider;
        private TreeView _treeView;
        private string SectionName = "Section";
        private string StepName = "Test_Step";

        public TestAdmin(MainWindow mw, String xmlFile)
        {
            InitializeComponent();
            main = mw;

            this.xmlFile = xmlFile;

            _xml = new XmlDocument();
            tItems = new List<TreeViewItem>();
            try
            {
                _xml.Load(xmlFile);

            }
            catch (Exception err)
            {
                MessageBox.Show(err.Message, "Error");
            }

            _treeView = FindName("treeView1") as TreeView;
            _xmlDataProvider = FindResource("xmlData") as XmlDataProvider;
            _xmlDataProvider.Document = _xml;

            
        }

        #region buttons

        private void move_down(object sender, RoutedEventArgs e)
        {
            StepForward();
        }

        private void move_up(object sender, RoutedEventArgs e)
        {
            //  BEGIN TEMPORARY TEST CODE
            StepBackwards();
        }

        private void removeStepButton_Click(object sender, RoutedEventArgs e)
        {
            
            XmlNode currentNode = _treeView.SelectedItem as XmlNode;
            XmlNode root = currentNode.ParentNode;
            
            if (currentNode != null)
            {
                root.RemoveChild(currentNode);
            }
            else
            {
                MessageBox.Show("Error");
            }
        }

        private void addStepButton_Click(object sender, RoutedEventArgs e)
        {
            XmlElement root = _treeView.SelectedItem as XmlElement;
            if (root.Name == "Test_Step")
                root = root.ParentNode as XmlElement;
            int count = 0;
            foreach(XmlElement x in root)
            {
                count++;
            }
            count--;
            string id = "" + count;
            //creating elements to add to test_step 
            XmlElement newTestStep = _xml.CreateElement("Test_Step");
            newTestStep.SetAttribute("id", id);
            XmlElement newStation = _xml.CreateElement("Station");
            XmlElement newExpResult = _xml.CreateElement("Expected_Result");
            XmlElement newControlAction = _xml.CreateElement("Control_Action");
            XmlElement newPass = _xml.CreateElement("Pass");
            XmlElement newFail = _xml.CreateElement("Fail");
            XmlElement newComments = _xml.CreateElement("Comments");
            XmlElement newImage = _xml.CreateElement("Image");

            //setting the innertext to the textbox in the xaml page
            newTestStep.InnerText = tbStep.Text;
            newStation.InnerText = tbStation.Text;
            newExpResult.InnerText = tbExpectedResult.Text;
            newControlAction.InnerText = tbControlAction.Text;

            //appending the subsections to test_step
            newTestStep.AppendChild(newStation);
            newTestStep.AppendChild(newControlAction);
            newTestStep.AppendChild(newExpResult);
            newTestStep.AppendChild(newPass);
            newTestStep.AppendChild(newFail);
            newTestStep.AppendChild(newComments);
            newTestStep.AppendChild(newImage);

            if (root.Name == "Section")
                root.AppendChild(newTestStep);

        }
        private void Exit_Button(object sender, RoutedEventArgs e)
        {
            this.Close();
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



        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            var dpd = DependencyPropertyDescriptor.FromProperty(ItemsControl.ItemsSourceProperty, typeof(TreeView));
            if (dpd != null)
            {
                dpd.AddValueChanged(treeView1, ThisIsCalledWhenPropertyIsChanged);
            }
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            main.Show();
        }

        private void ThisIsCalledWhenPropertyIsChanged(object sender, EventArgs e)
        {
            //MessageBox.Show("WOAH");
            //pbProcedureProgress.Value = getProcedureProgress(null);
            _treeView.Items.Refresh();
            _treeView.UpdateLayout();
        }

        #region otherFunctions
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

        private int getTotalSteps()//XmlElement searchRoot)
        {
            XmlNodeList nList = _xml.GetElementsByTagName("Test_Step");
            int o = 0;
            foreach (XmlNode n in nList)
            {
                o++;
                //if (XmlConvert.ToBoolean(n["Pass"].InnerText) || XmlConvert.ToBoolean(n["Fail"].InnerText)) o++;
            }
            return o;
            // return (((double)o / nList.Count) * 100);
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

        private void LoadListRecursive(ItemsControl ic, List<TreeViewItem> steps)
        {
            //Search for the object model in first level children (recursively)

            if (ic == null)//&& ((XmlElement)tvi.Header).Name == "Test_Steps")
            {
                return;
            }

            if (ic.GetType().Equals(typeof(TreeViewItem)))
            {
                TreeViewItem tvi = ic as TreeViewItem;
                XmlElement element = tvi.Header as XmlElement;

                if (element.Name.Equals(StepName))
                {
                    steps.Add(tvi);
                }
            }

            foreach (object i in ic.Items)
            {
                TreeViewItem extraction = ic.ItemContainerGenerator.ContainerFromItem(i) as TreeViewItem;
                LoadListRecursive(extraction, steps);
            }
        }

        private void Window_ContentRendered(object sender, EventArgs e)
        {
            LoadListRecursive(_treeView, tItems);
            StepToStart();
        }

        private void StepForward()
        {
            pos = (pos < tItems.Count - 1) ? pos + 1 : 0;
            tItems[pos].IsSelected = true;
        }

        private void StepBackwards()
        {
            pos = (pos == 0) ? tItems.Count - 1 : pos - 1;
            tItems[pos].IsSelected = true;
        }

        private void StepToStart()
        {
            Console.WriteLine(tItems.Count);

            tItems[0].IsSelected = true;
        }

     

        private void pbProcedureProgress_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            //do nothing
        }
    }
    #endregion otherFunctions
}