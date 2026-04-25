using UnityEngine;
using System.Collections.Generic;

public class Bed : MonoBehaviour, IInteractable
{
    [SerializeField] private GameObject sleepingPlayer;
    [SerializeField] private GameObject Player;
    [SerializeField] private GameObject Cursor;
    private bool isTimeToSleep;
    private bool isPlayerSleeping;
    private Vector3 _playerPos;


    void Awake()
    {
        DayLightHandler._OnTimeReached += CheckTimeToSleep;
        isTimeToSleep = DayLightHandler.IsSleepTime;
        if (Player == null) Player = PlayerSeeker.GetPlayer();
    }

    void CheckTimeToSleep((int hh, int mm) time)
    {
        if (time.hh >= 18 || time.hh < 7)
        {
            isTimeToSleep = true;
        }
        else
        {
            isTimeToSleep = false;

            if (isPlayerSleeping)
            {
                Player.transform.position = _playerPos;
                sleepingPlayer.SetActive(false);
                Player.SetActive(true);
                Cursor.SetActive(true);
                isPlayerSleeping = false;
            }
        }
    }

    (bool, List<IItem>) IInteractable.Interact(IItem item)
    {
        isTimeToSleep = DayLightHandler.IsSleepTime;
        if (isTimeToSleep)
        {
            _playerPos = Player.transform.position; 
            DayLightHandler.SpeedupForSleep();
            sleepingPlayer.SetActive(true);
            Player.SetActive(false);
            Cursor.SetActive(false);
            Player.transform.position = this.transform.position;
            isPlayerSleeping = true;
        }
        else
        {
            ShowItemName.instance.ShowActItemText("Спать можно только ночью");
        }
        return (false, null);
    }

    void OnDestroy()
    {
        DayLightHandler._OnTimeReached -= CheckTimeToSleep;
    }

}
