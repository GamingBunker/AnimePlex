using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Cesxhin.AnimeSaturn.Application.Parallel
{
    public class ParallelManager<T> where T : class
    {
        private readonly int NUMBER_PARALLEL_MAX = int.Parse(Environment.GetEnvironmentVariable("LIMIT_THREAD_PARALLEL") ?? "5");
        private readonly List<Func<T>> queue = new();
        private readonly List<Task> tasks = new();

        private List<T> list = new List<T>();

        public ParallelManager()
        {
            Task.Run(() => Process());
        }

        private async void Process()
        {
            //thread for download parallel
            int capacity = 0;
            int count = 0;
            int lastTriggerTime = 0;
            int intervalCheck;

            while (true)
            {
                //add task
                if (capacity < NUMBER_PARALLEL_MAX && queue.Count > 0)
                {
                    var task = queue[0];

                    try
                    {
                        tasks.Add(Task.Run(task));
                    }
                    catch (ArgumentNullException ex)
                    {
                        list = null;
                        break;
                    }

                    capacity++;

                    queue.Remove(task);
                }

                //must remove one task for continue download
                do
                {
                    List<Task> removeTask = new();
                    foreach (var task in tasks)
                    {
                        if (task.IsCompleted)
                        {
                            var objectBuffer = ((Task<T>)task).Result;

                            list.Add(objectBuffer);
                            count++;

                            capacity--;
                            removeTask.Add(task);
                        }
                    }

                    //remove rask completed
                    foreach (var task in removeTask)
                    {
                        tasks.Remove(task);
                    }

                    //send only one data every 3 seconds
                    intervalCheck = DateTime.Now.Second;
                    if (lastTriggerTime > intervalCheck)
                        lastTriggerTime = 3;

                    if (intervalCheck % 3 == 0 && (intervalCheck - lastTriggerTime) >= 3)
                    {
                        lastTriggerTime = DateTime.Now.Second;
                    }

                } while (capacity >= NUMBER_PARALLEL_MAX);
            }
        }

        public void AddTask(Func<T> run)
        {
            queue.Add(run);
        }

        public bool CheckFinish()
        {
            if(tasks.Count == 0 && queue.Count == 0)
                return true;
            else
                return false;
        }

        public void WhenCompleted()
        {
            while(tasks.Count != 0 || queue.Count != 0)
            {

            }
        }

        public int PercentualCompleted()
        {
            return (list.Count * 100)/(list.Count + tasks.Count + queue.Count); //43 : 100 = 4 : x
        }

        public List<T> GetResult()
        {
            return list;
        }
    }
}
