using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.IO;
using System;
using UnityEngine;
using XLua;



public class Test_Add_a_Player : MonoBehaviour
{

    //原版
    /// <summary>
    /// 纯测试使用
    /// </summary>
    //public void ButtonClicked()
    //{
    //    TestAI_1 ai = new();
    //    ai.OnInit(GlobalVar.roboName);
    //    GlobalVar.roboName++;
    //    Player p = new(ai);
    //    GameStat gs = new(p);
    //    ai.stats = gs;
    //    PlayerManager.instance.allPlayers.Add(p);
    //    InitialPanelManager.instance.AddSelectablePlayerButton(p);//这一句不应该写在这里！！
    //}

    /// <summary>
    /// 启动服务器
    /// </summary>
    public void ButtonClicked()
    {
        GlobalVar.ipAdress = InitialPanelManager.instance.ipAdress.text;
        GlobalVar.portNum = int.Parse(InitialPanelManager.instance.portNum.text);
        WebServer.instance.StartServer(GlobalVar.ipAdress, GlobalVar.portNum);
    }

}
