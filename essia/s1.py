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
    print("副函")
    while True:
        rec_d = bytes([])
        data = client_executor.recv(BUFSIZ)
        if not data or len(data) == 0:
            return rec_d
        else:
            rec_d = rec_d + data
    # return rec_d
#client連進來後會在這
def classfly(client_executor, addr):
    print("welcome to classfy")
    print('Accept new connection from %s:%s...' % addr)
    print("clent1=",client_executor)
    BUFSIZ = 1024*20
    who = client_executor.recv(1024)
    who = jpysocket.jpydecode(who)
    # client_executor.send(jpysocket.jpyencode("Hi1"))
    # client_executor.send(jpysocket.jpyencode("H2"))
    # client_executor.send(jpysocket.jpyencode("H3"))
    # client_executor.send(jpysocket.jpyencode("H4"))
    # client_executor.send(jpysocket.jpyencode("H5"))
    

    print("first=",who,"first=","face")
    if who == "face":
        client_executor.send(jpysocket.jpyencode("StartSend"))
        print("else")
        rec_d = imgs(client_executor)
        print("副函over")
        print("clent2=",client_executor)
        client_executor.send(jpysocket.jpyencode("while 結束"))

        print("jump")
        path = 'D:/essia/d.txt'
        f = open(path, 'w')
        f.write(str(rec_d))
        f.close()
        
        with open("D:/essia/d.txt","r") as f:
            img = base64.b64decode(f.read()[1:])
            print(type(f.read()))
            fh = open("D:/essia/pic_2_sucess.jpg","wb")
            fh.write(img)
            fh.close()
        time.sleep(1)
        fa = open("D:/essia/rec.txt","r")
        rec_send = fa.readline()
        print(rec_send)
        
        client_executor.send(jpysocket.jpyencode(rec_send))
        print('send complete')
        client_executor.close()
        
    # #不斷輸入文字傳給unity
    # while True:
    #     str = input("input:")
    #     client_executor.send(str.encode('utf-8'))






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
