import socket
import cv2
import sys
from sys import platform
import argparse
import random
import argparse
import os
data=""
im = "5"
# 构建一个实例，去连接服务端的监听端口。
client = socket.socket(socket.AF_INET, socket.SOCK_STREAM)
client.connect(('192.168.2.102', 5050))
client.send(im.encode('utf-8'))
# 告知
#client.send(bytes('client'.encode('utf-8')))

# 不断获取输入，并发送给服务端。

print('begin write image file "moonsave.png"')
imgFile = open('moonsave.png', 'wb+')  # 開始寫入圖片檔
while True:
    imgData = client.recv(1024)  # 接收遠端主機傳來的數據
    print("+++");
    print(imgData);
    print("+++");
    if imgData == b' ':
        break  # 讀完檔案結束迴圈
    imgFile.write(imgData)
imgFile.close()
print('image save')

print("enter imgshot")
path = "Shot.png"
# 將unity的截圖show出來
cap = cv2.imread("moonsave.png")
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
        sys.path.append(dir_path + '/Release')
        os.environ['PATH'] = os.environ['PATH'] + ';' + \
            dir_path + '/Release;' + dir_path + '/bin;'

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
params["model_folder"] = "models/"
params["net_resolution"] = "16x16"

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
    cap = cv2.imread("Shot.png")
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
        elif ang < -2:
            uans = "ust"
            user = 2
        else:
            #no ans
            uans = "usc"
            user = 1

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
            client.send(pose.encode('utf-8'))
            break

cv2.waitKey(0)
cv2.destroyAllWindows()

client.close()