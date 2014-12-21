using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace TicketingSystem.E2ETest
{
    /// <summary>
    /// Class to handle waiting on multiple threads.  Referenced from: http://stackoverflow.com/questions/2281926/c-sharp-waiting-for-multiple-threads-to-finish
    /// </summary>
    public class CountDownLatch
    {
        private int remain;
        private EventWaitHandle eventWaitHandle;

        public CountDownLatch(int count)
        {
            Reset(count);
        }

        public void Reset(int count)
        {
            if (0 > count)
            {
                throw new ArgumentOutOfRangeException();
            }

            remain = count;
            eventWaitHandle = new ManualResetEvent(false);

            if (0 == remain)
            {
                eventWaitHandle.Set();
            }
        }

        public void Signal()
        {
            if (0 == Interlocked.Decrement(ref remain))
            {
                eventWaitHandle.Set();
            }
        }

        public void Wait()
        {
            eventWaitHandle.WaitOne();
        }
    }
}