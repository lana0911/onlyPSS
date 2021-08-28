from os import truncate
import numpy as np
import cv2
import socket
import threading
import time
from threading import Timer
# from socket import *
import jpysocket
import base64
import time

def imgs(client_executor):
    BUFSIZ = 1024*20
    rec_d = bytes([])
    print("副函")
    while True:
        data = client_executor.recv(BUFSIZ)
        if not data or len(data) == 0:
            break
        else:
            rec_d = rec_d + data
    path = 'C:/Users/Lana/Documents/GitHub/onlyPSS/essia/d.txt'
    f = open(path, 'w')
    f.write(str(rec_d))
    f.close()
    #轉成圖片檔
    with open("C:/Users/Lana/Documents/GitHub/onlyPSS/essia/d.txt","r") as f:
        img = base64.b64decode(f.read()[1:])
        print(type(f.read()))
        fh = open("C:/Users/Lana/Documents/GitHub/onlyPSS/essia/pic_2_sucess.jpg","wb")
        fh.write(img)
        fh.close()
    time.sleep(1)


#client連進來後會在這
def classfly(client_executor, addr):
    print("welcome to classfy")
    print('Accept new connection from %s:%s...' % addr)
    print("clent1=",client_executor)
    BUFSIZ = 1024*20
    #_------------------------------------------------
    who = client_executor.recv(1024)
    who = jpysocket.jpydecode(who)
    print("who=",who)
    if(who == "2"):
        print("==2")
        #傳 "face"
        func = client_executor.recv(1024)
        func = jpysocket.jpydecode(func)
        print("first=",func,"first=","face")
        if func == "facer;":
            client_executor.send(jpysocket.jpyencode("StartSend"))
            #暫停3秒等照片+辨識
            time.sleep(5)
            print("5秒結束")
            #開始讀檔
            fa = open("C:/Users/Lana/Documents/GitHub/onlyPSS/essia/rec.txt","r")
            ans = fa.readline()
            print(ans)
            client_executor.send(jpysocket.jpyencode(ans))
            print('send complete')
            client_executor.close()

    if(who == "3"):
        print("==3")
        imgs(client_executor)


#主函式
if __name__ == '__main__':
    # IP , Port......設定
    listener = socket.socket(socket.AF_INET, socket.SOCK_STREAM)
    listener.bind(('192.168.50.21', 5050))
    listener.listen(5)
    print('Waiting for connect...')
    while True:
        client_executor, addr = listener.accept()
        t = threading.Thread(target=classfly, args=(client_executor, addr))
        t.start()
