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

        private TreeHelper treeHelper;

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
            //_xmlDataProvider = FindResource("xmlData") as XmlDataProvider;
            //_xmlDataProvider.Document = _xml;
            //_xmlDataProvider.XPath = "/Test_Procedure/Sections/Section";

            count = getTotalSteps();

            stopWatch = new Stopwatch();
            stopWatch.Start();

            t = new DispatcherTimer();// new TimeSpan(0, 0, 0, 0, 1), DispatcherPriority.Background, t_Tick, Dispatcher.CurrentDispatcher);
            t.Interval = TimeSpan.FromMilliseconds(1);
            t.Tick += t_Tick;
            t.Start();

            treeHelper = new TreeHelper(_treeView, SectionName, StepName);

            MainViewModel test = new MainViewModel(_xml);
            this.DataContext = test;
        }

        private void RedlineButton_Click(object sender, RoutedEventArgs e)
        {
            redlineClicked = !redlineClicked;
            if (redlineClicked)
            {
                changeColors(254, 1, 1);
                setBoolean(false);
                redlineIndicator.Text = "Redline Mode";
                //tItems[pos].IsSelected = true;
            }
            else
            {
                changeColors(2, 93, 186);
                setBoolean(true);
                redlineIndicator.Text = "";
                //treeView1.Items.Refresh();
                //treeView1.UpdateLayout();
                //tItems[pos].IsSelected = true;
            }

            //red.ShowDialog();

            //  TESTING FOR TREEVIEW
            //  ftn_Open_File();

            //  TESTING FOR TREEVIEW
        }

        #region OLD STUFF

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
                sel["Result"].SetAttribute("result", "pass");
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
                sel["Result"].SetAttribute("result", "fail");
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
            treeHelper.MoveForward();
            //StepForward();
        }

        private void move_up(object sender, RoutedEventArgs e)
        {
            treeHelper.MoveBackwards();

            //  BEGIN TEMPORARY TEST CODE
            //StepBackwards();
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
            return;
            /*
            // DEV GOAL:
            //  Retrieve Current id of step and section
            //  Detect position within section
            //  Report if first child or last child of section
            string stepID = String.Empty, sectionID = String.Empty;
            int nStep = 0, nSection = 0;
            string pStep = string.Empty, pSection = string.Empty;
            //bool? isFirst = null, isLast = null;

            bool? detectedElement = false;
            XmlElement currentElement = _treeView.SelectedItem as XmlElement;
            XmlNode parentNode = currentElement.ParentNode as XmlNode;
            XmlNodeList children = parentNode.SelectNodes("Test_Step");

            TreeViewItem cItem = _treeView.SelectedItem as TreeViewItem;

            stepID = currentElement.SelectSingleNode("@id").Value;
            sectionID = parentNode.SelectSingleNode("@id").Value;

            if (children.Count > 0 && children.Item(0).Equals(currentElement as XmlNode)) isFirst = true;
            else isFirst = false;
            if (children.Count > 0 && children.Item(children.Count - 1).Equals(currentElement as XmlNode)) isLast = true;
            else isLast = false;
            foreach (XmlNode c in children)
            {
                if (c.Equals(currentElement as XmlNode)) detectedElement = true;
            }
            if ((isFirst ?? false))
            {
                if (parentNode.PreviousSibling == null)
                {
                }
                else
                {
                    XmlNode pNode = parentNode.PreviousSibling;
                    pSection = pNode.SelectSingleNode("@id").Value;
                    pStep = pNode.LastChild.SelectSingleNode("@id").Value;
                }
            }
            else
            {
                pSection = sectionID;
                pStep = currentElement.PreviousSibling.SelectSingleNode("@id").Value;
            }
            if (isLast ?? false)
            {
                if (parentNode.NextSibling == null) ;
                else
                {
                    XmlNode nNode = parentNode.NextSibling;
                    //nSection = nNode.SelectSingleNode("@id").Value;
                    Int32.TryParse(nNode.SelectSingleNode("@id").Value, out nSection);
                    //nStep = nNode.SelectSingleNode("Test_Step").SelectSingleNode("@id").Value;
                    Int32.TryParse(nNode.SelectSingleNode("Test_Step").SelectSingleNode("@id").Value, out nStep);
                }
            }
            else
            {
                //nSection = sectionID;
                Int32.TryParse(sectionID, out nSection);
                //nStep = currentElement.NextSibling.SelectSingleNode("@id").Value;
                Int32.TryParse(currentElement.NextSibling.SelectSingleNode("@id").Value, out nStep);
            }

            MessageBox.Show(
                $"{pSection}.{pStep}\t|\tPrevious Position\n" +
                $"{sectionID}.{stepID}\t|\tCurrent Position\n" +
                $"{nSection}.{nStep}\t|\tNext Position\n" +

                $"{children.Count}\t|\tSteps in Section\n" +
                $"{(XmlConvert.ToString(detectedElement ?? false))}\t|\tDetected current Step\n" +
                $"{(XmlConvert.ToString(isFirst ?? false))}\t|\tStep is first child\n" +
                $"{(XmlConvert.ToString(isLast ?? false))}\t|\tStep is last child\n",
                "Extracted XML information");

            //TreeViewItem root = _treeView.ItemContainerGenerator.ContainerFromIndex(0) as TreeViewItem;
            if (nSection != 0)
            {
                TreeViewItem nextSection = _treeView.ItemContainerGenerator.ContainerFromIndex(nSection - 1) as TreeViewItem;
                TreeViewItem nextStep = nextSection.ItemContainerGenerator.ContainerFromIndex(nStep - 1) as TreeViewItem;
                //nextStep.IsSelected = true;
            }

            List<XmlNode> xList = new List<XmlNode>();
            foreach (XmlNode c in children)
            {
                xList.Add(c);
            }
            validateNode = i => i.Equals(currentElement as XmlNode);
            MessageBox.Show($"Step Position : {xList.FindIndex(validateNode)}");
            isFirst = xList.First<XmlNode>().Equals(currentElement);
            isLast = xList.Last<XmlNode>().Equals(currentElement);
            //_treeView.ItemContainerGenerator
            */
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

        #endregion Buttons

        #region TreeView

        private void TreeView1_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            //string section = String.Empty, step = String.Empty;
            //XmlElement _pos = _treeView.SelectedItem as XmlElement;
            //if (_pos != null)
            //{
            //    if (_pos.Name == "Test_Step")
            //    {
            //        step = _pos.SelectSingleNode("@id").Value;
            //        section = _pos.ParentNode.SelectSingleNode("@id").Value;
            //        lblProcedurePosition.Text = String.Format("Section {0}, Step {1}", section, step);
            //        for (int i = 0; i < tItems.Count; i++)
            //        {
            //            if (tItems[i].IsSelected)
            //                pos = i;
            //        }
            //    }
            //    else if (_pos.Name == "Section")
            //    {
            //        section = _pos.SelectSingleNode("@id").Value;
            //        lblProcedurePosition.Text = String.Format("Section {0}", section);
            //        pos = 0;
            //    }
            //}
            //else
            //{
            //    lblProcedurePosition.Text = String.Format("Section {0}, Step {1}", section, step);
            //}
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
                //dpd.AddValueChanged(treeView1, ThisIsCalledWhenPropertyIsChanged);
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
            //LoadListRecursive(_treeView, tItems);
           // StepToStart();
        }

        private void StepForward()
        {
            MoveStep(true);
            //pos = (pos < tItems.Count - 1) ? pos + 1 : 0;
            //tItems[pos].IsSelected = true;
            //Console.WriteLine(pos);
            //Console.WriteLine("The current count is" + tItems.Count);
        }

        private void StepBackwards()
        {
            MoveStep(false);
            //pos = (pos == 0) ? tItems.Count - 1 : pos - 1;
            //tItems[pos].IsSelected = true;
            //Console.WriteLine(pos);
        }

        private void MoveStep(bool moveForward)
        {
            const string sectionName = "Section";
            const string stepName = "Test_Step";
            int currSection, currStep, prevSection, prevStep, nextSection, nextStep;
            bool isFirst, isLast;
            var step = _treeView.SelectedItem as XmlNode;
            var section = step.ParentNode as XmlNode;

            var nodes = section.SelectNodes(stepName) as XmlNodeList;

            List<XmlNode> nList = new List<XmlNode>();
            foreach (XmlNode n in nodes) nList.Add(n);

            currStep = nList.FindIndex(i => i.Equals(step));
            isFirst = nList.First<XmlNode>().Equals(step);
            isLast = nList.Last<XmlNode>().Equals(step);

            nodes = section.ParentNode.SelectNodes(sectionName);

            nList.Clear();
            foreach (XmlNode n in nodes) nList.Add(n);

            currSection = nList.FindIndex(i => i.Equals(section));

            prevSection = currSection;
            prevStep = currStep - 1;
            nextSection = currSection;
            nextStep = currStep + 1;
            if (isFirst)
            {
                if (section.PreviousSibling == null)
                {
                    prevSection = 0;
                    prevStep = 0;
                }
                else
                {
                    prevSection = currSection - 1;
                    prevStep = section.PreviousSibling.SelectNodes("Test_Step").Count - 1;
                }
            }
            if (isLast)
            {
                if (section.NextSibling == null)
                {
                    nextStep = currStep;
                }
                else
                {
                    nextSection = currSection + 1;
                    nextStep = 0;
                }
            }

            //MessageBox.Show($"{currSection}.{currStep} -> {prevSection}.{prevStep}");

            TreeViewItem newSection = _treeView.ItemContainerGenerator.ContainerFromIndex(moveForward ? nextSection : prevSection) as TreeViewItem;
            TreeViewItem newStep = newSection.ItemContainerGenerator.ContainerFromIndex(moveForward ? nextStep : prevStep) as TreeViewItem;
            newStep.IsSelected = true;
        }

        private void StepToStart()
        {
            Console.WriteLine(tItems.Count);
            //tItems[0].IsSelected = true;
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
                    foreach (string change in changes)
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
            lblStation.Background = new SolidColorBrush(Color.FromRgb(r, g, b));
            lblControlAction.Background = new SolidColorBrush(Color.FromRgb(r, g, b));
            lblExpectedResult.Background = new SolidColorBrush(Color.FromRgb(r, g, b));
            borderStation.Background = new SolidColorBrush(Color.FromRgb(r, g, b));
            borderControl.Background = new SolidColorBrush(Color.FromRgb(r, g, b));
            borderExp.Background = new SolidColorBrush(Color.FromRgb(r, g, b));
        }

        private void setBoolean(bool value)
        {
            tbControlAction.IsReadOnly = value;
            tbExpectedResult.IsReadOnly = value;
            tbStation.IsReadOnly = value;
        }

        private void pbProcedureProgress_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            //do nothing
        }

        #endregion otherFunctions
        #endregion
    }

    #region MVVM Implementation

    public class RelayCommand : ICommand
    {
        private readonly Predicate<object> _canExecute;
        private readonly Action<object> _execute;

        public RelayCommand(Action<object> execute) : this(execute, null) { }
        public RelayCommand( Action<object> execute, Predicate<object> canExecute)
        {
            if (execute == null)
                throw new ArgumentNullException("execute");
            _execute = execute; 
            _canExecute = canExecute;
        }

        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        public bool CanExecute(object parameter)
        {
            return _canExecute == null ? true : _canExecute(parameter);
        }

        public void Execute(object parameter)
        {
            _execute(parameter);
        }
    }

    public class BaseViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        public event EventHandler SelectedItemUpdated;

        protected void OnSelectedItemChanged()
            => SelectedItemUpdated?.Invoke(this, null);
    }

    public class ResultToColorConverter : IValueConverter
    {
        public static ResultToColorConverter Instance = new ResultToColorConverter();

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            TestStepViewModel step = value as TestStepViewModel;
            if (step.ControlActionChanged || step.ExpectedResultChanged || step.StationChanged)
            {
                return new SolidColorBrush(Colors.Yellow);
            } 
            else if (step.ResultChanged)
            {
                if (step.Result)
                {
                    return new SolidColorBrush(Colors.Green);
                }
                else
                {
                    return new SolidColorBrush(Colors.Red);
                }
            } 
            else
            {
                return new SolidColorBrush(Colors.Transparent);
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class StepToLabelConverter : IValueConverter
    {
        public static StepToLabelConverter Instance = new StepToLabelConverter();

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            TestStepViewModel step = value as TestStepViewModel;
            string label = step.StepID + ".";
            if (step.ControlActionChanged || step.ExpectedResultChanged || step.StationChanged)
            {
                label += " (Edited)";
            }
            if (step.ResultChanged)
            {
                if (step.Result)
                {
                    label += " (Passed)";
                }
                else
                {
                    label += " (Failed)";
                }
            }
            return label;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class TestStepViewModel : BaseViewModel
    {
        private string _stationOriginal;
        private string _station;
        private bool _stationChanged = false;
        private string _controlActionOriginal;
        private string _controlAction;
        private bool _controlActionChanged = false;
        private string _expectedResultOriginal;
        private string _expectedResult;
        private bool _expectedResultChanged = false;
        private bool _result = false;
        private bool _resultChanged = false;
        private bool _isSelected = false;

        public TestStepViewModel(int stepID, string station, string controlAction, string expectedResult)
        {
            this.StepID = stepID;
            _stationOriginal = station;
            _station = station;
            _controlActionOriginal = controlAction;
            _controlAction = controlAction;
            _expectedResultOriginal = expectedResult;
            _expectedResult = expectedResult;
        }

        public Object ItSelf
        {
            get { return this; }
        }

        public int StepID { get; set; }

        public string Station
        {
            get { return _station; }
            set
            {
                if (!value.Equals(_station))
                {
                    _station = value;
                    if (_station.Equals(_stationOriginal))
                    {
                        StationChanged = false;
                    }
                    else
                    {
                        StationChanged = true;
                    }
                    OnPropertyChanged("Station");
                }
            }
        }
        public bool StationChanged
        {
            get { return _stationChanged; }
            set
            {
                if (value != _stationChanged)
                {
                    _stationChanged = value;
                    OnPropertyChanged("StationChanged");
                    OnPropertyChanged("ItSelf");
                }
            }
        }

        public string ControlAction
        {
            get { return _controlAction; }
            set
            {
                if (!value.Equals(_controlAction))
                {
                    _controlAction = value;
                    if (_controlAction.Equals(_controlActionOriginal))
                    {
                        ControlActionChanged = false;
                    }
                    else
                    {
                        ControlActionChanged = true;
                    }
                    OnPropertyChanged("ControlAction");
                }
            }
        }
        public bool ControlActionChanged
        {
            get { return _controlActionChanged; }
            set
            {
                if (value != _controlActionChanged)
                {
                    _controlActionChanged = value;
                    OnPropertyChanged("ControlActionChanged");
                    OnPropertyChanged("ItSelf");
                }
            }
        }

        public string ExpectedResult
        {
            get { return _expectedResult; }
            set
            {
                if (!value.Equals(_expectedResult))
                {
                    _expectedResult = value;
                    if (_expectedResult.Equals(_expectedResultOriginal))
                    {
                        ExpectedResultChanged = false;
                    }
                    else
                    {
                        ExpectedResultChanged = true;
                    }
                    OnPropertyChanged("ExpectedResult");
                }
            }
        }
        public bool ExpectedResultChanged
        {
            get { return _expectedResultChanged; }
            set
            {
                if (_expectedResultChanged != value)
                {
                    _expectedResultChanged = value;
                    OnPropertyChanged("ExpectedResultChanged");
                    OnPropertyChanged("ItSelf");
                }
            }
        }

        public bool Result
        {
            get { return _result; }
            set
            {
                _result = value;
                ResultChanged = true;
                OnPropertyChanged("Result");
                OnPropertyChanged("ItSelf");
            }
        }
        public bool ResultChanged
        {
            get { return _resultChanged; }
            set
            {
                if (_resultChanged != value)
                {
                    _resultChanged = value;
                }
            }
        }

        public string ResultComment { get; set; }
        public string Severity { get; set; }
        public string Comment { get; set; }

        public bool IsSelected
        {
            get { return _isSelected; }
            set
            {
                if (_isSelected != value)
                {
                    _isSelected = value;
                    OnPropertyChanged("IsSelected");
                    OnSelectedItemChanged();
                }
            }
        }
    }

    public class SectionsViewModel : BaseViewModel
    {
        private int _sectionID;
        private string _sectionHeading;
        private ObservableCollection<TestStepViewModel> _steps;
        private bool _isSelected = false;

        public SectionsViewModel(int sectionID, string sectionHeading)
        {
            _sectionID = sectionID;
            _sectionHeading = sectionHeading;
        }

        public ObservableCollection<TestStepViewModel> Steps
        {
            get { return _steps; }
            set
            {
                if (_steps != value)
                {
                    _steps = value;
                    OnPropertyChanged("Steps");
                }
            }
        }

        public int SectionID
        {
            get { return _sectionID; }
            set
            {
                if (_sectionID != value)
                {
                    _sectionID = value;
                    OnPropertyChanged("SectionID");
                }
            }
        }

        public string SectionHeading
        {
            get { return _sectionHeading; }
            set
            {
                if (_sectionHeading != value)
                {
                    _sectionHeading = value;
                    OnPropertyChanged("SectionHeading");
                }
            }
        }

        public bool IsSelected
        {
            get { return _isSelected; }
            set
            {
                if (_isSelected != value)
                {
                    _isSelected = value;
                    OnPropertyChanged("IsSelected");
                }
            }
        }
    }

    public class MainViewModel : BaseViewModel
    {
        #region Private Members

        /// <summary>
        /// List of all sections
        /// </summary>
        private ObservableCollection<SectionsViewModel> _sections;

        private TestStepViewModel _selectedStep;

        private ICommand _PassStepCommand;
        private ICommand _FailStepCommand;

        #endregion

        #region Constructor
        /// <summary>
        /// Default Constructor
        /// </summary>
        /// <param name="xmlDocument">The path to the xml Document to load</param>
        public MainViewModel(XmlDocument xmlDocument)
        {
            XmlNode root = xmlDocument.SelectSingleNode("Test_Procedure");

            _sections = new ObservableCollection<SectionsViewModel>();

            var sectionsList = root.SelectNodes("Sections/Section");
            foreach (XmlNode sectionNode in sectionsList)
            {
                SectionsViewModel newSection = new SectionsViewModel(Int32.Parse(sectionNode.Attributes.GetNamedItem("id").Value), sectionNode.SelectSingleNode("Heading").InnerText);
                var steps = new ObservableCollection<TestStepViewModel>();

                var testStepsList = sectionNode.SelectNodes("Test_Step");
                foreach (XmlNode testStepNode in testStepsList)
                {
                    int stepID = Int32.Parse(testStepNode.Attributes.GetNamedItem("id").Value);
                    string station = testStepNode.SelectSingleNode("Station").InnerText;
                    string controlAction = testStepNode.SelectSingleNode("Control_Action").InnerText;
                    string expectedResult = testStepNode.SelectSingleNode("Expected_Result").InnerText;
                    TestStepViewModel newStep = new TestStepViewModel(stepID, station, controlAction, expectedResult);
                    addEventHandlerToStep(newStep);
                    steps.Add(newStep);
                }

                newSection.Steps = steps;
                _sections.Add(newSection);
            }

            _sections[0].Steps[0].IsSelected = true;
        }
        #endregion

        #region Public Members

        /// <summary>
        /// List of Sections
        /// </summary>
        public ObservableCollection<SectionsViewModel> Sections
        {
            get { return _sections; }
            set
            {
                if (value != _sections)
                {
                    _sections = value;
                    OnPropertyChanged("Sections");
                }
            }
        }

        /// <summary>
        /// Returns currently selected test step
        /// </summary>
        public TestStepViewModel SelectedStep
        {
            get { return _selectedStep; }
            set
            {
                TestStepViewModel selectedStep = value;
                foreach (SectionsViewModel section in _sections)
                {
                    selectedStep = section.Steps.FirstOrDefault(i => i.IsSelected);
                    if (selectedStep != null)
                        break;
                }
                _selectedStep = selectedStep;
                OnPropertyChanged("SelectedStep");
            }
        }

        public ICommand PassStepCommand
        {
            get
            {
                if (_PassStepCommand == null)
                {
                    _PassStepCommand = new RelayCommand(p => this.PassStep());
                }
                return _PassStepCommand;
            }
        }

        public ICommand FailStepCommand
        {
            get
            {
                if (_FailStepCommand == null)
                {
                    _FailStepCommand = new RelayCommand(p => this.FailStep());
                }
                return _FailStepCommand;
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Event listener for test step changed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void StepSelectedChanged(object sender, EventArgs e)
        {
            this.SelectedStep = null;
        }

        /// <summary>
        /// Event listener for the values of the currently selected test step changing
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void StepSelectedValueChanged(object sender, PropertyChangedEventArgs e)
        {
            OnPropertyChanged("SelectedStep");
        }

        #endregion

        #region Commands

        private void PassStep()
        {
            if (SelectedStep != null)
            {
                SelectedStep.Result = true;
                selectNextStep();
            }
        }

        private void FailStep()
        {
            if (SelectedStep != null)
            {
                SelectedStep.Result = false;
                selectNextStep();
            }
        }

        #endregion

        #region Helper Functions

        /// <summary>
        /// Adds event listeners to the test step
        /// </summary>
        /// <param name="newStep">test step to add event handler to</param>
        private void addEventHandlerToStep(TestStepViewModel newStep)
        {
            newStep.SelectedItemUpdated += StepSelectedChanged;
            newStep.PropertyChanged += StepSelectedValueChanged;
        }

        private void selectNextStep()
        {
            int i, j;
            bool selectedFound = false;
            for (i = 0; i < _sections.Count(); i++)
            {
                for(j = 0; j < _sections[i].Steps.Count(); j++)
                {
                    if (selectedFound)
                    {
                        _sections[i].Steps[j].IsSelected = true;
                        return;
                    }
                    if (_sections[i].Steps[j].IsSelected)
                    {
                        selectedFound = true;
                    }
                }
            }
        }

        #endregion
    }

    #endregion

}