﻿using System.Collections.Generic;
using KBEngine;
using UnityEngine;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SkySwordKill.Next;

namespace SkySwordKill.NextMoreCommand.CustomMap
{
    public static class Extends
    {
        public static CustomMapManager GetCustomMapManager(this Avatar avatar)
        {
            return new CustomMapManager(avatar);
        }
    }

    public class CustomMapManager
    {
        protected Avatar _avatar;

        public CustomMapManager(Avatar avatar)
        {
            _avatar = avatar;
        }

        public static CustomMapManager GetPlayer()
        {
            return Tools.instance.getPlayer().GetCustomMapManager();
        }

        public static void Clear()
        {
            CustomMapDatas.Clear();
            CustomMapType.Clear();
            ChineseCustomMapType.Clear();
        }

        public static readonly Dictionary<int, CustomMapData> CustomMapDatas = new Dictionary<int, CustomMapData>();

        public static readonly Dictionary<string, ModCustomMapType> CustomMapType =
            new Dictionary<string, ModCustomMapType>();

        public static readonly Dictionary<string, string> ChineseCustomMapType = new Dictionary<string, string>();

        public static readonly Dictionary<string, ModCustomMapType>
            MapType = new Dictionary<string, ModCustomMapType>();

        public static readonly Dictionary<string, string> ChineseMapType = new Dictionary<string, string>();

        public static bool TryGetCustomMapType(string key, out ModCustomMapType mapType)
        {
            string cname;
            if (ChineseCustomMapType.TryGetValue(key, out cname) && CustomMapType.TryGetValue(cname, out mapType))
            {
                return true;
            }

            if (CustomMapType.TryGetValue(key, out mapType))
            {
                return true;
            }

            return false;
        }

        public static bool TryGetMapType(string key, out ModCustomMapType mapType)
        {
            string cname;
            if (ChineseMapType.TryGetValue(key, out cname) && MapType.TryGetValue(cname, out mapType))
            {
                return true;
            }

            if (MapType.TryGetValue(key, out mapType))
            {
                return true;
            }

            return false;
        }

        public void TestMap(int index)
        {
            var customMap = new CustomMap()
            {
                StartTime = _avatar.worldTimeMag.nowTime,
                Name = "测试",
                Uuid = Tools.getUUID(),
                High = 5,
                Wide = 5,
                NanDu = 1,
                ShuXing = 1,
                Type = 1,
                ShouldReset = false,
                Award = new CustomMapAward[] { },
                Event = new CustomMapEvent[] { },
                Map = new MapNodeType[,]
                {
                    { 0, 0, MapNodeType.Road, 0, 0 },
                    //         { 1, 6,11,16,21 },
                    { 0, 0, MapNodeType.Road, 0, 0 },
                    //         { 2, 7,12,17,22 },
                    { 0, 0, MapNodeType.Road, 0, 0 },
                    //         { 3, 8,13,18,23 },
                    {
                        MapNodeType.Exit, MapNodeType.Entrance, MapNodeType.Road,
                        MapNodeType.Road, MapNodeType.Road
                    },
                    //         { 4, 9,14,19,24 },
                    { 0, 0, MapNodeType.Road, 0, 0 },
                    //         { 5,10,15,20,25 },
                }
            };

            var msg = JsonConvert.SerializeObject(customMap, Formatting.Indented);
            Main.LogInfo(msg);
            _avatar.RandomFuBenList["111"] = customMap.ToJObject();
            _avatar.fubenContorl[customMap.Uuid].setFirstIndex(index);
            LoadMap(111);
        }

        public void LoadMapEntranceIndex(int index, int entranceIndex)
        {
            if (CustomMapDatas.TryGetValue(index, out CustomMapData customMapData))
            {
                var customMap = customMapData.ToCustomMap();
                customMap.StartTime = _avatar.worldTimeMag.nowTime;
                _avatar.RandomFuBenList[index.ToString()] = customMap.ToJObject();
                var entrance = customMap.GetEntrance(entranceIndex);

                _avatar.fubenContorl[customMap.Uuid].setFirstIndex(entrance.Index);
                LoadMap(index);
            }
        }
        public void LoadMap(int index, int position)
        {
            if (CustomMapDatas.TryGetValue(index, out CustomMapData customMapData))
            {
                var customMap = customMapData.ToCustomMap();
                customMap.StartTime = _avatar.worldTimeMag.nowTime;
                _avatar.RandomFuBenList[index.ToString()] = customMap.ToJObject();
                _avatar.fubenContorl[customMap.Uuid].setFirstIndex(position);
                LoadMap(index);
            }
        }

        public void CustomTestMap(int index, int position)
        {
            if (CustomMapDatas.TryGetValue(index, out CustomMapData customMapData))
            {
                var customMap = customMapData.ToCustomMap();
                customMap.StartTime = _avatar.worldTimeMag.nowTime;
                _avatar.RandomFuBenList[index.ToString()] = customMap.ToJObject();
                _avatar.fubenContorl[customMap.Uuid].setFirstIndex(position);
                LoadMap(index);
            }
        }

        public void LoadMap(int mapId)
        {
            if (_avatar.NowFuBen != "FRandomBase")
            {
                _avatar.zulinContorl.kezhanLastScence = Tools.getScreenName();
                _avatar.lastFuBenScence = Tools.getScreenName();
                _avatar.NowFuBen = "FRandomBase";
            }


            _avatar.NowRandomFuBenID = mapId;
            YSNewSaveSystem.SaveGame(PlayerPrefs.GetInt("NowPlayerFileAvatar"), 0, null, false);
            Tools.instance.loadMapScenes("FRandomBase", true);
        }
    }
}