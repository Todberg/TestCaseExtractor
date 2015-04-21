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

		public System.Collections.Generic.List<TestSuiteViewModel> ChildrenTestSuites
		{
			get;
			private set;
		}

		public System.Collections.Generic.List<TestCaseViewModel> ChildrenTestCases
		{
			get;
			private set;
		}

		public TestSuite TestSuite
		{
			get;
			private set;
		}

		public TestSuiteViewModel(TestSuite testSuite) : base(testSuite.TFSTestSuiteBase.get_TestSuiteEntry().get_Title())
		{
			this._testSuite = testSuite;
			this.ChildrenTestSuites = new System.Collections.Generic.List<TestSuiteViewModel>();
			this.ChildrenTestCases = new System.Collections.Generic.List<TestCaseViewModel>();
			this.TestSuite = this._testSuite;
			this.LoadChildren();
		}

		protected override void LoadChildren()
		{
			IOrderedEnumerable<TestSuite> testSuites = Database.getTestSuites(this._testSuite);
			if (testSuites != null)
			{
				foreach (TestSuite current in Database.getTestSuites(this._testSuite))
				{
					TestSuiteViewModel item = new TestSuiteViewModel(current);
					base.Children.Add(item);
					this.ChildrenTestSuites.Add(item);
				}
			}
			IOrderedEnumerable<TestCase> testCases = Database.getTestCases(this._testSuite);
			if (testCases != null)
			{
				foreach (TestCase current2 in testCases)
				{
					TestCaseViewModel item2 = new TestCaseViewModel(current2);
					base.Children.Add(item2);
					this.ChildrenTestCases.Add(item2);
				}
			}
		}
	}
}