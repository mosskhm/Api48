using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static Api.DataLayer.DBQueries;
using static Api.Logger.Logger;

namespace Api.CommonFuncations
{

    public static class TaskManager
    {
        private static ConcurrentQueue<Task> running_tasks = new ConcurrentQueue<Task>();
        private static readonly object queueLock = new object();

        public static int max_threads = 250;

        // returns whether we actually removed any tasks
        private static bool RemoveCompletedTasks()
        {
            bool removed = false;
            lock (queueLock)
            {
                var still_running = running_tasks.Where(t => !t.IsCompleted).ToList();
                removed = still_running.Count < running_tasks.Count;           // raise removed flag if we have fewer tasks still running
                running_tasks = new ConcurrentQueue<Task>(still_running);
            }

            return removed;
        } // RemoveCompletedTasks

        // will wait for a task to finish from the list, or the timeout -- which ever is first
        private static bool WaitAnyTimeout(int timeout)
        {

            // remove any completed tasks
            if (RemoveCompletedTasks() && running_tasks.Count < max_threads) return false;       // we were able to remove enough tasks to bring us back below max_treads 

            Task timeoutTask = Task.Delay(TimeSpan.FromSeconds(timeout));
            Task completed = Task.WhenAny(Task.WhenAny(running_tasks), timeoutTask);

            return (completed == timeoutTask);      // return whether the task that exited was the timeout
        } // WaitAnyTimeout

        public static void ExecuteInAnotherThread(Action action, int wait_timeout = 10)
        {

            // block if we have too many threads running
            if (running_tasks.Count >= max_threads && WaitAnyTimeout(wait_timeout)) throw new Exception($"unable to execute - # of threads = {running_tasks.Count}, maximum = {max_threads} -- and one did not finish within {wait_timeout} seconds");

            Task task = Task.Run(action);
            lock (queueLock)
            {
                running_tasks.Enqueue(task);
            }
        }


        public static void WaitForAllTasksToFinish()
        {
            Task.WaitAll(running_tasks.ToArray());
        }
    }

} // namespace
