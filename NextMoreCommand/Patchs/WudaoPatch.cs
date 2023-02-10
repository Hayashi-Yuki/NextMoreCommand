﻿using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using HarmonyLib;
using JSONClass;
using SkySwordKill.NextMoreCommand.Utils;

namespace SkySwordKill.NextMoreCommand.Patchs;

public class CustomWuDaoInfo
{
    public int Id;
    public int Value;
}

[HarmonyPatch(typeof(NPCFactory), nameof(NPCFactory.SetNpcWuDao))]
public static class NpcFactorySetNpcWudaoPatch
{
    private static JSONObject _wudaoJson;
    private static int _level;
    private static int _npcId;
    private static int _wudaoType;
    public static Dictionary<int, WuDaoAllTypeJson> WuDaoAllType => WuDaoAllTypeJson.DataDict;
    private static List<int> _customWuDaoTypeList;
    private static JSONObject _npcDate;

    public static List<int> CustomWuDaoTypeList =>
        _customWuDaoTypeList ??= WuDaoAllType.Keys.Where(index => index > 22).ToList();

    public static JSONObject NpcWuDaoJson => jsonData.instance.NPCWuDaoJson;

    private static int NpcId => _npcDate.HasField("BindingNpcID") && _npcDate["BindingNpcID"].I > 0
        ? _npcDate["BindingNpcID"].I
        : _npcDate["id"].I;

    public static void Postfix(int level, int wudaoType, JSONObject npcDate)
    {
        _level = level;
        _wudaoType = wudaoType;
        _wudaoJson = npcDate.GetField("wuDaoJson");
        _npcDate = npcDate;
        _npcId = NpcId;
        foreach (var wuDao in NPCWuDaoJson.DataDict)
        {
            SetWudao(wuDao);
        }

        Reset();
    }

    private static void Reset()
    {
        _level = 0;
        _wudaoType = 0;
        _wudaoJson = null;
        _customWuDaoTypeList = null;
        _npcDate = null;
        _npcId = 0;
    }

    public static void SetWudao(KeyValuePair<int, NPCWuDaoJson> wudao)
    {
        var npcWuDao = wudao.Value;
        var index = wudao.Key;
        var level = npcWuDao.lv;
        var wudaoType = npcWuDao.Type;
        var json = NpcWuDaoJson[index.ToString()];
        var customWuDaoInfos = new List<CustomWuDaoInfo>();
        foreach (var wudaoId in CustomWuDaoTypeList)
        {
            var key = $"value{wudaoId}";
            if (json.HasField(key))
            {
                var value = json[key].I;
                customWuDaoInfos.Add(new CustomWuDaoInfo() { Id = wudaoId, Value = value });
            }
        }

        if (customWuDaoInfos.Count == 0)
        {
            return;
        }

        if (level == _level && wudaoType == _wudaoType)
        {
            foreach (var customWuDao in customWuDaoInfos)
            {
                var wudaoJson = new WudaoJsonInfo()
                {
                    WudaoId = customWuDao.Id,
                    Level = customWuDao.Value
                };
                var id = customWuDao.Id;
                MyLog.Log("设置NPC自定义悟道", $"角色ID:{_npcId} 角色名字{_npcId.GetNpcName()}");
                MyLog.Log("设置NPC自定义悟道", $"悟道ID:{id} 悟道名字:{WuDaoAllType[id].name1} 悟道境界:{customWuDao.Value}");
                wudaoJson.SetExpByLevel();
                _wudaoJson.SetField(wudaoJson.Id, wudaoJson.ToJsonObject());
            }
        }
    }
}

[HarmonyPatch(typeof(NpcJieSuanManager), nameof(NpcJieSuanManager.UpdateNpcWuDao))]
public static class NpcJieSuanManagerUpdateNpcWuDaoPatch
{
    private static JSONObject _wudaoJson;
    private static int _npcId;
    private static int _level;
    private static int _wudaoType;
    public static Dictionary<int, WuDaoAllTypeJson> WuDaoAllType => WuDaoAllTypeJson.DataDict;
    private static List<int> _customWuDaoTypeList;
    private static JSONObject _npcDate;

    public static List<int> CustomWuDaoTypeList =>
        _customWuDaoTypeList ??= WuDaoAllType.Keys.Where(index => index > 22).ToList();

    public static JSONObject NpcWuDaoJson => jsonData.instance.NPCWuDaoJson;
    //
    // private static int NpcId => _npcDate.HasField("BindingNpcID") && _npcDate["BindingNpcID"].I > 0
    //     ? _npcDate["BindingNpcID"].I
    //     : _npcDate["id"].I;

    public static void Postfix(int npcId)
    {

        _npcId = npcId;
        _npcDate = jsonData.instance.AvatarJsonData[npcId.ToString()];
        _wudaoJson = _npcDate.GetField("wuDaoJson");
        _level = _npcDate["Level"].I;
        _wudaoType = _npcDate["Level"].I;
        foreach (var wuDao in NPCWuDaoJson.DataDict)
        {
            SetWudao(wuDao);
        }

        Reset();
    }

    private static void Reset()
    {
        _level = 0;
        _wudaoType = 0;
        _wudaoJson = null;
        _customWuDaoTypeList = null;
        _npcDate = null;
        _npcId = 0;
    }

    public static void SetWudao(KeyValuePair<int, NPCWuDaoJson> wudao)
    {
        var npcWuDao = wudao.Value;
        var index = wudao.Key;
        var level = npcWuDao.lv;
        var wudaoType = npcWuDao.Type;
        var json = NpcWuDaoJson[index.ToString()];
        var customWuDaoInfos = new List<CustomWuDaoInfo>();
        foreach (var wudaoId in CustomWuDaoTypeList)
        {
            var key = $"value{wudaoId}";
            if (json.HasField(key))
            {
                var value = json[key].I;
                customWuDaoInfos.Add(new CustomWuDaoInfo() { Id = wudaoId, Value = value });
            }
        }

        if (customWuDaoInfos.Count == 0)
        {
            return;
        }

   
        if (level == _level && wudaoType == _wudaoType)
        {
            foreach (var customWuDao in customWuDaoInfos)
            {
                var wudaoJson = new WudaoJsonInfo()
                {
                    WudaoId = customWuDao.Id,
                    Level = customWuDao.Value
                };
                var id = customWuDao.Id;
                MyLog.Log("设置NPC结算自定义悟道", $"角色ID:{_npcId} 角色名字{_npcId.GetNpcName()}");
                MyLog.Log("设置NPC结算自定义悟道", $"悟道ID:{id} 悟道名字:{WuDaoAllType[id].name1} 悟道境界:{customWuDao.Value}");
                wudaoJson.SetExpByLevel();
                _wudaoJson.SetField(wudaoJson.Id, wudaoJson.ToJsonObject());
            }
        }
    }
}