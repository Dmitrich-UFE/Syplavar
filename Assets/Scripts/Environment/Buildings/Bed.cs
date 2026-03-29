using UnityEngine;
using System.Collections.Generic;

public class Bed : MonoBehaviour, IInteractable
{
    [SerializeField] private GameObject sleepingPlayer;
    [SerializeField] private GameObject Player;
    private bool isTimeToSleep;
    private bool isPlayerSleeping;
    private Vector3 _playerPos;


    void Awake()
    {
        DayLightHandler._OnTimeReached += CheckTimeToSleep;
    }

    void CheckTimeToSleep((int hh, int mm) time)
    {
        switch (time)
        {
            case (18, 00):
                isTimeToSleep = true;
            break;
            case (00, 00):
                isTimeToSleep = true;
            break;
            case (02, 00):
                isTimeToSleep = true;
            break;
            case (06, 00):
                isTimeToSleep = true;
            break;
            case (7, 0):
                isTimeToSleep = false;

                if (isPlayerSleeping)
                {
                    Player.transform.position = _playerPos;
                    sleepingPlayer.SetActive(false);
                    Player.SetActive(true);
                    isPlayerSleeping = false;
                }
            break;
        }
    }

    (bool, List<IItem>) IInteractable.Interact(IItem item)
    {
        if (isTimeToSleep)
        {
            _playerPos = Player.transform.position; 
            DayLightHandler.SpeedupForSleep();
            sleepingPlayer.SetActive(true);
            Player.SetActive(false);
            Player.transform.position = this.transform.position;
            isPlayerSleeping = true;
        }
        return (false, null);
    }



}
