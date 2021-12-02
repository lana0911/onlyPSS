# -*- coding: utf8 -*-

import socket

# host = socket.gethostname()
# port = 5000
host = '10.22.7.213'  # 對server端為主機位置
port = 5555
address = (host, port)

socket02 = socket.socket(socket.AF_INET, socket.SOCK_STREAM)
# AF_INET:默認IPv4, SOCK_STREAM:TCP

socket02.connect(address)  # 用來請求連接遠程服務器

##################################
# 開始傳輸
print('start send image')
imgFile = open("moon.png", "rb")
while True:
    imgData = imgFile.readline(512)
    if not imgData:
        break  # 讀完檔案結束迴圈
    socket02.send(imgData)
imgFile.close()
print('transmit end')
##################################

socket02.close()  # 關閉
print('client close')