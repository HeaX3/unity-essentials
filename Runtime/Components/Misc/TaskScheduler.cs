using System;
using System.Collections;
using UnityEngine;

namespace Essentials
{
    public class TaskScheduler : MonoBehaviour
    {
        private static TaskScheduler _instance;

        private static TaskScheduler instance
        {
            get
            {
                if (!_instance) CreateInstance();
                return _instance;
            }
        }

        private static void CreateInstance()
        {
            var instance = new GameObject("Task Scheduler");
            instance.hideFlags |= HideFlags.HideInInspector | HideFlags.HideInHierarchy;
            DontDestroyOnLoad(instance);
            _instance = instance.AddComponent<TaskScheduler>();
        }

        private static IEnumerator DelayRoutine(Action action, uint timeoutMs)
        {
            yield return new WaitForSecondsRealtime(timeoutMs / 1000f);
            action();
        }

        public static Task RunLater(Action action, uint timeoutMs)
        {
            return new Task(instance.StartCoroutine(DelayRoutine(action, timeoutMs)));
        }

        public class Task
        {
            private readonly Coroutine coroutine;
            public bool isCancelled { get; private set; }

            internal Task(Coroutine coroutine)
            {
                this.coroutine = coroutine;
            }

            public void Cancel()
            {
                if (isCancelled) return;
                isCancelled = true;
                instance.StopCoroutine(coroutine);
            }
        }
    }
}