using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using DataModels;

public class DataSetInfo_Model : INotifyPropertyChanged
{
    public event PropertyChangedEventHandler PropertyChanged;

    private DataSet cache;
    public DataSet Cache
    {
        get => cache;
        set
        {
            if (cache != value)
            {
                cache = value;
                NotifyPropertyChanged();
            }
        }
    }

    private DataSet api;
    public DataSet Api
    {
        get => api;
        set
        {
            if (api != value)
            {
                api = value;
                NotifyPropertyChanged();
            }
        }
    }

    private Vuforia.DataSet loaded;
    public Vuforia.DataSet Loaded
    {
        get => loaded;
        set
        {
            if (loaded != value)
            {
                loaded = value;
                NotifyPropertyChanged();
            }
        }
    }

    public DataSet Prefered
    {
        get => cache ?? api;
    }
    public bool Syncronised
    {
        get => Cache?.Modified == Api?.Modified;
    }

    private void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}