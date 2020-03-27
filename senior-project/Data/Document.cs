using System;
using System.Windows;
using System.Windows.Input;
using System.Xml;

namespace senior_project
{
	public class XmlObject
	{
        private XmlDocument _xml;

        #region Constructor

		/// <summary>
        /// Constructor for the xmlDocument
        /// </summary>
        public XmlObject()
		{

		}

        #endregion

        #region Public Members

        /// <summary>
        /// Gets/Sets the xmlDocument
        /// </summary>
        public XmlDocument XmlFile { get { return _xml; } set { _xml = value ?? _xml; } }

        #endregion

        #region Public Functions

        /// <summary>
        /// Loads the XML specified by the path
        /// </summary>
        /// <param name="FullPath">The path to the xml file</param>
        public void LoadXML(string FullPath)
        {
            if (string.IsNullOrEmpty(FullPath))
            {
                throw new ArgumentException("message", nameof(FullPath));
            }
            _xml.LoadXml(FullPath);
            
        }

        #endregion

        public class DelegateCommand : ICommand
        {
            private readonly Action<object> _execute;
            private readonly Predicate<object> _canExecute;

            public DelegateCommand(Action<object> execute, Predicate<object> canExecute = null)
            {
                _execute = execute ?? throw new ArgumentNullException(nameof(execute));
                _canExecute = canExecute;
            }

            public bool CanExecute(object parameter) => _canExecute?.Invoke(parameter) ?? true;
            public void Execute(object parameter) => _execute(parameter);
            public void RaiseCanExecuteChanged() => CanExecuteChanged?.Invoke(this, EventArgs.Empty);

            public event EventHandler CanExecuteChanged;
        }
    }
}