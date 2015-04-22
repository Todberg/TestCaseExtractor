using Microsoft.TeamFoundation.TestManagement.Client;

namespace TestCaseExtractor.DataModel
{
	public class TestSuite : IDataModel
	{
        public ITestSuiteBase TFSTestSuiteBase { get; private set; }

		public TestSuite(ITestSuiteBase TFSTestSuiteBase)
		{
			this.TFSTestSuiteBase = TFSTestSuiteBase;
		}
	}
}