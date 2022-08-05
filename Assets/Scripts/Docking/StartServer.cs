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


    /// <summary>
    /// 启动服务器或关闭服务器
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
            int j = 0;
            for(int i=0;i<PlayerManager.instance.allPlayers.Count; i++)
            {
                Player p = PlayerManager.instance.allPlayers[i];
                if (p.type == Player.aiType.WebAI)
                {
                    PlayerManager.instance.allPlayers.Remove(p);
                    Destroy(InitialPanelManager.instance.panelRect.GetChild(i+j++).gameObject);
                }
                    
            }
            tx.text = "Start Server";
        }
            
    }

}
