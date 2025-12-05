using UnityEngine;
using System.Collections.Generic;


public class exit : MonoBehaviour
{
    public void Exit()
    {
        Application.Quit();
        Debug.Log("выход");
    }
}
