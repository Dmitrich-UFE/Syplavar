using UnityEngine;
using System.Collections;

public class TaskScriptLast : MonoBehaviour
{
    [SerializeField] private Task task;
    [SerializeField] private TaskManager taskManager;

    [SerializeField] private VNLDialogueWindow vnl;
    [SerializeField] private TextAsset text;
    WaitForSecondsRealtime tick = new WaitForSecondsRealtime(2f);
    Coroutine cor;

    void OnEnable()
    {
        if (taskManager.Status % 2 == 0) 
        {
            taskManager.CloseBlackPanel();
        }
        else
        {
            if (BattleStatusTracker.MonstersInBattleMode != 0) BattleStatusTracker.RemoveMonsterInBattleMode();
            BattleStatusTracker.BattleMode = BattleStatusTracker.MonstersInBattleMode != 0;
            
            taskManager.OpenBlackPanelNoFade();
            vnl.StartPrint(text);
            cor = StartCoroutine(checkStatus());
        }
    }

    IEnumerator checkStatus()
    {
        while (!(vnl.Status == VNLprintStatus.ended))
        {
            yield return tick;
        }

        taskManager.Status *= 2;
        taskManager.CloseBlackPanel();
    }
}
