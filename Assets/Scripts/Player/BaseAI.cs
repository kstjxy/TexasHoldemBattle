using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using System.Net.Sockets;
using System.Net;
using System;

public class BaseAI
{
    public string name = "my Name";
    public GameStat stats;
    public Socket server = WebServer.instance.server;
    public string file;
    string recivefrom;
    string sendto;
    public ITest test;

    public  void OnInit(Socket socketsend)
    {
        
        
        //name = test.name;
    }

    public void StartGame()
    {
        sendto = "Game Start";
        server.Send(sendto.)
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
        if (act > 0 && act < 5)
        {
            return act;
        } else
        {
            string bug = "玩家【" + name + "】所作操作不合法！默认弃牌！";
            Debug.Log(bug);
            UIManager.instance.PrintLog(bug);
            return 3; //如果操作错误就弃牌
        }
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
        //测试用 改完了删了就行
        List<int> finalCards(GameStat stats);
        //List<List<int>> finalCards(GameStat stats);
    }
}
