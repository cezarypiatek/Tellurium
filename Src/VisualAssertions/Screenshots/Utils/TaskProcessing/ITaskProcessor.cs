using System;

namespace Tellurium.VisualAssertions.Screenshots.Utils.TaskProcessing
{
    public interface ITaskProcessor<T> : IDisposable
    {
        void Post(T data);
    }
}