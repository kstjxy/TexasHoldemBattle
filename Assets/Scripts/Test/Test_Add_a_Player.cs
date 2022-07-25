using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.IO;
using UnityEngine;
using XLua;

public class Test_Add_a_Player : MonoBehaviour
{
    /// <summary>
    /// 纯测试使用
    /// </summary>
    public void ButtonClicked()
    {
        findLua(GameManager.instance.luaenv);
        TestAI_1 ai = new();
        ai.OnInit(GlobalVar.roboName);
        GlobalVar.roboName++;
        Player p = new(ai);
        GameStat gs = new(p);
        ai.stats = gs;
        PlayerManager.instance.allPlayers.Add(p);
        InitialPanelManager.instance.AddSelectablePlayerButton(p);//这一句不应该写在这里！！
    }

    private void findLua(LuaEnv luaenv)
    {
        string type = "txt";
        OpenFileName openFileName = new OpenFileName();
        openFileName.filter = "AI脚本(*" + type + ")\0*." + type + "\0";
        openFileName.structSize = Marshal.SizeOf(openFileName);
        openFileName.fileTitle = new string(new char[64]);
        openFileName.maxFileTitle = openFileName.fileTitle.Length;
        openFileName.initialDir = Application.streamingAssetsPath.Replace('/', '\\');//默认路径
        print(openFileName.initialDir);
        //openFileName.initialDir = "D:\\pro\\poker\\AI";
        openFileName.title = "选择AI脚本";
        openFileName.flags = 0x00080000 | 0x00001000 | 0x00000800 | 0x00000200;

        // Create buffer for file names
        string fileNames = new string(new char[2048]);
        openFileName.maxFile = fileNames.Length;
        if (LocalDialog.GetOpenFileName(openFileName))
        {
            string pointer = openFileName.file;
            print(pointer);
            
            foreach (string f in GameManager.instance.aiFile)
            {
                //luaenv.DoString("require" + f);
                TestAI_1 ai = new();
                ai.OnInit(GlobalVar.roboName);
                GlobalVar.roboName++;
                Player p = new(ai);
                GameStat gs = new(p);
                ai.stats = gs;
                PlayerManager.instance.allPlayers.Add(p);

            }
        }
    }
}
