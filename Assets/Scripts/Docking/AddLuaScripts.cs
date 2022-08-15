using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.IO;
using System;
using UnityEngine;
using XLua;


public class AddLuaScripts : MonoBehaviour
{
    /// <summary>
    /// 选择AI脚本
    /// </summary>
    public void ButtonClicked()
    {
        findLua();
    }
    public long NextFile(long pointer, string file)
    {
        return pointer + file.Length * 2 + 2;
    }
    private void findLua()
    {
        //设置OpenFileDlg
        string type = "lua";
        OpenFileDlg openFileName = new OpenFileDlg();
        openFileName.filter = "AI脚本(*" + type + ")\0*." + type + "\0";
        openFileName.structSize = Marshal.SizeOf(openFileName);
        openFileName.fileTitle = new string(new char[64]);
        openFileName.maxFileTitle = openFileName.fileTitle.Length;
        openFileName.initialDir = Application.streamingAssetsPath.Replace('/', '\\');//默认路径
        //print(openFileName.initialDir);
        //openFileName.initialDir = "D:\\pro\\poker\\AI";
        openFileName.title = "选择AI脚本";
        openFileName.flags = 0x00080000 | 0x00001000 | 0x00000800 | 0x00000200 | 0x00000008;

        // Create buffer for file names
        // 没有的话返回值就会为null
        string fileNames = new string(new char[2048]);
        openFileName.file = Marshal.StringToBSTR(fileNames);
        openFileName.maxFile = fileNames.Length;

        List<string> aiPath = new List<string>();           //多选文件名
        List<string> fileFullNames = new List<string>();    //文件完整地址
        if (OpenFileDialog.GetOpenFileName(openFileName))
        {
            long pointer = (long)openFileName.file;             //文件指针
            string file = Marshal.PtrToStringAuto(openFileName.file);

            while (file.Length > 0)
            {
                aiPath.Add(file);
                //指针迭代
                pointer = NextFile(pointer ,file);
                openFileName.file = (IntPtr)pointer;
                file = Marshal.PtrToStringAuto(openFileName.file);
            }

            if (aiPath.Count == 1)
            {
                fileFullNames = aiPath;
            }
            else
            {
                string[] selectedFiles = new string[aiPath.Count - 1];
                for (int i = 0; i < selectedFiles.Length; i++)
                {
                    selectedFiles[i] = aiPath[0] + "\\" + aiPath[i + 1];
                }
                fileFullNames = new List<string>(selectedFiles);
            }
        }

        if (fileFullNames.Count > 0)
        {
            GameManager.instance.aiFile = new List<string>(fileFullNames);
            foreach (string s in fileFullNames)
            {
                try
                {
                    LuaAI ai = new LuaAI();
                    ai.OnInit(s);
                    Player p = new(ai);
                    GameStat gs = new(p);
                    ai.stats = gs;
                    PlayerManager.instance.allPlayers.Add(p);
                }
                catch (Exception e)
                {
                    // 引起异常的语句
                    Debug.Log("提交的AI脚本有误，初始化失败，原因：" + e.Message);
                    string[] fileDetail = s.Split('\\');
                    InitialPanelManager.instance.CallLuaLog(fileDetail[fileDetail.Length-1] + "AI脚本有误：" + e.Message) ;
                    UIManager.instance.PrintLog(fileDetail[fileDetail.Length - 1] + "AI脚本有误：" + e.Message);
                }
            }
        }
        else
        {
            Debug.Log("没有选择任何AI脚本");
        }
    }

    
}

[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
public class OpenFileDlg
{
    public int structSize = 0;
    public IntPtr dlgOwner = IntPtr.Zero;
    public IntPtr instance = IntPtr.Zero;
    public String filter = null;
    public String customFilter = null;
    public int maxCustFilter = 0;
    public int filterIndex = 0;
    public IntPtr file;
    public int maxFile = 0;
    public String fileTitle = null;
    public int maxFileTitle = 0;
    public String initialDir = null;
    public String title = null;
    public int flags = 0;
    public short fileOffset = 0;
    public short fileExtension = 0;
    public String defExt = null;
    public IntPtr custData = IntPtr.Zero;
    public IntPtr hook = IntPtr.Zero;
    public String templateName = null;
    public IntPtr reservedPtr = IntPtr.Zero;
    public int reservedInt = 0;
    public int flagsEx = 0;
}

public class OpenFileDialog
{
    [DllImport("Comdlg32.dll", SetLastError = true, ThrowOnUnmappableChar = true, CharSet = CharSet.Auto)]
    public static extern bool GetOpenFileName([In, Out] OpenFileDlg ofd);
}
