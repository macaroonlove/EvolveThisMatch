namespace ScriptableObjectArchitecture
{
    public interface IGameEventListener<T, Y>
    {
        void OnEventRaised(T value, Y value2);
    }

    public interface IGameEventListener<T>
    {
        void OnEventRaised(T value);
    }
    public interface IGameEventListener
    {
        void OnEventRaised();
    } 
}