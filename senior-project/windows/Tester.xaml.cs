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
using System.Diagnostics;
using System.Xml;

namespace senior_project
{
    /// <summary>
    /// Interaction logic for Tester.xaml
    /// </summary>
    public partial class Tester : Window
    {
        //  Begin Declarations

        private MainWindow main;
        private String commentDefault = "Leave a comment";

        private bool hasCommented = false;
        private bool redlineClicked = false;
        private DispatcherTimer t;
        private DateTime start;
        private Stopwatch stopWatch;
        private exportWindow export = new exportWindow();
        private commentWindow cmt = new commentWindow();

        //  XML TreeView Implementation with XmlDataProvider and XmlDocument
        private XmlDocument _xml;

        private List<TreeViewItem> tItems;
        private int pos = 0;

        private XmlDataProvider _xmlDataProvider;
        private TreeView _treeView;
        private string SectionName = "Section";
        private string StepName = "Test_Step";

        public Tester(MainWindow mw, String xmlFile)
        {
            InitializeComponent();
            main = mw;

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

            userInfoPage x = new userInfoPage(xmlFile);
            x.ShowDialog();
            _treeView = FindName("treeView1") as TreeView;
            _xmlDataProvider = FindResource("xmlData") as XmlDataProvider;
            _xmlDataProvider.Document = _xml;

            stopWatch = new Stopwatch();
            stopWatch.Start();

            t = new DispatcherTimer();// new TimeSpan(0, 0, 0, 0, 1), DispatcherPriority.Background, t_Tick, Dispatcher.CurrentDispatcher);
            t.Interval = TimeSpan.FromMilliseconds(1);
            t.Tick += t_Tick;
            t.Start();
        }

        private void t_Tick(object sender, EventArgs e)
        {
            //timer.Text = Convert.ToString(DateTime.Now - start);
            timer.Text = stopWatch.Elapsed.ToString(@"hh\:mm\:ss\.ff");
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

        private void move_down(object sender, RoutedEventArgs e)
        {
            StepForward();
        }

        private void move_up(object sender, RoutedEventArgs e)
        {
            //  BEGIN TEMPORARY TEST CODE
            StepBackwards();
        }

        private void RedlineButton_Click(object sender, RoutedEventArgs e)
        {
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

        private void timer_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (stopWatch.IsRunning) stopWatch.Stop();
            else stopWatch.Start();
        }
    }
}