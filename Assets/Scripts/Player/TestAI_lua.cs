using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestAI_lua : BaseAI
{
    public override void OnInit(string n)
    {
        name = n;
    }

    public override Player.Action BetAction(int action)
    {
        if (action == 1) return Player.Action.CALL;
        if (action == 2) return Player.Action.RAISE;
        if (action == 3) return Player.Action.FOLD;
        if (action == 3) return Player.Action.ALL_IN;
        string bug = "玩家【" + name + "】所作操作不合法！默认弃牌！";
        Debug.Log(bug);
        UIManager.instance.PrintLog(bug);
        return Player.Action.FOLD;
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
