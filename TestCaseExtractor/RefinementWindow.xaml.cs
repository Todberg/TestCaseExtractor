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
        private IDataModel itemModelData;
        private IList<ItemViewModel> itemViewModelData;
        private CommandBinding _commandBinding;
        private string itemPath;
        
        public static bool createComments;
        public static bool includeDescription;

        public RefinementWindow(IDataModel itemDataModel, string itemPath)
        {
            InitializeComponent();
            this.itemModelData = itemDataModel;
            this.itemPath = itemPath;
            base.Closed += new System.EventHandler(this.RefinementWindow_Closed);

            MainWindow._asyncTasks.Execute(new Action(this.Start), new Action(this.End));
        }

        private void Start()
        {
            this.itemViewModelData = RootViewModel.ConstructTreeFromDataModel(this.itemModelData);
        }

        private void End()
        {
            this.Tree.ItemsSource = this.itemViewModelData;
            ItemViewModel root = this.Tree.Items[0] as ItemViewModel;

            this._commandBinding = new CommandBinding(ApplicationCommands.Undo, delegate(object sender, ExecutedRoutedEventArgs e)
            {
                e.Handled = true;
                RefinementWindow.createComments = (this.ChkComments.IsChecked.HasValue && this.ChkComments.IsChecked.Value);
                RefinementWindow.includeDescription = (this.ChkDescription.IsChecked.HasValue && this.ChkDescription.IsChecked.Value);
                ExcelWrapper excelWrapper = new ExcelWrapper();
                excelWrapper.Initialize(root, this.itemPath);
                MainWindow._asyncTasks.Execute(new Action(excelWrapper.CreateDocument), null);

                if (excelWrapper.DocumentIsValid)
                {
                    excelWrapper.SaveDocument();
                    return;
                }

                MessageBox.Show(
                    Config.NoTestCasesToExtract,
                    Config.NoTestCasesToExtractCaption,
                    MessageBoxButton.OK,
                    MessageBoxImage.Exclamation);

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
            this.Tree.Focus();
        }

        private void RefinementWindow_Closed(object sender, System.EventArgs e)
        {
            base.CommandBindings.Remove(this._commandBinding);
        }
    }
}
