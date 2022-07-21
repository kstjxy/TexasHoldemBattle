using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestAI_1 : BaseAI
{
    public override void OnInit(int n)
    {
        name = "Test_"+n;
    }

    public override Player.Action BetAction()
    {
        int ranNum = new System.Random().Next(0, 101);
        Debug.Log("Ëæ»úÖµÎª£º" + ranNum);
        if (ranNum <= 50) return Player.Action.CALL;
        if (ranNum <= 90) return Player.Action.RAISE;
        if (ranNum <= 98) return Player.Action.FOLD;
        return Player.Action.ALL_IN;
    }

    public override List<Card> FinalSelection()
    {
        List<Card> result = new List<Card>();
        result.AddRange(stats.CardsInHands);
        int ranNum = new System.Random().Next(0, 3);
        result.AddRange(stats.CommunityCards.GetRange(ranNum, 3));
        return result;
    }
}
