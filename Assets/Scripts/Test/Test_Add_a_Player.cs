using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.IO;
using System;
using UnityEngine;
using UnityEngine.UI;



public class Test_Add_a_Player : MonoBehaviour
{
    [Header("UI Components_InteractElements")]
    public Button serverButton;

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
        Text tx = serverButton.GetComponentInChildren<Text>();
        print(tx.text);
        if (tx.text == "Start Server")
        {
            
            GlobalVar.ipAdress = InitialPanelManager.instance.ipAdress.text;
            GlobalVar.portNum = int.Parse(InitialPanelManager.instance.portNum.text);
            GlobalVar.maxPlayerNum = int.Parse(InitialPanelManager.instance.MaxPlayerNum.text);
            if (!WebServer.instance.StartServer(GlobalVar.ipAdress, GlobalVar.portNum, GlobalVar.maxPlayerNum))
                tx.text = "Start Server";
            else
                tx.text = "Server Started !";
        }
        else
        {
            WebServer.instance.CloseServer();
            tx.text = "Start Server";
        }
            
    }

}
