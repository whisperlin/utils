using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Json解析为该对象
public class Response<T>
{
    public List<T> list;
}

[System.Serializable]
public class Student
{
    public int id;
    public string name;
}
public class ForText : MonoBehaviour {



    // 解析Json的方法
    public void ParseItemJson(string jsonStr)
    {
        // 将Json中的数组用一个list包裹起来，变成一个Wrapper对象
        jsonStr = "{ \"list\": " + jsonStr + "}";
        Response<Student> studentList = JsonUtility.FromJson<Response<Student>>(jsonStr);
        foreach (Student item in studentList.list)
        {
            Debug.Log(item.id+" "+item.name);
        }
    }

    
    public TextAsset text;
	void Start () {
        ParseItemJson(text.text);
        //Debug.Log(text.text);
		
	}
	
	
	void Update () {
		
	}
}
