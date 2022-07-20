using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestAI_1 : BaseAI
{
    public override void OnInit(int n)
    {
        name = "Test "+n;
    }

    public override Player.Action BetAction()
    {
        int ranNum = new System.Random().Next(0, 101);
        //Debug.Log("随机值为：" + ranNum);
        //50% 跟注 或 过
        if (ranNum <= 50) return Player.Action.CALL;
        //25% 加注
        if (ranNum <= 75) return Player.Action.RAISE;
        //20% 弃牌
        if (ranNum <= 95) return Player.Action.FOLD;
        //5% ALL IN
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
