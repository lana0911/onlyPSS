using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;

using UnityEngine.Video;
//引入庫
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

 public static class _Vector2
 {
     public static float AngleTo(this Vector2 this_, Vector2 to)
     {
         float angle = ( Mathf.Atan2(to.y, to.x) - Mathf.Atan2(this_.y, this_.x) ) / Mathf.PI * 180;
		 
		 //Vector2 direction = to - this_;
         //float angle = Mathf.Atan2(direction.y,  direction.x) / Mathf.PI * 180;  // * Mathf.Rad2Deg;
		 
         if (angle < 0f) angle += 360f;
         return angle;
     }
 }

public class ModelMove : MonoBehaviour
{
	
	//以下預設都是私有的成員
    Socket serverSocket; //伺服器端socket
    Socket clientSocket; //客戶端socket
    IPEndPoint ipEnd; //偵聽埠
    string recvStr; //接收的字串
    string sendStr; //傳送的字串
    byte[] recvData=new byte[1024]; //接收的資料，必須為位元組
    byte[] sendData=new byte[1024]; //傳送的資料，必須為位元組
    int recvLen; //接收的資料長度
    Thread connectThread, videoThread; //連線執行緒
	
	// Start is called before the first frame update
	GameObject camera;
	GameObject leftHand, leftForeArm, leftShoulder;
	GameObject rightHand, rightForeArm, rightShoulder;
	GameObject head, neck, spine2;
	GameObject leftFoot, leftLeg, leftupLeg;
	GameObject rightFoot, rightLeg, rightupLeg;
	GameObject temp;
	VideoPlayer playVideo;
	Vector3 leftHandPos, leftForeArmPos, leftShoulderPos;
	Vector3 headPos, neckPos, spine2Pos;
	Vector3 rightHandPos, rightForeArmPos, rightShoulderPos;
	Vector3 leftFootPos, leftLegPos, leftupLegPos;
	Vector3 rightFootPos, rightLegPos, rightupLegPos;
	Vector2 vector2From, vector2To;
	float angle, angle1;
	int flag = 0;




	private string[] text;
	int video = 0;
	int txtCount = 0;
	
	int time, time1;
	int second, abs = 0;
	int timeFlag;
	int frameCount = 0;
	int[] millisecond = new int[] {167, 166, 167, 167, 166, 167};
	int count = 0;

    
	

    //伺服
    void Start()
    {
		/**/
		leftHandPos = Gobal_TCP.leftHandPos;
		Debug.Log("~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~" + leftHandPos);

		leftForeArmPos = Gobal_TCP.leftForeArmPos;
		leftShoulderPos = Gobal_TCP.leftShoulderPos;

		headPos = Gobal_TCP.headPos;
		neckPos = Gobal_TCP.neckPos;
		spine2Pos = Gobal_TCP.spine2Pos;
		rightHandPos = Gobal_TCP.rightHandPos;
		rightForeArmPos = Gobal_TCP.rightForeArmPos;
		rightShoulderPos = Gobal_TCP.rightShoulderPos;
		leftFootPos = Gobal_TCP.leftFootPos;
		leftLegPos = Gobal_TCP.leftLegPos;
		leftupLegPos = Gobal_TCP.leftupLegPos;

		rightFootPos = Gobal_TCP.rightFootPos;
		rightLegPos = Gobal_TCP.rightLegPos;
		rightupLegPos = Gobal_TCP.rightupLegPos;
		//*--------------------------------------


		rightHand = GameObject.Find("Character1_RightHand");
	   rightForeArm = GameObject.Find("Character1_RightForeArm");
	   rightShoulder = GameObject.Find("Character1_RightShoulder");
	   leftShoulder = GameObject.Find("Character1_LeftShoulder");
	   leftForeArm = GameObject.Find("Character1_LeftForeArm");
	   leftHand = GameObject.Find("Character1_LeftHand");
	   head = GameObject.Find("Character1_Head");
	   neck = GameObject.Find("Character1_Neck");
	   spine2 = GameObject.Find("Character1_Spine1");
	   leftFoot = GameObject.Find("Character1_LeftFoot");
	   leftLeg = GameObject.Find("Character1_LeftLeg");
	   leftupLeg = GameObject.Find("Character1_LeftUpLeg");
	   rightFoot = GameObject.Find("Character1_RightFoot");
	   rightLeg = GameObject.Find("Character1_RightLeg");
	   rightupLeg = GameObject.Find("Character1_RightUpLeg");
	
	   //playVideo.Volume = 0;
	   
	   temp = GameObject.Find("Character1_Hips");
	   camera = GameObject.Find("Main Camera");
	   //print(leftShoulder.transform.rotation);
	   // print(leftHand.transform.rotation);
	   // print(rightHand.transform.rotation);
	   // print(temp.transform.rotation);
	   //print(rightHand.transform.position);
	   
		int diff = 728 + 97 * 4;
		int scoreDiff = 154;
		int totalCount = 765*15 + 765*8;
		
		int angleCheck = 45;
		int startcheck = 0;
		
		int bad = 0;
		int badcheck = 0;


	}

    // Update is called once per frame
    void Update()
    {
		if(flag == 1) {
        //print(rightHand.transform.position);
		//leftShoulder.transform.rotation = leftShoulderOri;
		//leftForeArm.transform.rotation = leftForeArmOri;
		//leftHand.transform.rotation = leftHandOri;
		
		//print(temp.transform.position.y);
		
		Quaternion opred = Quaternion.Euler(0f, 90f, 0f);

		//leftHand
		leftShoulder.transform.position = leftShoulderPos;
		leftForeArm.transform.position = leftForeArmPos;
		leftHand.transform.position = leftHandPos;	
		Quaternion rotation = Quaternion.LookRotation(leftForeArmPos - leftShoulderPos, Vector3.right);
		leftShoulder.transform.rotation = rotation * opred;	
		rotation = Quaternion.LookRotation(leftHandPos - leftForeArmPos, Vector3.right);
		leftForeArm.transform.rotation = rotation * opred;
		
		//head
		//head.transform.position = headPos;
		neck.transform.position = neckPos;
		spine2.transform.position = spine2Pos;
		rotation = Quaternion.LookRotation(headPos - neckPos, Vector3.forward); 
		neck.transform.rotation = rotation * opred;
		rotation = Quaternion.LookRotation(neckPos - spine2Pos, Vector3.forward); 
		spine2.transform.rotation = rotation * opred;
		
		//rightHand
		rightShoulder.transform.position = rightShoulderPos;
		rightForeArm.transform.position = rightForeArmPos;
		rightHand.transform.position = rightHandPos;	
		rotation = Quaternion.LookRotation(rightForeArmPos - rightShoulderPos, Vector3.left);
		rightShoulder.transform.rotation = rotation * opred;	
		rotation = Quaternion.LookRotation(rightHandPos - rightForeArmPos, Vector3.left);
		rightForeArm.transform.rotation = rotation * opred;
		
		//leftFoot
		leftupLeg.transform.position = leftupLegPos;
		leftLeg.transform.position = leftLegPos;
		leftFoot.transform.position = leftFootPos;
		rotation = Quaternion.LookRotation(leftLegPos - leftupLegPos, Vector3.forward);
		leftupLeg.transform.rotation = rotation * opred;
		rotation = Quaternion.LookRotation(leftFootPos - leftLegPos, Vector3.forward);
		leftLeg.transform.rotation = rotation * opred;

		//rightFoot
		rightupLeg.transform.position = rightupLegPos;
		rightLeg.transform.position = rightLegPos;
		rightFoot.transform.position = rightFootPos;
		rotation = Quaternion.LookRotation(rightLegPos - rightupLegPos, Vector3.forward);
		rightupLeg.transform.rotation = rotation * opred;
		rotation = Quaternion.LookRotation(rightFootPos - rightLegPos, Vector3.forward);
		rightLeg.transform.rotation = rotation * opred;
		
		vector2From = (Vector2)(leftForeArmPos - leftShoulderPos);
		vector2To = (Vector2)(leftHandPos - leftForeArmPos);
		angle = vector2From.AngleTo(vector2To);
		print(angle);
		
		vector2From = (Vector2)(leftShoulderPos - neckPos);
		vector2To = (Vector2)(leftForeArmPos - leftShoulderPos);
		angle1 = vector2From.AngleTo(vector2To);
		print(angle1);
		print("---");

		if( angle > 148 && angle < 238 && angle1 > 62 && angle1 < 152 && video == 0) {
			//videoThread = new Thread(new ThreadStart(playVideo.Play));
			//videoThread.Start();
			//playVideo.Play();
			video = 1;
			time = DateTime.Now.Millisecond;
			second = DateTime.Now.Second;
		}
		
		if(video == 1) {
			if(DateTime.Now.Second != second) {
				abs = 1000;
			}else{
				abs = 0;
			}
			if(DateTime.Now.Millisecond + abs - time > 160) {
				timeFlag = 1;
				time = DateTime.Now.Millisecond;
				second = DateTime.Now.Second;
				count++;
				if(count == 6) count = 0;
			}
		}
		
		if(video == 1 && timeFlag == 1) {	//一幀
			print(float.Parse(text[txtCount]));
			txtCount++;	
			/*for(int i=0; i<15; i++) {
				float.Parse(text[++txtCount]);
			
			}*/
			txtCount += 4;
			angle = float.Parse(text[txtCount]);
			angle1 = float.Parse(text[++txtCount]);
			if(angle <= 180) {
				angle = 180 - angle;
			}else{
				angle = 540 - angle;
			}
			if(angle1 <= 180) {
				angle1 = 180 - angle1;
			}else{
				angle1 = 540 - angle1;
			}
			//print(angle);
			//print(angle1);
			//print("***");
			txtCount += 10;
			
			if(txtCount == 2448) {	//reset
				video = 0;
				txtCount = 0;
			}
			
			timeFlag = 0;
		}
		
		} // if flag = 1		
    }
	
	void OnApplicationQuit()
    {
    }
}
