using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerObject : MonoBehaviour
{
    [Header("PlayerInfo")]
    public Player player;

    [Header("PlayerPanelElements")]
    public Text nameText;
    public Text coinsText;
    public Image card1Image;
    public Image card2Image;

    /// <summary>
    /// ��PlayerObject���г�ʼ��
    /// </summary>
    /// <param name="p">��֮������Player����</param>
    /// <param name="coins">��ʼ��������</param>
    public void InitializeThePlayer(Player p, int coins)
    { 
    
    }

    /// <summary>
    /// ��ע�⣺���ø÷���ʱ��player�Ѿ����ù�.AddPlayerCards()
    /// </summary>
    public void ShowCards()
    { 
    
    }
}
