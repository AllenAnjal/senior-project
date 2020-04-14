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
        private MainWindow main;
        private bool redlineClicked = false;

        public Tester(MainWindow mw, String xmlFile)
        {
            InitializeComponent();
            main = mw;

            IDialogService dialogService = new DialogService(mw);

            dialogService.Register<UserInfoDialogViewModel, userInfoPage>();
            dialogService.Register<CommentDialogViewModel, CommentDialog>();

            XmlDocument xml = new XmlDocument();
            try
            {
                xml.Load(xmlFile);
            }
            catch (Exception err)
            {
                throw new ArgumentException(err.ToString());
            }

            MainTesterViewModel test = new MainTesterViewModel(xml, dialogService);
            this.DataContext = test;
        }

        private void WindowClosed(object sender, EventArgs e)
        {
            main.Show();
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

        private void Exit_Button(object sender, RoutedEventArgs e)
        {
            this.Close();
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
    }

    #region MVVM Implementation

    public class RelayCommand : ICommand
    {
        private readonly Predicate<object> _canExecute;
        private readonly Action<object> _execute;

        public RelayCommand(Action<object> execute) : this(execute, null) { }
        public RelayCommand(Action<object> execute, Predicate<object> canExecute)
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


        public event EventHandler<SelectedItemUpdateEventArgs> SelectedItemUpdated;
        protected void OnSelectedItemChanged(TestStepViewModel testStep)
            => SelectedItemUpdated?.Invoke(this, new SelectedItemUpdateEventArgs(testStep));
    }

    public class SelectedItemUpdateEventArgs : EventArgs
    {
        private readonly TestStepViewModel _selectedStep;

        public SelectedItemUpdateEventArgs(TestStepViewModel selectedStep)
        {
            _selectedStep = selectedStep;
        }

        public TestStepViewModel SelectedStep
        {
            get { return _selectedStep; }
        }
    }

    public class ResultToColorConverter : IValueConverter
    {
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

    public class StepToTreeviewLabelConverter : IValueConverter
    {
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

    public class StepToFullLabelConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            TestStepViewModel testStep = value as TestStepViewModel;
            return "Section: " + testStep.Parent.SectionID.ToString() + ", Step: " + testStep.StepID.ToString();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class TestStepViewModel : BaseViewModel
    {
        #region Private Members

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
        private int _stepID;

        #endregion

        #region Constructor

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

        #endregion

        #region Public Members

        public SectionsViewModel Parent { get; set; }

        public Object ItSelf
        {
            get { return this; }
        }

        public int StepID
        {
            get { return _stepID; }
            set
            {
                if (value != _stepID)
                {
                    _stepID = value;
                    OnPropertyChanged("ItSelf");
                }
            }
        }

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
                    if (_isSelected)
                    {
                        OnSelectedItemChanged(this);
                        OnPropertyChanged("IsSelected");
                    }
                }
            }
        }

        #endregion
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

        public string Description { get; set; }

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

    public class MainTesterViewModel : BaseViewModel
    {
        #region Private Members

        /// <summary>
        /// List of all sections
        /// </summary>
        private ObservableCollection<SectionsViewModel> _sections;
        private TestStepViewModel _selectedStep;
        private readonly IDialogService _dialogService;
        private XmlDocument originalXML;

        private string _date = String.Empty;
        private string _timeStarted = String.Empty;

        #endregion

        #region Constructor

        public MainTesterViewModel(XmlDocument xmlDocument, IDialogService dialogService)
        {
            _dialogService = dialogService;
            _sections = new ObservableCollection<SectionsViewModel>();

            loadXML(xmlDocument);
            originalXML = xmlDocument;

            _sections[0].Steps[0].IsSelected = true;
            getUserInfo();
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
                if(_selectedStep != value)
                {
                    _selectedStep = value;
                    OnPropertyChanged("SelectedStep");
                }
                //TestStepViewModel selectedStep = value;
                //foreach (SectionsViewModel section in _sections)
                //{
                //    selectedStep = section.Steps.FirstOrDefault(i => i.IsSelected);
                //    if (selectedStep != null)
                //        break;
                //}
                //_selectedStep = selectedStep;
                //OnPropertyChanged("SelectedStep");
            }
        }

        public string Name { get; set; }
        public string SoftwareLoadVersion { get; set; }
        public string ProgramPhase { get; set; }
        public string ProgramType { get; set; }
        public string Classification { get; set; }

        #endregion

        #region Public Methods

        /// <summary>
        /// Event listener for test step changed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void StepSelectedChanged(object sender, SelectedItemUpdateEventArgs e)
        {
            this.SelectedStep = e.SelectedStep;
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

        private ICommand _PassStepCommand;
        private ICommand _FailStepCommand;
        private ICommand _MoveUpCommand;
        private ICommand _MoveDownCommand;
        private ICommand _CommentCommand;
        private ICommand _EditUserInfoCommand;
        private ICommand _SaveToXmlCommand;

        public ICommand PassStepCommand
        {
            get
            {
                if (_PassStepCommand == null)
                {
                    _PassStepCommand = new RelayCommand(p => PassStep());
                }
                return _PassStepCommand;
            }
        }
        private void PassStep()
        {
            if (SelectedStep != null)
            {
                SelectedStep.Result = true;
                selectNextStep();
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
        private void FailStep()
        {
            if (SelectedStep != null)
            {
                getFailComment();
            }
        }

        public ICommand MoveUpCommand
        {
            get
            {
                if (_MoveUpCommand == null)
                {
                    _MoveUpCommand = new RelayCommand(p => this.MoveUp());
                }
                return _MoveUpCommand;
            }
        }
        private void MoveUp()
        {
            selectPreviousStep();
        }

        public ICommand MoveDownCommand
        {
            get
            {
                if (_MoveDownCommand == null)
                {
                    _MoveDownCommand = new RelayCommand(p => this.MoveDown());
                }
                return _MoveDownCommand;
            }
        }
        private void MoveDown()
        {
            selectNextStep();
        }

        public ICommand CommentCommand
        {
            get
            {
                if (_CommentCommand == null)
                {
                    _CommentCommand = new RelayCommand(p => this.Comment());
                }
                return _CommentCommand;
            }
        }
        private void Comment()
        {
            getComment();
        }

        public ICommand EditUserInfoCommand
        {
            get
            {
                if (_EditUserInfoCommand == null)
                {
                    _EditUserInfoCommand = new RelayCommand(p => this.EditUserInfo());
                }
                return _EditUserInfoCommand;
            }
        }
        private void EditUserInfo()
        {
            getUserInfo();
        }

        public ICommand SaveToXmlCommand
        {
            get
            {
                if (_SaveToXmlCommand == null)
                {
                    _SaveToXmlCommand = new RelayCommand(p => this.SaveToXml());
                }
                return _SaveToXmlCommand;
            }
        }
        private void SaveToXml()
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            try
            {
                saveFileDialog.Filter = "XML file (*.xml)|*.xml";
                if (saveFileDialog.ShowDialog() == true)
                {
                    string fullpath = saveFileDialog.FileName;

                    XmlNode root = originalXML.SelectSingleNode("Test_Procedure");

                    // Fill Heading
                    XmlNode procedureHeading = root.SelectSingleNode("Procedure_Heading");
                    procedureHeading.SelectSingleNode("Name").InnerText = Name;
                    procedureHeading.SelectSingleNode("Date").InnerText = _date;
                    procedureHeading.SelectSingleNode("Software_Load_Version").InnerText = SoftwareLoadVersion;
                    procedureHeading.SelectSingleNode("Program_Phase").InnerText = ProgramPhase;
                    procedureHeading.SelectSingleNode("Program_Type").InnerText = ProgramType;
                    procedureHeading.SelectSingleNode("Classification").InnerText = Classification;
                    procedureHeading.SelectSingleNode("Start_Time").InnerText = _timeStarted;
                    procedureHeading.SelectSingleNode("Stop_Time").InnerText = DateTime.Now.ToString("HH:mm");

                    int i = 0, j = 0;
                    // Fill Test Steps
                    var sectionsList = root.SelectNodes("Sections/Section");
                    foreach (XmlNode sectionNode in sectionsList)
                    {
                        var testStepsList = sectionNode.SelectNodes("Test_Step");
                        j = 0;
                        foreach (XmlNode testStepNode in testStepsList)
                        {
                            TestStepViewModel testStep = _sections[i].Steps[j];
                            if (testStep.StationChanged)
                                testStepNode.SelectSingleNode("Station_Redline").InnerText = testStep.Station;
                            if (testStep.ControlActionChanged)
                                testStepNode.SelectSingleNode("Control_Action_Redline").InnerText = testStep.ControlAction;
                            if (testStep.ExpectedResultChanged)
                                testStepNode.SelectSingleNode("Expected_Result_Redline").InnerText = testStep.ExpectedResult;

                            XmlNode resultNode = testStepNode.SelectSingleNode("Result");
                            if (testStep.ResultChanged)
                            {
                                resultNode.Attributes.GetNamedItem("result").Value = testStep.Result.ToString();
                                resultNode.SelectSingleNode("Comment").InnerText = testStep.ResultComment;
                                resultNode.SelectSingleNode("Severity").InnerText = testStep.Severity;
                            }

                            testStepNode.SelectSingleNode("Comment").InnerText = testStep.Comment;

                            j++;
                        }
                        i++;
                    }
                    
                    originalXML.Save(fullpath);
                }
            } 
            catch (IOException e)
            {
                MessageBox.Show(e.ToString());
            }
        }

        #endregion

        #region Helper Functions

        /// <summary>
        /// Adds event listeners to the test step
        /// </summary>
        /// <param name="newStep">test step to add event handler to</param>
        
        private void loadXML(XmlDocument xmlDocument)
        {
            XmlNode root = xmlDocument.SelectSingleNode("Test_Procedure");

            XmlNode heading = root.SelectSingleNode("Procedure_Heading");

            this.Name = heading.SelectSingleNode("Name").InnerText;
            _date = String.IsNullOrEmpty(heading.SelectSingleNode("Date").InnerText) ? DateTime.Now.Date.ToString("MM/dd/yyyy") : heading.SelectSingleNode("Start_Time").InnerText;
            this.SoftwareLoadVersion = heading.SelectSingleNode("Software_Load_Version").InnerText;
            this.ProgramPhase = heading.SelectSingleNode("Program_Phase").InnerText;
            this.ProgramType = heading.SelectSingleNode("Program_Type").InnerText;
            this.Classification = heading.SelectSingleNode("Classification").InnerText;
            _timeStarted = String.IsNullOrEmpty(heading.SelectSingleNode("Start_Time").InnerText) ? DateTime.Now.ToString("HH:mm") : heading.SelectSingleNode("Start_Time").InnerText;

            var sectionsList = root.SelectNodes("Sections/Section");
            foreach (XmlNode sectionNode in sectionsList)
            {
                SectionsViewModel newSection = new SectionsViewModel(Int32.Parse(sectionNode.Attributes.GetNamedItem("id").Value), sectionNode.SelectSingleNode("Heading").InnerText);
                newSection.Description = sectionNode.SelectSingleNode("Description").InnerText;

                
                var steps = new ObservableCollection<TestStepViewModel>();
                var testStepsList = sectionNode.SelectNodes("Test_Step");
                foreach (XmlNode testStepNode in testStepsList)
                {
                    int stepID = Int32.Parse(testStepNode.Attributes.GetNamedItem("id").Value);
                    string station = testStepNode.SelectSingleNode("Station").InnerText;
                    string controlAction = testStepNode.SelectSingleNode("Control_Action").InnerText;
                    string expectedResult = testStepNode.SelectSingleNode("Expected_Result").InnerText;
                    TestStepViewModel newStep = new TestStepViewModel(stepID, station, controlAction, expectedResult);
                    XmlNode result = testStepNode.SelectSingleNode("Result");
                    if (result.Attributes.GetNamedItem("result").Value != "")
                    {
                        newStep.Result = bool.Parse(result.Attributes.GetNamedItem("result").Value);
                    }
                    newStep.ResultComment = result.SelectSingleNode("Comment").InnerText;
                    newStep.Severity = result.SelectSingleNode("Severity").InnerText;
                    newStep.Comment = testStepNode.SelectSingleNode("Comment").InnerText;

                    if (!String.IsNullOrEmpty(testStepNode.SelectSingleNode("Station_Redline").InnerText))
                        newStep.Station = testStepNode.SelectSingleNode("Station_Redline").InnerText;

                    if (!String.IsNullOrEmpty(testStepNode.SelectSingleNode("Control_Action_Redline").InnerText))
                        newStep.ControlAction = testStepNode.SelectSingleNode("Control_Action").InnerText;

                    if (!String.IsNullOrEmpty(testStepNode.SelectSingleNode("Expected_Result_Redline").InnerText))
                        newStep.ExpectedResult = testStepNode.SelectSingleNode("Expected_Result_Redline").InnerText;

                    newStep.Parent = newSection;
                    addEventHandlerToStep(newStep);
                    steps.Add(newStep);
                }

                newSection.Steps = steps;
                _sections.Add(newSection);
            }
        }

        private void addEventHandlerToStep(TestStepViewModel newStep)
        {
            newStep.SelectedItemUpdated += StepSelectedChanged;
            newStep.PropertyChanged += StepSelectedValueChanged;
        }

        private void selectNextStep()
        {
            if (_selectedStep == null) { return; }
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

        private void selectPreviousStep()
        {
            if (_selectedStep == null) { return; }
            int i, j;
            bool selectedFound = false;
            for (i = _sections.Count() - 1; i > -1; i--)
            {
                for (j = _sections[i].Steps.Count() - 1; j > -1; j--)
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

        private void getUserInfo()
        {
            UserInfoDialogViewModel userInfo = new UserInfoDialogViewModel(this);
            bool? result = _dialogService.ShowDialog(userInfo);
            if (result == true)
            {
                this.Name = userInfo.Name;
                this.SoftwareLoadVersion = userInfo.SoftwareLoadVersion;
                this.ProgramPhase = userInfo.ProgramPhase;
                this.ProgramType = userInfo.ProgramType;
                this.Classification = userInfo.Classification;
            } 
        }

        private void getFailComment()
        {
            CommentDialogViewModel comment = new CommentDialogViewModel(_selectedStep, true);
            bool? result = _dialogService.ShowDialog(comment);
            if (result == true)
            {
                _selectedStep.Severity = comment.Severity;
                _selectedStep.ResultComment = comment.Comment;
                _selectedStep.Result = false;
                selectNextStep();
            }
        }

        private void getComment()
        {
            CommentDialogViewModel comment = new CommentDialogViewModel(_selectedStep, false);
            bool? result = _dialogService.ShowDialog(comment);
            if (result == true)
            {
                _selectedStep.Comment = comment.Comment;
            }
        }

        #endregion
    }

    #endregion

}