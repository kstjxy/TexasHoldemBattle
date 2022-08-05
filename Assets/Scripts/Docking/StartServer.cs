using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.IO;
using System;
using UnityEngine;
using UnityEngine.UI;



public class StartServer : MonoBehaviour
{
    [Header("UI Components_InteractElements")]
    public Button serverButton;

    public static StartServer instance;

    private void Awake()
    {
        //单例模式初始化
        if (instance != null)
            Destroy(this);
        else
            instance = this;
    }

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
            for(int i=0;i<PlayerManager.instance.allPlayers.Count; i++)
            {
                Player p = PlayerManager.instance.allPlayers[i];
                if (p.type == Player.aiType.WebAI)
                {
                    PlayerManager.instance.allPlayers.Remove(p);
                    Destroy(InitialPanelManager.instance.panelRect.GetChild(i).gameObject);
                }
                    
            }
            tx.text = "Start Server";
        }
            
    }

}
