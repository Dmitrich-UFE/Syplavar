using UnityEngine;
using UnityEngine.InputSystem;

public class WaitForInputAction : CustomYieldInstruction
{
    private readonly InputAction _action;
    private bool _wasTriggered;

    // Инструкция будет "ждать", пока keepWaiting возвращает true
    public override bool keepWaiting
    {
        get
        {
            // Если кнопка нажата в этом кадре, помечаем, что сработало
            if (_action.triggered)
            {
                _wasTriggered = true;
            }
            
            // Ждем, пока не сработает триггер
            return !_wasTriggered;
        }
    }

    public WaitForInputAction(InputAction action)
    {
        _action = action;
        _wasTriggered = false;
    }
}
