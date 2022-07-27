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
        env.DoString("require 'test01'");
        
        name = "hahaha";
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

    //调用lua脚本中的方法
    delegate string Start();
    Start start = null;
    delegate string Bet();
    Bet bet = null;
    public byte[] MyLoader(ref string filepath)
    {
        Debug.Log(filepath);
        Debug.Log(Application.streamingAssetsPath);
        //string absPath = Application.streamingAssetsPath
        return System.Text.Encoding.UTF8.GetBytes(File.ReadAllText(filepath));
    }

}
