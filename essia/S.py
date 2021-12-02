import os
# from os import truncate
# import numpy as np
import cv2
import socket
import threading
import time
# from threading import Timer
import jpysocket
import base64
import time
# 聿涵區
import sys
from sys import platform
import argparse
# import imutils
# import glob
import random
import math
import argparse
import time
i=0

# 憶萱區
# import jieba
# import random
# import json
# import chatgui
# import os

#編號
index = 0
playing2 = False 
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
yesOrno = "yes"
ChatToWho = "empty"
jsonPath = 'empty' 
yuhanVar = False
cellphone=[]
imgStatus=[0] * 20
#分類
def classfly(client_executor, addr):
    print("welcome to classfy")
    print('Accept new connection from %s:%s...' % addr)
    #收到Client是誰訊息 =>加入聯絡人List
    who_recv = client_executor.recv(1024)
    who = who_recv.decode('utf-8') #我原本用的解碼
    
    
    #////////////////
    who_jpy = who #local run時
    # who_jpy = jpysocket.jpydecode(who_recv) #jpy解碼
    #////////////////



    print("一開始收到的->",who,"-<")
    #-------------------------------------------
    #加入通訊
    if(who==""):
        client_executor.close()
    if(who=="1"):#unity看板
        print("who==",who)
        clients.append(client_executor)#加入list
        clients[0]=(client_executor)
        print("--------------------------------------加入=",clients)
        unityRecv(client_executor)#開啟迴圈監聽
    elif(who_jpy=="2" or who == "2"):#手機cliet
        print("who==",who)
        #給編號
        global index
        print("index=",index)
        index_str = "index;" + str(index)
        # client_executor.send(jpysocket.jpyencode(index_str))
        # cellphone.insert(index, client_executor)
        print("-----------------------------------cellphone=",cellphone)
        index += 1
        #不斷接收client(手機)傳來的訊息
        # while True:
        msg = client_executor.recv(1024) 
        msg = msg.decode('utf-8')  
       
       
       
        #////////////////
        msg_jpy = msg    #local run時
        # msg_jpy = jpysocket.jpydecode(msg)
        #////////////////
        
        
        
        print("開始到")
        print("msg=",msg) ##msg範例 : text;welcome
        print("msg_jpy=",msg_jpy) ##msg範例 : text;welcome
        #將收到的訊息分割 [0]:目標 [1...]:內容
        global yesOrno   #chat; /msg/ yseNO /people
        global ChatToWho
        msg_split = msg_jpy.split(";")
        target = msg_split[0]
        target_msg = msg_split[1]
        print("tagrt=",target)
        #CHAT 要傳是誰 && 要不要給看板
        if(len(msg_split) == 3):
            print("msg_split[2]==", msg_split[2])
            yesOrno = msg_split[2] 
        else:
            yesOrno="yes"

        if(len(msg_split) == 4):
            ChatToWho = msg_split[3] 
        else:
            ChatToWho="empty"
        



        #測試用
        if(msg == "handup;"):
            print("hand")
            clients[0].send(bytes("handup;".encode('utf-8')))
            client_executor.send("好喔收到".encode('utf-8'))
            clients[0].send(bytes("Dcore;99".encode('utf-8')))
        #測試用
        if(msg == "Dcore;"):
            print("Dcore=", msg)
            clients[0].send(bytes(msg.encode('utf-8')))
            client_executor.send("好喔收到".encode('utf-8'))



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
                fa = open("rec.txt","r")
                ans = fa.readline()
                print(ans)
                client_executor.send(jpysocket.jpyencode(ans))
                print('send complete')
                client_executor.close()
                # break
        if(target == "pauma"):
            text(client_executor,msg)
        if(target == "game2"):
            print("target=game2")
            global playing2 
            if(playing2==True):#已經有人在玩
                print("game2有人在玩")
                client_executor.send("sorry, someone playing...".encode('utf-8'))
            else:
                client_executor.send("遊戲即將開始".encode('utf-8'))
                game2(client_executor,msg_jpy)
        if(target == "chat"):
            print("chat")
            t_chat = threading.Thread(target=chatRecv, args=(client_executor,)).start()   
            # t_chat.start()
            # chatCheck(client_executor, target_msg, yesOrno, ChatToWho)
            # chatCheck(client_executor)
            # chatRecv(client_executor)
            # chat(client_executor, target_msg, yesOrno, ChatToWho)
    elif(who_jpy == "3"):#手機專門傳圖片
        print("who==3+",who)
        # img_index = client_executor.recv(1024) 
        # img_index = jpysocket.jpydecode(img_index) 
        # print("img_index=",img_index)      
        imgWrite(client_executor)
    elif(who == "4"):
        print("收到4")
        kinect(client_executor)
    elif(who == "5"):
        print("收到5")
        yuhan(client_executor)

    else:
        print("不是空/不是unity/不是手機端")

def yuhan(client_executor):
    print("1231343ryjkyiluiou")
    global yuhanVar
    while(True):
        # print("yuhan funct",yuhanVar)

        if(yuhanVar):
            print('start send image')
            imgFile = open("Shot.png", "rb")
            while True:
                imgData = imgFile.readline(1024)
                client_executor.send(imgData)
                print("+++");
                print(imgData);
                print("+++");
                if not imgData:
                    time.sleep(1);
                    client_executor.send(b'')
                    break  # 讀完s檔案結束迴圈
            imgFile.close()
            
            yuhanVar = False
            print('transmit end')


    who_recv = client_executor.recv(1024)
    who = who_recv.decode('utf-8')
    clients[0].send(bytes(who.encode('utf-8')))

def chatRecv(client_executor):
    print("-----------------開始監聽Chat傳來的訊息----------------------")
    i=0
    while(True): 
        recv = client_executor.recv(1024)
        str = jpysocket.jpydecode(recv)
        msg_split = str.split(";")
        chat = msg_split[0]
        content = msg_split[1]
        yesNo = msg_split[2]
        person = msg_split[3]
        print("chat=",chat, "content=",content,"yesNo=",yesNo,"person=",person)
        if(chat == 'chat'):
            chatCheck(client_executor, content, person, yesNo)
##Chat BOT(英文版)------------------------------
def chatCheck(client_executor , content, person, yesNo):
    print("chatCheck")
    # while(True): 
    #     # str = "測試" 
    #     who_recv = client_executor.recv(1024) 
    #     who_jpy = jpysocket.jpydecode(who_recv) #jpy解碼 
    #     print("收:",who_jpy) 
    #     # str = input("input:") 
    #     # client_executor.send(jpysocket.jpyencode(str)) 
    #     # client_executor.send(str.encode('utf-8')) 
    #     client_executor.send(bytes("46",encoding = 'utf8'))
    # #分類給誰
    # print("進chatBot****************************")
    # print("content==",content, "person=",person)
    # client_executor.send(bytes("456",encoding = 'utf8')) 
    # return
    # return

    global jsonPath
    jsonPath = ''
    if(person=='mina'):
        jsonPath = 'mina'
    elif(person=='momo'):
        jsonPath = 'momo'
    jsonPath = jsonPath + '.json'
    ans = chatgui.chatbot_response(content)
    print("英文ans=",ans)
    if( ans=="Sorry, can't understand you" or 
        ans== "Please give me more info" or 
        ans=="Not sure I understand"
    ):
        print("英文版沒答案")
        chat(client_executor, content, yesNo, person)
    else:
        client_executor.send(bytes(ans,encoding = 'utf8'))#給手機
        answer = "text;" + ans
        if(yesNo == "yes"):
            print("yes")
            text(client_executor, answer) #給看板
        print("-----------------------------")
   

#Chat BOT(中文版)------------------------------
def chat(client_executor ,msg, sendTo, ChatToWho):
    #分類給誰
    global jsonPath
    jsonPath = ''
    if(ChatToWho=='mina'):
        jsonPath = 'mina'
    elif(ChatToWho=='momo'):
        jsonPath = 'momo'
    jsonPath = jsonPath + '.json'

    random.seed(time.time())

    with open(jsonPath,"r",encoding="utf-8") as json_data:
        dict = json.load(json_data)
        

    def predictIntent(word_list):
        for word in word_list:
            for dictCnt in range(0, len(dict)):
                for utterance in dict[dictCnt]['utterances']:
                    if word == utterance:
                        return dict[dictCnt]['intent']

        return "Unknown Intent"


    def Intent2Answer(input_intent):
        right_intent_dict_index = -1
        for dictCnt in range(0, len(dict)):
            if input_intent == dict[dictCnt]['intent']:
                right_intent_dict_index = dictCnt
                break

        answerNum = len(dict[right_intent_dict_index]['answers'])
        return dict[right_intent_dict_index]['answers'][random.randint(0, answerNum-1)]
    # while True:
    # print("Q:===",msg,"senTo==",sendTo, "msg===",msg, "ChatToWho===",ChatToWho)
    seg_list = jieba.lcut(msg, cut_all=True)

    # print("|".join(seg_list))

    intent = predictIntent(seg_list)
    # print(intent)

    answer = Intent2Answer(intent)

    print("中文版回覆: ", answer)
    # client_executor.send(jpysocket.jpyencode(answer))
    # client_executor.send(answer.encode('utf-8'))
    # client_executor.send(jpysocket.jpyencode(answer))
    # client_executor.send(str.encode('utf-8'))# 
    client_executor.send(bytes(answer,encoding = 'utf8'))
    answer = "text;"+answer
    if(sendTo == "yes"):
        print("yes")
        text(client_executor, answer)
    print("-----------------------------")
    return 
   
###怡君關節點--------------------------------
def kinect(client_executor):
    print("kinect副函")
    while True:
        recv = client_executor.recv(1024).decode('utf-8')
        print("收:"+recv)
        clients[0].send(bytes(recv.encode('utf-8')))
        
def imgWrite(client_executor):
    BUFSIZ = 1024*20
    rec_d = bytes([])
    print("副函")
    while True:
        data = client_executor.recv(BUFSIZ)
        print(data)
        if not data or len(data) == 0:
            break
        else:
            rec_d = rec_d + data
            # print(rec_d)
    print("break")
    path = 'd.txt'
    f = open(path, 'w')
    f.write(str(rec_d))
    f.close()
    print("ok1")
    #轉成圖片檔
    with open("d.txt","r") as f:
        img = base64.b64decode(f.read()[1:])
        print(type(f.read()))
        fh = open("pic_2_sucess.jpg","wb")
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
    global playing2 
    global yuhan
    while True:
        recv = client_executor.recv(1024).decode('utf-8')
        recv_split = recv.split(";")
        print("unity傳來:",recv_split)
        if(recv_split[0]=="pose"):#看板說現在給結果!
            startPose()
            global yuhan
            yuhan = True
        if(recv_split[0]=="shot"):#讀取截圖 測試用
            print("imgShot")
            imgShot(client_executor)
        elif(recv_split[0]=="over"):
            playing = False
            global t_face
            # if(recv_split[1]=="0"):
            client_executor.send("over".encode('utf-8'))
            # client_executor.close()
            time.sleep(3)
            t_face2 = threading.Thread(target=face)
            t_face2.start()
        elif(recv_split[0]=="over2"):
            playing2 = False
# -------------------------------------聿涵區🔻--------------------------------
def cal_ang(p1, p2, p3):
    # if p1[0]==0 or p1[1]==0 or p2[0]==0 or p2[1]==0 or p3[0]==0 or p3[1]==0:
    #     return -1

    vector1 = [p1[0]-p2[0], p1[1]-p2[1]]  # 8-11
    vector2 = [p3[0]-p2[0], p3[1]-p2[1]]  # 8-14
    angle = math.atan2(vector2[1], vector2[0]) - \
        math.atan2(vector1[1], vector1[0])
    angle = angle/math.pi*180  # change arc to degree
    # if angle < 0:
    #    angle= angle + 360
    return angle
def imgShot(client_executor): #unity IP
    global yuhanVar
    yuhanVar = True

    print("imgshot的yuhan",yuhanVar)
    

def imgShot2():
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
    # print("imgShot2")

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

#玩跳舞
def game2(client_executor,content):
    print("game2")
    #遊戲使用中
    global playing2 
    playing2 = True
    #傳給看板  e.g: game1
    clients[0].send(bytes(content.encode('utf-8')))


def text(client_executor, content):
    # print("text()中心收到訊息:",content)
    # #傳給看板 e.g.: text;Welcome
    # send = "text;" + content
    print("text()中心收到訊息:",content)
    clients[0].send(bytes(content.encode('utf-8')))
    # client_executor.send("收到".encode('utf-8'))
   

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
    # print("enter getface")
    global scale
    list = 0
    cnt = 0
    cvo = cv2.CascadeClassifier('haarcascade_frontalface_default.xml')
    cvo.load('C:\\Users\\user\\anaconda3\\Lib\\site-packages\\cv2\\data\\haarcascade_frontalface_default.xml')
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
    # print("getface")
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
        
        img = cv2.imread("pic_2_sucess.jpg")
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
            f = open('rec.txt','w')
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
    listener.bind(('10.22.27.161', 5050))
    listener.listen(20)
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


