using System;
using TestCaseExtractor.DataAccess;
using TestCaseExtractor.DataModel;

namespace TestCaseExtractor.ViewModel.CheckBoxTree
{
    public class TestPlanViewModel : ItemViewModel
    {
        private TestPlan _testPlan;

        public TestPlan TestPlan
        {
            get;
            private set;
        }

        public TestPlanViewModel(TestPlan testPlan) : base(testPlan.TFSTestPlan.get_Name())
        {
            this._testPlan = testPlan;
            this.TestPlan = this._testPlan;
            this.LoadChildren();
        }

        protected override void LoadChildren()
        {
            foreach (TestSuite current in Database.getTestSuites(this._testPlan))
            {
                base.Children.Add(new TestSuiteViewModel(current));
            }
        }
    }
}