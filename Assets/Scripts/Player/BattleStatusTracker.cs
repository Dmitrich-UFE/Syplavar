using UnityEngine;

public class BattleStatusTracker : MonoBehaviour
{
    private bool battleMode;
    private int _MonstersInBattleMode;

    private static BattleStatusTracker instance;
    void Awake()
    {
        instance = this;
    }

    internal static int MonstersInBattleMode 
    {
        get {return instance._MonstersInBattleMode;}
        private set {instance._MonstersInBattleMode = value;} 
    }

    internal static bool BattleMode 
    {
        get
        {
            return instance.battleMode;
        }
        set
        {
            if (instance.battleMode != value)
            {
                instance.battleMode = value;

                if (instance.battleMode) 
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

    internal static void AddMonsterInBattleMode() 
    {++MonstersInBattleMode;}
    internal static void RemoveMonsterInBattleMode() 
    {
        --MonstersInBattleMode;
        if (MonstersInBattleMode < 0) MonstersInBattleMode = 0;
    }
}
