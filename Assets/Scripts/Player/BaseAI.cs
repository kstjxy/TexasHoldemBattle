using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseAI
{
    public string name = "my Name";

    public virtual void BetAction()
    {
        
    }

    public virtual List<Card> FinalSelection()
    {
        return new List<Card>();
    }
}
