using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using WebSocketSharp;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public class makeTitle : MonoBehaviour
{
    public WebSocket ws = null;
    public int roomID = -1;

    public WebSocket python_ws = null;

    public GameObject daiNkai = null;
    bool daiNkaiSet = false;

    public GameObject input_field_object = null;
    InputField inputfield = null;

    public GameObject certification_panel = null;
    public GameObject notification_panel = null;

    public string odaiword;
    public GameObject odaiWordText = null;

    bool wsInited = false;
    public bool goViewCount = false;

    public matchDatas matchData;

    //通信が切断されました画面
    bool roomBroken = false;
    public GameObject broken_panel = null;

    void makeTitleOnmessage(object sender, MessageEventArgs e)
    {
        Debug.Log(e.Data);

        if (e.Data == "room broken")
        {
            //通信が切断されました画面出す
            roomBroken = true;
            Debug.Log("切断厨だ");
        }
        else if (e.Data == "show result")
        {
            goViewCount = true;
            //SceneManager.LoadScene(makeTitleSceneName);
        }
        else if (e.Data.Substring(0, 11) == "rivalTitle:")
        {
            matchData.battles[matchData.round].rivalTitle = e.Data.Substring(11);

            string titleText = e.Data.Substring(11);

            string charcode = "1,";
            for (int i = 0; i < titleText.Length; i++)
                charcode += (int)(titleText[i]) + ",";
            python_ws.Send(charcode);
        }
        /*
        else if (e.Data.Substring(0, 15) == "rivalViewCount:")
        {
            if (int.TryParse(e.Data.Substring(15), out matchData.battles[matchData.round].rivalViewCount))
            {
                Debug.Log("myViewCount = " + matchData.battles[matchData.round].rivalViewCount);
            }
        }
        
        else if (e.Data.Substring(0, 14) == "yourViewCount:")
        {
            if (int.TryParse(e.Data.Substring(14), out matchData.battles[matchData.round].myViewCount))
            {
                Debug.Log("myViewCount = " + matchData.battles[matchData.round].myViewCount);
            }
        }*/
    }

    private void getViewCount(object sender, MessageEventArgs e)
    {
        Debug.Log(e.Data);
        if (e.Data.Substring(0, 5) == "your:")
        {
            float vc = 0;
            if (float.TryParse(e.Data.Substring(5), out vc))
            {
                matchData.battles[matchData.round].myViewCount = (int)vc;
                Debug.Log("myViewCount = " + matchData.battles[matchData.round].myViewCount);
            }
        }
        else if (e.Data.Substring(0, 6) == "rival:")
        {
            float vc = 0;
            if (float.TryParse(e.Data.Substring(6), out vc))
            {
                matchData.battles[matchData.round].rivalViewCount = (int)vc;
                Debug.Log("rivalViewCount = " + matchData.battles[matchData.round].rivalViewCount);
            }
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        // pythonサーバーに送り付ける応答は確認画面の送信ボタンを押すまでには届いているはず
        string url = "ws://18.222.249.68:7001";
        Debug.Log(url);
        python_ws = new WebSocket(url);
        python_ws.Connect();
        python_ws.OnMessage += getViewCount;
    }

    // Update is called once per frame
    void Update()
    {
        if (matchData != null && !daiNkaiSet)
        {
            Text text = daiNkai.GetComponent<Text>();
            text.text = "第" + (matchData.round+1) + "回";

            text = odaiWordText.GetComponent<Text>();
            odaiword = matchData.battles[matchData.round].givenWord;
            text.text = "お題：" + odaiword;
        }
        if (ws != null && !wsInited)
        {
            wsInited = true;
            ws.OnMessage += makeTitleOnmessage;
        }
        //再生回数が帰ってくるまで待つ
        if (goViewCount && matchData.battles[matchData.round].rivalViewCount > 0 && matchData.battles[matchData.round].myViewCount > 0)
        {
            Debug.Log("go viewcount scene");
            SceneManager.sceneLoaded += GameSceneLoaded;
            SceneManager.LoadScene("showViewCount");
        }

        if (roomBroken)
        {
            GameObject panel = Instantiate(broken_panel, transform.position, Quaternion.identity, this.transform);
            ws.Close();
            roomBroken = false;
        }
    }

    private void OnDestroy()
    {
        ws.OnMessage -= makeTitleOnmessage;
        python_ws.Close();
    }

    public void confirmation()
    {
        inputfield = input_field_object.GetComponent<InputField>();
        Debug.Log(odaiword);
        Debug.Log(inputfield.text);
        Debug.Log(inputfield.text.IndexOf(odaiword));
        if(inputfield.text.IndexOf(odaiword) >= 0)
        {
            //pythonサーバーに送り付ける　応答は確認画面の送信ボタンを押すまでには届いているはず
            string titleText = inputfield.text;
            string charcode = "0,";
            for (int i = 0; i < titleText.Length; i++)
                charcode += (int)(titleText[i]) + ",";
            python_ws.Send(charcode);

            GameObject panel = Instantiate(certification_panel, transform.position, Quaternion.identity, this.transform);
            panel.GetComponent<certify_word>().phase = 1;
            Text text = panel.transform.Find("Text").gameObject.GetComponent<Text>();
            text.text = "投稿する動画のタイトルは\n「" + titleText + "」\nでよろしいですか？";
        }
        else
        {
            GameObject panel = Instantiate(notification_panel, transform.position, Quaternion.identity, this.transform);
            Text text = panel.transform.Find("Text").gameObject.GetComponent<Text>();
            text.text = "「" + odaiword + "」が含まれていません";
        }

        matchData.battles[matchData.round].myTitle = inputfield.text;
    }

    public void send()
    {
        titleViewCount a = new titleViewCount(roomID, inputfield.text, matchData.battles[matchData.round].myViewCount);
        ws.Send(JsonUtility.ToJson(a));
    }

    private void GameSceneLoaded(Scene next, LoadSceneMode mode)
    {
        // シーン切り替え後のスクリプトを取得
        var gameController = GameObject.FindWithTag("GameController").GetComponent<showViewCount>();

        // データを渡す処理
        gameController.ws = ws;
        gameController.roomID = roomID;

        gameController.matchData = new matchDatas(matchData);

        // イベントから削除
        SceneManager.sceneLoaded -= GameSceneLoaded;
    }

    
}
