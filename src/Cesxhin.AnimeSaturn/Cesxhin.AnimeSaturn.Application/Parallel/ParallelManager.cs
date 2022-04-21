using Cesxhin.AnimeSaturn.Application.NlogManager;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Cesxhin.AnimeSaturn.Application.Parallel
{
    public class ParallelManager<T> where T : class
    {
        //env
        private readonly int NUMBER_PARALLEL_MAX = int.Parse(Environment.GetEnvironmentVariable("LIMIT_THREAD_PARALLEL") ?? "5");

        //variable
        private List<Func<T>> queue = new();
        private readonly List<Task> tasks = new();

        //nlog
        private readonly NLogConsole _logger = new(LogManager.GetCurrentClassLogger());

        //completed
        private List<T> list = new();

        //task id
        private Task process;

        private void Process()
        {
            //thread for download parallel
            int capacity = 0;
            int count = 0;

            while (queue.Count != 0 || tasks.Count != 0)
            {
                //add task
                if (capacity < NUMBER_PARALLEL_MAX && queue.Count > 0)
                {
                    var task = queue.First();

                    if(task != null)
                    {
                        tasks.Add(Task.Run(task));
                        capacity++;
                        queue.Remove(task);
                    }
                    else
                    {
                        _logger.Error($"Problem task null");
                    }
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

                } while (capacity >= NUMBER_PARALLEL_MAX);
            }
        }

        public void Start()
        {
            process = Task.Run(() => Process());
        }

        public void AddTasks(List<Func<T>> tasks)
        {
            queue = tasks;
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
            Task.WaitAll(process);
        }

        public int PercentualCompleted()
        {
            if (list == null)
                return 0;
            return (list.Count * 100)/(list.Count + tasks.Count + queue.Count); //43 : 100 = 4 : x
        }

        public List<T> GetResult()
        {
            return list;
        }
        public List<T> GetResultAndClear()
        {
            List<T> copy = new(list);
            list.Clear();

            return copy;
        }

        public void ClearList()
        {
            list.Clear();
        }
    }
}
