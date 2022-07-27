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
    public ITest test;

    public  void OnInit(string file)
    {
        this.file = file;
        env = new LuaEnv();
        env.AddLoader(MyLoader);
        env.DoString("require 'file'");
        test = env.Global.Get<ITest>("M");
        name = test.name;
    }

    public void StartGame()
    {
        test.startfunction(stats);
        Debug.Log(name + "初始化成功！");
    }

    public void RoundStart()
    {
        test.round_start(stats);
        Debug.Log(name + "新一轮已就绪！");
    }

    public  Player.Action BetAction()
    {
        int act = test.action(stats);
        if (act == 1) return Player.Action.CALL;
        if (act == 2) return Player.Action.RAISE;
        if (act == 3) return Player.Action.FOLD;
        if (act == 4) return Player.Action.ALL_IN;
        string bug = "玩家【" + name + "】所作操作不合法！默认弃牌！";
        Debug.Log(bug);
        UIManager.instance.PrintLog(bug);
        return Player.Action.FOLD;
    }

    public  List<Card> FinalSelection()
    {
        List<int> cardNum = new List<int>();        
        List<Card> result = new List<Card>();
        cardNum = test.finalCards(stats);

        foreach (int i in cardNum)
        {
            if (i < 2)
                result.AddRange(stats.CardsInHands.GetRange(i, 1));
            else
                result.AddRange(stats.CommunityCards.GetRange(i-2, 1));
        };
        return result;
    }

    public byte[] MyLoader(ref string filepath)
    {
        return System.Text.Encoding.UTF8.GetBytes(File.ReadAllText(this.file));
    }


    [CSharpCallLua]
    public interface ITest
    {
        string name { get;}
        int myaction { get; set; }
        void startfunction(GameStat stats);
        void round_start(GameStat stats);
        int action(GameStat stats);
        List<int> finalCards(GameStat stats);
    }
}
