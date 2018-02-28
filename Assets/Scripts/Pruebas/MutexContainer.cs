using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;


namespace Pruebas
{
    public class MutexContainer : MonoBehaviour
    {

        public Mutex myMutex;
        public int contador;
        public static MutexContainer Instance;

        void Start()
        {
            myMutex = new Mutex();
            Instance = this;
            ThreadContainer.Instance.Comenzar();
        }
    }
}