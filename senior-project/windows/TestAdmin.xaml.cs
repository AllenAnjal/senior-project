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
    public partial class TestAdmin : Window
    {
        private MainWindow main;

        public TestAdmin(MainWindow mw) : this(mw, String.Empty) { }
        public TestAdmin(MainWindow mw, String xmlPath)
        {
            InitializeComponent();
            main = mw;

            IDialogService dialogService = new DialogService(mw);
            dialogService.Register<editSectionDialogViewModel, editSectionDialog>();
            XmlDocument xml = new XmlDocument();

            if (!(String.IsNullOrEmpty(xmlPath)))
            {
                try
                {
                    xml.Load(xmlPath);
                }
                catch (Exception err)
                {
                    throw new ArgumentException(err.ToString());
                }
                EditorViewModel test = new EditorViewModel(xml, dialogService);
                this.DataContext = test;
            } 
            else
            {
                EditorViewModel test = new EditorViewModel(dialogService);
                this.DataContext = test;
            }

        }

        private void WindowClosed(object sender, EventArgs e)
        {
            main.Show();
        }


        private void Exit_Button(object sender, RoutedEventArgs e)
        {
            this.Close();
        }


    }

    #region Converters

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
            if (value == null)
                return String.Empty;
            EditorStepViewModel testStep = value as EditorStepViewModel;
            return "Section: " + testStep.Parent.SectionID.ToString() + ", Step: " + testStep.StepID.ToString();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class EditorSectionToFullLabelConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return String.Empty;
            EditorSectionsViewModel section = value as EditorSectionsViewModel;
            return "Section: " + section.SectionID.ToString();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class GridToTextboxConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            double width = (double)value;
            return (width * .5) - 10;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class RedlineToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (String.IsNullOrEmpty((string)value)) ? "Collapsed" : "Visible";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    #endregion

    #region MVVM Implementation

    public class EditorBaseViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));


        public event EventHandler<EditorSelectedItemUpdateEventArgs> SelectedItemUpdated;
        protected void OnSelectedItemChanged(EditorStepViewModel testStep)
            => SelectedItemUpdated?.Invoke(this, new EditorSelectedItemUpdateEventArgs(testStep));

        public event EventHandler<EditorSelectedSectionUpdateEventArgs> SelectedSectionUpdated;
        protected void OnSelectedSectionChanged(EditorSectionsViewModel section)
            => SelectedSectionUpdated?.Invoke(this, new EditorSelectedSectionUpdateEventArgs(section));
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

    public class EditorSelectedSectionUpdateEventArgs : EventArgs
    {
        private readonly EditorSectionsViewModel _selectedSection;

        public EditorSelectedSectionUpdateEventArgs(EditorSectionsViewModel selectedSection)
        {
            _selectedSection = selectedSection;
        }

        public EditorSectionsViewModel SelectedSection
        {
            get { return _selectedSection; }
        }
    }

    public class EditorStepViewModel : EditorBaseViewModel
    {
        #region Private Members

        private int _stepID;

        private string _station;
        private string _stationRedline;

        private string _controlAction;
        private string _controlActionRedline;

        private string _expectedResult;
        private string _expectedResultRedline;

        private bool _isSelected;

        #endregion

        #region Constructors

        public EditorStepViewModel(int stepID, string station, string controlAction, string expectedResult, string redlineStation, string redlineControlAction, string redlineExpectedResult)
        {
            _stepID = stepID;

            _station = station;
            _stationRedline = redlineStation;

            _controlAction = controlAction;
            _controlActionRedline = redlineControlAction;

            _expectedResult = expectedResult;
            _expectedResultRedline = redlineExpectedResult;
        }

        public EditorStepViewModel(int stepID) : this(stepID, String.Empty, String.Empty, String.Empty, String.Empty, String.Empty, String.Empty) { }

        #endregion

        #region Public Members

        public EditorSectionsViewModel Parent { get; set; }

        public Object ItSelf
        {
            get { return this; }
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

        public string StationRedline
        {
            get { return _stationRedline; }
            set
            {
                if (!value.Equals(_stationRedline))
                {
                    _stationRedline = value;
                    OnPropertyChanged("StationRedline");
                }
            }
        }
        public string ControlActionRedline
        {
            get { return _controlActionRedline; }
            set
            {
                if (!value.Equals(_controlActionRedline))
                {
                    _controlActionRedline = value;
                    OnPropertyChanged("ControlActionRedline");
                }
            }
        }
        public string ExpectedResultRedline
        {
            get { return _expectedResultRedline; }
            set
            {
                if (!value.Equals(_expectedResultRedline))
                {
                    _expectedResultRedline = value;
                    OnPropertyChanged("ExpectedResultRedline");
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
                    if (_isSelected)
                    {
                        OnSelectedItemChanged(this);
                        OnPropertyChanged("IsSelected");
                    } 
                    else
                    {
                        OnSelectedItemChanged(null);
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

        public EditorSectionsViewModel(int sectionID)
        {
            _sectionID = sectionID;
            _steps = new ObservableCollection<EditorStepViewModel>();
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
                    if (_isSelected)
                    {
                        OnSelectedSectionChanged(this);
                        OnPropertyChanged("IsSelected");
                    }
                    else
                    {
                        OnSelectedSectionChanged(null);
                    }
                }
            }
        }
    }

    public class EditorViewModel : EditorBaseViewModel
    {
        #region Private Members

        private ObservableCollection<EditorSectionsViewModel> _sections;
        private EditorStepViewModel _selectedStep;
        private EditorSectionsViewModel _selectedSection;
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

            _sections[0].IsSelected = true;
        }

        public EditorViewModel(IDialogService dialogService)
        {
            _dialogService = dialogService;
            _sections = new ObservableCollection<EditorSectionsViewModel>();
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

        public EditorSectionsViewModel SelectedSection
        {
            get { return _selectedSection; }
            set
            {
                if (_selectedSection != value)
                {
                    _selectedSection = value;
                    OnPropertyChanged("SelectedSection");
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

        public void SectionSelectedChanged(object sender, EditorSelectedSectionUpdateEventArgs e)
        {
            this.SelectedSection = e.SelectedSection;
            OnPropertyChanged("SelectedStep");
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
        private ICommand _discardAll;

        #region redline stuff
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
            _sections[i].Steps[j].Station = _sections[i].Steps[j].StationRedline;
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
            _sections[i].Steps[j].ExpectedResult = _sections[i].Steps[j].ExpectedResultRedline;
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
            _sections[i].Steps[j].ControlAction = _sections[i].Steps[j].ControlActionRedline;
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
            _sections[i].Steps[j].Station += "\nRedline Changes: " + _sections[i].Steps[j].StationRedline;
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
            _sections[i].Steps[j].ExpectedResult += "\nRedline Changes: " + _sections[i].Steps[j].ExpectedResultRedline;
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
            _sections[i].Steps[j].ControlAction += "\nRedline Changes: " + _sections[i].Steps[j].ControlActionRedline;
        }

        #endregion

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
            // TO DO - REMOVE EVENT HANDLER
            if (SelectedStep != null)
            {
                EditorStepViewModel temp = SelectedStep;
                int i = 0;
                for (; i < temp.Parent.Steps.Count(); i++)
                {
                    if (temp.Parent.Steps[i].IsSelected)
                    {
                        break;
                    }
                }
                temp.Parent.Steps.RemoveAt(i);

                if (i < temp.Parent.Steps.Count())
                {
                    temp.IsSelected = false;
                    temp.Parent.Steps[i].IsSelected = true;
                } 
                else
                {
                    if (temp.Parent.Steps.Count() == 0)
                    {
                        temp.IsSelected = false;
                        temp.Parent.IsSelected = true;
                    }
                    else
                    {
                        temp.IsSelected = false;
                        temp.Parent.Steps.Last().IsSelected = true;
                    }
                }

                for (; i < temp.Parent.Steps.Count(); i++)
                {
                    temp.Parent.Steps[i].StepID--;
                }

                removeEventHandlerFromStep(temp);
            } 
            else if (SelectedSection != null)
            {
                EditorSectionsViewModel temp = SelectedSection;
                int i = 0;
                for (; i < _sections.Count(); i++)
                {
                    if (_sections[i].IsSelected)
                    {
                        break;
                    }
                }
                Sections.RemoveAt(i);

                if (i < _sections.Count())
                {
                    temp.IsSelected = false;
                    _sections[0].IsSelected = false;
                    _sections[i].IsSelected = true;
                }
                else
                {
                    if (_sections.Count() == 0)
                    {
                        temp.IsSelected = false;
                    }
                    else
                    {
                        temp.IsSelected = false;
                        _sections[0].IsSelected = false;
                        _sections.Last().IsSelected = true;
                    }
                }

                for (; i < _sections.Count(); i++)
                {
                    _sections[i].SectionID--;
                }

                removeEventHandlerFromSection(temp);
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
            if (SelectedStep != null)
            {
                int i;
                for (i = 0; i < SelectedStep.Parent.Steps.Count(); i++)
                {
                    if (SelectedStep.Parent.Steps[i].IsSelected)
                    {
                        break;
                    }
                }
                EditorStepViewModel newStep = new EditorStepViewModel(i + 2);
                addEventHandlerToStep(newStep);
                newStep.Parent = SelectedStep.Parent;

                SelectedStep.Parent.Steps.Insert(i + 1, newStep);
                
                for (i += 2; i < SelectedStep.Parent.Steps.Count(); i++)
                {
                    SelectedStep.Parent.Steps[i].StepID++;
                }
                SelectedStep.IsSelected = false;
                newStep.IsSelected = true;
            }
            else if (SelectedSection != null)
            {
                EditorStepViewModel newStep = new EditorStepViewModel(1);
                addEventHandlerToStep(newStep);
                SelectedSection.Steps.Insert(0, newStep);
                newStep.Parent = SelectedSection;

                for (int i = 1; i < SelectedSection.Steps.Count(); i++)
                {
                    SelectedSection.Steps[i].StepID++;
                }
                SelectedSection.Steps[0].IsSelected = true;
            }
            else
            {
                MessageBox.Show("Insert location not selected.");
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
            int i, j;
            bool found = false;
            for (i = 0; i < _sections.Count(); i++)
            {
                if (_sections[i].IsSelected)
                {
                    found = true;
                    break;
                }
                for (j = 0; j < _sections[i].Steps.Count(); j++)
                {
                    if (_sections[i].Steps[j].IsSelected)
                    {
                        found = true;
                        break;
                    }
                }
                if (found)
                    break;
            }

            if (found)
            {
                EditorSectionsViewModel newSection = getNewSection(i + 2);
                if (newSection == null)
                    return;

                _sections.Insert(i + 1, newSection);
                _sections[i].IsSelected = false;
                _sections[i + 1].IsSelected = true;
                for (i += 2; i < _sections.Count(); i++)
                {
                    _sections[i].SectionID++;
                }
            } 
            else
            {
                EditorSectionsViewModel newSection = getNewSection(_sections.Count() + 1);
                if (newSection == null)
                    return;

                _sections.Add(newSection);
                _sections.Last().IsSelected = true;
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
            if (SelectedSection != null)
            {
                editSection(SelectedSection);
            } 
            else
            {
                int i, j;
                bool found = false;
                for (i = 0; i < _sections.Count(); i++)
                {
                    if (_sections[i].IsSelected)
                    {
                        found = true;
                        break;
                    }
                    for (j = 0; j < _sections[i].Steps.Count(); j++)
                    {
                        if (_sections[i].Steps[j].IsSelected)
                        {
                            found = true;
                            break;
                        }
                    }
                    if (found)
                        break;
                }

                if (found)
                {
                    editSection(_sections[i]);
                }
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
            if (SelectedStep != null)
            {
                int i = 0, j = 0;
                bool found = false;
                for (; i < _sections.Count(); i++)
                {
                    for (j = 0; j < _sections[i].Steps.Count(); j++)
                    {
                        if (_sections[i].Steps[j].IsSelected)
                        {
                            found = true;
                            break;
                        }
                    }
                    if (found)
                        break;
                }

                if (j == 0)
                {
                    if (i == 0)
                    {
                        return;
                    } 
                    else
                    {
                        EditorStepViewModel temp = SelectedStep;
                        _sections[i].Steps.RemoveAt(j);
                        for (;j < _sections[i].Steps.Count(); j++)
                        {
                            _sections[i].Steps[j].StepID--;
                        }

                        temp.Parent = _sections[i-1];
                        temp.StepID = _sections[i-1].Steps.Count() + 1;
                        _sections[i-1].Steps.Add(temp);
                    }
                }
                else 
                {
                    EditorStepViewModel temp = SelectedStep;
                    _sections[i].Steps[j] = _sections[i].Steps[j - 1];
                    _sections[i].Steps[j].StepID = temp.StepID;
                    temp.StepID--;
                    _sections[i].Steps[j - 1] = temp; 
                }
            }
            else if (SelectedSection != null)
            {
                int i = 0;
                for (; i < _sections.Count(); i++)
                {
                    if (_sections[i].IsSelected)
                        break;
                }

                if (i == 0)
                {
                    return;
                }
                else
                {
                    EditorSectionsViewModel temp = SelectedSection;
                    _sections[i] = _sections[i - 1];
                    _sections[i].SectionID = temp.SectionID;
                    temp.SectionID--;
                    _sections[i - 1] = temp;
                    OnPropertyChanged("SelectedSection");
                }
            }
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
            if (SelectedStep != null)
            {
                int i = 0, j = 0;
                bool found = false;
                for (; i < _sections.Count(); i++)
                {
                    for (j = 0; j < _sections[i].Steps.Count(); j++)
                    {
                        if (_sections[i].Steps[j].IsSelected)
                        {
                            found = true;
                            break;
                        }
                    }
                    if (found)
                        break;
                }

                if (_sections[i].Steps[j] == _sections[i].Steps.Last())
                {
                    if (i == _sections.Count() - 1)
                    {
                        return;
                    }
                    else
                    {
                        EditorStepViewModel temp = SelectedStep;
                        _sections[i].Steps.RemoveAt(j);

                        temp.Parent = _sections[i + 1];
                        temp.StepID = 1;
                        _sections[i + 1].Steps.Insert(0, temp);

                        for (j = 1; j < _sections[i+1].Steps.Count(); j++)
                        {
                            _sections[i+1].Steps[j].StepID++;
                        }
                    }
                }
                else
                {
                    EditorStepViewModel temp = SelectedStep;
                    _sections[i].Steps[j] = _sections[i].Steps[j + 1];
                    _sections[i].Steps[j].StepID = temp.StepID;
                    temp.StepID++;
                    _sections[i].Steps[j + 1] = temp;
                }
            }
            else if (SelectedSection != null)
            {
                int i = 0;
                for (; i < _sections.Count(); i++)
                {
                    if (_sections[i].IsSelected)
                        break;
                }

                if (_sections[i] == _sections.Last())
                {
                    return;
                } 
                else
                {
                    EditorSectionsViewModel temp = SelectedSection;
                    _sections[i] = _sections[i + 1];
                    _sections[i].SectionID = temp.SectionID;
                    temp.SectionID++;
                    _sections[i + 1] = temp;
                    OnPropertyChanged("SelectedSection");
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

        private void loadXML(XmlDocument xmlDocument)
        {
            XmlNode root = xmlDocument.SelectSingleNode("Test_Procedure");

            XmlNode heading = root.SelectSingleNode("Procedure_Heading");

            var sectionsList = root.SelectNodes("Sections/Section");
            foreach (XmlNode sectionNode in sectionsList)
            {
                EditorSectionsViewModel newSection = new EditorSectionsViewModel(Int32.Parse(sectionNode.Attributes.GetNamedItem("id").Value));
                newSection.SectionHeading = sectionNode.SelectSingleNode("Heading").InnerText;
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

                    EditorStepViewModel newStep = new EditorStepViewModel(stepID, station, controlAction, expectedResult, rStation, rControl, rExp);
                    newStep.Parent = newSection;
                    addEventHandlerToStep(newStep);
                    steps.Add(newStep);
                }

                newSection.Steps = steps;
                addEventHandlerToSection(newSection);
                _sections.Add(newSection);
            }
        }

        private void addEventHandlerToStep(EditorStepViewModel newStep)
        {
            newStep.SelectedItemUpdated += StepSelectedChanged;
            newStep.PropertyChanged += StepSelectedValueChanged;
        }
        
        private void removeEventHandlerFromStep(EditorStepViewModel step)
        {
            step.SelectedItemUpdated -= StepSelectedChanged;
            step.PropertyChanged -= StepSelectedValueChanged;
        }

        private void addEventHandlerToSection(EditorSectionsViewModel newSection)
        {
            newSection.SelectedSectionUpdated += SectionSelectedChanged;
        }

        private void removeEventHandlerFromSection(EditorSectionsViewModel section)
        {
            section.SelectedSectionUpdated -= SectionSelectedChanged;
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

        private EditorSectionsViewModel getNewSection(int sectionID)
        {
            EditorSectionsViewModel newSection = new EditorSectionsViewModel(sectionID);
            editSectionDialogViewModel newSectionDialog = new editSectionDialogViewModel(newSection);
            bool? result = _dialogService.ShowDialog(newSectionDialog);
            if (result == true)
            {
                addEventHandlerToSection(newSection);
                newSection.SectionHeading = newSectionDialog.Name;
                newSection.Description = newSectionDialog.Description;
                return newSection;
            }
            else
            {
                return null;
            }
        }

        private void editSection(EditorSectionsViewModel section)
        {
            editSectionDialogViewModel newSectionDialog = new editSectionDialogViewModel(section);
            bool? result = _dialogService.ShowDialog(newSectionDialog);
            if (result == true)
            {
                section.SectionHeading = newSectionDialog.Name;
                section.Description = newSectionDialog.Description;
            }
            else
            {
                return;
            }
        }

        #endregion
    }


    #endregion
}