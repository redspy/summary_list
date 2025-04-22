using System;
using System.Collections.ObjectModel;
using System.Windows.Input;
using summary_list.Models;
using summary_list.ViewModels;

namespace summary_list.ViewModels
{
    public class GroupViewModel : BaseViewModel
    {
        private string _name;
        private ObservableCollection<SummaryItem> _items;
        private bool _isExpanded;

        public string Name
        {
            get => _name;
            set => SetProperty(ref _name, value);
        }

        public ObservableCollection<SummaryItem> Items
        {
            get => _items;
            set => SetProperty(ref _items, value);
        }

        public bool IsExpanded
        {
            get => _isExpanded;
            set => SetProperty(ref _isExpanded, value);
        }

        public ICommand ToggleExpandCommand { get; }

        public GroupViewModel(string name)
        {
            Name = name;
            Items = new ObservableCollection<SummaryItem>();
            IsExpanded = true;

            ToggleExpandCommand = new RelayCommand(() => IsExpanded = !IsExpanded);
        }

        public void AddItem(SummaryItem item)
        {
            Items.Add(item);
        }
    }
} 