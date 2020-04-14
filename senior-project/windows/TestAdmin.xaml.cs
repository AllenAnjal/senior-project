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
    /// 
    public partial class TestAdmin : Window
    {
        private MainWindow main;
        private bool redlineClicked = false;

        public TestAdmin(MainWindow mw, String xmlFile)
        {
            InitializeComponent();
            main = mw;

            IDialogService dialogService = new DialogService(mw);
            dialogService.Register<renameDialogViewModel, renameDialog>();
            dialogService.Register<DescriptionDialogViewModel, DescriptionDialog>();
            XmlDocument xml = new XmlDocument();
            try
            {
                xml.Load(xmlFile);
            }
            catch (Exception err)
            {
                throw new ArgumentException(err.ToString());
            }

            EditorViewModel test = new EditorViewModel(xml, dialogService);
            this.DataContext = test;
        }

        #region more buttons

        private void toggleAll(object sender, EventArgs e)
        {
            summonControlAction();
            summonExpectedResult();
            summonStation();
        }

        private void toggleCtrl(object sender, EventArgs e)
        {
            summonControlAction();
        }

        private void toggleExp(object sender, EventArgs e)
        {
            summonExpectedResult();
        }

        private void toggleStation(object sender, EventArgs e)
        {
            summonStation();
        }

        private void summonStation()
        {
            acceptStation.Visibility = Visibility.Visible;
            rejectStation.Visibility = Visibility.Visible;
        }
        private void summonExpectedResult()
        {
            acceptExpectedbtn.Visibility = Visibility.Visible;
            rejectExpectedResult.Visibility = Visibility.Visible;
        }
        private void summonControlAction()
        {
            acceptControl.Visibility = Visibility.Visible;
            rejectControlAction.Visibility = Visibility.Visible;
        }
        private void WindowClosed(object sender, EventArgs e)
        {
            main.Show();
        }


        private void Exit_Button(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        #endregion


    }

    #region MVVM Implementation

    public class EditorRelayCommand : ICommand
    {
        private readonly Predicate<object> _canExecute;
        private readonly Action<object> _execute;

        public EditorRelayCommand(Action<object> execute) : this(execute, null) { }
        public EditorRelayCommand(Action<object> execute, Predicate<object> canExecute)
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

    public class EditorBaseViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));


        public event EventHandler<EditorSelectedItemUpdateEventArgs> SelectedItemUpdated;
        protected void OnSelectedItemChanged(EditorStepViewModel testStep)
            => SelectedItemUpdated?.Invoke(this, new EditorSelectedItemUpdateEventArgs(testStep));
    }

    public class EditorSelectedItemUpdateEventArgs : EventArgs
    {
        private readonly EditorStepViewModel _selectedStep;

        public EditorSelectedItemUpdateEventArgs(EditorStepViewModel selectedStep)
        {
            _selectedStep = selectedStep;
        }

        public EditorStepViewModel SelectedStep
        {
            get { return _selectedStep; }
        }
    }

    public class EditorStepToTreeviewLabelConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            EditorStepViewModel step = value as EditorStepViewModel;
            string label = step.StepID + ".";

            return label;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class EditorStepToFullLabelConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            EditorStepViewModel testStep = value as EditorStepViewModel;
            return "Section: " + testStep.Parent.SectionID.ToString() + ", Step: " + testStep.StepID.ToString();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class EditorStepViewModel : EditorBaseViewModel
    {
        #region Private Members

        private string _stationOriginal;
        private string _station;
        private int _stepID;
        private string _controlActionOriginal;
        private string _controlAction;
        private string _expectedResultOriginal;
        private string _expectedResult;
        private bool _isSelected;
        private string _redlineStation;
        private string _redlineControlAction;
        private string _redlineExpectedResult;

        #endregion

        #region Constructor

        public EditorStepViewModel(int stepID, string station, string controlAction, string expectedResult, string redlineStation, string redlineExpectedResult, string redlineControlAction)
        {
            _stepID = stepID;
            _stationOriginal = station;
            _station = station;
            _redlineStation = redlineStation;
            _controlActionOriginal = controlAction;
            _controlAction = controlAction;
            _redlineControlAction = redlineControlAction;
            _expectedResultOriginal = expectedResult;
            _expectedResult = expectedResult;
            _redlineExpectedResult = redlineExpectedResult;
        }

        #endregion

        #region Public Members

        public EditorSectionsViewModel Parent { get; set; }

        public Object ItSelf
        {
            get { return this; }
        }

        public string orgControlAction
        {
            get { return _controlActionOriginal; }
        }
        public string orgStation
        {
            get { return _stationOriginal; }
        }
        public string orgExpectedResult
        {
            get { return _expectedResultOriginal; }
        }

        public int StepID { 
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
                    OnPropertyChanged("Station");
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
                    OnPropertyChanged("ControlAction");
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
                    OnPropertyChanged("ExpectedResult");
                }
            }
        }

        public string RedlineStation
        {
            get { return _redlineStation; }
        }
        public string RedlineControlAction
        {
            get { return _redlineControlAction; }
        }
        
        public string RedlineExpectedResult
        {
            get { return _redlineExpectedResult; }
        }

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

    public class EditorSectionsViewModel : EditorBaseViewModel
    {
        private int _sectionID;
        private string _sectionHeading;
        private ObservableCollection<EditorStepViewModel> _steps;
        private bool _isSelected = false;

        public EditorSectionsViewModel(int sectionID, string sectionHeading)
        {
            _sectionID = sectionID;
            _sectionHeading = sectionHeading;
        }

        public ObservableCollection<EditorStepViewModel> Steps
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


    public class EditorViewModel : EditorBaseViewModel
    {
        #region Private Members

        /// <summary>
        /// List of all sections
        /// </summary>
        private ObservableCollection<EditorSectionsViewModel> _sections;
        private EditorStepViewModel _selectedStep;
        private readonly IDialogService _dialogService;
        private XmlDocument originalXML;

        private string _date = String.Empty;
        private string _timeStarted = String.Empty;

        #endregion

        #region Constructor

        public EditorViewModel(XmlDocument xmlDocument, IDialogService dialogService)
        {
            _dialogService = dialogService;
            _sections = new ObservableCollection<EditorSectionsViewModel>();

            loadXML(xmlDocument);
            originalXML = xmlDocument;

            _sections[0].Steps[0].IsSelected = true;
        }
        #endregion

        #region Public Members

        /// <summary>
        /// List of Sections
        /// </summary>
        public ObservableCollection<EditorSectionsViewModel> Sections
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
        public EditorStepViewModel SelectedStep
        {
            get { return _selectedStep; }
            set
            {
                if (_selectedStep != value)
                {
                    _selectedStep = value;
                    OnPropertyChanged("SelectedStep");
                }

            }
        }



        #endregion

        #region Public Methods

        /// <summary>
        /// Event listener for test step changed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void StepSelectedChanged(object sender, EditorSelectedItemUpdateEventArgs e)
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

        private ICommand _removeCommand;
        private ICommand _addStepCommand;
        private ICommand _addSectionCommand;
        private ICommand _renameCommand;
        private ICommand _MoveUpCommand;
        private ICommand _MoveDownCommand;
        private ICommand _SaveToXmlCommand;
        private ICommand _MoveSectionUp;
        private ICommand _MoveSectionDown;
        private ICommand _loadAll;
        private ICommand _loadStationCommand;
        private ICommand _loadExpectedCommand;
        private ICommand _loadActionCommand;
        private ICommand _discardAll;
        private ICommand _discardStationCommand;
        private ICommand _discardExpectedCommand;
        private ICommand _discardActionCommand;
        private ICommand _CommitAll;
        private ICommand _commitStationCommand;
        private ICommand _commitExpectedCommand;
        private ICommand _commitActionCommand;


        #region redline stuff

        public ICommand discardStationCommand
        {
            get
            {
                if (_discardStationCommand == null)
                {
                    _discardStationCommand = new RelayCommand(p => this.discardStation());
                }
                return _discardStationCommand;
            }
        }
        public ICommand discardExpectedCommand
        {
            get
            {
                if (_discardExpectedCommand == null)
                {
                    _discardExpectedCommand = new RelayCommand(p => this.discardExpectedResult());
                }
                return _discardExpectedCommand;
            }
        }
        public ICommand discardActionCommand
        {
            get
            {
                if (_discardActionCommand == null)
                {
                    _discardActionCommand = new RelayCommand(p => this.discardControlAction());
                }
                return _discardActionCommand;
            }
        }

        public ICommand discardAllCommand
        {
            get
            {
                if (_discardAll == null)
                {
                    _discardAll = new RelayCommand(p => this.discardAll());
                }
                return _discardAll;
            }
        }

        private void discardAll()
        {
            discardControlAction();
            discardStation();
            discardExpectedResult();
        }

        private void discardStation()
        {
            int i = 0, j = 0;
            bool found = false;
            for (i = 0; i < _sections.Count(); i++)
            {
                for (j = 0; j < _sections[i].Steps.Count(); j++)
                {

                    if (_sections[i].Steps[j].IsSelected)
                    {
                        found = true;
                        break;
                    }

                }
                if (found) break;
            }
            _sections[i].Steps[j].Station = _sections[i].Steps[j].orgStation;
        }

        private void discardExpectedResult()
        {
            int i = 0, j = 0;
            bool found = false;
            for (i = 0; i < _sections.Count(); i++)
            {
                for (j = 0; j < _sections[i].Steps.Count(); j++)
                {

                    if (_sections[i].Steps[j].IsSelected)
                    {
                        found = true;
                        break;
                    }

                }
                if (found) break;
            }
            _sections[i].Steps[j].ExpectedResult = _sections[i].Steps[j].orgExpectedResult;
        }

        private void discardControlAction()
        {
            int i = 0, j = 0;
            bool found = false;
            for (i = 0; i < _sections.Count(); i++)
            {
                for (j = 0; j < _sections[i].Steps.Count(); j++)
                {

                    if (_sections[i].Steps[j].IsSelected)
                    {
                        found = true;
                        break;
                    }

                }
                if (found) break;
            }
            _sections[i].Steps[j].ControlAction = _sections[i].Steps[j].orgControlAction;
        }

        public ICommand commitStationCommand
        {
            get
            {
                if (_commitStationCommand == null)
                {
                    _commitStationCommand = new RelayCommand(p => this.commitStation());
                }
                return _commitStationCommand;
            }
        }
        public ICommand commitExpectedCommand
        {
            get
            {
                if (_commitExpectedCommand == null)
                {
                    _commitExpectedCommand = new RelayCommand(p => this.commitExpectedResult());
                }
                return _commitExpectedCommand;
            }
        }
        public ICommand commitActionCommand
        {
            get
            {
                if (_commitActionCommand == null)
                {
                    _commitActionCommand = new RelayCommand(p => this.commitControlAction());
                }
                return _commitActionCommand;
            }
        }

        public ICommand CommitAllCommand
        {
            get
            {
                if (_CommitAll == null)
                {
                    _CommitAll = new RelayCommand(p => this.CommitAll());
                }
                return _CommitAll;
            }
        }

        private void CommitAll()
        {
            commitControlAction();
            commitStation();
            commitExpectedResult();
        }

        private void commitStation()
        {
            int i = 0, j = 0;
            bool found = false;
            for (i = 0; i < _sections.Count(); i++)
            {
                for (j = 0; j < _sections[i].Steps.Count(); j++)
                {

                    if (_sections[i].Steps[j].IsSelected)
                    {
                        found = true;
                        break;
                    }

                }
                if (found) break;
            }
            _sections[i].Steps[j].Station = _sections[i].Steps[j].RedlineStation;
        }

        private void commitExpectedResult()
        {
            int i = 0, j = 0;
            bool found = false;
            for (i = 0; i < _sections.Count(); i++)
            {
                for (j = 0; j < _sections[i].Steps.Count(); j++)
                {

                    if (_sections[i].Steps[j].IsSelected)
                    {
                        found = true;
                        break;
                    }

                }
                if (found) break;
            }
            _sections[i].Steps[j].ExpectedResult = _sections[i].Steps[j].RedlineExpectedResult;
        }

        private void commitControlAction()
        {
            int i = 0, j = 0;
            bool found = false;
            for (i = 0; i < _sections.Count(); i++)
            {
                for (j = 0; j < _sections[i].Steps.Count(); j++)
                {

                    if (_sections[i].Steps[j].IsSelected)
                    {
                        found = true;
                        break;
                    }

                }
                if (found) break;
            }
            _sections[i].Steps[j].ControlAction = _sections[i].Steps[j].RedlineControlAction;
        }

        public ICommand loadStationCommand
        {
            get
            {
                if (_loadStationCommand == null)
                {
                    _loadStationCommand = new RelayCommand(p => this.loadStation());
                }
                return _loadStationCommand;
            }
        }
        public ICommand loadExpectedCommand
        {
            get
            {
                if (_loadExpectedCommand == null)
                {
                    _loadExpectedCommand = new RelayCommand(p => this.loadExpectedResult());
                }
                return _loadExpectedCommand;
            }
        }
        public ICommand loadActionCommand
        {
            get
            {
                if (_loadActionCommand == null)
                {
                    _loadActionCommand = new RelayCommand(p => this.loadControlAction());
                }
                return _loadActionCommand;
            }
        }

        public ICommand loadAllCommand
        {
            get
            {
                if (_loadAll == null)
                {
                    _loadAll = new RelayCommand(p => this.LoadAll());
                }
                return _loadAll;
            }
        }

        private void LoadAll()
        {

            loadStation();
            loadExpectedResult();
            loadControlAction();
            
        }

        private void loadStation()
        {
            int i = 0, j = 0;
            bool found = false;
            for (i = 0; i < _sections.Count(); i++)
            {
                for (j = 0; j < _sections[i].Steps.Count(); j++)
                {

                    if (_sections[i].Steps[j].IsSelected)
                    {
                        found = true;
                        break;
                    }

                }
                if (found) break;
            }
            _sections[i].Steps[j].Station += "\nRedline Changes: " + _sections[i].Steps[j].RedlineStation;
            


        }

        private void loadExpectedResult()
        {
            int i = 0, j = 0;
            bool found = false;
            for (i = 0; i < _sections.Count(); i++)
            {
                for (j = 0; j < _sections[i].Steps.Count(); j++)
                {

                    if (_sections[i].Steps[j].IsSelected)
                    {
                        found = true;
                        break;
                    }

                }
                if (found) break;
            }
            _sections[i].Steps[j].ExpectedResult += "\nRedline Changes: " + _sections[i].Steps[j].RedlineExpectedResult;
        }

        private void loadControlAction()
        {
            int i = 0, j = 0;
            bool found = false;
            for (i = 0; i < _sections.Count(); i++)
            {
                for (j = 0; j < _sections[i].Steps.Count(); j++)
                {

                    if (_sections[i].Steps[j].IsSelected)
                    {
                        found = true;
                        break;
                    }

                }
                if (found) break;
            }
            _sections[i].Steps[j].ControlAction += "\nRedline Changes: " + _sections[i].Steps[j].RedlineControlAction;
        }

        #endregion


        #region functionality portion for editor

        public ICommand removeCommand
        {
            get
            {
                if (_removeCommand == null)
                {
                    _removeCommand = new RelayCommand(p => this.remove());
                }
                return _removeCommand;
            }
        }

        private void remove()
        {
            int i = 0;
            bool sectionSelected = false;
            for (i = 0; i < _sections.Count(); i++) {
                if (_sections[i].IsSelected)
                {
                    sectionSelected = true;
                    break;
                }
            }
            if(sectionSelected)
                _sections.RemoveAt(i);
            else
            {
                
                int j = 0, location = 0;
                bool testSelected = false;
                for(i = 0; i< _sections.Count(); i++)
                {
                    for(j = 0; j< _sections[i].Steps.Count(); j++)
                    {
                                       
                        if (_sections[i].Steps[j].IsSelected) {
                            testSelected = true;
                            location = j;
                        }
                        else if (testSelected)
                        {
                            
                            _sections[i].Steps[j].StepID--;
                            
                        }

                    }
                    if (testSelected) break;
                }

                _sections[i].Steps.RemoveAt(location);
            }

            
        }

        public ICommand addStepCommand
        {
            get
            {
                if (_addStepCommand == null)
                {
                    _addStepCommand = new RelayCommand(p => this.addStep());
                }
                return _addStepCommand;
            }
        }

        private void addStep()
        {

            int i = 0, j = 0;
            bool testSelected = false;
            for (i = 0; i < _sections.Count(); i++)
            {
                for (j = 0; j < _sections[i].Steps.Count(); j++)
                {

                    if (_sections[i].Steps[j].IsSelected)
                    {
                        testSelected = true;
                        break;
                    }

                }
                if (testSelected) break;
            }
            EditorStepViewModel newStep = new EditorStepViewModel((j + 1), "", "", "", "", "", "");
            if (!testSelected)
            {
                
                //add under section as first element 
                for (i = 0; i < _sections.Count(); i++)
                {
                    if (_sections[i].IsSelected)
                    {
                        break;
                    }
                }
                newStep.StepID = _sections[i].Steps.Count() + 1;
                _sections[i].Steps.Add(newStep);
            }
            else
            {
                _sections[i].Steps.Insert(j, newStep);
                for (int k = j + 1; k < _sections[i].Steps.Count(); k++)
                {
                    _sections[i].Steps[k].StepID++;
                }
            }
        }

        public ICommand addSectionCommand
        {
            get
            {
                if (_addSectionCommand == null)
                {
                    _addSectionCommand = new RelayCommand(p => this.addSection());
                }
                return _addSectionCommand;
            }
        }

        private void addSection()
        {
            int i = 0;
            bool sectionSelected = false;
            for (i = 0; i < _sections.Count(); i++)
            {
                if (_sections[i].IsSelected)
                {
                    sectionSelected = true;
                    break;
                }
            }
            if (!sectionSelected)
            {
                bool testSelected = false;
                for (i = 0; i < _sections.Count(); i++)
                {
                    for (int j = 0; j < _sections[i].Steps.Count(); j++)
                    {

                        if (_sections[i].Steps[j].IsSelected)
                        {
                            testSelected = true;
                            break;
                        }

                    }
                    if (testSelected) break;
                }
            }

                string name = getNewSectionName();
                string description = getDescription();
                EditorSectionsViewModel newSection = new EditorSectionsViewModel((i + 1), name);
                newSection.Description = description;
                newSection.Steps = new ObservableCollection<EditorStepViewModel>();
                EditorStepViewModel newStep = new EditorStepViewModel(1, "new station", "new control", "new expected", "", "", "");
                newSection.Steps.Add(newStep);
                
                _sections.Insert(i + 1, newSection);
                for(int j = i + 1; j < _sections.Count(); j++)
                {
                    _sections[j].SectionID++;
                }

        }

        public ICommand renameCommand
        {
            get
            {
                if (_renameCommand == null)
                {
                    _renameCommand = new RelayCommand(p => this.renameSection());
                }
                return _renameCommand;
            }
        }

        private void renameSection()
        {
            int i = 0;
            bool sectionSelected = false;
            for (i = 0; i < _sections.Count(); i++)
            {
                if (_sections[i].IsSelected)
                {
                    sectionSelected = true;
                    break;
                }
            }
            if (!sectionSelected)
            {
                MessageBox.Show("No section is selected"); 
            }
            else
            {
                string newName = getNewSectionName();
                if (newName == "")
                {
                    return;
                }
                else
                {
                    _sections[i].SectionHeading = newName;
                }
            }
            
        }

        private string getNewSectionName()
        {
            renameDialogViewModel rename = new renameDialogViewModel();
            bool? result = _dialogService.ShowDialog(rename);
            if (result == true)
            {
                return rename.Comment;
            }
            else return "";
        }

        private string getDescription()
        {
            DescriptionDialogViewModel descript = new DescriptionDialogViewModel();
            bool? result = _dialogService.ShowDialog(descript);
            if (result == true) return descript.Comment;
            else return "";
        }

        #endregion

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
            int i = 0, j = 0;
            bool stepSelected = false;
            EditorStepViewModel tmp;
            for (i = 0; i < _sections.Count(); i++)
            {
                for (j = 0; j < _sections[i].Steps.Count(); j++)
                {

                    if (_sections[i].Steps[j].IsSelected)
                    {
                        stepSelected = true;

                        break;
                    }
                }
                if (stepSelected) break;
            }
            if (stepSelected)
            {
                if (j == 0 && i > 0)
                {
                    _sections[i].Steps[j].IsSelected = false;
                    _sections[i - 1].Steps.Add(_sections[i].Steps[j]);
                    _sections[i].Steps.RemoveAt(j);
                    _sections[i - 1].Steps[_sections[i - 1].Steps.Count() - 1].IsSelected = true;
                    _sections[i - 1].Steps[_sections[i - 1].Steps.Count() - 1].StepID = _sections[i - 1].Steps.Count();
                    for (int k = 0; k < _sections[i].Steps.Count(); k++)
                    {
                        _sections[i].Steps[k].StepID = k + 1;
                    }
                }
                else if (j == 0 && i == 0)
                {
                    _sections[_sections.Count() -  1].Steps.Add(_sections[i].Steps[j]);
                    _sections[0].Steps.RemoveAt(0);
                    _sections[0].Steps[0].IsSelected = false;
                    _sections[_sections.Count() - 1].Steps[_sections[_sections.Count() - 1].Steps.Count() - 1].IsSelected = true;
                    _sections[_sections.Count() - 1].Steps[_sections[_sections.Count() - 1].Steps.Count() - 1].StepID = _sections[_sections.Count() - 1].Steps.Count();
                    for (int k = 0; k < _sections[0].Steps.Count(); k++)
                    {
                        _sections[0].Steps[k].StepID = k + 1;
                    }


                }
                else
                {

                    int id1 = _sections[i].Steps[j].StepID;
                    int id2 = _sections[i].Steps[j - 1].StepID;

                    tmp = _sections[i].Steps[j];
                    _sections[i].Steps[j] = _sections[i].Steps[j - 1];
                    _sections[i].Steps[j].IsSelected = false;
                    _sections[i].Steps[j - 1] = tmp;
                    _sections[i].Steps[j].StepID = id1;
                    _sections[i].Steps[j - 1].StepID = id2;
                }
                //selectPreviousStep();
            }
            else { MessageBox.Show("No test step selected"); }
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
            //selectNextStep();
            int i = 0, j = 0;
            bool stepSelected = false;
            EditorStepViewModel tmp;
            for (i = 0; i < _sections.Count(); i++)
            {
                for (j = 0; j < _sections[i].Steps.Count(); j++)
                {

                    if (_sections[i].Steps[j].IsSelected)
                    {
                        stepSelected = true;

                        break;
                    }
                }
                if (stepSelected) break;
            }
            if (stepSelected)
            {
                if (j == _sections[_sections.Count() - 1].Steps.Count() - 1 && i < _sections.Count() - 1 )
                {


                    _sections[i].Steps[j].IsSelected = false;
                    _sections[i + 1].Steps.Insert(0, _sections[i].Steps[j]);
                    _sections[i].Steps.RemoveAt(j);
                    _sections[i + 1].Steps[0].IsSelected = true;
                    for(int k = 0; k < _sections[i + 1].Steps.Count(); k++)
                    {
                        _sections[i + 1].Steps[k].StepID = k + 1;
                    }

                }
                else if (j == _sections[_sections.Count() - 1].Steps.Count() - 1 && i == _sections.Count() - 1)
                {
                    _sections[0].Steps.Insert(0, _sections[i].Steps[j]);
                    _sections[_sections.Count() - 1].Steps.RemoveAt(_sections[_sections.Count() - 1].Steps.Count() - 1);
                    _sections[_sections.Count() - 1].Steps[_sections[_sections.Count() - 1].Steps.Count() - 1].IsSelected = false;
                    _sections[0].Steps[0].IsSelected = true;

                    for (int k = 0; k < _sections[0].Steps.Count(); k++)
                    {
                        _sections[0].Steps[k].StepID = k + 1;
                    }
                }
                else
                {
                    int id1 = _sections[i].Steps[j].StepID;
                    int id2 = _sections[i].Steps[j + 1].StepID;
                    tmp = _sections[i].Steps[j];
                    _sections[i].Steps[j] = _sections[i].Steps[j + 1];
                    _sections[i].Steps[j].IsSelected = false;
                    _sections[i].Steps[j + 1] = tmp;
                    _sections[i].Steps[j + 1].StepID = id2;
                    _sections[i].Steps[j].StepID = id1;
                }
                //selectPreviousStep();
            }
            else { MessageBox.Show("No test step selected"); }
        }


        public ICommand MoveSectionDownCommand
        {
            get
            {
                if(_MoveSectionDown == null)
                {
                    _MoveSectionDown = new RelayCommand(p => this.MoveSectionDown());
                }
                return _MoveSectionDown;
            }
        }

        private void MoveSectionDown()
        {
            int i = 0;
            bool sectionSelected = false;
            for (i = 0; i < _sections.Count(); i++)
            {
                if (_sections[i].IsSelected)
                {
                    sectionSelected = true;
                    break;
                }
            }
            if (!sectionSelected)
            {
                MessageBox.Show("No section is selected");
            }
            else
            {
                if (i == _sections.Count() - 1)
                {
                    int id1 = _sections[i].SectionID;
                    int id2 = _sections[0].SectionID;
                    EditorSectionsViewModel tmp = _sections[i];
                    _sections[i] = _sections[0];
                    _sections[0] = tmp;
                    _sections[i].SectionID = id1;
                    _sections[0].SectionID = id2;
                }
                else
                {
                    int id1 = _sections[i].SectionID;
                    int id2 = _sections[i + 1].SectionID;
                    EditorSectionsViewModel tmp = _sections[i];
                    _sections[i] = _sections[i + 1];
                    _sections[i + 1] = tmp;
                    _sections[i].SectionID = id1;
                    _sections[i + 1].SectionID = id2;
                }
            }
        }

        public ICommand MoveSectionUpCommand
        {
            get
            {
                if (_MoveSectionUp == null)
                {
                    _MoveSectionUp = new RelayCommand(p => this.MoveSectionUp());
                }
                return _MoveSectionUp;
            }
        }

        private void MoveSectionUp()
        {
            int i = 0;
            bool sectionSelected = false;
            for (i = 0; i < _sections.Count(); i++)
            {
                if (_sections[i].IsSelected)
                {
                    sectionSelected = true;
                    break;
                }
            }
            if (!sectionSelected)
            {
                MessageBox.Show("No section is selected");
            }
            else
            {
                if (i == 0)
                {
                    int id1 = _sections[i].SectionID;
                    int id2 = _sections[_sections.Count()].SectionID;
                    EditorSectionsViewModel tmp = _sections[i];
                    _sections[i] = _sections[_sections.Count()];
                    _sections[_sections.Count()] = tmp;
                    _sections[i].SectionID = id1;
                    _sections[_sections.Count()].SectionID = id2;
                }
                else
                {
                    int id1 = _sections[i].SectionID;
                    int id2 = _sections[i - 1].SectionID;
                    EditorSectionsViewModel tmp = _sections[i];
                    _sections[i] = _sections[i - 1];
                    _sections[i - 1] = tmp;
                    _sections[i].SectionID = id1;
                    _sections[i - 1].SectionID = id2;
                }
            }
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
                    /*
                    XmlNode procedureHeading = root.SelectSingleNode("Procedure_Heading");
                    procedureHeading.SelectSingleNode("Name").InnerText = Name;
                    procedureHeading.SelectSingleNode("Date").InnerText = _date;
                    procedureHeading.SelectSingleNode("Software_Load_Version").InnerText = SoftwareLoadVersion;
                    procedureHeading.SelectSingleNode("Program_Phase").InnerText = ProgramPhase;
                    procedureHeading.SelectSingleNode("Program_Type").InnerText = ProgramType;
                    procedureHeading.SelectSingleNode("Classification").InnerText = Classification;
                    procedureHeading.SelectSingleNode("Start_Time").InnerText = _timeStarted;
                    procedureHeading.SelectSingleNode("Stop_Time").InnerText = DateTime.Now.ToString("HH:mm");
                    */
                    int i = 0, j = 0;
                    // Fill Test Steps
                    var sectionsList = root.SelectNodes("Sections/Section");
                    foreach (XmlNode sectionNode in sectionsList)
                    {
                        var testStepsList = sectionNode.SelectNodes("Test_Step");
                        j = 0;
                        foreach (XmlNode testStepNode in testStepsList)
                        {
                            EditorStepViewModel testStep = _sections[i].Steps[j];
                                testStepNode.SelectSingleNode("Station_Redline").InnerText = testStep.Station;
                                testStepNode.SelectSingleNode("Control_Action_Redline").InnerText = testStep.ControlAction;
                                testStepNode.SelectSingleNode("Expected_Result_Redline").InnerText = testStep.ExpectedResult;

                            XmlNode resultNode = testStepNode.SelectSingleNode("Result");


                            testStepNode.SelectSingleNode("Comment").InnerText = "";

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

            var sectionsList = root.SelectNodes("Sections/Section");
            foreach (XmlNode sectionNode in sectionsList)
            {
                EditorSectionsViewModel newSection = new EditorSectionsViewModel(Int32.Parse(sectionNode.Attributes.GetNamedItem("id").Value), sectionNode.SelectSingleNode("Heading").InnerText);
                newSection.Description = sectionNode.SelectSingleNode("Description").InnerText;


                var steps = new ObservableCollection<EditorStepViewModel>();
                var testStepsList = sectionNode.SelectNodes("Test_Step");
                foreach (XmlNode testStepNode in testStepsList)
                {
                    int stepID = Int32.Parse(testStepNode.Attributes.GetNamedItem("id").Value);
                    string station = testStepNode.SelectSingleNode("Station").InnerText;
                    string controlAction = testStepNode.SelectSingleNode("Control_Action").InnerText;
                    string expectedResult = testStepNode.SelectSingleNode("Expected_Result").InnerText;
                    string rControl = "";
                    string rExp = "";
                    string rStation = "";


                    XmlNode result = testStepNode.SelectSingleNode("Result");

                    if (!String.IsNullOrEmpty(testStepNode.SelectSingleNode("Station_Redline").InnerText))
                        rStation = testStepNode.SelectSingleNode("Station_Redline").InnerText;

                    if (!String.IsNullOrEmpty(testStepNode.SelectSingleNode("Control_Action_Redline").InnerText))
                        rControl = testStepNode.SelectSingleNode("Control_Action_Redline").InnerText;

                    if (!String.IsNullOrEmpty(testStepNode.SelectSingleNode("Expected_Result_Redline").InnerText))
                        rExp = testStepNode.SelectSingleNode("Expected_Result_Redline").InnerText;
                    EditorStepViewModel newStep = new EditorStepViewModel(stepID, station, controlAction, expectedResult, rStation, rExp, rControl);
                    newStep.Parent = newSection;
                    addEventHandlerToStep(newStep);
                    steps.Add(newStep);
                }

                newSection.Steps = steps;
                _sections.Add(newSection);
            }
        }

        private void addEventHandlerToStep(EditorStepViewModel newStep)
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
                for (j = 0; j < _sections[i].Steps.Count(); j++)
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


        #endregion
    }


    #endregion
}