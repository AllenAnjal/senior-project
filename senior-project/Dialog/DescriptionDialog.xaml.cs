using System;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;

namespace senior_project
{
    public partial class DescriptionDialog : Window, IDialog
    {
        public DescriptionDialog()
        {
            InitializeComponent();
        }
    }



    public class DescriptionDialogViewModel : BaseViewModel, IDialogRequestClose
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


        public DescriptionDialogViewModel()
        {

            SubmitCommand = new RelayCommand(p => CloseRequested?.Invoke(this, new DialogCloseRequestedEventArgs(true)));
            CancelCommand = new RelayCommand(p => CloseRequested?.Invoke(this, new DialogCloseRequestedEventArgs(false)));
        }

        public event EventHandler<DialogCloseRequestedEventArgs> CloseRequested;
        public ICommand SubmitCommand { get; }
        public ICommand CancelCommand { get; }
    }
}
