from os import truncate
import numpy as np
import cv2
import socket
import threading
import time
from threading import Timer
i=0
playing = False 
playing2 = False 
list =0
scale =0 
cam = cv2.VideoCapture(0)
"""
    unity -> python : Send(Encoding.UTF32)
    python -> unity : .send(bytes(sentense.encode('utf-8')))
    python -> python : utf-8 
"""
"""
step 1: Client連線進來後會到 classfly 進行分類 (ps. 每一個client連線後第一個動作要傳分類訊息過來)
step 2: classfly 會收到一則client的分類訊息,根據分類去不同副函式

##副函式
    1. 分類訊息==img:
        setting()->Getface() =>開啟鏡頭 + 計算距離傳給unity
    2. 分類訊息==tex1 or text2:
        text1() / text2():不斷輸入訊息傳給unity顯示 (！！！要改成不斷接收到java client 傳的訊息再傳給unity)
    3. 分類訊息==switch:
        java client要玩遊戲 =>告知unity切換場景
"""
clients=[]
#分類
def classfly(client_executor, addr):
    print("welcome to classfy")
    print('Accept new connection from %s:%s...' % addr)
    
    #收到Client是誰訊息 =>加入聯絡人List
    who = client_executor.recv(1024).decode('utf-8')
    print("一開始收到的->",who,"-<")
    #加入通訊
    if(who==""):
        client_executor.close()
    if(who=="1"):#unity看板
        print("who==",who)
        clients.append(client_executor)
        unityRecv(client_executor)
        #print(clients[0])

    elif(who=="2"):#手機cliet
        print("who==",who)
        
        #不斷接收client(手機)傳來的訊息
        while True:
            msg = client_executor.recv(1024).decode('utf-8')            
            print("開始到")
            print(msg) ##msg範例 : text;welcome
            #將收到的訊息分割 [0]:目標 [1...]:內容
            msg_split = msg.split(";")
            target = msg_split[0]
            #taget是要傳訊息到看板
            if(target == "text"):
                text(client_executor,msg)
            #game1是要玩猜拳
            if(target == "game1"):
                global playing 
                if(playing==True):#已經有人在玩
                    client_executor.send("sorry, someone playing...".encode('utf-8'))
                else:
                    client_executor.send("遊戲即將開始".encode('utf-8'))
                    game1(client_executor,msg)
            if(target == "game2"):
                global playing2 
                if(playing2==True):#已經有人在玩
                    client_executor.send("sorry, someone playing...".encode('utf-8'))
                else:
                    client_executor.send("遊戲即將開始".encode('utf-8'))
                    game2(client_executor,msg)
    elif(who=="4"):#kinect
        print("收到4")
        kinect(client_executor)


###怡君關節點 (不斷收怡君的 && 不斷給unity)--------------------------------
def kinect(client_executor):
    print("kinect副函")
    while True:
        recv = client_executor.recv(1024).decode('utf-8')
        if(recv != None):
            print("收:"+recv)
        clients[0].send(bytes(recv.encode('utf-8')))
        
#接收unity傳來的
def unityRecv(client_executor):
    #img_scale(client_executor)
    print("aloha")
    global playing 
    global playing2 
    while True:
        recv = client_executor.recv(1024).decode('utf-8')
        recv_split = recv.split(";")
        print("unity REcv",recv_split)
        if(recv_split[0]=="pose"):#看板說現在給結果!
            startPose()
        if(recv_split[0]=="shot"):#讀取截圖 測試用
            imgShot()
        elif(recv_split[0]=="over"):
            playing = False
            global t_face
            # if(recv_split[1]=="0"):
            client_executor.send("over".encode('utf-8'))
            time.sleep(3)
            t_face2 = threading.Thread(target=face)
            t_face2.start()

def imgShot():
    print("enter imgshot")
    path = "D:/screenshot/Shot.png"
    #將unity的截圖show出來
    img = cv2.imread(path)
    """
    !!!!!!!!!!!!!To聿涵: 
                    要show的話imshow就不能傳值 
                    所以你要測試的話要注意
                    建議直接去看D:/screenshot的Shot.png我存在那
    """
    #cv2.namedWindow('My Image', cv2.WINDOW_NORMAL | cv2.WINDOW_KEEPRATIO)#調整show大小
    #cv2.imshow('My Image', img)
    # 按下任意鍵則關閉所有視窗
    #cv2.waitKey(0)
    #cv2.destroyAllWindows()
    """------------------------------------"""
    #--------從shot.png 辨識截國再傳給server----------------
    #傳給看板節果
    pose= "1 3"
    #pose = input("輸入兩數 (用空格隔開)(1=剪刀,2=石頭,3=布)EX.1 2 : ")
    ans = "pose;" + pose
    print("ans=",ans)
    if(pose != ""):
       clients[0].send(bytes(ans.encode('utf-8')))


#OPENPSE (聿涵)
def startPose():
    #傳給看板節果
    pose= "1 3"
    #pose = input("輸入兩數 (用空格隔開)(1=剪刀,2=石頭,3=布)EX.1 2 : ")
    ans = "pose;" + pose
    print("ans=",ans)
    if(pose != ""):
        clients[0].send(bytes(ans.encode('utf-8')))

#玩猜拳
def game1(client_executor,content):
    
    #遊戲使用中
    global playing 
    playing = True
    #傳給看板  e.g: game1
    clients[0].send(bytes(content.encode('utf-8')))

#玩跳舞
def game2(client_executor,content):
    
    #遊戲使用中
    global playing2 
    playing2 = True
    #傳給看板  e.g: game1
    clients[0].send(bytes(content.encode('utf-8')))
    

        

def text(client_executor, content):
    print("text()中心收到訊息:",content)
    #傳給看板 e.g.: text;Welcome
    clients[0].send(bytes(content.encode('utf-8')))
    client_executor.send("收到".encode('utf-8'))
   











def seand_scale():
    global scale
    global playing
    global playing2
    while (True):
        # print("scale=",scale)
        scale_send = "scale; "+ str(scale)
       # print("scale_send=",scale_send)
        if(len(clients)==0):
            # print("none")
            n=2
        else:
            # print("yes")
            if(playing==False or playing2==False):
                clients[0].send(bytes(scale_send.encode('utf-8')))
        time.sleep(0.5)
def Getface(image):
    #print("enter getface")
    global scale
    list = 0
    cnt = 0
    cvo = cv2.CascadeClassifier('haarcascade_frontalface_default.xml')
    cvo.load('C:/Users/Lana/AppData/Local/Programs/Python/Python39/cv2/data/haarcascade_frontalface_default.xml')
    #灰階
    gray = cv2.cvtColor(image, cv2.COLOR_BGR2GRAY)
    #辨識
    faces = cvo.detectMultiScale (
        gray,
        scaleFactor=1.3,
        minNeighbors = 5,
        minSize = (30,30),
        flags = cv2.CASCADE_SCALE_IMAGE
    )
    # print("flags=",faces)
    # print("types=",type(faces))
    # print("types=",len(faces))
    # X_row=np.size(faces,0)

    # print("X_row:",X_row)
    area = 0
    scale = 0
    #框框
    for(x, y, w, h) in faces:
        cv2.rectangle(image, (x,y), (x+w, y+h), (0,255,0), 2)
        area = abs(w) * abs(h)
        if(area == None): 
            area = 0
        #print(area)
        text = str(area)
        who = str(cnt)
        if(area > list):
            list = area
            #print(area)

        cnt+=1
        scale = list
        # print("len=",len(faces))
        # print("fa=",(faces))

        # if(len(faces) is None):
        #     scale = 0
        # if(len(faces) < 1):
        #     scale = 0    
        # if(len(faces) is False):
        #     print("false")
        #     scale = 0
        # if(not len(faces)):
        #     print("fal2se")
        #     scale = 0

            

  
        cv2.putText(image, text, (x+5,y+5), cv2.FONT_HERSHEY_SIMPLEX, 1, (0, 255, 255), 1,cv2.LINE_AA)
        cv2.putText(image, who, (x-10,y), cv2.FONT_HERSHEY_SIMPLEX, 1, (255, 0,0 ), 1,cv2.LINE_AA)
    return image
def face():
    global playing
    global playing2
      
    #開啟鏡頭
    global cam
    print("isopen",cam.isOpened())
    if(cam.isOpened()==False and playing==False or cam.isOpened()==False and playing2==False) :
        print("jump here")
        cam = cv2.VideoCapture(0)
        cam.open(0)
    #cam = cv2.VideoCapture('talk.mp4')
    #cam = cv2.VideoCapture(0, cv2.CAP_DSHOW) #captureDevice = camera
    width = int(cam.get(cv2.CAP_PROP_FRAME_WIDTH) + 0.5)
    height = int(cam.get(cv2.CAP_PROP_FRAME_HEIGHT) + 0.5)
    #定義編碼
    fourcc = cv2.VideoWriter_fourcc(*'mp4v')
    print("enterface setting",playing)  
    print("後isopen",cam.isOpened())
    #out = cv2.VideoWriter('output.mp4', fourcc, 20.0, (width,height))
    while(cam.isOpened()):
        if(playing== True or playing2==True) :
            break
        ##############print("while")
        ret, frame = cam.read()
        # print("while")
        area = 0
        if ret == True:
            frame = Getface(frame)
            #out.write(frame)
        
            cv2.imshow('My Camera', frame)

            #案Q退出
            if(cv2.waitKey(1) & 0xFF) == ord('q'):
                break
        else:
            break

    cam.release()
    cv2.destroyAllWindows()


t_face = threading.Thread(target=face)

#主函式
if __name__ == '__main__':
    # IP , Port......設定
    listener = socket.socket(socket.AF_INET, socket.SOCK_STREAM)
    listener.bind(('10.22.20.114', 5050))
    listener.listen(5)
    print('Waiting for connect...')
    #建List
    list_num=0
    list = []
    t_face.start()     
    t_send = threading.Thread(target=seand_scale)
    t_send.start()
    while True:
        client_executor, addr = listener.accept()
        
        t = threading.Thread(target=classfly, args=(client_executor, addr))
        t.start()

#     # first=client_executor.recv(1024)
#     # print("first msg ==")
#     # print(first)
#     # if(first == "img"):
#     #     setting(client_executor, addr)
#     # elif(first == "text1"):
#     #     text1(client_executor)
#     # elif(first=="switch"):
#     #     playgame_dance(client_executor)
#     # elif(first=="client"):
#     #     client_center(client_executor)
#     # elif(first=="PSS"):
#     #     PSS_game(client_executor)
#     # elif(first=="test"):
#     #     test(client_executor)
#     # else:
#     #     text2(client_executor)
        
# def test(client_executor):
#     print("this is PSS test center")
#     while True:
        
#         sentense = input("input1=")
#         if(sentense == 'exit'):
#             break
#         client_executor.send(bytes(sentense.encode('utf-8')))
#         print(sentense)
        
# #猜拳遊戲
# def PSS_game(client_executor):
#     print("this is PSS game center")
#     pose = "2,2" #第一個數字=model, 第二個數字=player ; (1,2,3) = (剪刀,石頭,布)
#     #傳給unity猜拳訊息
#     client_executor.send(bytes(pose.encode('utf-8')))

# #處裡手機client端傳來的訊息
# def client_center(client_executor):
#     print("this is client center")
#     while True:
#         msg = client_executor.recv(1024).decode('utf-8')
#         print(msg)
# #選擇完跳舞遊戲
# def playgame_dance(client_executor):
    
#     print("enter dance game")
#     game_num = input("chose")
#     print(game_num)
#     client_executor.send(bytes(game_num.encode('utf-8')))
# #跑馬燈訊息1
# def text1(client_executor):
#     global i
    
#     while True:
        
#         sentense = input("input1=")
#         if(sentense == 'exit'):
#             break
#         print(i,"%2=",i%2)
#         i+=1
#         if i%2==0:
#             client_executor.send(bytes(sentense.encode('utf-8')))
        
#         print(sentense)
#         time.sleep(0.3)
        
#     client_executor.close()
#     print('Connection from %s:%s closed.' % addr)
# #跑馬燈訊息2
# def text2(client_executor):
#     global i
#     while True:
#         sentense = input("input22=")
#         if(sentense == 'exit'):
#             break
#         if i%2==1:
#             client_executor.send(bytes(sentense.encode('utf-8')))
#         i+=1
#         print(sentense)
#         time.sleep(0.3)

#     client_executor.close()
#     print('Connection from %s:%s closed.' % addr)

# #鏡頭開啟設定(測人的距離遠近)
# def setting(client_executor, addr):
#     #開啟鏡頭
#     cam = cv2.VideoCapture(0)
#     #cam = cv2.VideoCapture(0, cv2.CAP_DSHOW) #captureDevice = camera
#     width = int(cam.get(cv2.CAP_PROP_FRAME_WIDTH) + 0.5)
#     height = int(cam.get(cv2.CAP_PROP_FRAME_HEIGHT) + 0.5)
#     #定義編碼
#     fourcc = cv2.VideoWriter_fourcc(*'mp4v')
#     out = cv2.VideoWriter('output.mp4', fourcc, 20.0, (width,height))
#     while(cam.isOpened()):

#         ret, frame = cam.read()
#         area = 0
#         if ret == True:
#             frame = Getface(frame,client_executor, addr)
#             out.write(frame)
        
#             cv2.imshow('My Camera', frame)

#             #案Q退出
#             if(cv2.waitKey(1) & 0xFF) == ord('q'):
#                 break
#         else:
#             break

#     out.release()
#     cam.release()
#     cv2.destroyAllWindows()
# #執行測試距離(測人的距離遠近)
# def Getface(image,client_executor, addr):
#     cvo = cv2.CascadeClassifier('haarcascade_frontalface_default.xml')
#     cvo.load('C:/Users/Lana/AppData/Local/Programs/Python/Python39/cv2/data/haarcascade_frontalface_default.xml')
#     #灰階
#     gray = cv2.cvtColor(image, cv2.COLOR_BGR2GRAY)
#     #辨識
#     faces = cvo.detectMultiScale (
#         gray,
#         scaleFactor=1.3,
#         minNeighbors = 5,
#         minSize = (30,30),
#         flags = cv2.CASCADE_SCALE_IMAGE
#     )
#     area = 0
#     #框框
#     for(x, y, w, h) in faces:
#         cv2.rectangle(image, (x,y), (x+w, y+h), (0,255,0), 2)
#         area = int(abs(w) * abs(h))
#         if(area == None): 
#             area = 0
        
#         text = str(area)
#         cv2.putText(image, text, (x+5,y+5), cv2.FONT_HERSHEY_SIMPLEX, 1, (0, 255, 255), 1,cv2.LINE_AA)
#         if(area == 400):
#             break
#         msg = str(area)
#         client_executor.send(bytes(msg.encode('utf-8')))
#         # print("area=",area)
#         # print("msg=",msg)
#         # print("\n\n")
#         #event = threading.Event()
#        # event.wait(0.5)
#         time.sleep(0.3)
    

#     return image