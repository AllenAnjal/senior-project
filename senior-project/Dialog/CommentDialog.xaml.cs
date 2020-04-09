using System;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;

namespace senior_project
{
    public partial class CommentDialog : Window, IDialog
    {
        public CommentDialog()
        {
            InitializeComponent();
        }
    }

    public class BoolToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool? visible = value as bool?;
            if (visible == true)
            {
                return "Visible";
            } 
            else
            {
                return "Hidden";
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class CommentDialogViewModel : BaseViewModel, IDialogRequestClose
    {
        private string _comment;
        private string _severity;
        
        public ObservableCollection<string> severities { get; set; }

        public bool SeverityNeeded { get; set; }
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
        public string Severity
        {
            get { return _severity; }
            set
            {
                if (!(value.Equals(_severity)))
                {
                    _severity = value;
                    OnPropertyChanged("Severity");
                }
            }
        }

        public CommentDialogViewModel(TestStepViewModel testStep, bool severityNeeded)
        {
            SeverityNeeded = severityNeeded;
            if (severityNeeded)
            {
                _comment = testStep.ResultComment;
                _severity = testStep.Severity;
            } 
            else
            {
                _comment = testStep.Comment;
            }

            severities = new ObservableCollection<string>()
            {
                "Mild", 
                "Medium", 
                "Critical"
            };

            SubmitCommand = new RelayCommand(p => CloseRequested?.Invoke(this, new DialogCloseRequestedEventArgs(true)));
            CancelCommand = new RelayCommand(p => CloseRequested?.Invoke(this, new DialogCloseRequestedEventArgs(false)));
        }

        public event EventHandler<DialogCloseRequestedEventArgs> CloseRequested;
        public ICommand SubmitCommand { get; }
        public ICommand CancelCommand { get; }
    }
}
