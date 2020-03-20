using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using System.Xml;
using System.Xml.Schema;
using System.IO;
using Microsoft.Win32;
using System.Reflection;
using System.Xml.Serialization;

namespace senior_project
{
    /// <summary>
    /// Interaction logic for Redlines.xaml
    /// </summary>
    public partial class Redlines : Window
    {
        #region Initilization

        private MainWindow mw = new MainWindow();

        private TestAdmin testAdmin;
        private string redlineTypeLoaded;

        public Redlines(bool createNewTP)
        {
            InitializeComponent();

            testAdmin = new TestAdmin(mw, "tmp.xml"); //creates the test admin object with the pre-existing test procedure
        }

        #endregion Initilization
    }
}