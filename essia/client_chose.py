import socket
data=""
im = "2"
# 构建一个实例，去连接服务端的监听端口。
client = socket.socket(socket.AF_INET, socket.SOCK_STREAM)
client.connect(('192.168.1.106', 5050))
client.send(im.encode('utf-8'))
# 告知
#client.send(bytes('client'.encode('utf-8')))

# 不断获取输入，并发送给服务端。

while(data!='exit'):
    data=input("請輸入 ")
    client.send(data.encode('utf-8'))
    rec= client.recv(1024).decode('utf-8')
    print("server 回傳 " ,rec)
client.close()