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
        Player p = new Player("Test" + Random.Range(0.1f, 3.2f).ToString());
        PlayerManager.instance.allPlayers.Add(p);
        InitialPanelManager.instance.AddSelectablePlayerButton(p);//��һ�䲻Ӧ��д�������
    }
}
