using Microsoft.TeamFoundation.TestManagement.Client;

namespace TestCaseExtractor.DataModel
{
    public class TestPlan : IDataModel
    {
        public ITestPlan TFSTestPlan { get; private set; }

        public TestPlan(ITestPlan TFSTestPlan)
        {
            this.TFSTestPlan = TFSTestPlan;
        }
    }
}