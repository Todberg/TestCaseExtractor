using Microsoft.TeamFoundation.Client;
using Microsoft.TeamFoundation.Server;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using TestCaseExtractor.DataAccess;
using TestCaseExtractor.DataModel;
using TestCaseExtractor.ViewModel;
using DialogResult = System.Windows.Forms.DialogResult;

namespace TestCaseExtractor
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public static AsyncTasks _asyncTasks;
        public static Button _btnExtract;
        
        private TfsTeamProjectCollection _tfs;
        private ProjectViewModel _projectViewModel;
        private RefinementWindow _refinementWindow;

        public MainWindow()
        {
            InitializeComponent();
            MainWindow._asyncTasks = new AsyncTasks(this);
            MainWindow._btnExtract = this.BtnExtract;
        }

        private void btn_connect_Click(object sender, RoutedEventArgs e)
        {
            using(var teamProjectPicker = new TeamProjectPicker(TeamProjectPickerMode.MultiProject, false))
            {
                DialogResult dialogResult = teamProjectPicker.ShowDialog();
                
                if (dialogResult == System.Windows.Forms.DialogResult.OK && teamProjectPicker.SelectedProjects.Length > 0)
                {
                    ProjectInfo[] selectedProjects = teamProjectPicker.SelectedProjects;

                    this._tfs = teamProjectPicker.SelectedTeamProjectCollection;

                    MainWindow._asyncTasks.Execute(() => {
                        IOrderedEnumerable<TeamProject> teamProjects = TfsRepository.GetTeamProjects(this._tfs, selectedProjects);
                        this._projectViewModel = new ProjectViewModel(this.Tree, teamProjects, Config.LAZY_LOAD_LEVELS);
                    }, () => {
                        base.DataContext = this._projectViewModel;
                    });
                } 
            }
        }

        private void btn_extract_Click(object sender, RoutedEventArgs e)
        {
            TreeViewItemViewModel treeViewItemViewModel = (TreeViewItemViewModel)this._projectViewModel.Tree.SelectedItem;
            IDataModel dataModel = treeViewItemViewModel.GetDataModel();
            System.Type type = treeViewItemViewModel.GetType();
            string name;
            if (type == typeof(TestPlanViewModel))
            {
                name = ((TestPlanViewModel)treeViewItemViewModel).Name;
            }
            else
            {
                name = ((TestSuiteViewModel)treeViewItemViewModel).Name;
            }
            this._refinementWindow = new RefinementWindow(dataModel, this.GetSelectedItemPath(treeViewItemViewModel, name));
            this._refinementWindow.Owner = this;
            this._refinementWindow.ShowDialog();
        }
        private string GetSelectedItemPath(TreeViewItemViewModel item, string itemName)
        {
            System.Collections.Generic.IList<string> list = new System.Collections.Generic.List<string>();
            TreeViewItemViewModel parent = item.Parent;
            do
            {
                if (parent != null)
                {
                    System.Type type = parent.GetType();
                    if (type == typeof(TestSuiteViewModel))
                    {
                        list.Add(((TestSuiteViewModel)parent).Name);
                    }
                    else if (type == typeof(TestPlanViewModel))
                    {
                        list.Add(((TestPlanViewModel)parent).Name);
                    }
                    else if (type == typeof(TeamProjectViewModel))
                    {
                        list.Add(((TeamProjectViewModel)parent).Name);
                    }
                    parent = parent.Parent;
                }
            }
            while (parent != null);
            return string.Join("/", list.Reverse<string>().ToArray<string>());
        }
    }
}
