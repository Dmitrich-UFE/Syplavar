using UnityEngine;

public class BattleStatusTracker : MonoBehaviour
{
    private static bool battleMode;

    internal static bool BattleMode 
    {
        get
        {
            return battleMode;
        }
        set
        {
            if (battleMode != value)
            {
                battleMode = value;

                if (battleMode) 
                    _OnBattleModeOn?.Invoke(); 
                else
                    _OnBattleModeOff?.Invoke();
            }
        }
    }


    internal delegate void OnBattleModeOn();
    internal static event OnBattleModeOn _OnBattleModeOn;

    internal delegate void OnBattleModeOff();
    internal static event OnBattleModeOff _OnBattleModeOff;

    internal static void SetBattleMode(bool _battleMode)
    {
        BattleMode = _battleMode;
    }
}
