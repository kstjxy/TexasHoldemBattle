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
    public string file; //AI�ļ�·��
    public ITest test; //Interface�ӿ�

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
            string bug = fileDetail[fileDetail.Length - 1] + "��ʼ��ʧ��";
            Debug.Log(bug);
        }
        
        name = test.name;
    }

    public void StartGame()
    {
        test.startfunction(stats);
        Debug.Log(name + "��ʼ���ɹ���");
    }

    public void RoundStart()
    {
        test.round_start(stats);
        Debug.Log(name + "��һ���Ѿ�����");
    }

    //1:��ע��2����ע��3�����ƣ�4��ALLIN
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
            string bug = "��ҡ�" + name + "�������������Ϸ���Ĭ�����ƣ�";
            Debug.Log(bug);
            UIManager.instance.PrintLog(bug);
            return 3; //����������������
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