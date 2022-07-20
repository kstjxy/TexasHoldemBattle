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
        TestAI_1 ai = new();
        ai.OnInit(GolbalVar.roboName);
        GolbalVar.roboName++;
        Player p = new(ai);
        GameStat gs = new(p);
        ai.stats = gs;
        PlayerManager.instance.allPlayers.Add(p);
        InitialPanelManager.instance.AddSelectablePlayerButton(p);//这一句不应该写在这里！！
    }
}
