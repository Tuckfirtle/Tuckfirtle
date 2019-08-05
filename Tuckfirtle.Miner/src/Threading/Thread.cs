// Copyright (C) 2019, The Tuckfirtle Developers
// 
// Please see the included LICENSE file for more information.

using System;
using System.Diagnostics;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Threading;
using ThreadState = System.Threading.ThreadState;

namespace Tuckfirtle.Miner.Threading
{
    internal sealed class Thread
    {
        public int ProcessorAffinity { get; set; }

        public CultureInfo CurrentCulture
        {
            get => ManagedThread.CurrentCulture;
            set => ManagedThread.CurrentCulture = value;
        }

        public CultureInfo CurrentUICulture
        {
            get => ManagedThread.CurrentUICulture;
            set => ManagedThread.CurrentUICulture = value;
        }

        public ExecutionContext ExecutionContext => ManagedThread.ExecutionContext;

        public bool IsAlive => ManagedThread.IsAlive;

        public bool IsBackground
        {
            get => ManagedThread.IsBackground;
            set => ManagedThread.IsBackground = value;
        }

        public bool IsThreadPoolThread => ManagedThread.IsThreadPoolThread;

        public int ManagedThreadId => ManagedThread.ManagedThreadId;

        public string Name
        {
            get => ManagedThread.Name;
            set => ManagedThread.Name = value;
        }

        public ThreadPriority Priority
        {
            get => ManagedThread.Priority;
            set => ManagedThread.Priority = value;
        }

        public ThreadState ThreadState => ManagedThread.ThreadState;

        private System.Threading.Thread ManagedThread { get; }

        private ThreadStart ThreadStart { get; }

        private ParameterizedThreadStart ParameterizedThreadStart { get; }

        public Thread(ThreadStart threadStart) : this()
        {
            ThreadStart = threadStart;
        }

        public Thread(ThreadStart threadStart, int maxStackSize) : this(maxStackSize)
        {
            ThreadStart = threadStart;
        }

        public Thread(ParameterizedThreadStart threadStart) : this()
        {
            ParameterizedThreadStart = threadStart;
        }

        public Thread(ParameterizedThreadStart threadStart, int maxStackSize) : this(maxStackSize)
        {
            ParameterizedThreadStart = threadStart;
        }

        private Thread()
        {
            ManagedThread = new System.Threading.Thread(DistributedThreadStart);
        }

        private Thread(int maxStackSize)
        {
            ManagedThread = new System.Threading.Thread(DistributedThreadStart, maxStackSize);
        }

        [DllImport("kernel32.dll")]
        private static extern int GetCurrentThreadId();

        [DllImport("libc.so.6", EntryPoint = "sched_setaffinity")]
        private static extern int LinuxSetAffinity(int pid, IntPtr cpusetsize, ref ulong cpuset);

        public void Abort()
        {
            ManagedThread.Abort();
        }

        public void Abort(object stateInfo)
        {
            ManagedThread.Abort(stateInfo);
        }

        public void DisableComObjectEagerCleanup()
        {
            ManagedThread.DisableComObjectEagerCleanup();
        }

        public ApartmentState GetApartmentState()
        {
            return ManagedThread.GetApartmentState();
        }

        public override int GetHashCode()
        {
            return ManagedThread.GetHashCode();
        }

        public void Interrupt()
        {
            ManagedThread.Interrupt();
        }

        public void Join()
        {
            ManagedThread.Join();
        }

        public bool Join(int millisecondsTimeout)
        {
            return ManagedThread.Join(millisecondsTimeout);
        }

        public bool Join(TimeSpan timeout)
        {
            return ManagedThread.Join(timeout);
        }

        public void SetApartmentState(ApartmentState state)
        {
            ManagedThread.SetApartmentState(state);
        }

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

        public bool TrySetApartmentState(ApartmentState state)
        {
            return ManagedThread.TrySetApartmentState(state);
        }

        private void DistributedThreadStart(object parameter)
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
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

                        if (ProcessorAffinity > 0)
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
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                try
                {
                    System.Threading.Thread.BeginThreadAffinity();

                    if (ProcessorAffinity > 0)
                    {
                        var affinity = (ulong) ProcessorAffinity;
                        LinuxSetAffinity(0, new IntPtr(sizeof(ulong)), ref affinity);
                    }

                    if (ThreadStart != null)
                        ThreadStart();
                    else if (ParameterizedThreadStart != null)
                        ParameterizedThreadStart(parameter);
                    else
                        throw new InvalidOperationException();
                }
                finally
                {
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