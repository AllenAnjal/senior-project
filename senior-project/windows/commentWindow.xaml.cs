using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace senior_project
{
    /// <summary>
    /// Interaction logic for commentWindow.xaml
    /// </summary>
    public partial class commentWindow : Window
    {
        String defaultText = "Leave a comment";
        TestProcedureSectionTest_Step newStep;
        public commentWindow(ref TestProcedureSectionTest_Step step)
        {
            InitializeComponent();
            commentBox.Text = defaultText;
            newStep = step;
        }

        private void submitAction()
        {
            if (commentBox.Text == defaultText)
            {
                MessageBox.Show("Must leave a comment");
            }
            else
            {
                newStep.Comments = commentBox.Text;
                this.Close();
            }
        }

        private void SubmitButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                submitAction();
            }
            catch(Exception ex)
            {
                MessageBox.Show($"Exception Handled: {ex.Message}");
            }
        }

        private void CommentBox_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            XmlVerification.textBoxClear(ref commentBox, defaultText);
        }
    }
}
