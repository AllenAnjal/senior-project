using System;
using System.Windows;
using System.Windows.Input;
using System.ComponentModel;

namespace senior_project
{
    /// <summary>
    /// Interaction logic for userInfoPage.xaml
    /// </summary>
    public partial class userInfoPage : Window, IDialog
    {
        public userInfoPage()
        {
            InitializeComponent();
        }
    }

    public class UserInfoDialogViewModel : BaseViewModel, IDialogRequestClose
    {
        #region Class Members

        private string _name;
        private string _softwareLoadVersion;
        private string _programPhase;
        private string _programType;
        private string _classification;

        public string Name
        {
            get { return _name; }
            set
            {
                if (!(_name.Equals(value)))
                {
                    _name = value;
                    OnPropertyChanged("Name");
                }
            }
        }
        public string SoftwareLoadVersion
        {
            get { return _softwareLoadVersion; }
            set
            {
                if (!(_softwareLoadVersion.Equals(value)))
                {
                    _softwareLoadVersion = value;
                    OnPropertyChanged("SoftwareLoadVersion");
                }
            }
        }
        public string ProgramPhase
        {
            get { return _programPhase; }
            set
            {
                if (!(_programPhase.Equals(value)))
                {
                    _programPhase = value;
                    OnPropertyChanged("ProgramPhase");
                }
            }
        }
        public string ProgramType
        {
            get { return _programType; }
            set
            {
                if (!(_programType.Equals(value)))
                {
                    _programType = value;
                    OnPropertyChanged("ProgramType");
                }
            }
        }
        public string Classification
        {
            get { return _classification; }
            set
            {
                if (!(_classification.Equals(value)))
                {
                    _classification = value;
                    OnPropertyChanged("Classification");
                }
            }
        }

        #endregion

        public UserInfoDialogViewModel(MainTesterViewModel procedure)
        {
            _name = procedure.Name ?? String.Empty;
            _softwareLoadVersion = procedure.SoftwareLoadVersion ?? String.Empty;
            _programPhase = procedure.ProgramPhase ?? String.Empty;
            _programType = procedure.ProgramType ?? String.Empty;
            _classification = procedure.Classification ?? String.Empty;

            SubmitCommand = new RelayCommand(p => CloseRequested?.Invoke(this, new DialogCloseRequestedEventArgs(true)));
            CancelCommand = new RelayCommand(p => CloseRequested?.Invoke(this, new DialogCloseRequestedEventArgs(false)));
        }

        public event EventHandler<DialogCloseRequestedEventArgs> CloseRequested;

        public ICommand SubmitCommand { get; }
        public ICommand CancelCommand { get; }
    }
}