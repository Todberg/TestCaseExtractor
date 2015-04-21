using System;
using TestCaseExtractor.DataAccess;
using TestCaseExtractor.DataModel;

namespace TestCaseExtractor.ViewModel
{
    public class TeamProjectViewModel : TreeViewItemViewModel
    {
        private readonly TeamProject _teamProject;
        public string Name
        {
            get
            {
                return this._teamProject.TFSTestManagementTeamProject.get_TeamProjectName();
            }
        }
        public TeamProjectViewModel(TeamProject teamProject, byte lazyLoadLevels)
            : base(null)
        {
            this._teamProject = teamProject;
            if (lazyLoadLevels > 0)
            {
                this.LoadChildren(lazyLoadLevels -= 1);
            }
        }
        public override IDataModel GetDataModel()
        {
            return this._teamProject;
        }
        protected override void LoadChildren(byte lazyLoadLevels)
        {
            foreach (TestPlan current in Database.getTestPlans(this._teamProject))
            {
                base.Children.Add(new TestPlanViewModel(current, this, lazyLoadLevels));
            }
        }
    }
}
