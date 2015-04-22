using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using TestCaseExtractor.DataModel;
namespace TestCaseExtractor.ViewModel
{
    public class TreeViewItemViewModel : INotifyPropertyChanged
    {
        private readonly ObservableCollection<TreeViewItemViewModel> _children;
        private readonly TreeViewItemViewModel _parent;
        private bool _isLoaded;
        private bool _isExpanded;
        private bool _isSelected;
        public event PropertyChangedEventHandler PropertyChanged;

        public ObservableCollection<TreeViewItemViewModel> Children
        {
            get { return this._children; }
        }

        public bool IsExpanded
        {
            get { return this._isExpanded; }
            set
            {
                if (value != this._isExpanded)
                {
                    this._isExpanded = value;
                    this.OnPropertyChanged("IsExpanded");
                }
                if (this._isExpanded && this._parent != null)
                {
                    this._parent.IsExpanded = true;
                }
                if (!this._isLoaded)
                {
                    MainWindow._asyncTasks.Execute(new Action(this.loadChildrensChildren), null);
                }
            }
        }

        public bool IsSelected
        {
            get { return this._isSelected; }
            set
            {
                if (value != this._isSelected)
                {
                    this._isSelected = value;
                    this.OnPropertyChanged("IsSelected");
                    System.Type type = base.GetType();
                    if (type == typeof(TestSuiteViewModel))
                    {
                        MainWindow._btnExtract.IsEnabled = true;
                        return;
                    }
                    if (type == typeof(TestPlanViewModel))
                    {
                        MainWindow._btnExtract.IsEnabled = true;
                        return;
                    }
                    MainWindow._btnExtract.IsEnabled = false;
                }
            }
        }

        public TreeViewItemViewModel Parent
        {
            get { return this._parent; }
        }

        protected TreeViewItemViewModel(TreeViewItemViewModel parent)
        {
            this._parent = parent;
            this._children = new ObservableCollection<TreeViewItemViewModel>();
        }

        public virtual IDataModel GetDataModel()
        {
            return null;
        }

        private void loadChildrensChildren()
        {
            foreach (TreeViewItemViewModel current in this.Children)
            {
                if (current.Children.Count == 0)
                {
                    current.LoadChildren(Config.LAZY_LOAD_LEVELS);
                }
            }
            this._isLoaded = true;
        }

        protected virtual void LoadChildren(byte lazyLoadLevels)
        {
        }

        protected virtual void OnPropertyChanged(string propertyName)
        {
            if (this.PropertyChanged != null)
            {
                this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
