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

    /*
    public  Player.Action BetAction()
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

    public  List<Card> FinalSelection()
    {
        List<Card> result = new List<Card>();
        result.AddRange(stats.CardsInHands);
        int ranNum = new System.Random().Next(0, 3);
        result.AddRange(stats.CommunityCards.GetRange(ranNum, 3));
        return result;
    }
    */
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
    }

}
