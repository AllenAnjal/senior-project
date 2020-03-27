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
        private String defaultText = "Leave a comment";

        public commentWindow()
        {
            InitializeComponent();
            commentBox.Text = defaultText;
        }


        private void submitAction()
        {
            if (commentBox.Text == defaultText)
            {
                MessageBox.Show("Must leave a comment");
            }
            else
            {

                this.Hide();
            }
        }

        private void SubmitButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                submitAction();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Exception Handled: {ex.Message}");
            }
        }

        private void commentBox_GotFocus(object sender, RoutedEventArgs e)
        {
            commentBox.Text = string.Empty;
        }

        private void Window_ContentRendered(object sender, EventArgs e)
        {
            commentBox.Text = defaultText;
        }

        bool _shown;
        protected override void OnContentRendered(EventArgs e)
        {
            base.OnContentRendered(e);
            if (_shown)
                return;
            _shown = true;

            commentBox.Text = defaultText;
        }
    }
}