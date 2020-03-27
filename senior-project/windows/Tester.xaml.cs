﻿using Microsoft.Win32;
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
using System.Runtime.CompilerServices;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.IO;

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
        private XmlDataProvider _xmlDataProvider;
        private XmlDocument _xml;
        private string xmlFile;

        private List<TreeViewItem> tItems;
        private int pos = 0;
        private int count = 0;
        private int tmp = 0;

        private TreeView _treeView;
        private string SectionName = "Section";
        private string StepName = "Test_Step";

        public Tester(MainWindow mw, String xmlFile)
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

            userInfoPage x = new userInfoPage(xmlFile);
            x.ShowDialog();
            _treeView = FindName("treeView1") as TreeView;
            _xmlDataProvider = FindResource("xmlData") as XmlDataProvider;
            _xmlDataProvider.Document = _xml;
            _xmlDataProvider.XPath = "/TestProcedure/Sections/Section";

            count = getTotalSteps();

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

        #region Buttons

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
            tmp++;
            pbProcedureProgress.Maximum = count;
            pbProcedureProgress.Value = (double)tmp;

            //writeStep(true);
            //XmlVerification.writeXmltoFile(xmlProcedure, "tmp.xml");
            //forwardStep();
        }

        private void failAction()
        {
            if (_treeView.SelectedItem == null) return;
            hasCommented = true;
            XmlElement sel = _treeView.SelectedItem as XmlElement;

            if (sel.Name == "Test_Step")
            {
                
                sel["Pass"].InnerText = "false";
                sel["Fail"].InnerText = "true";
                cmt.ShowDialog();

            }
            tmp++;
            pbProcedureProgress.Maximum = count;
            pbProcedureProgress.Value = (double)tmp;

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
                //LastStep();
                StepForward();
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
                StepForward();
                //if (!cmt.IsActive)
                //LastStep();

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
                changeColors(254, 1, 1);
                failButton.IsEnabled = false;
                passButton.IsEnabled = false;
                tbStep.IsReadOnly = false;
                tbStation.IsReadOnly = false;
                tbControlAction.IsReadOnly = false;
                tbExpectedResult.IsReadOnly = false;
                _xmlDataProvider.XPath = "/TestProcedure/RedlinesList/Section";
                //treeView1.Items.Refresh();
                //treeView1.UpdateLayout();
                //tItems.Clear();
                //LoadListRecursive(treeView1, tItems);
                //tItems[pos].IsSelected = true;

            }
            else
            {
                changeColors(2, 93, 186);
                failButton.IsEnabled = true;
                passButton.IsEnabled = true;
                tbStep.IsReadOnly = true;
                tbStation.IsReadOnly = true;
                tbControlAction.IsReadOnly = true;
                tbExpectedResult.IsReadOnly = true;
                _xmlDataProvider.XPath = "/TestProcedure/Sections/Section";
                //treeView1.Items.Refresh();
                //treeView1.UpdateLayout();
                //tItems.Clear();
                //LoadListRecursive(treeView1, tItems);
                //tItems[pos].IsSelected = true;
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
                _xml.Save(saveFile.FileName);
                Console.WriteLine(saveFile.FileName);
            }
        }
        private void CommentButton_Click(object sender, RoutedEventArgs e)
        {
            //tItems[2].IsSelected = true;
            //try
            //{
            //    //  TESTING
            //    if (_treeView.SelectedItem == null) return;
            //    XmlElement x = _treeView.SelectedItem as XmlElement;
            //    if (x.Name == "Test_Step")
            //    {
            //        //MessageBox.Show(x["Pass"].InnerText + "\n" + x["Pass"].Value);
            //        cmt.Show();
            //        MessageBox.Show(String.Format("{0}", getProcedureProgress(x.ParentNode as XmlElement)));
            //    }

            //    //  TESTING
            //}
            //catch (Exception ex)
            //{
            //    MessageBox.Show($"Error: {ex.Message}");
            //}
        }
        private void Exit_Button(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
        private void timerClick(object sender, RoutedEventArgs e)
        {
            if (stopWatch.IsRunning)
            {
                stopWatch.Stop();
                timerButton.Content = FindResource("Resume");
            }
            else
            {
                stopWatch.Start();
                timerButton.Content = FindResource("Stop");
            }
        }

        #endregion
        #region TreeView

        private void TreeView1_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            string section = String.Empty, step = String.Empty;
            XmlElement _pos = _treeView.SelectedItem as XmlElement;
            if (_pos != null)
            {
                if (_pos.Name == "Test_Step")
                {
                    step = _pos.SelectSingleNode("@id").Value;
                    section = _pos.ParentNode.SelectSingleNode("@id").Value;
                    lblProcedurePosition.Text = String.Format("Section {0}, Step {1}", section, step);
                }
                else if (_pos.Name == "Section")
                {
                    section = _pos.SelectSingleNode("@id").Value;
                    lblProcedurePosition.Text = String.Format("Section {0}", section);
                }
                

            }
            else
            {
                lblProcedurePosition.Text = String.Format("Section {0}, Step {1}", section, step);
            }
            
            stepTxt = tbStep.Text;
            stationTxt = tbStation.Text;
            controlTxt = tbControlAction.Text;
            expectedTxt = tbExpectedResult.Text;
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

        //    if ((bool)openFileDialog.ShowDialog())
        //    {
        //        _xml = new XmlDocument();

        //        try
        //        {
        //            _xml.Load(openFileDialog.FileName);
        //        }
        //        catch (Exception err)
        //        {
        //            MessageBox.Show(err.Message, "Error");
        //        }

        //        _xmlDataProvider.Document = _xml;
        //    }

        //    MessageBox.Show(openFileDialog.FileName);
        //    _treeView.Items.Refresh();
        //    _treeView.UpdateLayout();
        //}

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
                    _xmlDataProvider.Document.Save(saveFileDialog.FileName);
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
        
        private void LastStep()
        {
            if (pos == tItems.Count - 1)
            {
                MessageBox.Show("Test Procedure is Done! Export window will now display");
                export.ShowDialog();
                string newfile = xmlFile.Substring(0, xmlFile.Length - 4) + "_new.xml";
                using (StreamWriter sw = File.AppendText(newfile))
                {
                    sw.WriteLine("\n<redline>\n");
                    foreach(string change in changes)
                    {
                        sw.WriteLine(change);
                    }
                    sw.WriteLine("</redline>\n");
                }
                this.Hide();
            }
        }

        private void timer_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (stopWatch.IsRunning)
            {
                stopWatch.Stop();
                timerButton.Content = FindResource("Stop");
            }
            else
            {
                stopWatch.Start();
                timerButton.Content = FindResource("Resume");
            }
        }

        private void changeColors(byte r, byte g, byte b)
        {
            lblStep.Background = new SolidColorBrush(Color.FromRgb(r, g, b));
            lblStation.Background = new SolidColorBrush(Color.FromRgb(r, g, b));
            lblControlAction.Background = new SolidColorBrush(Color.FromRgb(r, g, b));
            lblExpectedResult.Background = new SolidColorBrush(Color.FromRgb(r, g, b));
            borderStep.Background = new SolidColorBrush(Color.FromRgb(r, g, b));
            borderStation.Background = new SolidColorBrush(Color.FromRgb(r, g, b));
            borderControl.Background = new SolidColorBrush(Color.FromRgb(r, g, b));
            borderExp.Background = new SolidColorBrush(Color.FromRgb(r, g, b));
        }

        private void setTextboxWrite(bool value)
        {
            tbControlAction.IsReadOnly = value;
            tbStep.IsReadOnly = value;
            tbExpectedResult.IsReadOnly = value;
            tbStation.IsReadOnly = value;
        }

     

        private void addRedlineChanges()
        {
            if (tbStep.Text.Length != stepTxt.Length)
            {
                changes.Add("Original: " + stepTxt + " \nModified: " + tbStep.Text + "\n\n");
            }
            if (tbStation.Text.Length != stationTxt.Length)
            {
                changes.Add("Original: " + stationTxt + " \nModified: " + tbStation.Text + "\n\n");
            }
            if (tbControlAction.Text.Length != controlTxt.Length)
            {
                changes.Add("Original: " + controlTxt + " \nModified: " + tbControlAction.Text + "\n\n");
            }
            if (tbExpectedResult.Text.Length != expectedTxt.Length)
            {
                changes.Add("Original: " + expectedTxt + " \nModified: " + tbExpectedResult.Text + "\n\n");
            }
        }

        private void pbProcedureProgress_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            //do nothing
        }
    }
    #endregion otherFunctions
}