using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseAI
{
    public string name = "my Name";
    public GameStat stats;

    public virtual void OnInit()
    {
        this.name = "my name";
    }

    public virtual Player.Action BetAction()
    {
        return Player.Action.IDLE;
    }

    public virtual List<Card> FinalSelection()
    {
        return new List<Card>();
    }
}
