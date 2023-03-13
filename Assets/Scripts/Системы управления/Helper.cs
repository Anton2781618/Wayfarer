using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//класс для хранения вспомогательных методов
public class Helper : MonoBehaviour
{
    private void TestTime()
    {
        System.Diagnostics.Stopwatch stopWatch = new System.Diagnostics.Stopwatch();
     
        stopWatch.Start();

        // System.Threading.Thread.Sleep(100);
        // TestMetod();

        stopWatch.Stop();

        TimeSpan ts = stopWatch.Elapsed;

        string elapsedTime = String.Format("{0:00}:{1:00}:{2:00}.{3:00}",ts.Hours, ts.Minutes, ts.Seconds,ts.Milliseconds / 10);

        Debug.Log("Тиков " + ts.Ticks);
    }
}
