using System.ComponentModel;

namespace summary_list.Models
{
    public class SummaryItem : INotifyPropertyChanged
    {
        private string _text;
        public string Text
        {
            get { return _text; }
            set
            {
                _text = value;
                OnPropertyChanged(nameof(Text));
            }
        }

        private string _group;
        public string Group
        {
            get { return _group; }
            set
            {
                _group = value;
                OnPropertyChanged(nameof(Group));
            }
        }

        private bool _isChecked;
        public bool IsChecked
        {
            get { return _isChecked; }
            set
            {
                _isChecked = value;
                OnPropertyChanged(nameof(IsChecked));
                OnPropertyChanged(nameof(CheckSymbol));
            }
        }

        public string CheckSymbol => IsChecked ? "V" : "X";

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
} 