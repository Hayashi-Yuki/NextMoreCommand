﻿using HarmonyLib;
using SkySwordKill.Next;
using SkySwordKill.Next.DialogEvent;
using SkySwordKill.Next.DialogSystem;
using SkySwordKill.NextMoreCommand.Utils;

namespace SkySwordKill.NextMoreCommand.DialogTrigger
{
    internal static class RoundUtils
    {
        private static bool TryTrigger(params string[] param)=> DialogAnalysis.TryTrigger(param,NewEnv,true);
        private static bool TryTrigger(DialogEnvironment env,params string[] param)=> DialogAnalysis.TryTrigger(param,env);
        private static bool TryTrigger(bool triggerAll,params string[] param)=> DialogAnalysis.TryTrigger(param,NewEnv,triggerAll);
        private static bool TryTrigger(DialogEnvironment env,bool triggerAll,params string[] param)=> DialogAnalysis.TryTrigger(param,env,triggerAll);
        public static DialogEnvironment NewEnv => new DialogEnvironment()
        {
            fightTags = StartFight.FightTags,
            roleID = Tools.instance.MonstarID,
        };
        public static bool StartRound(bool isBefore = true) => isBefore ? StartRoundBefore() : StartRoundAfter();
        public static bool EndRound(bool isBefore = true) => isBefore ? EndRoundBefore() : EndRoundAfter();
        public static bool UseSkill(DialogEnvironment env  = null ,bool isBefore = true) => isBefore ? PlayerUseSkillBefore() : PlayerUseSkillAfter();
        private static bool PlayerUseSkillBefore(DialogEnvironment env  = null) => TryTrigger(env ??NewEnv,true,"PlayerUseSkillBefore", "玩家技能使用前");
        private static bool PlayerUseSkillAfter(DialogEnvironment env  = null) => TryTrigger(env ??NewEnv,true,"PlayerUseSkillAfter", "玩家技能使用后");
        public static bool FinishFight()=> TryTrigger("FinishFight","结束战斗");
        private static bool StartRoundBefore() => TryTrigger("StartRoundBefore", "回合开始前");
        private static bool StartRoundAfter() => TryTrigger("StartRoundAfter", "回合开始后");
        private static bool EndRoundBefore() => TryTrigger("EndRoundBefore", "回合结束前");
        private static bool EndRoundAfter() => TryTrigger("EndtRoundAfter", "回合结束后");
        
    }
    [HarmonyPatch(typeof(RoundManager),nameof(RoundManager.startRound))]
    public static class OnStartRound
    {
        public static void Prefix(RoundManager __instance)
        {
            if (__instance.StaticRoundNum != 0)
            {
                
                if (RoundUtils.StartRound())
                {
                    MyLog.FungusLog("进入开始回合之前");
                }
            }
         
        }
        public static void Postfix(RoundManager __instance)
        {
            
            if (  RoundUtils.StartRound(false))
            {
                MyLog.FungusLog("进入开始回合之后");
            }
        }
    }
    [HarmonyPatch(typeof(RoundManager),nameof(RoundManager.endRound))]
    public static class OnEndRound
    {
        public static void Prefix(RoundManager __instance)
        {
            if (  RoundUtils.EndRound())
            {
                MyLog.FungusLog("进入结束回合之前");
            }
        }
        public static void Postfix(RoundManager __instance)
        {
            
            if (  RoundUtils.EndRound(false))
            {
                MyLog.FungusLog("进入结束回合之后");
            }
        }
    }
    [HarmonyPatch(typeof(RoundManager),"OnDestroy")]
    public static class OnFinishFight
    {
        public static void Postfix()
        {
            if (RoundUtils.FinishFight())
            {
                MyLog.FungusLog("进入结束战斗触发器");
            }
        }
    }
    [HarmonyPatch(typeof(RoundManager),nameof(RoundManager.UseSkill))]
    public static class OnUseSkill
    {
        private static RoundManager instance => RoundManager.instance;
        public static void Prefix()
        {
            var env = RoundUtils.NewEnv;
            env.customData.Add("CurSkill",instance.ChoiceSkill);
            var curr = instance.ChoiceSkill;
            if (RoundUtils.UseSkill(env))
            {
                MyLog.FungusLog("进入玩家技能使用前触发器");
            }
        }
        public static void Postfix()
        {
            var env = RoundUtils.NewEnv;
            env.customData.Add("CurSkill",instance.CurSkill);
            var curr = instance.CurSkill;
            if (RoundUtils.UseSkill(env,false))
            {
                MyLog.FungusLog("进入玩家技能使用后触发器");
            }
        }
    }
}