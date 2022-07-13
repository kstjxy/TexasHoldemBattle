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
    /// 对PlayerObject进行初始化
    /// </summary>
    /// <param name="p">与之关联的Player本身</param>
    /// <param name="coins">初始筹码数量</param>
    public void InitializeThePlayer(Player p, int coins)
    { 
    
    }

    /// <summary>
    /// 请注意：调用该方法时，player已经调用过.AddPlayerCards()
    /// </summary>
    public void ShowCards()
    { 
    
    }
}
