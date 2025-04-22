using System.Collections.ObjectModel;
using summary_list.Models;

namespace summary_list.ViewModels
{
    public class ProtocolViewModel : BaseViewModel
    {
        private string _name;
        public string Name
        {
            get { return _name; }
            set { SetProperty(ref _name, value); }
        }

        private ObservableCollection<GroupViewModel> _groups;
        public ObservableCollection<GroupViewModel> Groups
        {
            get { return _groups; }
            set { SetProperty(ref _groups, value); }
        }
    }
} 