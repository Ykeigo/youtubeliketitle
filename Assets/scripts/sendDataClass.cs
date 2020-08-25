using System;
using System.Collections.Generic;

[Serializable]
public class ObjectStatus
{
    public int roomID;
    public string content;

    public ObjectStatus(int id, string con)
    {
        roomID = id;
        content = con;
    }

}

public class IDByte
{
    public int roomID;
    public byte[] content;

    public IDByte(int id, byte[] con)
    {
        roomID = id;
        content = con;
    }

}

public class titleViewCount
{
    public int roomID;
    public string content;
    public int viewCount;


    public titleViewCount(int id, string con, int vc)
    {
        roomID = id;
        content = con;
        viewCount = vc;
    }

}

public class battleDatas
{
    public string givenWord = "";
    public string mySendWord = "";
    public string myTitle = "";
    public int myViewCount = 0;
    public string rivalTitle = "";
    public int rivalViewCount = 0;

    public battleDatas(){
        givenWord = "";
        mySendWord = "";
        myTitle = "";
        myViewCount = 0;
        rivalTitle = "";
        rivalViewCount = 0;
    }

    public battleDatas(battleDatas source)
    {
        this.givenWord = source.givenWord;
        this.mySendWord = source.mySendWord;
        this.myTitle = source.myTitle;
        this.myViewCount = source.myViewCount;
        this.rivalTitle = source.rivalTitle;
        this.rivalViewCount = source.rivalViewCount;
    }
}

public class matchDatas
{
    public List<battleDatas> battles = new List<battleDatas>();
    public int round = 0;
    public int maxRound = 0;

    public matchDatas(int roundNum)
    {
        maxRound = roundNum;
        round = 0;
        battles = new List<battleDatas>();
        for(int i = 0; i < roundNum; i++)
        {
            battles.Add(new battleDatas());
        }
    }

    public matchDatas(matchDatas a)
    {
        maxRound = a.maxRound;
        round = a.round;
        battles = new List<battleDatas>(a.battles);
    }
}