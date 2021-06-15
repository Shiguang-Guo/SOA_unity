using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class GameModeBtnScript : MonoBehaviour
{
    [SerializeField] private Text modeText;
    [SerializeField] private Button btn;

    public Button Btn => btn;

    public void SetButton(string title)
    {
        modeText.text = title;
    }
}
