import cv2
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

#釋放資源
# cam.release()
cv2.destroyAllWindows()