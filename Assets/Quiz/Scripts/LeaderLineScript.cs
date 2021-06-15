using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class LeaderLineScript : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] private Text userText;
    [SerializeField] private Text modeText;
    [SerializeField] private Text scoreText;

    // Update is called once per frame
    public void SetLine(string user, string mode, string score)
    {
        userText.text = user;
        modeText.text = mode;
        scoreText.text = score;
    }
}
