using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseAI
{
    public string name = "my Name";

    public Player.Action BetAction()
    {
        return Player.Action.IDLE;
    }

    public virtual List<Card> FinalSelection()
    {
        return new List<Card>();
    }
}
