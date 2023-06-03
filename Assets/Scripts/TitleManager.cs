using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TitleManager : MonoBehaviour
{
    [SerializeField]
    public GameObject startButton;

    public void disableButton()
    {
        startButton.SetActive(false);
    }
}
