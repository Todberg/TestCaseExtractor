using System;
using System.Collections.Generic;
using TestCaseExtractor.DataModel;
namespace TestCaseExtractor.ViewModel.CheckBoxTree
{
    public class RootViewModel : ItemViewModel
    {
        public static ItemViewModel Root;
        public static System.Collections.Generic.List<ItemViewModel> ConstructTreeFromDataModel(IDataModel itemDataModel)
        {
            RootViewModel.Root = null;
            System.Type type = itemDataModel.GetType();
            if (type == typeof(TestPlan))
            {
                RootViewModel.Root = new TestPlanViewModel((TestPlan)itemDataModel);
            }
            else if (type == typeof(TestSuite))
            {
                RootViewModel.Root = new TestSuiteViewModel((TestSuite)itemDataModel);
            }
            if (RootViewModel.Root != null)
            {
                RootViewModel.Root.initialize();
                RootViewModel.Root.IsInitiallySelected = true;
                RootViewModel.Root.IsChecked = new bool?(true);
            }
            return new System.Collections.Generic.List<ItemViewModel>
			{
				RootViewModel.Root
			};
        }
    }
}