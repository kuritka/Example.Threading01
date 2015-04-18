using System;
using System.Linq;
using NUnit.Framework;

namespace ThreadPerformance.Test
{


[TestFixture]
partial class UnitTest1
{

    private readonly  int[] _threadCounts =
    {
        1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 13, 14, 15, 16, 17, 18, 
        19, 20, 21, 22, 23, 24, 32, 48, 64, 128, 256, 512, 1024,2048
        , 4096, 8192, 16384, 32768, 65536
    };

    [Test]
    public void ThreadClassTest()
    {
        const int load = 10 * 1000 * 1000;
        foreach (var threadCount in _threadCounts)
        {
            var multiThreadTotalMiliseconds = MultiThreadCompute(threadCount, load / threadCount);
            var singleThreadTotalMiliseconds = SingleThreadCompute(load, _threadAction);
            Console.WriteLine("MultiThread ({0:D5}): {1}ms; SingleThread: {2}ms", threadCount, 
                multiThreadTotalMiliseconds, singleThreadTotalMiliseconds);
        }
    }
        
    [Test]
    public void ParalelForEachTest()
    {
        const int load = 10 * 1000 * 1000;
            
        foreach (var threadCount in _threadCounts)
        {
            var multiThreadTotalMiliseconds = MultiThreadComputeWithTpl(threadCount, load / threadCount);
            var singleThreadTotalMiliseconds = SingleThreadCompute(load, _threadAction);
            Console.WriteLine("Pool MultiThread ({0:D5}): {1}ms; SingleThread: {2}ms", threadCount, 
                multiThreadTotalMiliseconds, singleThreadTotalMiliseconds);
        }
    }

    [Test]
    public void ThreadPoolTest()
    {
        const int load = 10 * 1000 * 1000;
         
        foreach (var threadCount in _threadCounts.Where(d=>d <=64))
        {
            var multiThreadTotalMiliseconds = MultiThreadComputeWithThreadPool(threadCount, load / threadCount);
            var singleThreadTotalMiliseconds = SingleThreadCompute(load , _threadAction);
            Console.WriteLine("Pool MultiThread ({0:D5}): {1}ms; SingleThread: {2}ms", threadCount, 
                multiThreadTotalMiliseconds, singleThreadTotalMiliseconds);
        }
    }
}
}
