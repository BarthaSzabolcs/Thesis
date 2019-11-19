using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace AssetBundleManagment
{
    public class AssetBundleInfo
    {
        public DataModels.AssetBundle Cache { get; set; }
        public DataModels.AssetBundle Api { get; set; }
        public DataModels.AssetBundle Prefered => Cache ?? Api;
        public bool Syncronised => Cache?.Modified == Api?.Modified;

        public bool LoadStarted { get; set; }
        public AssetBundle Loaded { get; set; }
        public List<Action<AssetBundle>> LoadCallbacks { get; set; } = new List<Action<AssetBundle>>();
    }
}
