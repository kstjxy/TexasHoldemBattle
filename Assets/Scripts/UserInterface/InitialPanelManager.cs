using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InitialPanelManager : MonoBehaviour
{
    public Animator panelAnimator;

    [Header("InputFields")]
    public InputField InitialChips;
    public InputField smallBlindInjection;
    public InputField maximumGames;
    public InputField raisisLimit;

    public static InitialPanelManager instance;

    private void Awake()
    {
        //单例模式初始化
        if (instance != null)
            Destroy(this);
        else
            instance = this;
    }
    //游戏开始时执行该方法
    public void StartButtonClicked()
    {
        //根据用户的输入进行游戏全局的初始化：包括选择的AI玩家上座（实例化）、相关参数设置读取并提交给GameManager
        panelAnimator.Play("GameStarted");//使用动画让Panel退出画面
    }

    public void CallInitialPanel()
    {
        panelAnimator.Play("StartInitializing");//使用动画让Panel退出画面
    }
}
