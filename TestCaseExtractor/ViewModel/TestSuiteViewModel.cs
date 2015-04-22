using System;
using System.Linq;
using TestCaseExtractor.DataAccess;
using TestCaseExtractor.DataModel;

namespace TestCaseExtractor.ViewModel
{
    public class TestSuiteViewModel : TreeViewItemViewModel
    {
        public readonly TestSuite _testSuite;

        public string Name
        {
            get { return this._testSuite.TFSTestSuiteBase.Title; }
        }

        public TestSuiteViewModel(TestSuite testSuite, TreeViewItemViewModel parent, byte lazyLoadLevels): base(parent)
        {
            this._testSuite = testSuite;

            if (lazyLoadLevels > 0)
            {
                this.LoadChildren(lazyLoadLevels -= 1);
            }
        }

        public override IDataModel GetDataModel()
        {
            return this._testSuite;
        }

        protected override void LoadChildren(byte lazyLoadLevels)
        {
            IOrderedEnumerable<TestSuite> testSuites = TfsRepository.getTestSuites(this._testSuite);

            if (testSuites != null)
            {
                foreach (TestSuite current in TfsRepository.getTestSuites(this._testSuite))
                {
                    base.Children.Add(new TestSuiteViewModel(current, this, lazyLoadLevels));
                }
            }
        }
    }
}