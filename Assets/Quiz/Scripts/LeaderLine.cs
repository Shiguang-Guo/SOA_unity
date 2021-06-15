using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LeaderLine
{
    public LeaderLine(string user, string mode, string score) :this(){
        this.UserName = user;
        this.ModeName = mode;
        this.Score = score;
    }
    public LeaderLine()
    {
        
    }
    public string UserName { get; set; }
    public string ModeName { get; set; }
    public string Score { get; set; }
}