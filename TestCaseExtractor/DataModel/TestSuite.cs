using Microsoft.TeamFoundation.TestManagement.Client;
using System;
using System.Collections.Generic;

namespace TestCaseExtractor.DataModel
{
	public class TestSuite : IDataModel
	{
		private readonly List<TestSuite> _testSuites = new List<TestSuite>();

        public ITestSuiteBase TFSTestSuiteBase { get; private set; }

		public List<TestSuite> TestSuites
		{
			get { return this._testSuites; }
		}

		public TestSuite(ITestSuiteBase TFSTestSuiteBase)
		{
			this.TFSTestSuiteBase = TFSTestSuiteBase;
		}
	}
}