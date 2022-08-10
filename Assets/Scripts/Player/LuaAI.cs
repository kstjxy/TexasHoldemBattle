using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using XLua;

public class LuaAI
{
    public string name = "my Name";
    public GameStat stats;
    public LuaEnv env;
    public Player player;
    public string file; //AI文件路径
    public ITest test; //Interface接口

    public void OnInit(string file)
    {
        this.file = file;
        env = new LuaEnv();
        env.AddLoader(MyLoader);
        env.DoString("require 'file'");
        try
        {
            test = env.Global.Get<ITest>("M");
        }
        catch(Exception e)
        {
            string[] fileDetail = file.Split('\\');
            string bug = fileDetail[fileDetail.Length - 1] + "初始化失败";
            Debug.Log(bug);
        }
        
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

    //1:跟注；2：加注；3：弃牌；4：ALLIN
    public int BetAction()
    {
        int act = test.action(stats);
        Debug.Log(name + act);
        if (act > 0 && act < 5)
        {
            return act;
        }
        else
        {
            string bug = "玩家【" + name + "】所作操作不合法！默认弃牌！";
            Debug.Log(bug);
            UIManager.instance.PrintLog(bug);
            return 3; //如果操作错误就弃牌
        }
    }

    public List<Card> FinalSelection()
    {
        return test.finalCards(stats);
    }

    public byte[] MyLoader(ref string filepath)
    {
        return System.Text.Encoding.UTF8.GetBytes(File.ReadAllText(this.file));
    }


    [CSharpCallLua]
    public interface ITest
    {
        string name { get; }
        void startfunction(GameStat stats);
        void round_start(GameStat stats);
        int action(GameStat stats);
        List<Card> finalCards(GameStat stats);
    }
}