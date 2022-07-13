using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InitialPanelManager : MonoBehaviour
{
    //游戏开始时执行该方法
    public void StartButtonClicked(Animator panelAnimator)
    {
        //根据用户的输入进行游戏全局的初始化：包括选择的AI玩家上座（实例化）、相关参数设置读取并提交给GameManager
        panelAnimator.Play("GameStarted");//使用动画让Panel退出画面
    }
}
