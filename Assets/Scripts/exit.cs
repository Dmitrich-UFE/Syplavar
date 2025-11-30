using UnityEngine;
using System.Collections.Generic;
using UnityEngine;


public class exit : MonoBehaviour
{
    public void Exit()
    {
        Application.Quit();
        Debug.Log("выход");
    }
}
