using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerSelectButton : MonoBehaviour
{
    public Player player;

    [Header("UI_Elements")]
    public Image avatarImage;
    public Text stateText;
    public Text nameText;

    /// <summary>
    /// 初始化 PlayerSelectButton ，和对应的Player挂钩
    /// </summary>
    /// <param name="p"></param>
    public void InitializeSelectButton(Player p)
    {
        player = p;
        nameText.text = p.playerName;
        this.GetComponent<Button>().onClick.AddListener(delegate { ButtonClicked(); });
    }

    void ButtonClicked()
    {
        if (player == null)
            return;
        player.isInGame = !player.isInGame;
        RefreshButtonUI();
    }

    /// <summary>
    /// 根据 Player 的状态刷新对应的 UI 显示
    /// </summary>
    public void RefreshButtonUI()
    {
        if (player == null)
            return;
        if (player.isInGame)
        {
            avatarImage.sprite = Resources.Load<Sprite>("Avatars/avatar_actioning");
            stateText.text = "<color=#83FFC7>selected!!</color>";
        }
        else
        {
            avatarImage.sprite = Resources.Load<Sprite>("Avatars/avatar_waiting");
            stateText.text = "<color=#A0EBFF>waiting...</color>";
        }
    }
}
