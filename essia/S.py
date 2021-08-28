from os import truncate
import numpy as np
import cv2
import socket
import threading
import time
from threading import Timer
import jpysocket
import base64
import time
i=0
#編號
index = 0
playing = False 
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
cellphone=[]
imgStatus=[0] * 20
#分類
def classfly(client_executor, addr):
    print("welcome to classfy")
    print('Accept new connection from %s:%s...' % addr)
    
    #收到Client是誰訊息 =>加入聯絡人List
    who_recv = client_executor.recv(1024)
    who = who_recv.decode('utf-8') #我原本用的解碼
    who_jpy = jpysocket.jpydecode(who_recv) #jpy解碼
    print("一開始收到的->",who,"-<")
    #-------------------------------------------
    #加入通訊
    if(who==""):
        client_executor.close()
    if(who=="1"):#unity看板
        print("who==",who)
        clients.append(client_executor)#加入list
        unityRecv(client_executor)#開啟迴圈監聽
    elif(who_jpy=="2"):#手機cliet
        print("who==",who)
        #給編號
        global index
        print("index=",index)
        index_str = "index;" + str(index)
        client_executor.send(jpysocket.jpyencode(index_str))
        cellphone.insert(index, client_executor)
        print("-----------------------------------cellphone=",cellphone)
        index += 1
        #不斷接收client(手機)傳來的訊息
        while True:
            msg = client_executor.recv(1024) 
            msg_jpy = jpysocket.jpydecode(msg)
            msg = msg.decode('utf-8')      
            print("開始到")
            print("msg=",msg) ##msg範例 : text;welcome
            print("msg_jpy=",msg_jpy) ##msg範例 : text;welcome
            #將收到的訊息分割 [0]:目標 [1...]:內容
            msg_split = msg_jpy.split(";")
            target = msg_split[0]
            print("tagrt=",target)
            #taget是要傳訊息到看板
            if(target == "text"):
                text(client_executor,msg_jpy)
            #game1是要玩猜拳
            if target == "game1" :
                print("猜拳")
                global playing 
                if(playing==True):#已經有人在玩
                    client_executor.send("sorry, someone playing...".encode('utf-8'))
                else:
                    game1(client_executor,msg)
                    #client_executor.send("遊戲即將開始".encode('utf-8'))
            #要辨識人臉
            if(target == "facer"):
                print("收到手機傳facer")
                client_executor.send(jpysocket.jpyencode("StartSend"))
                img_over_str = client_executor.recv(1024) 
                img_over_str = jpysocket.jpydecode(img_over_str) 
                #暫停3秒等照片+辨識
                # time.sleep(5)
                # print("5秒結束")
                print("img_over_str=",img_over_str)
                if(img_over_str == "imgover"):
                    print("status==1")
                    #開始讀檔
                    fa = open("C:/Users/Lana/Documents/GitHub/onlyPSS/essia/rec.txt","r")
                    ans = fa.readline()
                    print(ans)
                    client_executor.send(jpysocket.jpyencode(ans))
                    print('send complete')
                    client_executor.close()
                    break
            if(target == "pauma"):
                text(client_executor,msg)
    elif(who_jpy == "3"):#手機專門傳圖片
        print("who==3+",who)
        # img_index = client_executor.recv(1024) 
        # img_index = jpysocket.jpydecode(img_index) 
        # print("img_index=",img_index)      
        imgWrite(client_executor)

    else:
        print("不是空/不是unity/不是手機端")

def imgWrite(client_executor):
    BUFSIZ = 1024*20
    rec_d = bytes([])
    print("副函")
    while True:
        data = client_executor.recv(BUFSIZ)
        if not data or len(data) == 0:
            break
        else:
            rec_d = rec_d + data
            # print(rec_d)
    print("break")
    path = 'C:/Users/Lana/Documents/GitHub/onlyPSS/essia/d.txt'
    f = open(path, 'w')
    f.write(str(rec_d))
    f.close()
    print("ok1")
    #轉成圖片檔
    with open("C:/Users/Lana/Documents/GitHub/onlyPSS/essia/d.txt","r") as f:
        img = base64.b64decode(f.read()[1:])
        print(type(f.read()))
        fh = open("C:/Users/Lana/Documents/GitHub/onlyPSS/essia/pic_2_sucess.jpg","wb")
        fh.write(img)
        fh.close()
    print("ok2")


    # img_index = client_executor.recv(1024) 
    # img_index = jpysocket.jpydecode(img_index) 
    # print("img_index=",img_index)   
    # imgStatus[int(img_index)] = 1
    # print("更改",imgStatus)
    time.sleep(1)        
 

#接收unity傳來的
def unityRecv(client_executor):
    #img_scale(client_executor)
    print("-----------------開始監聽unity傳來的訊息----------------------")
    global playing 
    while True:
        recv = client_executor.recv(1024).decode('utf-8')
        recv_split = recv.split(";")
        print("unity傳來:",recv_split)
        if(recv_split[0]=="pose"):#看板說現在給結果!
            startPose()
        if(recv_split[0]=="shot"):#讀取截圖 測試用
            imgShot()
        elif(recv_split[0]=="over"):
            playing = False
            global t_face
            # if(recv_split[1]=="0"):
            client_executor.send("over".encode('utf-8'))
            # client_executor.close()
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

# def img_scale(client_executor) :
#     setting(client_executor, addr)

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
    print("game1")
    clients[0].send(bytes("game1;".encode('utf-8')))

def text(client_executor, content):
    print("text()中心收到訊息:",content)
    # #傳給看板 e.g.: text;Welcome
    clients[0].send(bytes(content.encode('utf-8')))
    client_executor.send("收到".encode('utf-8'))
   

def seand_scale():
    global scale
    global playing
    while (True):
        # print("scale=",scale)
        scale_send = "scale; "+ str(scale)
       # print("scale_send=",scale_send)
        if(len(clients)==0):
            # print("none")
            n=2
        else:
            # print("yes")
            if(playing==False):
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
      
    #開啟鏡頭
    global cam
    print("isopen",cam.isOpened())
    if(cam.isOpened()==False and playing==False):
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
        if(playing== True):
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

def face_recognizer():
    print("face_recognizer")
    #準備好識別方法
    recognizer = cv2.face.LBPHFaceRecognizer_create()
    #使用之前訓練好的模型
    recognizer.read('trainner/trainner.yml')
    #再次呼叫人臉分類器
    cascade_path = "haarcascade_frontalface_default.xml" 
    face_cascade = cv2.CascadeClassifier(cascade_path)

    #載入一個字型，用於識別後，在圖片上標註出物件的名字
    font = cv2.FONT_HERSHEY_SIMPLEX

    idnum = 0
    #設定好與ID號碼對應的使用者名稱，如下，如0對應的就是初始

    names = ['初始','Chaeyoung','mina','Nayeon','momo']

    #呼叫攝像頭
    # cam = cv2.VideoCapture(0)
    # minW = 0.1*cam.get(3)
    # minH = 0.1*cam.get(4)

    while True:
        # ret,img = cam.read()
        
        img = cv2.imread("C:/Users/Lana/Documents/GitHub/onlyPSS/essia/pic_2_sucess.jpg")
        gray = cv2.cvtColor(img, cv2.COLOR_BGR2GRAY)
        #識別人臉
        faces = face_cascade.detectMultiScale(
                gray,
                scaleFactor = 1.2,
                minNeighbors = 5,
                # minSize = (int(minW),int(minH))
                )
        #進行校驗
        for(x,y,w,h) in faces:
            cv2.rectangle(img,(x,y),(x+w,y+h),(0,255,0),2)
            idnum,confidence = recognizer.predict(gray[y:y+h,x:x+w])

            #計算出一個檢驗結果
            if confidence < 100:
                idum = names[idnum]
                confidence = "{0}%",format(round(100-confidence))
            else:
                idum = "unknown"
                confidence = "{0}%",format(round(100-confidence))

            #輸出檢驗結果以及使用者名稱
            cv2.putText(img,str(idum),(x+5,y-5),font,1,(0,0,255),1)
            # cv2.putText(img,str(confidence),(x+5,y+h-5),font,1,(0,0,0),1)
            f = open('C:/Users/Lana/Documents/GitHub/onlyPSS/essia/rec.txt','w')
            f.write(str(idum))
            #展示結果
            cv2.imshow('camera',img)
            k = cv2.waitKey(20)
            if k == 27:
                break

#主函式
if __name__ == '__main__':
    # IP , Port......設定
    listener = socket.socket(socket.AF_INET, socket.SOCK_STREAM)
    listener.bind(('192.168.50.21', 5050))
    listener.listen(5)
    print('Waiting for connect...')
    #建List
    list_num=0
    list = []
    t_face.start()     
    t_send = threading.Thread(target=seand_scale)
    t_send.start()
    #辨識code
    t_recog = threading.Thread(target=face_recognizer)
    t_recog.start()
    #辨識圖片
    while True:
        client_executor, addr = listener.accept()
        
        t = threading.Thread(target=classfly, args=(client_executor, addr))
        t.start()


