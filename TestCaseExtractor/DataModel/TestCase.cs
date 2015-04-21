using Microsoft.TeamFoundation.TestManagement.Client;
using System;

namespace TestCaseExtractor.DataModel
{
    public class TestCase : IDataModel
    {
        public ITestSuiteEntry TFSTestSuiteEntry { get; private set; }

        public TestCase(ITestSuiteEntry TFSTestSuiteEntry)
        {
            this.TFSTestSuiteEntry = TFSTestSuiteEntry;
        }
    }
}
