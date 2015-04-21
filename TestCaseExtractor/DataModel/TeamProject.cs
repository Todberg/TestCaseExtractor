using Microsoft.TeamFoundation.TestManagement.Client;
using System;
using System.Collections.Generic;

namespace TestCaseExtractor.DataModel
{
    public class TeamProject : IDataModel
    {
        private readonly List<TestPlan> _testPlans = new List<TestPlan>();

        public ITestManagementTeamProject TFSTestManagementTeamProject { get; private set; }

        public List<TestPlan> TestPlans
        {
            get { return this._testPlans; }
        }

        public TeamProject(ITestManagementTeamProject TFSTestManagementTeamProject)
        {
            this.TFSTestManagementTeamProject = TFSTestManagementTeamProject;
        }
    }
}