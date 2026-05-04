using System;

public static class EventManager
{
    // Событие, на которое будут подписываться квесты
    public static event Action<EventMessage> OnEventHappened;

    public static void SendEvent(string tag, object data)
    {
        OnEventHappened?.Invoke(new EventMessage(tag, data));
    }
}
