using System;

namespace Tellurium.VisualAssertions.Screenshots.Utils.TaskProcessing
{
    public class AsynchronousTaskProcessor<T> : ITaskProcessor<T>
    {
        private ProducerSingleThreadConsumer<T> producerConsumer;

        public AsynchronousTaskProcessor(Action<T> processAction)
        {
            this.producerConsumer = new ProducerSingleThreadConsumer<T>(processAction);
        }

        public void Post(T data)
        {
            producerConsumer.Post(data);
        }

        public void Dispose()
        {
           producerConsumer.Dispose();
        }
    }
}