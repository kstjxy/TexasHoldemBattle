using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.IO;
using System;
using UnityEngine;
using UnityEngine.UI;
using System.Net;
using System.Net.NetworkInformation;

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

    public static bool PortInUse(int port)
    {
        bool inUse = false;

        IPGlobalProperties ipProperties = IPGlobalProperties.GetIPGlobalProperties();
        IPEndPoint[] ipEndPoints = ipProperties.GetActiveTcpListeners();

        foreach (IPEndPoint endPoint in ipEndPoints)
        {
            if (endPoint.Port == port)
            {
                inUse = true;
                break;
            }
        }

        return inUse;
    }
    /// <summary>
    /// 启动服务器
    /// </summary>
    public void ButtonClicked()
    {
        Text tx = serverButton.GetComponentInChildren<Text>();
        if (tx.text == "Start Server")
        {
            IPAddress ipTest;
            if (InitialPanelManager.instance.ipAdress.text == "")
            {
                InitialPanelManager.instance.CallWebLog("IP地址不能为空,自动填入本机地址!");
                //自动填入本机地址
                InitialPanelManager.instance.ipAdress.text = GlobalVar.ipAdress;
                return;
            }
            if (!IPAddress.TryParse(InitialPanelManager.instance.ipAdress.text, out ipTest))
            {
                InitialPanelManager.instance.CallWebLog("IP地址非法,自动填入本机地址!");
                //自动填入本机地址
                InitialPanelManager.instance.ipAdress.text = GlobalVar.ipAdress;
                return;
            }
            GlobalVar.ipAdress = InitialPanelManager.instance.ipAdress.text;

            if (InitialPanelManager.instance.portNum.text == "")
            {
                InitialPanelManager.instance.CallWebLog("端口号不能为空,自动填入可用端口!");
                //自动填入可用端口
                for (int i = 1025; i <= 65535; i++)
                {
                    if (!PortInUse(i))
                    {
                        GlobalVar.portNum = i;
                        break;
                    }
                }
                InitialPanelManager.instance.portNum.text = GlobalVar.portNum.ToString();
                return;
            }
            GlobalVar.portNum = int.Parse(InitialPanelManager.instance.portNum.text);
            if (GlobalVar.portNum < 1025 || GlobalVar.portNum > 65535)
            {
                InitialPanelManager.instance.CallWebLog("端口号非法,自动填入可用端口!");
                //自动填入可用端口
                for (int i = 1025; i <= 65535; i++)
                {
                    if (!PortInUse(i))
                    {
                        GlobalVar.portNum = i;
                        break;
                    }
                }
                InitialPanelManager.instance.portNum.text = GlobalVar.portNum.ToString();
                return;
            }
            GlobalVar.portNum = int.Parse(InitialPanelManager.instance.portNum.text);

            if (InitialPanelManager.instance.MaxPlayerNum.text == "")
            {
                InitialPanelManager.instance.CallWebLog("最大连接数不能为空，自动填入10!");
                //自动填入10
                InitialPanelManager.instance.MaxPlayerNum.text = GlobalVar.maxPlayerNum.ToString();
                return;
            }
            GlobalVar.maxPlayerNum = int.Parse(InitialPanelManager.instance.MaxPlayerNum.text);
            if (GlobalVar.maxPlayerNum <= 0)
            {
                InitialPanelManager.instance.CallWebLog("最大连接数必须为正数,自动填入10!");
                //自动填入10
                GlobalVar.maxPlayerNum = 10;
                InitialPanelManager.instance.ipAdress.text = GlobalVar.ipAdress;
                return;
            }
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
                    Destroy(InitialPanelManager.instance.panelRect.GetChild(i--+j++).gameObject);                    
                }
                    
            }
            tx.text = "Start Server";
        }
            
    }

}
