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
using System.ComponentModel;
using System.Windows.Threading;
using System.Timers;

namespace senior_project
{
    /// <summary>
    /// Interaction logic for TestAdmin.xaml
    /// </summary>
    /// 
    public partial class TestAdmin : Window
    {
        private MainWindow main;
        private String commentDefault = "Leave a comment";
        private TestProcedure xmlProcedure;
        Random rand = new Random();


        //  XML TreeView Implementation with XmlDataProvider and XmlDocument
        private XmlDocument _xml;

        private List<XmlElement> listTestSteps;
        private DispatcherTimer t;

        private XmlDataProvider _xmlDataProvider;
        private TreeView _treeView;

        public TestAdmin(MainWindow mw, String xmlFile)
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

         //   t = new DispatcherTimer(new TimeSpan(0, 0, 0, 0, 75), DispatcherPriority.Background, t_Tick, Dispatcher.CurrentDispatcher);

        }

        /*
        private void t_Tick(object sender, EventArgs e)
        {
            lblStep.Background = new SolidColorBrush(Color.FromRgb((byte)rand.Next(0, 255), (byte)rand.Next(0, 255), (byte)rand.Next(0, 255)));
            lblStation.Background = new SolidColorBrush(Color.FromRgb((byte)rand.Next(0, 255), (byte)rand.Next(0, 255), (byte)rand.Next(0, 255)));
            lblExpectedResult.Background = new SolidColorBrush(Color.FromRgb((byte)rand.Next(0, 255), (byte)rand.Next(0, 255), (byte)rand.Next(0, 255)));
            lblControlAction.Background = new SolidColorBrush(Color.FromRgb((byte)rand.Next(0, 255), (byte)rand.Next(0, 255), (byte)rand.Next(0, 255)));
        }
        */
        #region buttons

        private void removeStepButton_Click(object sender, RoutedEventArgs e)
        {
            lblStep.Background = new SolidColorBrush(Color.FromRgb((byte)rand.Next(0, 255), (byte)rand.Next(0, 255), (byte)rand.Next(0, 255)));
        }

        private void addStepButton_Click(object sender, RoutedEventArgs e)
        {
            exportWindow ep = new exportWindow();
            ep.Show();
            this.Close();
        }
        private void move_down(object sender, RoutedEventArgs e)
        {

        }

        private void move_up(object sender, RoutedEventArgs e)
        {

        }



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
        /*
        //Initialize the treeView to section 0, step 0

        //Navigate to next step in test procedure

        #endregion TreeView

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
    }*/

        #endregion Test Steps

        private void updateTextBoxes()
        {
            //_treeView.Items.Refresh();
            //_treeView.UpdateLayout();
            
            TreeViewItem item = (TreeViewItem)treeView1.SelectedItem;
            if (item?.Tag is TestProcedureSectionTest_Step)
            {
                TestProcedureSectionTest_Step step = (TestProcedureSectionTest_Step)item.Tag;

                tbStep.Text = step.id.ToString();
                tbStation.Text = step.Station.ToString();
                tbControlAction.Text = step.Control_Action.ToString();
                tbExpectedResult.Text = step.Expected_Result.ToString();
            }
            return;
        }

        private void Exit_Button(object sender, RoutedEventArgs e)
        {
            this.Close();
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
           // pbProcedureProgress.Value = getProcedureProgress(null);
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