# TexasHoldemBattle/德州扑克AI对战平台
基于UNITY开发的德州扑克AI对抗平台，用户可以编写AI程序控制机器人进行各种出牌操作，多人对战，在达到目标局数后获得最多筹码者获胜。

目前项目已实现第一期工程并且通过了部分测试。

![捕获](https://user-images.githubusercontent.com/50037765/180354891-e0d09b04-041a-4fe5-ad5a-1a2fe04fd882.PNG)
![捕获2](https://user-images.githubusercontent.com/50037765/180355966-0225751e-65a3-4c23-a4d5-c36233a4f7e5.PNG)
![捕获3](https://user-images.githubusercontent.com/50037765/180355985-0919eefc-0d27-4764-a880-50b03d0a39dc.PNG)

# 开发目标/基本功能
游戏开始前，选定参赛的人员，每位参赛者获得一定数量的金币(可设置)用于游戏，比赛持续N(可设置)轮，或直到仅剩一名未破产玩家终止，游戏终止时根据玩家金币给出排名。为减少AI难度，采用限注模式，限制每次加注和跟注的金币数。
选定参赛人员：提交代码的参赛者和通过网络连接连入的参赛者即可加入比赛，裁判可以勾选哪些参赛者进入游戏。

比赛中：由平台调用参赛者的程序接口进行比赛。界面中显示当前参赛的玩家、各玩家拥有的金币数、已下注的金币数、当前场上牌面、谁是庄家等信息。

比赛后：根据金币数量给出排名

调节速度：出于模拟比赛需要，要求程序可以调整比赛速度（接口调用时间间隔）。

记录日志：为回溯比赛结果，每场比赛均需要记录日志，日志内容包含游戏进程和数据。

# 程序实现阶段
第一期工程： 实现C#文件版
1、完成界面、代码实现细节。
2、特别对于计分模块，需要编写一些测试代码。
3、编写两个AI测试模块，采用不同的策略(如随机)，用于后续验证

第二期工程：引入XLua，实现Lua脚本版，并提供示例

第三期工程：引入XLua，实现websocket模块接口版本，并提供示例

# 目录结构
TexasHoldemBattle：AI对抗平台核心代码，使用Unity2021.2.14

Assets/Scripts 主要脚本说明：

GameManager: 最核心的类，负责整个游戏的更新流程，调用其他Manager类进行操作

GameStat: 用于储存AI可接收到的信息

GolbalVar: 用于储存所有类都可以共享的全局变量

Card:牌的基本类，包括花色和大小

CardManager: 用于管理所有卡牌，包括收发牌，和最终的牌力大小获胜条件判定

BaseAI: 玩家所写的AI脚本的父类，为抽象类

Player: 玩家的基本类，记录手牌和当前的状态，接收AI脚本传回的出牌指令

PlayerObject: 继承了单个Player并且和UI进行对接的类

PlayerManager: 管理所有Player类在游戏中的主要操作，包括实时排名，和处理AI脚本传回的出牌指令等

UIManager: 管理所有UI elements和脚本的接口

RecordManager: 用于记录LOG message可以保存在本地

Test: 用户编写的机器人脚本文件夹
