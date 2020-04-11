using System;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;

namespace senior_project
{
    public partial class renameDialog : Window, IDialog
    {
        public renameDialog()
        {
            InitializeComponent();
        }
    }



    public class renameDialogViewModel : BaseViewModel, IDialogRequestClose
    {
        private string _comment;

        public string Comment
        {
            get { return _comment; }
            set
            {
                if (!(value.Equals(_comment)))
                {
                    _comment = value;
                    OnPropertyChanged("Comment");
                }
            }
        }


        public renameDialogViewModel()
        {

            SubmitCommand = new RelayCommand(p => CloseRequested?.Invoke(this, new DialogCloseRequestedEventArgs(true)));
            CancelCommand = new RelayCommand(p => CloseRequested?.Invoke(this, new DialogCloseRequestedEventArgs(false)));
        }

        public event EventHandler<DialogCloseRequestedEventArgs> CloseRequested;
        public ICommand SubmitCommand { get; }
        public ICommand CancelCommand { get; }
    }
}
