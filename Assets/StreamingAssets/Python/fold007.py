import socket   # 导入 socket 模块 用于网络通信
import json     # 导入 json 模块 用于处理接收的 GameStat
import random


class Player:
    def __init__(self):
        self.name = 'Wfold07'
        self.recivefrom = 'No Message'
        self.reciveString = ''
        self.buff = ''
        self.mark = '$$'
        self.sendto = ''
        self.sendtoByte = ''
        self.gameStat = ''          # 用于处理接收的 GameStat
        self.st = ''
        self.actionList = ['跟注', '加注', '弃牌', 'ALL_IN']
        self.s = socket.socket()    # 创建 socket 对象

    def Receive(self):
        while True:
            self.recivefrom = self.s.recv(2048)
            self.reciveString = self.recivefrom.decode()
            self.buff += self.reciveString
            if '$$' in self.buff:
                self.reciveString = self.buff[0:self.buff.index('$')]
                self.buff = self.buff[self.buff.index('$') + 2:]
                break

    def Send(self, st):
        self.sendto = st + self.mark
        self.sendtoByte = self.sendto.encode()
        self.s.send(self.sendtoByte)

    def start(self):
        host = socket.gethostname() # 获取本地主机名，运行时改成服务器的ip地址
        port = 53042                # 设置端口号
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
        self.Send('我是大聪明')
        # 游戏开始 code below
        #
        #   code here
        #

    def RoundStart(self):
        self.Send('开始')
        self.Receive()
        self.gameStat = json.loads(self.reciveString)
        # 每一回合的开始 code below
        #
        #   code here
        #
        print(self.reciveString)

    def BetAction(self):
        self.Receive()
        self.gameStat = json.loads(self.reciveString)
        # 进行操作 code below
        #
        #   code here
        #   改变 self.bet 来操作
        #   self.bet = 1 为 '跟注'
        #   self.bet = 2 为 '加注'
        #   self.bet = 3 为 '弃牌'
        #   self.bet = 4 为 'ALL_IN'
        #
        tmp = random.randint(1, 100)
        if tmp < 6:
            self.bet = 1
        elif tmp < 11:
            self.bet = 2
        elif tmp < 98:
            self.bet = 3
        else:
            self.bet = 4
        print(self.name + ' 选择【' + self.actionList[self.bet - 1] + '】')
        self.Send(str(self.bet))

    def FinalSelection(self):
        self.Receive()
        self.gameStat = json.loads(self.reciveString)
        # 选择最后的手牌 code below
        #
        #   code here
        #
        #   返回一个 长度为5 的 整数列表 名为 card
        #   例如 card = [0, 1, 3, 6, 2]
        #   每个整数表示你选择了哪张牌
        #   0，1 表示手牌的第一张，第二张
        #   2，3，4，5，6  表示公共牌的第一张，第二张...到第五张
        #   顺序无要求，如果有不合法整数（>6 || <0，或者中英文）服务器会默认处理为0
        #   长度超过5 服务器只会处理前5个，忽略后面的
        #
        card = [0, 1, 2, 3, 4, 5, 6]
        sample = random.sample(card, 5)
        st = ''
        for i in sample:
            st += str(i)
        self.Send(st)



if __name__ == '__main__':
    p = Player()
    p.start()
