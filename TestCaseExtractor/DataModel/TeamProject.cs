using Microsoft.TeamFoundation.TestManagement.Client;

namespace TestCaseExtractor.DataModel
{
    public class TeamProject : IDataModel
    {
        public ITestManagementTeamProject TFSTestManagementTeamProject { get; private set; }

        public TeamProject(ITestManagementTeamProject TFSTestManagementTeamProject)
        {
            this.TFSTestManagementTeamProject = TFSTestManagementTeamProject;
        }
    }
}