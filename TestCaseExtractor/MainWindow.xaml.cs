using Microsoft.TeamFoundation.Client;
using Microsoft.TeamFoundation.Server;
using System;
using System.Collections.Generic;
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

        private ProjectViewModel projectViewModel;
        private RefinementWindow refinementWindow;

        public MainWindow()
        {
            InitializeComponent();
            MainWindow._asyncTasks = new AsyncTasks(this);
            MainWindow._btnExtract = this.BtnExtract;
        }

        private void BtnConnectClick(object sender, RoutedEventArgs e)
        {
            using(var teamProjectPicker = new TeamProjectPicker(TeamProjectPickerMode.MultiProject, false))
            {
                DialogResult dialogResult = teamProjectPicker.ShowDialog();
                
                if (dialogResult == System.Windows.Forms.DialogResult.OK && teamProjectPicker.SelectedProjects.Length > 0)
                {
                    MainWindow._asyncTasks.Execute(() => {
                        TfsRepository.Initialize(teamProjectPicker.SelectedTeamProjectCollection);
                        IOrderedEnumerable<TeamProject> teamProjects = TfsRepository.GetTeamProjects(teamProjectPicker.SelectedProjects);
                        this.projectViewModel = new ProjectViewModel(this.Tree, teamProjects, Config.LazyLoadLevels);
                    }, () => {
                        base.DataContext = this.projectViewModel;
                    });
                }
            }
        }

        private void BtnExtractClick(object sender, RoutedEventArgs e)
        {
            var treeViewItemViewModel = (TreeViewItemViewModel)this.projectViewModel.Tree.SelectedItem;
            
            IDataModel dataModel = treeViewItemViewModel.GetDataModel();
            string selectedItemPath = this.GetSelectedItemPath(treeViewItemViewModel);

            this.refinementWindow = new RefinementWindow(dataModel, selectedItemPath);
            this.refinementWindow.Owner = this;
            this.refinementWindow.ShowDialog();
        }

        private string GetSelectedItemPath(TreeViewItemViewModel item)
        {
            var items = new List<string>();
            TreeViewItemViewModel parent = item.Parent;

            do
            {
                if (parent != null)
                {
                    Type type = parent.GetType();
                    if (type == typeof(TestSuiteViewModel))
                        items.Add(((TestSuiteViewModel)parent).Name);
                    else if (type == typeof(TestPlanViewModel))
                        items.Add(((TestPlanViewModel)parent).Name);
                    else if (type == typeof(TeamProjectViewModel))
                        items.Add(((TeamProjectViewModel)parent).Name);
                    
                    parent = parent.Parent;
                }
            } while (parent != null);

            return string.Join("/", items.Reverse<string>().ToArray<string>());
        }
    }
}
