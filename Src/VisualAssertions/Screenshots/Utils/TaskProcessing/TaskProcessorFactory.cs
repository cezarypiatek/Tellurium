using System;

namespace Tellurium.VisualAssertions.Screenshots.Utils.TaskProcessing
{
    static class TaskProcessorFactory
    {
        public static ITaskProcessor<T> Create<T>(TaskProcessorType processorType, Action<T> processAction)
        {
            switch (processorType)
            {
                case TaskProcessorType.Sync:
                    return new SynchronousTaskProcess<T>(processAction);
                case TaskProcessorType.Async:
                    return new AsynchronousTaskProcessor<T>(processAction);
                default:
                    throw new ArgumentOutOfRangeException(nameof(processorType), processorType, null);
            }
        }
    }

    enum TaskProcessorType
    {
        Sync=1,
        Async
    }
}