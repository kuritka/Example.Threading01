using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using NCommon.Extensions;

namespace ThreadPerformance.Test
{
public partial class UnitTest1
{
    private sealed class State
    {
        public ManualResetEvent ResetEvent { get; set; }
        public long Load { get; set; }
    }

    private readonly Action<long> _threadAction = load =>
    {
        var k = 2;
        for (var j = 0; j < load; j++)
        {
            k += k * k + j;
        }
    };

    private readonly Action<Action<long>,State> _threadSuperAction 
        = (action, load) =>
    {
        action.Invoke(load.Load);
        load.ResetEvent.Set();
    };

    private double MultiThreadCompute(int threadCount, int threadLoad)
    {
        var list = new List<Thread>();

        Enumerable.Range(0, threadCount)
            .ForEach(d => list.Add(
                new Thread(load => _threadAction(threadLoad)) 
                { Name = string.Format("thread{0:D4}", d) }));

        var s1 = DateTime.Now;
        list.ForEach(d => d.Start(threadLoad));
        list.ForEach(d => d.Join());
        return (DateTime.Now - s1).TotalMilliseconds;
    }


    private double SingleThreadCompute(long threadLoad, Action<long> action)
    {
        var s1 = DateTime.Now;
        action(threadLoad);
        return (DateTime.Now - s1).TotalMilliseconds;
    }
        
    private double MultiThreadComputeWithThreadPool(int threadCount, long threadLoad)
    {
        var events = new List<State>(threadCount);
        ThreadPool.SetMaxThreads(threadCount, threadCount);
        for (int i = 0; i < threadCount; i++)
        {
            var state = new State {Load = threadLoad, ResetEvent 
                = new ManualResetEvent(false)};
            events.Add(state);
            ThreadPool.QueueUserWorkItem(o=>
                _threadSuperAction(_threadAction,state), state);
        }

        var s1 = DateTime.Now;
        WaitHandle.WaitAll( events.Select(d=>d.ResetEvent).ToArray());
        return (DateTime.Now - s1).TotalMilliseconds;
    }



    public double MultiThreadComputeWithTpl(int threadCount, long threadLoad)
    {
        var items = new List<long>();
        Enumerable.Range(0, threadCount).ForEach(d => items.Add(threadLoad));
        var s1 = DateTime.Now;
        Parallel.ForEach(items, item => _threadAction(item));
        return (DateTime.Now - s1).TotalMilliseconds;
    }

}
}
