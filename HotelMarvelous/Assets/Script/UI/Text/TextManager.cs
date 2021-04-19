using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextManager : MonoBehaviour
{
    private List<Dictionary<string, object>> talkDictionary;
    private List<TextMemory> textmemory = new List<TextMemory>();

    private Dictionary<byte, string[]> talkData;

    private TextMemory memory = new TextMemory();

    void Awake()
    {
        talkDictionary = CSVReader.Read("Text/testtext");
        talkData = new Dictionary<byte, string[]>();
        LoadTextData();
    }

    void LoadTextData()
    {
        for (int i = 0; i < talkDictionary.Count; i++)
        {
            Dictionary<string, object> dictext = talkDictionary[i];
            
            string _talklocation = dictext["Talk Point"].ToString();
            string _id = dictext["Id"].ToString();
            string[] _textarr = dictext["Text"].ToString().Split('&');
            byte _point = byte.Parse(_talklocation);

            if (memory.GetLocationNum() == _point)
            {
                talkData.Add(byte.Parse(_id), _textarr);
                memory.SetMemory(byte.Parse(_talklocation), talkData);

                talkData = new Dictionary<byte, string[]>();
            }
            else
            {
                textmemory.Add(memory);

                memory = new TextMemory();
                talkData = new Dictionary<byte, string[]>();

                talkData.Add(byte.Parse(_id), _textarr);
                memory.SetMemory(byte.Parse(_talklocation), talkData);
            }        
        }

        //talkData.Add(1, new string[] { "호텔 마블러스에 오신것을 환영합니다.", "프로그래머인 김선민입니다.^^" });
    }

    public string GetTalk(byte _location, byte _id, byte _talkIndex)
    {
        memory = textmemory[_location];

        if(_talkIndex == memory.GetTextData(_id)[_id].Length)
        {
            return null;
        }
        else
        {
            return memory.GetTextData(_id)[_id][_talkIndex];
            //return talkData[_id][_talkIndex];
        }
    }
}

class TextMemory
{
    public byte locationnum;
    private List<Dictionary<byte, string[]>> _talklist = new List<Dictionary<byte, string[]>>();

    public void SetMemory(byte _name, Dictionary<byte, string[]> _talkdata)
    {
        locationnum = _name;
        _talklist.Add(_talkdata);
    }

    public Dictionary<byte, string[]> GetTextData(byte _id)
    {
        //Dictionary<byte, string[]> dic = _talklist[_id];
        return _talklist[_id - 1];
    }

    public byte GetLocationNum()
    {
        return locationnum;
    }
}
