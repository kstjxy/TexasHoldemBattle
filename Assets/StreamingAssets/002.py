import socket  # 导入 socket 模块
import random


class Player:
    def __init__(self):
        self.name = 'hehe_002'
        self.recivefrom = 'No Message'
        self.reciveString = ''
        self.sendto = ''
        self.sendtoByte = ''
        self.bet = 0
        self.actionList = ['跟注','加注','弃牌','ALL_IN']
        self.s = socket.socket()  # 创建 socket 对象

    def Receive(self):
        self.recivefrom = self.s.recv(1024)
        self.reciveString = self.recivefrom.decode()

    def Send(self, st):
        self.sendto = st
        self.sendtoByte = self.sendto.encode()
        self.s.send(self.sendtoByte)

    def start(self):
        host = socket.gethostname()  # 获取本地主机名
        port = 53042  # 设置端口号
        self.s.connect((host, port))

        while True:
            self.Receive()
            print('服务器传值为 ' + self.reciveString)
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
                print('服务器传值有误！！\n关闭连接！传值为' + self.reciveString)
                break

    def OnInit(self):
        self.Send(self.name)
        self.Receive()
        print('已连接到 ' + self.reciveString + ' 初始化成功！')

    def StartGame(self):
        self.s.send('我是大聪明'.encode())

    def RoundStart(self):
        self.s.send('开始'.encode())

    def BetAction(self):
        tmp = random.randint(1, 100)
        if tmp < 51:
            self.bet = 1
        elif tmp < 91:
            self.bet = 2
        elif tmp < 98:
            self.bet = 3
        else:
            self.bet = 4
        print(self.name + ' 选择【' + self.actionList[self.bet-1] + '】')
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
