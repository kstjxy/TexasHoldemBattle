# TexasHoldemBattle/德州扑克AI对战平台
基于UNITY开发的德州扑克AI对抗平台，用户可以编写AI程序控制机器人进行各种出牌操作，多人对战，在达到目标局数后获得最多筹码者获胜。

德州扑克AI是一个非完全信息问题，也是AI实现的一个难点。德州扑克AI对抗平台作为AI对抗赛、AI课程的一个基础设施，参赛者可以编写AI程序，模拟对抗，以评估AI算法的能力。

2022.7.22 第一阶段c#脚本版完成 （存于Version_1_CSharp分支）

2022.7.28 第二阶段Lua模块接口完成 （存于Version_2_Lua分支）

此分支正在开发第三期WebSocket接口并且已有部分进展

项目验收日志： https://4399.feishu.cn/docx/doxcnZjbH1n2UBNVLpMt9IQ9HCg

示例视频：     ./示例视频（由于录制视频的时候使用的编辑器版本与当前使用的编辑器版本不同，所以会出现一些bug，实际运行中不会出现）

（注：如果出现一些奇怪的bug可以尝试使用WenSocket分支，main在Merge的时候好像出现了未知错误，但是目前没有发现Bug）

![捕获](https://user-images.githubusercontent.com/50037765/182520005-ee4db28c-577c-4b02-934d-335ca9240ce5.PNG)
![捕获2](https://user-images.githubusercontent.com/50037765/180355966-0225751e-65a3-4c23-a4d5-c36233a4f7e5.PNG)
![捕获3](https://user-images.githubusercontent.com/50037765/180355985-0919eefc-0d27-4764-a880-50b03d0a39dc.PNG)

# 开发目标/基本功能
游戏开始前，选定参赛的人员，每位参赛者获得一定数量的金币(可设置)用于游戏，比赛持续N(可设置)轮，或直到仅剩一名未破产玩家终止，游戏终止时根据玩家金币给出排名。为减少AI难度，采用限注模式，限制每次加注和跟注的金币数。
选定参赛人员：提交代码的参赛者和通过网络连接连入的参赛者即可加入比赛，裁判可以勾选哪些参赛者进入游戏。

比赛中：由平台调用参赛者的程序接口进行比赛。界面中显示当前参赛的玩家、各玩家拥有的金币数、已下注的金币数、当前场上牌面、谁是庄家等信息。

比赛后：根据金币数量给出排名

调节速度：出于模拟比赛需要，要求程序可以调整比赛速度（接口调用时间间隔）。

记录日志：为回溯比赛结果，每场比赛均需要记录日志，日志内容包含游戏进程和数据。

参赛者可以选择三种方式之一提交代码。
1、按照指定格式，提交C#代码模块，将C#文件放置到指定文件夹下（仅调试期间用）
2、按照指定格式，编写一个Lua模块，将Lua文件放置到指定文件夹下
3、（可延后）提供websocket接口，与开发者提供的程序做交互

# 程序实现阶段
第一期工程： 实现C#文件版
1、完成界面、代码实现细节。
2、特别对于计分模块，需要编写一些测试代码。
3、编写两个AI测试模块，采用不同的策略(如随机)，用于后续验证

第二期工程：引入XLua，实现Lua脚本版，并提供示例

第三期工程：引入XLua，实现websocket模块接口版本，并提供示例
另外，所有的输入框都加上了合法性判断，ip地址会自动填入本机地址，post会自动从1025往后找到一个本机未占用端口填入。

# 目录结构
TexasHoldemBattle：AI对抗平台核心代码，使用Unity2021.2.14

Assets/Scripts 主要脚本说明：

GameManager: 最核心的类，负责整个游戏的更新流程，调用其他Manager类进行操作

GameStat: 用于储存AI可接收到的信息

GolbalVar: 用于储存所有类都可以共享的全局变量

Card:牌的基本类，包括花色和大小

CardManager: 用于管理所有卡牌，包括收发牌，和最终的牌力大小获胜条件判定

BaseAI: 玩家所写的Lua脚本与C#的接口，通过Interface将AI脚本模块映射到这里。

Player: 玩家的基本类，记录手牌和当前的状态，接收AI脚本传回的出牌指令

PlayerObject: 继承了单个Player并且和UI进行对接的类

PlayerManager: 管理所有Player类在游戏中的主要操作，包括实时排名，和处理AI脚本传回的出牌指令等

UIManager: 管理所有UI elements和脚本的接口

RecordManager: 用于记录LOG message可以保存在本地

streamingAssets: 用户编写的AI脚本文件夹

# 使用方法
在初始化界面点击ADD A TEST PLAYER按钮，并在弹出的文件夹中选择后缀为.Lua的AI游戏脚本。这时玩家初始化就会完成，头像会显示在列表中。

在初始化界面点击StartServer按钮，会开始监听，此时开启客户端连接上服务器之后，头像会显示在列表中。再次按下按钮会关闭服务器，所有客户端断开连接，头像消失（lua脚本不受影响）

点击头像选择参与比赛的玩家，可选择2-8人。下方的比赛设置可以进行输入调整。

然后点击START自动开始比赛，可以通过左下角的SLIDER调整运行速度，中途拉出左侧的设置按钮可以在任何时间更改游戏规则（当然也有合法性判断，并且在log中会实时显示更新情况）。点击信息栏中的SAVE键消息日志会自动保存到本地设定的路径中。

当全部游戏回合结束或者场上仅剩一名未破产玩家时游戏自动结束，需要点击RESTART重新选择玩家开始新一轮比赛。
