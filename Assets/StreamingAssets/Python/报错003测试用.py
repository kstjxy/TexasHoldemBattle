import socket  # 导入 socket 模块 用于网络通信
import json  # 导入 json 模块 用于解析接收的 GameStat
import random


class Player:
    def __init__(self):
        self.name = 'Whihi03'           # 名字自定义，最好不要重名，没有重名检测
        self.recivefrom = 'No Message'  # 接收的信息byte[]
        self.reciveString = ''          # 接收的信息string
        self.sendto = ''                # 发送的信息string
        self.sendtoByte = ''            # 发送的信息byte[]
        self.gameStat = ''              # 用于处理接收的 GameStat
        self.st = ''                    # 真正传送的选定的牌组string
        self.actionList = ['跟注', '加注', '弃牌', 'ALL_IN']
        self.s = socket.socket()        # 创建 socket 对象

    def Receive(self):
        while True:
            self.recivefrom = self.s.recv(2048)
            self.reciveString = self.recivefrom.decode()
            self.buff += self.reciveString
            if '$$' in self.buff:
                self.reciveString = self.buff[0:self.buff.index('$')]
                self.buff = self.buff[self.buff.index('$') + 2:]

    def Send(self, st):
        self.sendto = st
        self.sendtoByte = self.sendto.encode()
        self.s.send(self.sendtoByte)

    def start(self):
        host = socket.gethostname()  # 获取本地主机名，运行时改为服务器的ip地址
        port = 53042  # 设置端口号
        self.s.connect((host, port))

        while True:
            self.Receive()
            print('服务器传值为' + self.reciveString)
            if self.reciveString == 'OnInit':
                self.OnInit()
            elif self.reciveString == 'StartGame':
                self.StartGame()
            elif self.reciveString == 'RoundStart':
                self.RoundStart()
            elif self.reciveString == 'BetAction':
                self.BetAction()
            elif self.reciveString == 'FinalSelection':
                self.FinalSelection()
            elif self.reciveString == 'No Message':
                continue
            else:
                print('服务器传值有误！！\n关闭连接！')
                break

    def OnInit(self):
        self.Send(self.name)
        self.Receive()
        print('已连接到' + self.reciveString + ' 初始化成功！')
        # 初始化
        #
        #   code here
        #

    def StartGame(self):
        self.s.send('随便说点啥 给服务器一个信号 知道你还活着 网络没有断开'.encode())
        # 故意写错让客户端报错断开
        self.Receive()
        self.gameStat = json.loads(self.reciveString)
        # 游戏开始
        #
        #   code here
        #

    def RoundStart(self):
        self.s.send('同上'.encode())
        self.Receive()
        self.gameStat = json.loads(self.reciveString)
        # 每一回合的开始
        #
        #   code here
        #

    def BetAction(self):
        self.Receive()
        self.gameStat = json.loads(self.reciveString)
        tmp = random.randint(1, 100)
        if tmp < 51:
            self.bet = 1
        elif tmp < 91:
            self.bet = 2
        elif tmp < 98:
            self.bet = 3
        else:
            self.bet = 4
        print(self.name + ' 选择【' + self.actionList[self.bet - 1] + '】')
        self.s.send(str(self.bet).encode())

    def FinalSelection(self):
        card = [0, 1, 2, 3, 4, 5, 6]
        sample = random.sample(card, 5)
        st = ''
        for i in sample:
            st += str(i)
        print(self.name + ' 选择了最后的手牌' + st)
        self.s.send(st.encode())


if __name__ == '__main__':
    p = Player()
    p.start()