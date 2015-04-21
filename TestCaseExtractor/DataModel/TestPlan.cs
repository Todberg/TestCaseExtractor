using Microsoft.TeamFoundation.TestManagement.Client;
using System;
using System.Collections.Generic;

namespace TestCaseExtractor.DataModel
{
    public class TestPlan : IDataModel
    {
        private readonly List<TestSuite> _testSuites = new List<TestSuite>();

        public ITestPlan TFSTestPlan { get; private set; }

        public List<TestSuite> TestSuites
        {
            get { return this._testSuites; }
        }

        public TestPlan(ITestPlan TFSTestPlan)
        {
            this.TFSTestPlan = TFSTestPlan;
        }
    }
}