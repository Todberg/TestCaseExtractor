using Microsoft.TeamFoundation.Client;
using Microsoft.TeamFoundation.TestManagement.Client;
using Microsoft.TeamFoundation.WorkItemTracking.Client;
using System;
using System.Linq;
using TestCaseExtractor.DataModel;

namespace TestCaseExtractor.DataAccess
{
    public class Database
    {
        private static TfsTeamProjectCollection _tfs;
        private static ITestManagementService _testService;
        private static WorkItemStore _store;

        private static void initialize(TfsTeamProjectCollection tfs)
        {
            Database._tfs = tfs;
            Database._testService = (ITestManagementService)Database._tfs.GetService(typeof(ITestManagementService));
            Database._store = (WorkItemStore)Database._tfs.GetService(typeof(WorkItemStore));
        }

        public static IOrderedEnumerable<TeamProject> getTeamProjects(TfsTeamProjectCollection tfs)
        {
            Database.initialize(tfs);
            ProjectCollection projects = Database._store.get_Projects();
            TeamProject[] array = new TeamProject[projects.get_Count()];

            for (int i = 0; i < projects.get_Count(); i++)
            {
                array[i] = new TeamProject(Database._testService.GetTeamProject(projects.get_Item(i)));
            }

            return
                from p in array
                orderby p.TFSTestManagementTeamProject.get_TeamProjectName()
                select p;
        }

        public static IOrderedEnumerable<TestPlan> getTestPlans(TeamProject teamProject)
        {
            ITestPlanCollection testPlanCollection = teamProject.TFSTestManagementTeamProject.get_TestPlans().Query("Select * From TestPlan");
            TestPlan[] array = new TestPlan[testPlanCollection.Count];

            for (int i = 0; i < testPlanCollection.Count; i++)
            {
                array[i] = new TestPlan(testPlanCollection[i]);
            }

            return
                from p in array
                orderby p.TFSTestPlan.get_Name()
                select p;
        }

        public static IOrderedEnumerable<TestSuite> getTestSuites(TestPlan testPlan)
        {
            ITestSuiteCollection subSuites = testPlan.TFSTestPlan.get_RootSuite().get_SubSuites();
            TestSuite[] array = new TestSuite[subSuites.Count];

            for (int i = 0; i < subSuites.Count; i++)
            {
                array[i] = new TestSuite(subSuites[i]);
            }

            return
                from s in array
                orderby s.TFSTestSuiteBase.get_Title()
                select s;
        }

        public static IOrderedEnumerable<TestSuite> getTestSuites(TestSuite testSuite)
        {
            IStaticTestSuite staticTestSuite = testSuite.TFSTestSuiteBase as IStaticTestSuite;

            if (staticTestSuite != null)
            {
                ITestSuiteCollection subSuites = ((IStaticTestSuite)testSuite.TFSTestSuiteBase).get_SubSuites();
                TestSuite[] array = new TestSuite[subSuites.Count];

                for (int i = 0; i < subSuites.Count; i++)
                {
                    array[i] = new TestSuite(subSuites[i]);
                }

                return
                    from s in array
                    orderby s.TFSTestSuiteBase.get_Title()
                    select s;
            }

            return null;
        }

        public static IOrderedEnumerable<TestCase> getTestCases(TestSuite testSuite)
        {
            ITestSuiteEntryCollection testCases = testSuite.TFSTestSuiteBase.get_TestCases();
            TestCase[] array = new TestCase[testCases.Count];

            for (int i = 0; i < testCases.Count; i++)
            {
                array[i] = new TestCase(testCases[i]);
            }

            return
                from t in array
                orderby t.TFSTestSuiteEntry.get_Title()
                select t;
        }
    }
}
