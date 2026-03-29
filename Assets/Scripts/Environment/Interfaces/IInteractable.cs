//Булевая переменная отвечает за то, нужно ли списывать предмет
//Возвращённый предмет может быть пустым, если нужно
using System.Collections.Generic;

public interface IInteractable
{
    (bool isDebitNeed, List<IItem> gettingItems) Interact(IItem item);
}