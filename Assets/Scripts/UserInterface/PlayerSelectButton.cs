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
    /// ��ʼ�� PlayerSelectButton ���Ͷ�Ӧ��Player�ҹ�
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
    /// ���� Player ��״̬ˢ�¶�Ӧ�� UI ��ʾ
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
