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
        TestAI_1 ai = new();
        ai.OnInit(GlobalVar.roboName);
        GlobalVar.roboName++;
        Player p = new(ai);
        GameStat gs = new(p);
        ai.stats = gs;
        PlayerManager.instance.allPlayers.Add(p);
        InitialPanelManager.instance.AddSelectablePlayerButton(p);//��һ�䲻Ӧ��д�������
    }
}
