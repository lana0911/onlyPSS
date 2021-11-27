import os
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
# 聿涵區
import sys
from sys import platform
import argparse
# import imutils
import glob
import random
import math
import argparse
i=0

# 憶萱區
import jieba
import random
import json

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

cellphone=[]
imgStatus=[0] * 20
#分類
def classfly(client_executor, addr):
    print("welcome to classfy")
    print('Accept new connection from %s:%s...' % addr)
    #收到Client是誰訊息 =>加入聯絡人List
    who_recv = client_executor.recv(1024)
    who = who_recv.decode('utf-8') #我原本用的解碼
    who_jpy = who #local run時
    # who_jpy = jpysocket.jpydecode(who_recv) #jpy解碼
    print("一開始收到的->",who,"-<")
    #-------------------------------------------
    #加入通訊
    if(who==""):
        client_executor.close()
    if(who=="1"):#unity看板
        print("who==",who)
        clients.append(client_executor)#加入list
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
        while True:
            msg = client_executor.recv(1024) 
            # msg_jpy = jpysocket.jpydecode(msg)
            msg = msg.decode('utf-8')  
            msg_jpy = msg    #local run時
            print("開始到")
            print("msg=",msg) ##msg範例 : text;welcome
            print("msg_jpy=",msg_jpy) ##msg範例 : text;welcome
            #將收到的訊息分割 [0]:目標 [1...]:內容
            global yesOrno
            global ChatToWho
            msg_split = msg_jpy.split(";")
            target = msg_split[0]
            target_msg = msg_split[1]
            print("tagrt=",target)
            #CHAT 要傳是誰 && 要不要給看板
            if(len(msg_split) == 3):
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
                    break
            if(target == "pauma"):
                text(client_executor,msg)
            if(target == "game2"):
                global playing2 
                if(playing2==True):#已經有人在玩
                    client_executor.send("sorry, someone playing...".encode('utf-8'))
                else:
                    client_executor.send("遊戲即將開始".encode('utf-8'))
                    game2(client_executor,msg_jpy)
            if(target == "chat"):
                print("chat")
                chat(client_executor, target_msg, yesOrno, ChatToWho)
    elif(who_jpy == "3"):#手機專門傳圖片
        print("who==3+",who)
        # img_index = client_executor.recv(1024) 
        # img_index = jpysocket.jpydecode(img_index) 
        # print("img_index=",img_index)      
        imgWrite(client_executor)
    elif(who == "4"):
        print("收到4")
        kinect(client_executor)

    else:
        print("不是空/不是unity/不是手機端")


##Chat BOT------------------------------
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
    print("Q:===",msg,"senTo==",sendTo, "msg===",msg, "ChatToWho===",ChatToWho)
    seg_list = jieba.lcut(msg, cut_all=True)

    # print("|".join(seg_list))

    intent = predictIntent(seg_list)
    # print(intent)

    answer = Intent2Answer(intent)

    print("回覆: ", answer)
    client_executor.send(jpysocket.jpyencode(answer))
    # client_executor.send(answer.encode('utf-8'))
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
def imgShot():
    print("enter imgshot")
    path = "D:/openpose-1.6.0/build/Shot.png"
    # 將unity的截圖show出來
    cap = cv2.imread("D:/openpose-1.6.0//build//Shot.png")
    # img = cv2.resize(cap, (256, 320))
    # cv2.imwrite("D:/openpose-1.6.0//build//Shot.png", img)
    """ 
    !!!!!!!!!!!!!To聿涵: 
                    要show的話imshow就不能傳值 
                    所以你要測試的話要注意
                    建議直接去看D:/screenshot的Shot.png我存在那
    """
    dir_path = os.path.dirname(os.path.realpath(__file__))
    try:
        # Windows Import
        if platform == "win32":
            # Change these variables to point to the correct folder (Release/x64 etc.)
            sys.path.append(dir_path + '/../../python/openpose/Release')
            # os.environ['PATH'] = os.environ['PATH'] + ';' + \
            #    dir_path + '/../../x64/Release;' + dir_path + '/../../bin;'
            os.add_dll_directory(dir_path + '/../../x64/Release')
            os.add_dll_directory(dir_path + '/../../bin')

            import pyopenpose as op
        else:
            # Change these variables to point to the correct folder (Release/x64 etc.)
            sys.path.append('../../python')
            # If you run `make install` (default path is `/usr/local/python` for Ubuntu), you can also access the OpenPose/python module from there. This will install OpenPose and the python library at your desired installation path. Ensure that this is in your python path in order to use it.
            # sys.path.append('/usr/local/python')
            from openpose import pyopenpose as op
    except ImportError as e:
        print('Error: OpenPose library could not be found. Did you enable `BUILD_PYTHON` in CMake and have this Python script in the right folder?')
        raise e

    # Flags
    parser = argparse.ArgumentParser()
    parser.add_argument("--image_path", default="../../../examples/media/COCO_val2014_000000000192.jpg",
                        help="Process an image. Read all standard formats (jpg, png, bmp, etc.).")
    args = parser.parse_known_args()

    # Custom Params (refer to include/openpose/flags.hpp for more parameters)
    params = dict()
    params["model_folder"] = "../../../models/"
    params["net_resolution"] = "256x320"

    # Add others in path?
    for i in range(0, len(args[1])):
        curr_item = args[1][i]
        if i != len(args[1])-1:
            next_item = args[1][i+1]
        else:
            next_item = "1"
        if "--" in curr_item and "--" in next_item:
            key = curr_item.replace('-', '')
            if key not in params:
                params[key] = "1"
        elif "--" in curr_item and "--" not in next_item:
            key = curr_item.replace('-', '')
            if key not in params:
                params[key] = next_item

    # Construct it from system arguments
    # op.init_argv(args[1])
    # oppython = op.OpenposePython()

    # Starting OpenPose

    #opWrapper = op.WrapperPython(op.ThreadManagerMode.Synchronous)
    opWrapper = op.WrapperPython()
    opWrapper.configure(params)
    opWrapper.start()
    # opWrapper.execute()
    print("start")
    datum = op.Datum()
    pose = ""
    uans = "a"
    cans = "a"
    num = "0"
    user = 0
    while True:
        print("read")
        cap = cv2.imread("D:/openpose-1.6.0//build//Shot.png")
        print("read end")
        datum.cvInputData = cap
        print("op")
        opWrapper.emplaceAndPop([datum])
        # print(str(datum.poseKeypoints))
        print("frame")
        frame = datum.cvOutputData

        if(str(datum.poseKeypoints) == "2.0" or str(datum.poseKeypoints) == "0.0"):  # 無偵測到人
            print("countinue")
            continue
        else:
            ans = random.randint(1, 3)
            print("cal ang")
            ang = cal_ang(
                datum.poseKeypoints[0][11], datum.poseKeypoints[0][8], datum.poseKeypoints[0][14])
            print("ang end")
            if ang >= 0:
                uans = "usc"
                user = 1
            elif ang <= -17:
                uans = "up"
                user = 3
            elif ang < -3:
                uans = "ust"
                user = 2
            else:
                uans = " "
                user = 0

            if ans == 1:
                if user == 1:
                    pose = "pose;1 1"
                elif user == 2:
                    pose = "pose;2 1"
                else:
                    pose = "pose;3 1"
                cans = "psc"
            elif ans == 2:
                if user == 1:
                    pose = "pose;1 2"
                elif user == 2:
                    pose = "pose;2 2"
                else:
                    pose = "pose;3 2"
                cans = "pst"
            elif ans == 3:
                if user == 1:
                    pose = "pose;1 3"
                elif user == 2:
                    pose = "pose;2 3"
                else:
                    pose = "pose;3 3"
                cans = "pp"
            print("pose;" + uans + " " + cans)
            print(pose)
            if(pose != ""):
                clients[0].send(pose.encode('utf-8'))
                break

    cv2.waitKey(0)
    cv2.destroyAllWindows()

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
    area = 0
    scale = 0
    #框框
    for(x, y, w, h) in faces:
        cv2.rectangle(image, (x,y), (x+w, y+h), (0,255,0), 2)
        area = abs(w) * abs(h)
        if(area == None): 
            area = 0
        text = str(area)
        who = str(cnt)
        if(area > list):
            list = area
        cnt+=1
        scale = list


            

  
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
    listener.bind(('192.168.1.106', 5050))
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


