﻿using System;
using HarmonyLib;
using SkySwordKill.Next;
using SkySwordKill.Next.FCanvas;
using SkySwordKill.Next.Mod;
using SkySwordKill.NextMoreCommand.Utils;
using UnityEngine;
using YSGame.TuJian;

namespace SkySwordKill.NextMoreCommand;

public class NextMoreCommand : MonoBehaviour
{
    public static NextMoreCommand instance;
    public bool? AchivementDebug;
    private void Awake()
    {

       
        instance = this;
        ModManager.TryGetModSetting("Quick_AchivementDebug", out AchivementDebug);
        ModManager.ModSettingChanged += () =>
        {
            ModManager.TryGetModSetting("Quick_AchivementDebug", out AchivementDebug);
        };
        ModManager.ModLoadComplete += () =>
        {
            ModManager.TryGetModSetting("Quick_AchivementDebug", out AchivementDebug);
        };
    
    }

}