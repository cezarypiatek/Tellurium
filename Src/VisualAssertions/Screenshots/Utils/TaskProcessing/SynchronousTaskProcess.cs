using System;

namespace Tellurium.VisualAssertions.Screenshots.Utils.TaskProcessing
{
    class SynchronousTaskProcess<T>:ITaskProcessor<T>
    {
        private readonly Action<T> processAction;

        public SynchronousTaskProcess(Action<T> processAction)
        {
            this.processAction = processAction;
        }

        public void Post(T data)
        {
            processAction(data);
        }

        public void Dispose()
        {
        }
    }
}