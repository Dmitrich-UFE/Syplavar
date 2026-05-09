public class EventMessage
{
    public string Tag;      // Например, "MonsterKilled" или "ItemPickedUp"
    public object Data;     // Сам объект (инвентарь, скрипт монстра и т.д.)

    public EventMessage(string tag, object data)
    {
        Tag = tag;
        Data = data;
    }
}
