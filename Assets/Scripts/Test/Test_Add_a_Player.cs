using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test_Add_a_Player : MonoBehaviour
{
    /// <summary>
    /// ������ʹ��
    /// </summary>
    public void ButtonClicked()
    {
        BaseAI ai = new BaseAI();
        ai.OnInit();
        Player p = new Player(ai);
        PlayerManager.instance.allPlayers.Add(p);
        InitialPanelManager.instance.AddSelectablePlayerButton(p);//��һ�䲻Ӧ��д�������
    }
}
