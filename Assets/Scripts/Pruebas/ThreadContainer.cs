using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;


namespace Pruebas
{
    public class ThreadContainer : MonoBehaviour
    {

        public static ThreadContainer Instance;

        void Awake()
        {
            Instance = this;
        }


        public void Comenzar()
        {
            for(int i = 0; i < 5; i++)
            {
                Thread newThread = new Thread(Hilo);
                newThread.Name = string.Format("Thread{0}", i + 1);
                newThread.Start();
            }
        }

        public void Hilo()
        {
            for(int i = 0; i < 3; i++)
                UsaRecurso();
        }

        public void UsaRecurso()
        {
            print(Thread.CurrentThread.Name + " is requesting the mutex");
            MutexContainer.Instance.myMutex.WaitOne();

            print(Thread.CurrentThread.Name + " is in SAFE ZONE, contador = " + MutexContainer.Instance.contador);
            Thread.Sleep(500);

            for (int i = 0; i < 100; i++)
                MutexContainer.Instance.contador++;

            MutexContainer.Instance.myMutex.ReleaseMutex();
            print(Thread.CurrentThread.Name + " releasedMutex, contador = " + MutexContainer.Instance.contador);
        }
    }
}