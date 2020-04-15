using System.Collections.ObjectModel;
using Demo.Classes;

namespace Demo.ViewModels
{
    public class MainWindowViewModel
    {
        public MainWindowViewModel()
        {
            Items = new ObservableCollection<SimpleItem>();

            Items.Add(new SimpleItem(8, "11"));
            Items.Add(new SimpleItem(2, "100"));
            Items.Add(new SimpleItem(3, "X3"));
            Items.Add(new SimpleItem(4, "1"));
            Items.Add(new SimpleItem(4, "33"));
            Items.Add(new SimpleItem(9, "23"));
            Items.Add(new SimpleItem(9, "23UX"));
            Items.Add(new SimpleItem(7, "2"));
            Items.Add(new SimpleItem(9, "J23P92H"));
        }

        public ObservableCollection<SimpleItem> Items { get; }
    }
}