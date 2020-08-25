using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class showResult : MonoBehaviour
{
    public matchDatas matchData;

    public GameObject mytitle1 = null;
    public GameObject mytitle2 = null;
    public GameObject myViewCount1 = null;
    public GameObject myViewCount2 = null;
    public GameObject myViewCount = null;

    public GameObject rivaltitle1 = null;
    public GameObject rivaltitle2 = null;
    public GameObject rivalViewCount1 = null;
    public GameObject rivalViewCount2 = null;
    public GameObject rivalViewCount = null;

    public GameObject winner = null;
    public GameObject endButton = null;

    int phase = 0;
    float phaseTimer = 0;

    int mySum = 0;
    int rivalSum = 0;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (phase <= 3) phaseTimer += Time.deltaTime;
        if (phaseTimer >= 0.8)
        {
            if (phase == 0)
            {
                //自分の動画
                Text text = mytitle1.GetComponent<Text>();
                text.text = matchData.battles[0].myTitle;
                text = mytitle2.GetComponent<Text>();
                text.text = matchData.battles[1].myTitle;
                text = myViewCount1.GetComponent<Text>();
                text.text = matchData.battles[0].myViewCount.ToString() + "回再生";
                text = myViewCount2.GetComponent<Text>();
                text.text = matchData.battles[1].myViewCount.ToString() + "回再生";

                int sum = matchData.battles[0].myViewCount + matchData.battles[1].myViewCount;
                text = myViewCount.GetComponent<Text>();
                text.text = "合計 " + sum + "回再生";

                phase += 1;
                phaseTimer = 0;
            }

            else if (phase == 1)
            {
                //相手の動画
                Text text = rivaltitle1.GetComponent<Text>();
                text.text = matchData.battles[0].rivalTitle;
                text = rivaltitle2.GetComponent<Text>();
                text.text = matchData.battles[1].rivalTitle;
                text = rivalViewCount1.GetComponent<Text>();
                text.text = matchData.battles[0].rivalViewCount.ToString() + "回再生";
                text = rivalViewCount2.GetComponent<Text>();
                text.text = matchData.battles[1].rivalViewCount.ToString() + "回再生";

                int sum = matchData.battles[0].rivalViewCount + matchData.battles[1].rivalViewCount;
                text = rivalViewCount.GetComponent<Text>();
                text.text = "合計 " + sum + "回再生";

                phase += 1;
                phaseTimer = 0;
            }
            else if (phase == 2)
            {
                int win = 1;
                if (mySum < rivalSum) win = 2;

                Text text = winner.GetComponent<Text>();
                text.text = "プレイヤー" + win;

                phase += 1;
                phaseTimer = 0;
            }
        }
        if (phaseTimer > 2)
        {
            if (phase == 3)
            {
                endButton.SetActive(true);
                phase += 1;
            }
        }
    }

    public void returnToTitle()
    {
        SceneManager.LoadScene("title");
    }
}
