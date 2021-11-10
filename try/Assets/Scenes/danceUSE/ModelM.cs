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



public class ModelM : MonoBehaviour
{

	//以下預設都是私有的成員
	Socket serverSocket; //伺服器端socket
	Socket clientSocket; //客戶端socket
	IPEndPoint ipEnd; //偵聽埠
	string recvStr; //接收的字串
	string sendStr; //傳送的字串
	byte[] recvData = new byte[1024]; //接收的資料，必須為位元組
	byte[] sendData = new byte[1024]; //傳送的資料，必須為位元組
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
	public VideoPlayer playVideo;
	Vector3 leftHandPos, leftForeArmPos, leftShoulderPos;
	Vector3 headPos, neckPos, spine2Pos;
	Vector3 rightHandPos, rightForeArmPos, rightShoulderPos;
	Vector3 leftFootPos, leftLegPos, leftupLegPos;
	Vector3 rightFootPos, rightLegPos, rightupLegPos;
	Vector2 vector2From, vector2To;
	Vector3[] vectorArray;
	float angle, angle1;
	int flag = 0;


	float i = 0.0f;

	private string[] text;

	int video_start;
	int txtCount = 1;
	int tesFlag = 0;

	//score1101
	int score = 0;

	int time, time1;
	int second, abs = 0;
	int frameCount = 0;
	int[] millisecond = new int[] { 167, 166, 167, 167, 166, 167 };
	int count = 0;


	Vector3 zoom(Vector3 start, Vector3 end, float length)
	{
		Vector3 vector = end - start;

		return (float)(length / Math.Sqrt((double)vector.x * vector.x + vector.y * vector.y + vector.z * vector.z)) * vector;
	}


	//伺服
	void Start()
	{

		vectorArray = new Vector3[15];

		transform.rotation = Quaternion.Euler(0.0f, 0.0f, 0.0f);


		//vectorArray
		vectorArray = new Vector3[] {Gobal_TCP.leftShoulderPos, Gobal_TCP.leftForeArmPos, Gobal_TCP.leftHandPos,
										Gobal_TCP.headPos, Gobal_TCP.neckPos, Gobal_TCP.spine2Pos,
										Gobal_TCP.rightShoulderPos, Gobal_TCP.rightForeArmPos, rightHandPos,
										Gobal_TCP.leftupLegPos, Gobal_TCP.leftLegPos, Gobal_TCP.leftFootPos,
										Gobal_TCP.rightupLegPos, Gobal_TCP.rightLegPos, Gobal_TCP.rightFootPos};

		/**/
		leftHandPos = Gobal_TCP.leftHandPos;
		// Debug.Log("~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~start~" + leftHandPos);
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

		//video GameObject.FindGameObjectsWithTag("NPC");
		playVideo = GetComponent<VideoPlayer>();
		//playVideo = GameObject.FindGameObjectsWithTag("MV").GetComponent<VideoPlayer>();
		playVideo.Stop();

		//model length test
		//    print("1: " + Vector3.Distance(head.transform.position, neck.transform.position));
		//    print("2: " + Vector3.Distance(neck.transform.position, spine2.transform.position));
		//    print("3: " + Vector3.Distance(neck.transform.position, leftShoulder.transform.position));
		//    print("4: " + Vector3.Distance(leftShoulder.transform.position, leftForeArm.transform.position));
		//    print("5: " + Vector3.Distance(leftForeArm.transform.position, leftHand.transform.position));
		//    print("6: " + Vector3.Distance(neck.transform.position, rightShoulder.transform.position));
		//    print("7: " + Vector3.Distance(rightShoulder.transform.position, rightForeArm.transform.position));
		//    print("8: " + Vector3.Distance(rightForeArm.transform.position, rightHand.transform.position));
		//    print("9: " + Vector3.Distance(spine2.transform.position, leftupLeg.transform.position));
		//    print("10: " + Vector3.Distance(leftupLeg.transform.position, leftLeg.transform.position));
		//    print("11: " + Vector3.Distance(leftLeg.transform.position, leftFoot.transform.position));
		//    print("12: " + Vector3.Distance(spine2.transform.position, rightupLeg.transform.position));
		//    print("13: " + Vector3.Distance(rightupLeg.transform.position, rightLeg.transform.position));
		//    print("14: " + Vector3.Distance(rightLeg.transform.position, rightFoot.transform.position));

		//playVideo.Volume = 0;

		temp = GameObject.Find("Character1_Hips");
		camera = GameObject.Find("Main Camera");

		int diff = 728 + 97 * 4;
		int scoreDiff = 154;
		int totalCount = 765 * 15 + 765 * 8;

		int angleCheck = 45;
		int startcheck = 0;

		int bad = 0;
		int badcheck = 0;

		//text
		// text = 	.ReadAllLines("D:\\大學\\PoseAd\\pose\\point\\test07.txt");
		//text = File.ReadAllLines("D:\\大學\\onlyPSS\\onlyPSS\\test110101_1_800.txt");
		//text = File.ReadAllLines("D:\\test110101_1_800.txt");
		//path修改
		var path = @"D:\example.txt";
		var txt = File.ReadAllText(path);
		Debug.Log(txt);
		// string path = @"d:\大學\onlyPSS\onlyPSS\\test.txt";
		// string[] createText = {""};
		// File.WriteAllLines(path, createText, Encoding.UTF8);
		// Fil.WriteLine("test");


	}

	// Update is called once per frame
	void Update()
	{
		Vector3 test_s = new Vector3(0.0f, 0.0f, 0.0f);
		Vector3 test_e = new Vector3(3.0f, 4.0f, 0.0f);
		// print(zoom(test_s, test_e, 2.0f));

		//vectorArray
		vectorArray = new Vector3[] {Gobal_TCP.leftShoulderPos, Gobal_TCP.leftForeArmPos, Gobal_TCP.leftHandPos,
										Gobal_TCP.headPos, Gobal_TCP.neckPos, Gobal_TCP.spine2Pos,
										Gobal_TCP.rightShoulderPos, Gobal_TCP.rightForeArmPos, rightHandPos,
										Gobal_TCP.leftupLegPos, Gobal_TCP.leftLegPos, Gobal_TCP.leftFootPos,
										Gobal_TCP.rightupLegPos, Gobal_TCP.rightLegPos, Gobal_TCP.rightFootPos};

		leftHandPos = Gobal_TCP.leftHandPos;
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

		//zoom test
		neckPos = Gobal_TCP.headPos + zoom(Gobal_TCP.headPos, Gobal_TCP.neckPos, 0.07827914f);
		spine2Pos = Gobal_TCP.spine2Pos + neckPos - Gobal_TCP.neckPos;
		spine2Pos = neckPos + zoom(neckPos, spine2Pos, 0.2430313f);
		leftShoulderPos = Gobal_TCP.leftShoulderPos + neckPos - Gobal_TCP.neckPos;
		leftShoulderPos = neckPos + zoom(neckPos, leftShoulderPos, 0.05515538f);
		leftForeArmPos = Gobal_TCP.leftForeArmPos + leftShoulderPos - Gobal_TCP.leftShoulderPos;
		leftForeArmPos = leftShoulderPos + zoom(leftShoulderPos, leftForeArmPos, 0.2892137f);
		leftHandPos = Gobal_TCP.leftHandPos + leftForeArmPos - Gobal_TCP.leftForeArmPos;
		leftHandPos = leftForeArmPos + zoom(leftForeArmPos, leftHandPos, 0.1624821f);
		rightShoulderPos = Gobal_TCP.rightShoulderPos + neckPos - Gobal_TCP.neckPos;
		rightShoulderPos = neckPos + zoom(neckPos, rightShoulderPos, 0.05515538f);
		rightForeArmPos = Gobal_TCP.rightForeArmPos + rightShoulderPos - Gobal_TCP.rightShoulderPos;
		rightForeArmPos = rightShoulderPos + zoom(rightShoulderPos, rightForeArmPos, 0.2892137f);
		rightHandPos = Gobal_TCP.rightHandPos + rightForeArmPos - Gobal_TCP.rightForeArmPos;
		rightHandPos = rightForeArmPos + zoom(rightForeArmPos, rightHandPos, 0.1624821f);

		leftupLegPos = Gobal_TCP.leftupLegPos + spine2Pos - Gobal_TCP.spine2Pos;
		leftupLegPos = spine2Pos + zoom(spine2Pos, leftupLegPos, 0.1893908f);
		leftLegPos = Gobal_TCP.leftLegPos + leftupLegPos - Gobal_TCP.leftupLegPos;
		leftLegPos = leftupLegPos + zoom(leftupLegPos, leftLegPos, 0.3519265f);
		leftFootPos = Gobal_TCP.leftFootPos + leftLegPos - Gobal_TCP.leftLegPos;
		leftFootPos = leftLegPos + zoom(leftLegPos, leftFootPos, 0.3787041f);

		rightupLegPos = Gobal_TCP.rightupLegPos + spine2Pos - Gobal_TCP.spine2Pos;
		rightupLegPos = spine2Pos + zoom(spine2Pos, rightupLegPos, 0.1893908f);
		rightLegPos = Gobal_TCP.rightLegPos + rightupLegPos - Gobal_TCP.rightupLegPos;
		rightLegPos = rightupLegPos + zoom(rightupLegPos, rightLegPos, 0.3519265f);
		rightFootPos = Gobal_TCP.rightFootPos + rightLegPos - Gobal_TCP.rightLegPos;
		rightFootPos = rightLegPos + zoom(rightLegPos, rightFootPos, 0.3787041f);




		Quaternion opred = Quaternion.Euler(0f, 90f, 0f);
		//Debug.Log("~~~~~~~~~~~~~~~~~~~~~POSITION~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~" + leftShoulder.transform.position);
		// Debug.Log("~~~~~~~~~~~~~~~~~~~Gobal_TCP.rightHandPos~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~" + Gobal_TCP.rightHandPos);
		// Debug.Log("!!!!!!!!!!!!!!!!!!!!rightHandPos!!!!!!!!!!!!!!!!!!" + rightHandPos);


		//leftHand
		leftShoulder.transform.position = leftShoulderPos;
		leftForeArm.transform.position = leftForeArmPos;
		leftHand.transform.position = leftHandPos;
		Quaternion rotation = Quaternion.LookRotation(leftForeArmPos - leftShoulderPos, Vector3.right);
		leftShoulder.transform.rotation = rotation * opred;
		rotation = Quaternion.LookRotation(leftHandPos - leftForeArmPos, Vector3.right);
		leftForeArm.transform.rotation = rotation * opred;

		//head
		head.transform.position = headPos;
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




		if (Gobal_TCP.leftHandPos.y > Gobal_TCP.headPos.y)
		{
			tesFlag = 1;
			Gobal_TCP.handup = true;
		}

		if (tesFlag == 1 && Gobal_video.status == 0)
		{
			//videoThread = new Thread(new ThreadStart(playVideo.Play));
			//videoThread.Start();
			playVideo.Play();
			Gobal_video.status = 1;
			time = DateTime.Now.Millisecond;
			second = DateTime.Now.Second;
			video_start = second;
		}

		if (Gobal_video.status == 1)
		{
			if (DateTime.Now.Second != second)
			{
				abs = 1000;
			}
			else
			{
				abs = 0;
			}

			if (Mathf.Abs(DateTime.Now.Second - second) >= 1)
			{
				second = DateTime.Now.Second;
				Gobal_video.timeFlag = 1;

				// print(text[txtCount].Substring(2,9));
				print("count: " + txtCount);
				print(text);

				int a, b;
				a = 2;
				for (int i = 0; i < 15; i++)
				{

					b = (text[txtCount].Substring(a, 1) == "-") ? 9 : 8;
					if (Mathf.Abs(float.Parse(text[txtCount].Substring(a, b)) - vectorArray[i].x) < 0.10f)
					{
						score++;
					}
					a = a + b + 1;

					b = (text[txtCount].Substring(a, 1) == "-") ? 9 : 8;
					if (Mathf.Abs(float.Parse(text[txtCount].Substring(a, b)) - vectorArray[i].y) < 0.10f)
					{
						score++;
					}
					a = a + b + 1;

					b = (text[txtCount].Substring(a, 1) == "-") ? 9 : 8;
					if (Mathf.Abs(float.Parse(text[txtCount].Substring(a, b)) - vectorArray[i].z) < 0.10f)
					{
						score++;
					}
					a = a + b + 1;
				}

				txtCount++;

				// print("modelMove second");
			}
			if (txtCount == 25)
			{   //reset
				print("reset!!!!!!!!!11");
				print(score);
				Gobal_TCP.Dcore = score;
				Gobal_video.status = 0;
				Gobal_TCP.tu = true;
				tesFlag = 0;
				txtCount = 1;
			}
		}



	}

	void OnApplicationQuit()
	{
	}
}
