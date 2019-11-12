using DataAcces;
using DataModels;
using DataResources;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;

namespace Assets.Editor
{
    public class TestMenu
    {
        [MenuItem("Tests/SQLite/CreateStatement")]
        static void TestCreate()
        {
            var manager = new CachedTableManager();
            manager.CreateTables();
        }

        [MenuItem("Tests/SQLite/RecognizedResource")]
        static void TestInsert()
        {
            var manager = new CachedTableManager();

            var list = new List<RecognizedObjectResource>();

            list.Add(new RecognizedObjectResource()
            {
                Id = 1,
                Content = new ContentResource()
                {
                    Id = 1,
                    Name = "asd",
                    AssetBundle = new DataModels.AssetBundle()
                    {
                        Id = 1,
                        Name = "asdasd",
                        Modified = DateTime.Now
                    },
                    Modified = DateTime.Now
                },
                Name = "asdasdasd"
            });

            manager.CacheRecognizedObject(list[0]);
        }

        [MenuItem("Tests/SQLite/SelectRecognizedObjectResource")]
        static void TestSelect()
        {
            var manager = new CachedTableManager();

            var recognizedObjects = manager.GetRecognizedObjects();

            var str = "";
            //foreach (var recognizedObject in recognizedObjects)
            //{
            //    str += $"Id: { recognizedObject.Id }, {recognizedObject.}";
            //}

            Debug.Log(str);
        }

        //[MenuItem("Tests/SQLite/UpdateStatment")]
        //static void TestUpdate()
        //{
        //    var repo = new CachedRepository<RecognizedObject>();

        //    var list = new List<RecognizedObject>();

        //    list.Add(new RecognizedObject()
        //    {
        //        Id = 3,
        //        ContentId = 3,
        //        Name = "jozsika",
        //        Modified = DateTime.Now
        //    });

        //    Debug.Log(repo.Update(list[0]));
        //}
    }
}
