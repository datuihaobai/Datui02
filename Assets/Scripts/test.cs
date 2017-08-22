using UnityEngine;
using System.Collections;
using System.IO;

public class test : MonoBehaviour 
{
	void Start ()
    {
        //int type = 1;
        //int toType = 1;
        //byte bits = 0;
        //int index = 2;
        //byte[] byteDatas = new byte[4];
        //byteDatas[0] = (byte)(type & 0xFF);
        //byteDatas[1] = (byte)(toType & 0xFF);
        //byteDatas[2] = bits;
        //byteDatas[3] = (byte)(index & 0xFF);

        //int intdata = System.BitConverter.ToInt32(byteDatas, 0);
        //Debug.Log(intdata);

        //byteDatas = System.BitConverter.GetBytes(intdata);

        //type = byteDatas[0];
        //toType = byteDatas[1];
        //bits = byteDatas[2];
        //index = byteDatas[3];

        Random.InitState(123456);

        Debug.Log("=========================================");
        for (int i = 0; i < 10; i ++ )
        {
            Debug.Log(Random.Range(0,100000));
        }
    }
	
	void Update () 
    {
	
	}
}