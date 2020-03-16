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
    /// Interaction logic for exportWindow.xaml
    /// </summary>
    public partial class exportWindow : Window
    {
        private bool docxToggle = false;
        private bool xmlToggle = false;
        private bool csvToggle = false;
        public exportWindow()
        {
            InitializeComponent();
        }

        private void folder_btn(object sender, RoutedEventArgs e)
        {

        }
        private void Submit_btn(object sender, RoutedEventArgs e)
        {
            //export stuff
            this.Close();
        }
        private void docx_btn(object sender, RoutedEventArgs e)
        {
            docxToggle = !docxToggle;
            if (docxToggle)
            {
                doxcBorder.BorderBrush = new SolidColorBrush(Color.FromRgb(0, 255, 0));
            }
            else
            {
                doxcBorder.BorderBrush = new SolidColorBrush(Color.FromRgb(255, 255, 255));
            }
        }
        private void xml_btn(object sender, RoutedEventArgs e)
        {
            xmlToggle = !xmlToggle;
            if (xmlToggle)
            {
                xmlBorder.BorderBrush = new SolidColorBrush(Color.FromRgb(0, 255, 0));
            }
            else
            {
                xmlBorder.BorderBrush = new SolidColorBrush(Color.FromRgb(255, 255, 255));
            }
        }
        private void csv_btn(object sender, RoutedEventArgs e)
        {
            csvToggle = !csvToggle;
            if (csvToggle)
            {
                csvBorder.BorderBrush = new SolidColorBrush(Color.FromRgb(0, 255, 0));
            }
            else
            {
                csvBorder.BorderBrush = new SolidColorBrush(Color.FromRgb(255, 255, 255));
            }
        }
    }
}
