using DataModels;
using Mono.Data.Sqlite;
using Repository;
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
            var repo = new CachedRepository<RecognizedObject>();
        }

        [MenuItem("Tests/SQLite/InsertStatement")]
        static void TestInsert()
        {
            var repo = new CachedRepository<RecognizedObject>();

            var list = new List<RecognizedObject>();

            list.Add(new RecognizedObject()
            {
                Id = 1,
                ContentId = 1,
                Name = "sad"
            });

            list.Add(new RecognizedObject()
            {
                Id = 2,
                ContentId = 2,
                Name = "dsa"
            });

            list.Add(new RecognizedObject()
            {
                Id = 3,
                ContentId = 3,
                Name = "asd"
            });

            Debug.Log($"Test CreateStatement: {repo.Add(list)}");
        }

        [MenuItem("Tests/SQLite/SelectStatement")]
        static void TestSelect()
        {
            var repo = new CachedRepository<RecognizedObject>();
            var list = repo.GetAll();

            var str = "";
            foreach (var item in list)
            {
                str += $"{ item.Id }, { item.Name }, { item.ContentId }, { item.Modified }\n";
            }

            Debug.Log(str);
        }

        [MenuItem("Tests/SQLite/UpdateStatment")]
        static void TestUpdate()
        {
            var repo = new CachedRepository<RecognizedObject>();

            var list = new List<RecognizedObject>();

            list.Add(new RecognizedObject()
            {
                Id = 3,
                ContentId = 3,
                Name = "jozsika",
                Modified = DateTime.Now
            });

            Debug.Log(repo.Update(list[0]));
        }
    }
}
