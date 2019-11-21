using DataModels;
using System.Reflection;

namespace DllManagment
{
    public class DllInfo
    {
        public Dll Cache { get; set; }
        public Dll Api { get; set; }
        public Dll Prefered => Cache ?? Api;
        public bool Syncronised => Cache?.Modified == Api?.Modified;

        public Assembly assembly { get; set; }
    }
}
