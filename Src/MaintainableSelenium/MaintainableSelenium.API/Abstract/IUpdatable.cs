namespace MaintainableSelenium.API.Abstract
{
    public interface IUpdatable<T>
    {
        void UpdateFrom(T entity);
    }
}