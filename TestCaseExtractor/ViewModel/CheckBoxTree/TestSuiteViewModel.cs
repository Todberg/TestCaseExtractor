using System;
using System.Collections.Generic;
using System.Linq;
using TestCaseExtractor.DataAccess;
using TestCaseExtractor.DataModel;

namespace TestCaseExtractor.ViewModel.CheckBoxTree
{
	public class TestSuiteViewModel : ItemViewModel
	{
		private TestSuite _testSuite;

        public IList<TestSuiteViewModel> ChildrenTestSuites { get; private set; }

		public IList<TestCaseViewModel> ChildrenTestCases { get; private set; }

		public TestSuite TestSuite { get; private set; }

		public TestSuiteViewModel(TestSuite testSuite) : base(testSuite.TFSTestSuiteBase.TestSuiteEntry.Title)
		{
			this._testSuite = testSuite;
			this.ChildrenTestSuites = new List<TestSuiteViewModel>();
			this.ChildrenTestCases = new List<TestCaseViewModel>();
			this.TestSuite = this._testSuite;
			this.LoadChildren();
		}

		protected override void LoadChildren()
		{
			IOrderedEnumerable<TestSuite> testSuites = TfsRepository.getTestSuites(this._testSuite);
			if (testSuites != null)
			{
				foreach (TestSuite testSuite in TfsRepository.getTestSuites(this._testSuite))
				{
                    TestSuiteViewModel testSuiteViewModel = new TestSuiteViewModel(testSuite);
                    base.Children.Add(testSuiteViewModel);
                    this.ChildrenTestSuites.Add(testSuiteViewModel);
				}
			}

			IOrderedEnumerable<TestCase> testCases = TfsRepository.getTestCases(this._testSuite);
			if (testCases != null)
			{
				foreach (TestCase testCase in testCases)
				{
                    TestCaseViewModel testCaseViewModel = new TestCaseViewModel(testCase);
                    base.Children.Add(testCaseViewModel);
                    this.ChildrenTestCases.Add(testCaseViewModel);
				}
			}
		}
	}
}