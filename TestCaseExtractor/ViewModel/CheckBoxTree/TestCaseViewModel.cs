using System;
using TestCaseExtractor.DataModel;
namespace TestCaseExtractor.ViewModel.CheckBoxTree
{
    public class TestCaseViewModel : ItemViewModel
    {
        public TestCase TestCase { get; private set; }

        public TestCaseViewModel(TestCase testCase) : base(testCase.TFSTestSuiteEntry.get_Title())
        {
            this.TestCase = testCase;
        }
    }
}