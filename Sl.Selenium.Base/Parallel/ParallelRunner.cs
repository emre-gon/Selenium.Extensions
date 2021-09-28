using Serilog;
using Serilog.Core;
using Sl.Extensions;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sl.Selenium.Parallel
{
    public enum QueueingOrder
    {
        /// <summary>
        /// first in first out (queue)
        /// </summary>
        FIFO,

        /// <summary>
        /// first in last out (stack)
        /// </summary>
        FILO,

        /// <summary>
        /// unordered list (bag)
        /// </summary>
        Unordered,

        /// <summary>
        /// unordered set (hashset)
        /// </summary>
        UnorderedSet
    }
    public class ParallelRunner<T>
    {
        private IProducerConsumerCollection<T> SearchQueue { get; }
        public int ParallelTaskCount { get; private set; }
        public bool StopSignal { get; private set; }
        public IParallelRunnableSpider<T> Spider { get; }




        private int searchQueueInitialCount;
        public ParallelRunner(IEnumerable<T> ToBeSearched,
            int ParallelTaskCount,
            IParallelRunnableSpider<T> Spider,
            QueueingOrder Order = QueueingOrder.FIFO)
        {


            switch (Order)
            {
                case QueueingOrder.FIFO:
                    SearchQueue = new ConcurrentQueue<T>();
                    break;
                case QueueingOrder.FILO:
                    SearchQueue = new ConcurrentStack<T>();
                    break;
                case QueueingOrder.Unordered:
                    SearchQueue = new ConcurrentBag<T>();
                    break;
                case QueueingOrder.UnorderedSet:
                    SearchQueue = new ConcurrentHashSet<T>();
                    break;
                default:
                    throw new Exception("Unknown Order " + Order);
            }


            
            foreach (var s in ToBeSearched)
            {
                SearchQueue.TryAdd(s);
            }

            searchQueueInitialCount = SearchQueue.Count;

            if (ParallelTaskCount > SearchQueue.Count)
                this.ParallelTaskCount = SearchQueue.Count;
            else
                this.ParallelTaskCount = ParallelTaskCount;

            this.Spider = Spider;
        }

        #region counters
        public int SuccessfulPasses(int TaskID)
        {
            return tasksDictionary[TaskID].SuccessfulRecordsPassed;
        }

        public int SuccessfulPasses()
        {
            return tasksDictionary.Sum(f => f.Value.SuccessfulRecordsPassed);
        }

        public int FailedPasses(int TaskID)
        {
            return tasksDictionary[TaskID].FailedRecordsPassed;
        }

        public int FailedPasses()
        {
            return tasksDictionary.Sum(f => f.Value.FailedRecordsPassed);
        }

        public int TotalPasses(int TaskID)
        {
            return tasksDictionary[TaskID].TotalRecordsPassed;
        }

        public int TotalPasses()
        {
            return tasksDictionary.Sum(f => f.Value.TotalRecordsPassed);
        }

        public int StillToDoCount()
        {
            return SearchQueue.Count;
        }

        public int TotalRecordsCount()
        {
            return searchQueueInitialCount;
        }

        public void Enqueue(T Item)
        {
            SearchQueue.TryAdd(Item);
            searchQueueInitialCount++;
        }


        #endregion


        Dictionary<int, TaskContainer> tasksDictionary = new Dictionary<int, TaskContainer>();

        object runLock = new object();
        public virtual void Run(ILogger Logger = null)
        {
            lock (runLock)
            {
                StopSignal = false;


                if (ParallelTaskCount > TotalRecordsCount())
                    this.ParallelTaskCount = TotalRecordsCount();

                if (Logger != null)
                    Logger.Information($"Running parallel with {this.ParallelTaskCount} threads.");

                object taskStarterLock = new object();
                for (int i = 0; i < this.ParallelTaskCount; i++)
                {
                    int TaskID = i;


                    Task task = Task.Factory.StartNew(() =>
                            ParallelLoop(TaskID), TaskCreationOptions.LongRunning);

                    tasksDictionary[TaskID] = new TaskContainer(TaskID, task);

                }

                Task.WaitAll(tasksDictionary.Select(f => f.Value.Task).ToArray());
            }
        }

        private object pLoopCounterLock = new object();
        private Task ParallelLoop(int TaskID)
        {
            try
            {
                while (SearchQueue.Any() && !StopSignal)
                {
                    SearchQueue.TryTake(out T Record);
                    try
                    {
                        bool isSuccess = Spider.RunSingleRecord(Record, TaskID);

                        lock (pLoopCounterLock)
                        {
                            if (isSuccess)
                            {
                                tasksDictionary[TaskID].SuccessfulRecordsPassed++;
                            }
                            else
                            {
                                tasksDictionary[TaskID].FailedRecordsPassed++;
                            }
                        }

                        Spider.AfterSingleRecord(Record, isSuccess, TaskID, SuccessfulPasses(), FailedPasses(), TotalRecordsCount());

                    }
                    catch (ReEnquableException)
                    {
                        SearchQueue.TryAdd(Record);
                        break;
                    }
                }
            }
            catch(Exception exc)
            {
                Console.WriteLine(exc);
            }

            return Task.CompletedTask;
        }


        public void Stop()
        {
            StopSignal = true;
        }
    }

    public abstract class ReEnquableException : Exception
    {
        //kalan threadler tamamlasın türündeki exceptionlar

        public ReEnquableException(string Message)
            : base(Message)
        {

        }
    }
}
