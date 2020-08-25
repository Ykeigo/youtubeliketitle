using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using WebSocketSharp;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class showViewCount : MonoBehaviour
{
    public WebSocket ws = null;
    public int roomID = -1;

    public matchDatas matchData;

    public GameObject daiNkai = null;
    bool daiNkaiSet = false;
    public GameObject tobecontinue = null;

    public GameObject myTitleText = null;
    public GameObject rivalTitleText = null;

    public GameObject myViewCountText = null;
    public GameObject rivalViewCountText = null;

    int showPhase = 0;

    float phaseTime = 0;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (matchData != null && !daiNkaiSet)
        {
            Text text = daiNkai.GetComponent<Text>();
            text.text = "第"+(matchData.round+1)+"回";

            if (matchData.round < matchData.maxRound-1)
            {
                text = tobecontinue.GetComponent<Text>();
                text.text = "第" + (matchData.round + 2) + "回に続く…";
            }
            else
            {
                text = tobecontinue.GetComponent<Text>();
                text.text = "集計しています…";
            }
        }

        if (showPhase < 5)
        {
            phaseTime += Time.deltaTime;
        }

        if (showPhase == 0)
        {
            Text text = myTitleText.GetComponent<Text>();
            text.text = matchData.battles[matchData.round].myTitle;
            if(phaseTime > 0.5)
            {
                showPhase = 1;
                phaseTime = 0;
            }
        }
        else if (showPhase == 1)
        {
            Text text = myViewCountText.GetComponent<Text>();
            text.text = matchData.battles[matchData.round].myViewCount.ToString()+"回再生";
            if (phaseTime > 0.5)
            {
                showPhase = 2;
                phaseTime = 0;
            }
        }
        else if (showPhase == 2)
        {
            Text text = rivalTitleText.GetComponent<Text>();
            text.text = matchData.battles[matchData.round].rivalTitle;
            if (phaseTime > 0.5)
            {
                showPhase = 3;
                phaseTime = 0;
            }
        }
        else if (showPhase == 3)
        {
            Text text = rivalViewCountText.GetComponent<Text>();
            text.text = matchData.battles[matchData.round].rivalViewCount.ToString() + "回再生";
            if (phaseTime > 0.5)
            {
                showPhase = 4;
                phaseTime = 0;
            }
        }
        else if (showPhase == 4)
        {
            //次のお題単語交換もしくはゲーム終了
            if (phaseTime > 3)
            {
                Debug.Log("round"+matchData.round+"/"+matchData.maxRound+" finish");
                matchData.round += 1;
                if(matchData.round >= matchData.maxRound)
                {
                    //ゲーム終了画面
                    SceneManager.sceneLoaded += showResultdSceneLoaded;
                    SceneManager.LoadScene("showResult");
                }
                else
                {
                    //お題考え画面
                    SceneManager.sceneLoaded += sendWordSceneLoaded;
                    SceneManager.LoadScene("giveword");
                }

                //意味ない？
                showPhase = 5;
            }
        }

    }

    private void sendWordSceneLoaded(Scene next, LoadSceneMode mode)
    {
        // シーン切り替え後のスクリプトを取得
        var gameController = GameObject.FindWithTag("GameController").GetComponent<sendWord>();

        // データを渡す処理
        gameController.ws = ws;
        gameController.roomID = roomID;

        gameController.matchData = new matchDatas(matchData);

        // イベントから削除
        SceneManager.sceneLoaded -= sendWordSceneLoaded;
    }

    private void showResultdSceneLoaded(Scene next, LoadSceneMode mode)
    {
        // シーン切り替え後のスクリプトを取得
        var gameController = GameObject.FindWithTag("GameController").GetComponent<showResult>();

        // データを渡す処理
        //gameController.ws = ws;
        //gameController.roomID = roomID;

        ws.Close();

        gameController.matchData = new matchDatas(matchData);

        // イベントから削除
        SceneManager.sceneLoaded -= showResultdSceneLoaded;
    }

}
