using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using XLua;

public class BaseAI
{
    public string name = "my Name";
    public GameStat stats;
    public LuaEnv env;
    public string file;

    public  void OnInit(string file)
    {
        this.file = file;
        env = new LuaEnv();
        env.AddLoader(MyLoader);
        env.DoString("require 'file'");

        //初始化
        GetM = env.Global.Get<IM>("M");
        name = GetM.startfunction(stats);
    }

    
    public  Player.Action BetAction()
    {
        int act = GetM.action(stats);
        if (act == 1) return Player.Action.CALL;
        if (act == 2) return Player.Action.RAISE;
        if (act == 3) return Player.Action.FOLD;
        if (act == 3) return Player.Action.ALL_IN;
        string bug = "玩家【" + name + "】所作操作不合法！默认弃牌！";
        Debug.Log(bug);
        UIManager.instance.PrintLog(bug);
        return Player.Action.FOLD;
    }

    public  List<Card> FinalSelection()
    {
        List<int> cardNum = new List<int>();        
        List<Card> result = new List<Card>();
        cardNum = GetM.finalCards(stats);

        foreach (int i in cardNum)
        {
            if (i < 2)
                result.AddRange(stats.CardsInHands.GetRange(i, 1));
            else
                result.AddRange(stats.CommunityCards.GetRange(i, 1));
        };
        return result;
    }
    

    //调用lua脚本中的方法
    //delegate string startfunction();    
    //delegate void round_start();
    //delegate int Action();
    //delegate int finalCards();
    //startfunction startfunction = null;
    //round_start round_start = null;
    //Action action1 = null;
    //finalCards finalCards = null;

    [CSharpCallLua]
    public interface IM
    {
        string name { get; set; }
        int myaction { get; set; }
        string startfunction(GameStat stats);
        void round_start(GameStat stats);
        int action(GameStat stats);
        List<int> finalCards(GameStat stats);
    }
    IM GetM;

    public byte[] MyLoader(ref string filepath)
    {
        return System.Text.Encoding.UTF8.GetBytes(File.ReadAllText(this.file));
    }

}
