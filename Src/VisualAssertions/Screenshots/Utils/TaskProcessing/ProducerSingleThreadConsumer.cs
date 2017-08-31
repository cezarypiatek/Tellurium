using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;

namespace Tellurium.VisualAssertions.Screenshots.Utils.TaskProcessing
{
    internal class ProducerSingleThreadConsumer<T>:IDisposable
    {
        readonly BlockingCollection<T> dataToProcess = new BlockingCollection<T>();
        
        private static AutoResetEvent finishEvent = new AutoResetEvent(false);

        public ProducerSingleThreadConsumer(Action<T> action)
        {
            Task.Factory.StartNew(() =>
            {
                foreach (var data in dataToProcess.GetConsumingEnumerable())
                {
                    action(data);
                }
                finishEvent.Set();
            });

            AppDomain.CurrentDomain.DomainUnload += OnCurrentDomainOnDomainUnload;
        }

        private void OnCurrentDomainOnDomainUnload(object sender, EventArgs args)
        {
            AppDomain.CurrentDomain.DomainUnload -= OnCurrentDomainOnDomainUnload;
            this.Dispose();
        }

        public void Post(T data)
        {
            dataToProcess.Add(data);
        }

        private bool isDisposed;
        
        public void Dispose()
        {
            if (isDisposed == false)
            {
                dataToProcess.CompleteAdding();
                isDisposed = true;
                finishEvent.WaitOne();
            }
        }
    }
}