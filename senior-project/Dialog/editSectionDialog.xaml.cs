using System;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;

namespace senior_project
{
    public partial class editSectionDialog : Window, IDialog
    {
        public editSectionDialog()
        {
            InitializeComponent();
        }
    }

    public class editSectionDialogViewModel : BaseViewModel, IDialogRequestClose
    {
        private string _name;
        private string _description;

        public string Name
        {
            get { return _name; }
            set
            {
                if (!(value.Equals(_name)))
                {
                    _name = value;
                    OnPropertyChanged("Name");
                }
            }
        }
        public string Description
        {
            get { return _description; }
            set
            {
                if (!(value.Equals(_description)))
                {
                    _description = value;
                    OnPropertyChanged("Description");
                }
            }
        }

        public editSectionDialogViewModel(EditorSectionsViewModel section)
        {
            _description = section.Description;
            _name = section.SectionHeading;
            SubmitCommand = new RelayCommand(p => CloseRequested?.Invoke(this, new DialogCloseRequestedEventArgs(true)));
            CancelCommand = new RelayCommand(p => CloseRequested?.Invoke(this, new DialogCloseRequestedEventArgs(false)));
        }

        public event EventHandler<DialogCloseRequestedEventArgs> CloseRequested;
        public ICommand SubmitCommand { get; }
        public ICommand CancelCommand { get; }
    }
}
