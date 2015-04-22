using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Controls;
using TestCaseExtractor.DataModel;

namespace TestCaseExtractor.ViewModel
{
    public class ProjectViewModel
    {
        private readonly TreeView _tree;
        private readonly ReadOnlyCollection<TeamProjectViewModel> _teamProjects;
        
        public TreeView Tree
        {
            get { return this._tree; }
        }

        public IReadOnlyCollection<TeamProjectViewModel> TeamProjects
        {
            get { return this._teamProjects; }
        }

        public ProjectViewModel(TreeView tree, IOrderedEnumerable<TeamProject> teamProjects, byte lazyLoadLevels)
        {
            this._tree = tree;
            this._teamProjects = new System.Collections.ObjectModel.ReadOnlyCollection<TeamProjectViewModel>((
                from teamProject in teamProjects
                select new TeamProjectViewModel(teamProject, lazyLoadLevels)).ToList<TeamProjectViewModel>());
        }
    }
}
