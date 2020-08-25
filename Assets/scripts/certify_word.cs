using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class certify_word : MonoBehaviour
{
    public GameObject sender = null;
    public GameObject waitplayer_panel = null;

    public int phase = -1;

    // Start is called before the first frame update
    void Start()
    {
        sender = transform.root.gameObject;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void send()
    {
        //sendwordのときはphaseを0にされてる
        if (phase == 0)
        {
            sender.GetComponent<sendWord>().send();
        }
        //sendwordのときはphaseを1にされてる
        else if (phase == 1)
        {
            sender.GetComponent<makeTitle>().send();
        }
        GameObject panel = Instantiate(waitplayer_panel, transform.position, Quaternion.identity, this.transform.root);
    }

    public void delete()
    {
        Destroy(this.gameObject);
    } 
}
