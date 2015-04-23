using System;
using TestCaseExtractor.DataAccess;
using TestCaseExtractor.DataModel;

namespace TestCaseExtractor.ViewModel
{
    public class TestPlanViewModel : TreeViewItemViewModel
    {
        public readonly TestPlan _testPlan;

        public string Name
        {
            get { return this._testPlan.TFSTestPlan.Name; }
        }

        public TestPlanViewModel(TestPlan testPlan, TeamProjectViewModel parentTeamProject, byte lazyLoadLevels) : base(parentTeamProject)
        {
            this._testPlan = testPlan;

            if (lazyLoadLevels > 0)
            {
                this.LoadChildren(lazyLoadLevels -= 1);
            }
        }

        public override IDataModel GetDataModel()
        {
            return this._testPlan;
        }

        protected override void LoadChildren(byte lazyLoadLevels)
        {
            foreach (TestSuite current in TfsRepository.getTestSuites(this._testPlan))
                base.Children.Add(new TestSuiteViewModel(current, this, lazyLoadLevels));
        }
    }
}