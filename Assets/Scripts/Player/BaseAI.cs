using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseAI
{
    public string name = "my Name";
    public GameStat stats;    


    public virtual void OnInit(string n)
    {
        this.name ="Test_" + (Random.Range(100,900)).ToString();
    }

    public virtual Player.Action BetAction(int action)
    {
        return Player.Action.IDLE;
    }

    public virtual List<Card> FinalSelection()
    {
        return new List<Card>();
    }    
}
