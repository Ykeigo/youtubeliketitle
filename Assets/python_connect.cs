using System.Collections;
using System.Collections.Generic;
using UnityEngine;


using WebSocketSharp;

public class python_connect : MonoBehaviour
{
    public WebSocket python_ws = null;

    private void calcViewCount(object sender, MessageEventArgs e)
    {
        Debug.Log(e.Data);
        float vc = 0;
        if (float.TryParse(e.Data, out vc))
        {
            vc = (int)vc;
            Debug.Log("myViewCount = " + vc);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        //pythonサーバーに送り付ける　応答は確認画面の送信ボタンを押すまでには届いているはず
        string url = "ws://18.222.249.68:7001";
        Debug.Log(url);
        python_ws = new WebSocket(url);
        python_ws.Connect();
        python_ws.OnMessage += calcViewCount;
        //python_ws.Send("hello");
        int a = (int)('【');
        Debug.Log(a);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void send()
    {
        string text = "【悲報】骨が折れました。";

        string charcode = "0,";
        for(int i = 0; i < text.Length; i++){
            charcode += (int)(text[i])+",";
        }

        //ObjectStatus a = new ObjectStatus(1, charcode);
        //python_ws.Send(JsonUtility.ToJson(a));
        //python_ws.Send(System.Text.Encoding.UTF8.GetBytes("【悲報】骨が折れました。"));
        python_ws.Send(charcode);
    }

    private void OnDestroy()
    {
        python_ws.Close();
    }
}
