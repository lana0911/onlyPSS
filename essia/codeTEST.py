import base64
import jpysocket

import socket
import threading
def code (client_executor, addr):
    print("welcome to classfy")
    print('Accept new connection from %s:%s...' % addr)
    who_recv = client_executor.recv(1024)
    who_jpy = jpysocket.jpydecode(who_recv) #jpy解碼
    print("收:",who_jpy)
    while(True):
        # str = "測試"
        str = input("input:")
        client_executor.send(jpysocket.jpyencode(str))
        # client_executor.send(str.encode('utf-8'))
    
#主函式
if __name__ == '__main__':
    # IP , Port......設定
    listener = socket.socket(socket.AF_INET, socket.SOCK_STREAM)
    listener.bind(('10.22.3.205', 5050))
    listener.listen(5)
    print('Waiting for connect...')

    #辨識圖片
    while True:
        client_executor, addr = listener.accept()
        
        t = threading.Thread(target=code, args=(client_executor, addr))
        t.start()

