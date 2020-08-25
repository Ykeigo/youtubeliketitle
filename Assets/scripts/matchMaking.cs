using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using WebSocketSharp;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class matchMaking : MonoBehaviour
{
    // Start is called before the first frame update
    private int count = 0; // click counter
    private WebSocket ws;

    private int roomID = -1;

    public string battleSceneName = "giveword";
    public GameObject score_object = null; // Textオブジェクト

    public float battlewait = 0;
    public bool matched = false;

    //通信が切断されました画面
    bool roomBroken = false;
    public GameObject broken_panel = null; // Textオブジェクト

    void matchmakeOnmessage(object sender, MessageEventArgs e)
    {
        Debug.Log(e.Data);
        if (e.Data == "you matched")
        {
            Debug.Log("まっちした");
            // Text score_text = score_object.GetComponent<Text>();
            // テキストの表示を入れ替える
            matched = true;
            battlewait = 0;
            //なんでunity上でだと↓のlogが出てこん？タイムアウト？
        }

        if (e.Data.Substring(0, 7) == "roomID:")
        {
            if (int.TryParse(e.Data.Substring(7), out roomID))
            {
                Debug.Log("roomID = " + roomID);
            }
            else
            {
                Debug.Log("unknown roomID received");
            }
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
        string url = "ws://18.222.249.68:7000";
        Debug.Log(url);
        ws = new WebSocket(url);
        ws.Connect();

        ws.OnMessage += matchmakeOnmessage;
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {

            Debug.Log("clicked!");
            count++;
            //ws.Send("clicked No. " + count.ToString());

            ObjectStatus a = new ObjectStatus(1, "hello");
            ws.Send(JsonUtility.ToJson(a));
        }
        // Debug.Log(matched);
        if (matched && !roomBroken)
        {
            Text score_text = score_object.GetComponent<Text>();
            score_text.text = "対戦相手が見つかりました！";

            battlewait += Time.deltaTime;
            //Debug.Log(battlewait);
            if(battlewait >= 2)
            {
                Debug.Log("go next scene");

                SceneManager.sceneLoaded += GameSceneLoaded;
                SceneManager.LoadScene(battleSceneName);
            }
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
        ws.OnMessage -= matchmakeOnmessage;
    }

    private void GameSceneLoaded(Scene next, LoadSceneMode mode)
    {
        // シーン切り替え後のスクリプトを取得
        var gameController = GameObject.FindWithTag("GameController").GetComponent<sendWord>();

        // データを渡す処理
        gameController.ws = ws;
        gameController.roomID = roomID;

        // イベントから削除
        SceneManager.sceneLoaded -= GameSceneLoaded;
    }
}