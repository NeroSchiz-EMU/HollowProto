using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Win : MonoBehaviour
{
    public Canvas WinScreen;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            TriggerWin();
        }
    }

    public void OnBossDroneDestroyed()
    {
        TriggerWin();
    }

    private void TriggerWin()
    {
        WinScreen.gameObject.SetActive(true);
        Time.timeScale = 0;
    }
}



//OLD CODE

//public class Win : MonoBehaviour
//{
//    public Canvas WinScreen;

//    private void OnTriggerEnter2D(Collider2D collision)
//    {
//        if (collision.tag == "Player")
//        {
//            WinScreen.gameObject.SetActive(true);
//            Time.timeScale = 0;
//        }

//    }
//}
