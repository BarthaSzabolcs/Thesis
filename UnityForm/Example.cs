using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace UnityForm
{
    public class ExampleMonobehaviour : MonoBehaviour
    {
        [SerializeField] private string message;
        [SerializeField] private float updateTime;

        private float timer;

        public void Start()
        {
            Debug.Log("This is a test, and it's fucking succesfull.");
        }

        public void Update()
        {
            if (Time.time - timer > updateTime)
            {
                timer = Time.time;
                Debug.Log("Update called.");
            }
        }
    }
}
