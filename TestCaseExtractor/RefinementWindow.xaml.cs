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
using TestCaseExtractor.DataModel;
using TestCaseExtractor.ViewModel.CheckBoxTree;

namespace TestCaseExtractor
{
    /// <summary>
    /// Interaction logic for RefinementWindow.xaml
    /// </summary>
    public partial class RefinementWindow : Window
    {
        private IDataModel _itemDataModel;
        private System.Collections.Generic.List<ItemViewModel> _treeData;
        private CommandBinding _commandBinding;
        internal TreeView tree;
        private string _path;
        public static bool createComments;
        public static bool includeDescription;
        internal CheckBox ChkComments;
        internal CheckBox ChkDescription;

        public RefinementWindow(IDataModel itemDataModel, string path)
        {
            InitializeComponent();
            this._itemDataModel = itemDataModel;
            this._path = path;
            base.Closed += new System.EventHandler(this.RefinementWindow_Closed);
            MainWindow._asyncTasks.Execute(new Action(this.Start), new Action(this.End));
        }

        private void Start()
        {
            this._treeData = RootViewModel.ConstructTreeFromDataModel(this._itemDataModel);
        }

        private void End()
        {
            this.tree.ItemsSource = this._treeData;
            ItemViewModel root = this.tree.Items[0] as ItemViewModel;
            this._commandBinding = new CommandBinding(ApplicationCommands.Undo, delegate(object sender, ExecutedRoutedEventArgs e)
            {
                e.Handled = true;
                RefinementWindow.createComments = (this.ChkComments.IsChecked.HasValue && this.ChkComments.IsChecked.Value);
                RefinementWindow.includeDescription = (this.ChkDescription.IsChecked.HasValue && this.ChkDescription.IsChecked.Value);
                ExcelWrapper excelWrapper = new ExcelWrapper();
                excelWrapper.Initialize(root, this._path);
                MainWindow._asyncTasks.Execute(new Action(excelWrapper.CreateDocument), null);

                if (excelWrapper.DocumentIsValid)
                {
                    excelWrapper.SaveDocument();
                    return;
                }

                MessageBox.Show(Config.MSG_NO_TEST_CASES_TO_EXTRACT_TEXT, Config.MSG_NO_TEST_CASES_TO_EXTRACT_CAPTION, MessageBoxButton.OK, MessageBoxImage.Exclamation);
            }, delegate(object sender, CanExecuteRoutedEventArgs e)
            {
                e.Handled = true;
                if (RootViewModel.Root == null)
                {
                    return;
                }
                if (!RootViewModel.Root.IsChecked.HasValue)
                {
                    e.CanExecute = true;
                    return;
                }
                e.CanExecute = RootViewModel.Root.IsChecked.Value;
            });
            base.CommandBindings.Add(this._commandBinding);
            this.tree.Focus();
        }

        private void RefinementWindow_Closed(object sender, System.EventArgs e)
        {
            base.CommandBindings.Remove(this._commandBinding);
        }
    }
}
