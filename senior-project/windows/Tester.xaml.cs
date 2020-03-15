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
using System.Xml.Schema;

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

        //  XML TreeView Implementation with XmlDataProvider and XmlDocument
        private XmlDocument _xml;

        private XmlDataProvider _xmlDataProvider;
        private TreeView _treeView;
        private List<TreeViewItem> tItem;
        private int pos = 0;

        private XmlNodeList _nList;

        private string SectionName = "Section";

        //private string StepName = "Test_Step";
        private string StepName = "Section_Step";

        public Tester(MainWindow mw, String xmlFile)
        {
            InitializeComponent();
            main = mw;

            _xml = new XmlDocument();
            //tItem = new List<XmlElement>();

            try
            {
                _xml = LoadValidXML(xmlFile);
                Console.WriteLine("xml file loaded.");
            }
            catch (Exception err)
            {
                MessageBox.Show(err.Message, "Error");
            }

            userInfoPage x = new userInfoPage(xmlProcedure);
            x.ShowDialog();
            //  TEST EDIT

            tItem = new List<TreeViewItem>();

            _xmlDataProvider = FindResource("xmlData") as XmlDataProvider;
            _treeView = FindName("treeView1") as TreeView;
            _xmlDataProvider.Document = _xml;

            _nList = _xml.GetElementsByTagName(StepName);    //  Used to update Progress Bar

            //  END TEST EDIT

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

        #region Button Event Handlers

        private void SetStepResult(bool result)
        {
            if (_treeView.SelectedItem == null)
                return;

            hasCommented = false;

            XmlElement sel = _treeView.SelectedItem as XmlElement;

            if (sel.Name.Equals(StepName))
            {
                sel["Pass"].InnerText = XmlConvert.ToString(result);
                sel["Fail"].InnerText = XmlConvert.ToString(!result);

                StepForward();
            }
        }

        private void passAction()
        {
            SetStepResult(true);
        }

        private void failAction()
        {
            SetStepResult(false);
            //  TODO:   More stuff
        }

        //Update XML tmp file, forward step
        private void PassButton_Click(object sender, RoutedEventArgs e)
        {
            Console.WriteLine(13 % 8);
            Console.WriteLine(13 & 7);
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
            }
            else
            {
                lblStep.Background = new SolidColorBrush(Color.FromRgb(2, 93, 186));
                lblStation.Background = new SolidColorBrush(Color.FromRgb(2, 93, 186));
                lblControlAction.Background = new SolidColorBrush(Color.FromRgb(2, 93, 186));
                lblExpectedResult.Background = new SolidColorBrush(Color.FromRgb(2, 93, 186));
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

        private void CommentButton_Click(object sender, RoutedEventArgs e)
        {
            Console.WriteLine((_treeView.Items != null) ? _treeView.Items.Count : -1);
            return;

            return;
            try
            {
                //  TESTING
                if (_treeView.SelectedItem == null) return;
                XmlElement x = _treeView.SelectedItem as XmlElement;
                if (x.Name == StepName)
                {
                    //TreeViewItem parentItem = selectedItem?.Parent as TreeViewItem;

                    //MessageBox.Show(x["Pass"].InnerText + "\n" + x["Pass"].Value);
                    string debug = String.Format("Type of:\n{0}\t_treeView.SelectedItem\n",
                                                (_treeView.SelectedItem).GetType()
                                                );
                    //MessageBox.Show(debug);

                    //MessageBox.Show(String.Format("{0}", getProcedureProgress(x.ParentNode as XmlElement)));
                }

                //  TESTING
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}");
            }
        }

        private void Exit_Button(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        #endregion Button Event Handlers

        #region TreeView

        private void TreeView1_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            XmlElement xe = _treeView.SelectedItem as XmlElement;

            string section = String.Empty, step = String.Empty;
            if (xe != null)
            {
                if (xe.Name.Equals(StepName))
                {
                    step = xe.SelectSingleNode("@id").Value;
                    section = xe.ParentNode.SelectSingleNode("@id").Value;

                    Predicate<TreeViewItem> validatePos = i => (i.Header as XmlElement).Equals(xe);
                    pos = tItem.FindIndex(validatePos);
                }
                else if (xe.Name.Equals(SectionName))
                {
                    XmlElement oldElem = (e.OldValue as XmlElement);
                    if (oldElem != null)
                    {
                        Predicate<TreeViewItem> validatePos = i => (i.Header as XmlElement).Equals(oldElem);
                        pos = tItem.FindIndex(validatePos);
                        tItem[pos].IsSelected = true;
                    }
                    else
                    {
                        StepToStart();
                    }
                    //section = xe.SelectSingleNode("@id").Value;
                    //lblProcedurePosition.Text = String.Format("Section {0}", section);
                }
            }

            lblProcedurePosition.Text = String.Format("Section {0}, Step {1}", section, step);
            pbProcedureProgress.Value = GetProcedureProgress(null);
        }

        private void TreeViewItem_Collapsed(object sender, RoutedEventArgs e)
        {
            (sender as TreeViewItem).IsExpanded = true;
        }

        //Initialize the treeView to section 0, step 0

        #endregion TreeView

        #region Test Steps

        //Navigate to next step in test procedure
        private void StepForward()
        {
            tItem[(++pos) % tItem.Count].IsSelected = true;
        }

        private void StepBackwards()
        {
            tItem[(--pos) % tItem.Count].IsSelected = true;
        }

        private void StepToStart()
        {
            Console.WriteLine(tItem.Count);

            tItem[0].IsSelected = true;
        }

        #endregion Test Steps

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

        //private void ThisIsCalledWhenPropertyIsChanged(object sender, EventArgs e)
        //{
        //}

        private void Ftn_Open_File()
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

        /*
         * functions to create:
         *      stepForward()
         *      stepForward(unsigned n)
         *
         *      stepBackwards()
         *      stepBackwards(unsigned n)
         *
         *      updateProgressBar()
         *
         */

        #region Window Event Handlers

        private void Window_ContentRendered(object sender, EventArgs e)
        {
            //var dpd = DependencyPropertyDescriptor.FromProperty(ItemsControl.ItemsSourceProperty, typeof(TreeView));
            //if (dpd != null)
            //{
            //    dpd.AddValueChanged(treeView1, ThisIsCalledWhenPropertyIsChanged);
            //}
            LoadListRecursive(_treeView, tItem);
            StepToStart();
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            main.Show();
        }

        #endregion Window Event Handlers

        #region Helper Functions

        private void LoadListRecursive(ItemsControl ic, List<TreeViewItem> steps)
        {
            //Search for the object model in first level children (recursively)

            if (ic == null)//&& ((XmlElement)tvi.Header).Name == "Test_Steps")
            {
                Console.WriteLine("Tree View does not exist.");
                return;
            }

            if (ic.GetType().Equals(typeof(TreeViewItem)))
            {
                TreeViewItem tvi = ic as TreeViewItem;
                Console.WriteLine(((XmlElement)tvi.Header).Name);
                if (((XmlElement)tvi.Header).Name.Equals(StepName))
                {
                    steps.Add(tvi);
                    Console.WriteLine("Found step");
                }
            }
            foreach (object i in ic.Items)
            {
                //Get the TreeViewItem associated with the iterated object model
                TreeViewItem tvi2 = ic.ItemContainerGenerator.ContainerFromItem(i) as TreeViewItem;
                LoadListRecursive(tvi2, steps);
            }
            //Loop through user object models
        }

        private double GetProcedureProgress(XmlElement searchRoot)
        {
            //XmlNodeList nList = _xml.GetElementsByTagName(StepName);
            int o = 0;
            foreach (XmlNode n in _nList)
            {
                if (XmlConvert.ToBoolean(n["Pass"].InnerText) || XmlConvert.ToBoolean(n["Fail"].InnerText)) o++;
            }
            double output = (((double)o / _nList.Count) * 100);
            Console.WriteLine(output);
            return output;
        }

        private XmlDocument LoadValidXML(string xmlFile)
        {
            XmlReader reader;

            XmlReaderSettings settings = new XmlReaderSettings();

            XmlSchemaSet xs = new XmlSchemaSet();
            XmlSchema schema; // Load schema
            try
            {
                schema = xs.Add("http://tempuri.org/ProcedureSchema.xsd", "procedure.xsd");
            }
            catch (Exception e)
            {
                Console.WriteLine("ERROR: " + e.Message);
                return null;
            }
            settings.Schemas.Add(schema);
            //

            settings.ValidationEventHandler += settings_ValidationEventHandler;
            settings.ValidationFlags = settings.ValidationFlags | XmlSchemaValidationFlags.ReportValidationWarnings;
            settings.ValidationType = ValidationType.Schema;

            try
            {
                reader = XmlReader.Create(xmlFile, settings);
            }
            catch (Exception e)
            {
                Console.WriteLine("ERROR: " + e.Message);
                return null;
            }

            XmlDocument doc = new XmlDocument();
            doc.PreserveWhitespace = true;
            doc.Load(reader);
            reader.Close();

            return doc;
        }

        private void validateXML(XmlDocument doc)
        {
            if (doc.Schemas.Count == 0)
            {
                return;
            }

            // Use an event handler to validate the XML node against the schema.
            doc.Validate(settings_ValidationEventHandler);
        }

        private void settings_ValidationEventHandler(object sender,
    System.Xml.Schema.ValidationEventArgs e)
        {
            if (e.Severity == XmlSeverityType.Warning)
            {
                Console.WriteLine("The following validation warning occurred: " + e.Message);
            }
            else if (e.Severity == XmlSeverityType.Error)
            {
                Console.WriteLine("The following critical validation errors occurred: " + e.Message);
                Console.WriteLine("\t" + e.Exception.ToString());
                Type objectType = sender.GetType();
            }
        }

        #endregion Helper Functions
    }
}