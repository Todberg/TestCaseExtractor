using Microsoft.TeamFoundation.Client;
using Microsoft.TeamFoundation.Server;
using Microsoft.TeamFoundation.TestManagement.Client;
using Microsoft.TeamFoundation.WorkItemTracking.Client;
using System.Linq;
using TestCaseExtractor.DataModel;

namespace TestCaseExtractor.DataAccess
{
    public class TfsRepository
    {
        private static TfsTeamProjectCollection _tfs;
        private static ITestManagementService _testService;
        private static WorkItemStore _store;

        public static void Initialize(TfsTeamProjectCollection tfs)
        {
            TfsRepository._tfs = tfs;
            TfsRepository._testService = (ITestManagementService)TfsRepository._tfs.GetService(typeof(ITestManagementService));
            TfsRepository._store = (WorkItemStore)TfsRepository._tfs.GetService(typeof(WorkItemStore));
        }

        public static IOrderedEnumerable<TeamProject> GetTeamProjects(ProjectInfo[] projectInfo)
        {
            return projectInfo
                    .Select((pi) => new TeamProject(TfsRepository._testService.GetTeamProject(pi.Name)))
                    .OrderBy(tp => tp.TFSTestManagementTeamProject.TeamProjectName);
       }

        public static IOrderedEnumerable<TeamProject> GetTeamProjects(TfsTeamProjectCollection tfs)
        {
            TfsRepository.Initialize(tfs);

            ProjectCollection projects = TfsRepository._store.Projects;

            TeamProject[] teamProjects = new TeamProject[projects.Count];

            var a = TfsRepository._testService.GetTeamProject("STAR.JobKon");

            for (int i = 0; i < projects.Count; i++)
                teamProjects[i] = new TeamProject(TfsRepository._testService.GetTeamProject(projects[i]));

            return teamProjects.OrderBy(tp => tp.TFSTestManagementTeamProject.TeamProjectName);
        }

        public static IOrderedEnumerable<TestPlan> getTestPlans(TeamProject teamProject)
        {
            ITestPlanCollection testPlanCollection = teamProject.TFSTestManagementTeamProject.TestPlans.Query("Select * From TestPlan");
            TestPlan[] testPlans = new TestPlan[testPlanCollection.Count];

            for (int i = 0; i < testPlanCollection.Count; i++)
                testPlans[i] = new TestPlan(testPlanCollection[i]);

            return testPlans.OrderBy(tp => tp.TFSTestPlan.Name);
        }

        public static IOrderedEnumerable<TestSuite> getTestSuites(TestPlan testPlan)
        {
            ITestSuiteCollection subSuites = testPlan.TFSTestPlan.RootSuite.SubSuites;
            TestSuite[] testSuites = new TestSuite[subSuites.Count];

            for (int i = 0; i < subSuites.Count; i++)
                testSuites[i] = new TestSuite(subSuites[i]);

            return testSuites.OrderBy(ts => ts.TFSTestSuiteBase.Title);
        }

        public static IOrderedEnumerable<TestSuite> getTestSuites(TestSuite testSuite)
        {
            IStaticTestSuite staticTestSuite = testSuite.TFSTestSuiteBase as IStaticTestSuite;

            if (staticTestSuite == null)
                return null;

            ITestSuiteCollection subSuites = ((IStaticTestSuite)testSuite.TFSTestSuiteBase).SubSuites;
            TestSuite[] testSuites = new TestSuite[subSuites.Count];

            for (int i = 0; i < subSuites.Count; i++)
                testSuites[i] = new TestSuite(subSuites[i]);

            return testSuites.OrderBy(ts => ts.TFSTestSuiteBase.Title);
        }

        public static IOrderedEnumerable<TestCase> getTestCases(TestSuite testSuite)
        {
            ITestSuiteEntryCollection testSuiteEntries = testSuite.TFSTestSuiteBase.TestCases;
            TestCase[] testCases = new TestCase[testSuiteEntries.Count];

            for (int i = 0; i < testSuiteEntries.Count; i++)
                testCases[i] = new TestCase(testSuiteEntries[i]);

            return testCases.OrderBy(tc => tc.TFSTestSuiteEntry.Title);
        }
    }
}
