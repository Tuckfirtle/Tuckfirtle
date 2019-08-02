using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading;

namespace Tuckfirtle.Miner.Threading.Windows
{
    internal sealed class Thread
    {
        public int ProcessorAffinity { get; set; }

        public System.Threading.Thread ManagedThread { get; }

        private ThreadStart ThreadStart { get; }

        private ParameterizedThreadStart ParameterizedThreadStart { get; }

        public Thread(ThreadStart threadStart) : this()
        {
            ThreadStart = threadStart;
        }

        public Thread(ParameterizedThreadStart threadStart) : this()
        {
            ParameterizedThreadStart = threadStart;
        }

        private Thread()
        {
            ManagedThread = new System.Threading.Thread(DistributedThreadStart);

            if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                throw new PlatformNotSupportedException();
        }

        [DllImport("kernel32.dll")]
        public static extern int GetCurrentThreadId();

        public void Start()
        {
            if (ThreadStart == null)
                throw new InvalidOperationException();

            ManagedThread.Start(null);
        }

        public void Start(object parameter)
        {
            if (ParameterizedThreadStart == null)
                throw new InvalidOperationException();

            ManagedThread.Start(parameter);
        }

        private void DistributedThreadStart(object parameter)
        {
            var currentThreadId = GetCurrentThreadId();
            var currentProcessThreads = Process.GetCurrentProcess().Threads;

            ProcessThread currentThread = null;

            foreach (ProcessThread currentProcessThread in currentProcessThreads)
            {
                if (currentProcessThread.Id != currentThreadId)
                    continue;

                currentThread = currentProcessThread;
                break;
            }

            if (currentThread != null)
            {
                try
                {
                    System.Threading.Thread.BeginThreadAffinity();

                    if (ProcessorAffinity != 0)
                        currentThread.ProcessorAffinity = new IntPtr(ProcessorAffinity);

                    if (ThreadStart != null)
                        ThreadStart();
                    else if (ParameterizedThreadStart != null)
                        ParameterizedThreadStart(parameter);
                    else
                        throw new InvalidOperationException();
                }
                finally
                {
                    currentThread.ProcessorAffinity = new IntPtr(0xFFFF);
                    System.Threading.Thread.EndThreadAffinity();
                }
            }
            else
            {
                if (ThreadStart != null)
                    ThreadStart();
                else if (ParameterizedThreadStart != null)
                    ParameterizedThreadStart(parameter);
                else
                    throw new InvalidOperationException();
            }
        }
    }
}