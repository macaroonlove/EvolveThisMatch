namespace ScriptableObjectArchitecture
{
    public interface IGameEvent<T, Y>
    {
        void Raise(T value, Y value2);
        void AddListener(IGameEventListener<T, Y> listener);
        void RemoveListener(IGameEventListener<T, Y> listener);
        void RemoveAll();
    }

    public interface IGameEvent<T>
    {
        void Raise(T value);
        void AddListener(IGameEventListener<T> listener);
        void RemoveListener(IGameEventListener<T> listener);
        void RemoveAll();
    }
    public interface IGameEvent
    {
        void Raise();
        void AddListener(IGameEventListener listener);
        void RemoveListener(IGameEventListener listener);
        void RemoveAll();
    } 
}