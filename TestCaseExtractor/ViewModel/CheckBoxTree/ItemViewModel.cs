using System;
using System.Collections.Generic;
using System.ComponentModel;
namespace TestCaseExtractor.ViewModel.CheckBoxTree
{
    public class ItemViewModel : INotifyPropertyChanged
    {
        private bool? _isChecked = new bool?(false);
        private ItemViewModel _parent;
        public event PropertyChangedEventHandler PropertyChanged;

        public List<ItemViewModel> Children { get; set; }

        public bool IsInitiallySelected
        {
            get;
            set;
        }

        public string Name
        {
            get;
            private set;
        }

        public bool? IsChecked
        {
            get
            {
                return this._isChecked;
            }
            set
            {
                this.SetIsChecked(value, true, true);
            }
        }

        public ItemViewModel()
        {
        }

        public ItemViewModel(string name)
        {
            this.Children = new System.Collections.Generic.List<ItemViewModel>();
            this.Name = name;
        }

        public void initialize()
        {
            foreach (ItemViewModel current in this.Children)
            {
                current._parent = this;
                current.initialize();
            }
        }

        private void SetIsChecked(bool? value, bool updateChildren, bool updateParent)
        {
            if (value == this._isChecked)
            {
                return;
            }
            this._isChecked = value;
            if (updateChildren && this._isChecked.HasValue)
            {
                this.Children.ForEach(delegate(ItemViewModel c)
                {
                    c.SetIsChecked(this._isChecked, true, false);
                });
            }
            if (updateParent && this._parent != null)
            {
                this._parent.VerifyCheckState();
            }
            this.OnPropertyChanged("IsChecked");
            this.OnPropertyChanged("IsEnabled");
        }

        private void VerifyCheckState()
        {
            bool? flag = null;
            for (int i = 0; i < this.Children.Count; i++)
            {
                bool? isChecked = this.Children[i].IsChecked;
                if (i == 0)
                {
                    flag = isChecked;
                }
                else if (flag != isChecked)
                {
                    flag = null;
                    break;
                }
            }
            this.SetIsChecked(flag, false, true);
        }
        protected virtual void LoadChildren()
        {
        }
        private void OnPropertyChanged(string prop)
        {
            if (this.PropertyChanged != null)
            {
                this.PropertyChanged(this, new PropertyChangedEventArgs(prop));
            }
        }
    }
}
