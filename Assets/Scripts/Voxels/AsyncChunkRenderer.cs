using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class AsyncChunkRenderer : MonoBehaviour
{
    public static AsyncChunkRenderer Instance;
    public ConcurrentQueue<Action> TasksQueue = new ConcurrentQueue<Action>();

    private Thread[] Threads;
    private bool Stop;

    private void Awake()
    {
        Instance = this;
        Threads = new Thread[Environment.ProcessorCount];
        for (int i = 0; i < Threads.Length; ++i)
        {
            Threads[i] = new Thread(Consume);
            Threads[i].Start();
        }
    }

    private void Consume()
    {
        while (!Stop)
        {
            if (TasksQueue.TryDequeue(out Action action))
            {
                action();
            }
            Thread.Sleep(20);
        }
    }

    private void OnDestroy()
    {
        Stop = true;
        for (int i = 0; i < Threads.Length; ++i)
        {
            Threads[i].Abort();
        }
    }
    private void OnApplicationQuit()
    {
        Stop = true;
        for (int i = 0; i < Threads.Length; ++i)
        {
            Threads[i].Abort();
        }
    }
}
