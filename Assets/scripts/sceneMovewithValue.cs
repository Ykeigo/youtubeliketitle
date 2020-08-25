using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.SceneManagement;

public class sceneMovewithValue : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            SceneManager.sceneLoaded += GameSceneLoaded;
            Debug.Log("hello");
            // シーン切り替え
            SceneManager.LoadScene("testscene2");
        }
    }
    

    private void GameSceneLoaded(Scene next, LoadSceneMode mode)
    {
        // シーン切り替え後のスクリプトを取得
        var gameManager = GameObject.FindWithTag("GameController").GetComponent<haveValue>();

        // データを渡す処理
        gameManager.score = 100;

        // イベントから削除
        SceneManager.sceneLoaded -= GameSceneLoaded;
    }
}
