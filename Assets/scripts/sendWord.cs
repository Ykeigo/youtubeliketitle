using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using WebSocketSharp;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class sendWord : MonoBehaviour
{
    //2ラウンドで終わり
    const int ROUNDNUM = 2;

    // Start is called before the first frame update
    public WebSocket ws = null;
    public int roomID = -1;

    public GameObject daiNkai = null;
    bool daiNkaiSet = false;

    public GameObject input_field_object = null;
    InputField inputfield = null;

    public GameObject certification_panel = null;

    public GameObject notification_panel = null;

    private string makeTitleSceneName = "maketitle";

    bool wsInited = false;
    bool gotitle = false;

    //public battleDatas datas = new battleDatas();
    public matchDatas matchData = null;

    //通信が切断されました画面
    bool roomBroken = false;
    public GameObject broken_panel = null;

    void sendWordOnmessage(object sender, MessageEventArgs e)
    {
        Debug.Log(e.Data);

        if (e.Data == "make title")
        {
            gotitle = true;
        }
        else if (e.Data.Substring(0, 5) == "word:")
        {
            matchData.battles[matchData.round].givenWord = e.Data.Substring(5);
        }
        else if (e.Data == "room broken")
        {
            //通信が切断されました画面出す
            roomBroken = true;
            Debug.Log("切断厨だ");
        }
    }

    void Start()
    {
        inputfield = input_field_object.GetComponent<InputField>();
        if (matchData == null)
        {
            matchData = new matchDatas(ROUNDNUM);
        }

    }
    void Update()
    {
        if (matchData != null && !daiNkaiSet)
        {
            Text text = daiNkai.GetComponent<Text>();
            text.text = "第" + (matchData.round+1) + "回";
        }

        if (ws != null && !wsInited)
        {
            wsInited = true;
            ws.OnMessage += sendWordOnmessage;
            
        }

        if (gotitle)
        {
            SceneManager.sceneLoaded += GameSceneLoaded;
            SceneManager.LoadScene(makeTitleSceneName);
        }
        if (roomBroken)
        {
            GameObject panel = Instantiate(broken_panel, transform.position, Quaternion.identity, this.transform);
            ws.Close();
            roomBroken = false;
        }
    }

    public void confirmation()
    {
        //useMaceb parser = new useMaceb();
        //parser.initDictionaly();

        if (inputfield.text.Length == 0)
        {
            GameObject panel = Instantiate(notification_panel, transform.position, Quaternion.identity, this.transform);

            Text text = panel.transform.Find("Text").gameObject.GetComponent<Text>();
            text.text = "何か入力してください";
        }
        else
        {
            GameObject panel = Instantiate(certification_panel, transform.position, Quaternion.identity, this.transform);
            panel.GetComponent<certify_word>().phase = 0;

            Text text = panel.transform.Find("Text").gameObject.GetComponent<Text>();

            text.text = "相手に送るお題は\n「" + inputfield.text + "」\nでよろしいですか？";
            /*
            if (parser.isOneWord(inputfield.text))
                text.text = "相手に送るお題は\n「" + inputfield.text + "」\nでよろしいですか？";
            else
                text.text = "「" + inputfield.text + "」\nは不明な単語です\n一般的な単語ならこのまま送信してください。";
            //何を送ったか覚えておく
            matchData.battles[matchData.round].mySendWord = inputfield.text;
            */
        }
    }

    public void send()
    {
        ObjectStatus a = new ObjectStatus(roomID, inputfield.text);
        ws.Send(JsonUtility.ToJson(a));
    }

    private void OnDestroy()
    {
        ws.OnMessage -= sendWordOnmessage;
    }

    private void GameSceneLoaded(Scene next, LoadSceneMode mode)
    {
        // シーン切り替え後のスクリプトを取得
        var gameController = GameObject.FindWithTag("GameController").GetComponent<makeTitle>();

        // データを渡す処理
        gameController.ws = ws;
        gameController.roomID = roomID;

        gameController.matchData = new matchDatas(matchData);

        // イベントから削除
        SceneManager.sceneLoaded -= GameSceneLoaded;
    }
}