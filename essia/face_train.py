import os
import cv2
import numpy as np
from PIL import Image
#匯入pillow庫，用於處理影象
#設定之前收集好的資料檔案路徑
path = 'data'

#初始化識別的方法
recog = cv2.face.LBPHFaceRecognizer_create()

#呼叫熟悉的人臉分類器
detector = cv2.CascadeClassifier('haarcascade_frontalface_default.xml')

#建立一個函數，用於從資料集資料夾中獲取訓練圖片,並獲取id
#注意圖片的命名格式為User.id.sampleNum
def get_images_and_labels(path):
    image_paths = [os.path.join(path,f) for f in os.listdir(path)]
    #新建連個list用於存放
    face_samples = []
    ids = []

    #遍歷圖片路徑，匯入圖片和id新增到list中
    for image_path in image_paths:

        #通過圖片路徑將其轉換為灰度圖片
        img = Image.open(image_path).convert('L')

        #將圖片轉化為陣列
        img_np = np.array(img,'uint8')

        if os.path.split(image_path)[-1].split(".")[-1] != 'jpg':
            continue

        #為了獲取id，將圖片和路徑分裂並獲取
        print(image_path)
        id = int(os.path.split(image_path)[-1].split(".")[1])
        faces = detector.detectMultiScale(img_np)

        #將獲取的圖片和id新增到list中
        for(x,y,w,h) in faces:
            face_samples.append(img_np[y:y+h,x:x+w])
            ids.append(id)
    return face_samples,ids

#呼叫函數並將資料餵給識別器訓練
print('Training...')
faces,ids = get_images_and_labels(path)
#訓練模型
recog.train(faces,np.array(ids))
#儲存模型
recog.save('trainner/trainner.yml')