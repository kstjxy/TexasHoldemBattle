using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test_Add_a_Player : MonoBehaviour
{
    /// <summary>
    /// 纯测试使用
    /// </summary>
    public void ButtonClicked()
    {
        Player p = new Player("Test" + Random.Range(0.1f, 3.2f).ToString());
        PlayerManager.instance.allPlayers.Add(p);
        InitialPanelManager.instance.AddSelectablePlayerButton(p);//这一句不应该写在这里！！
    }
}
