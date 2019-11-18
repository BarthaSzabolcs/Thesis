using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.Events;

namespace ContentBar
{
    public class ContentTab_Model : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private string name;
        public string Name
        {
            get => name;
            set
            {
                if (name != value)
                {
                    name = value;
                    NotifyPropertyChanged();
                }
            }
        }

        private bool open;
        public bool Open 
        {
            get => open;
            set
            {
                if (open != value)
                {
                    open = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public UnityAction OpenAction { get; private set; }
        public UnityAction CloseAction { get; private set; }

        public ContentTab_Model(string name, UnityAction open, UnityAction close)
        {
            this.name = name;
            OpenAction = open;
            CloseAction = close;
        }

        private void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
