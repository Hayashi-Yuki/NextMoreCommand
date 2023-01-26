using SkySwordKill;
using SkySwordKill.Next;
using System;
using Fungus;
using SkySwordKill.Next.DialogEvent;
using SkySwordKill.Next.DialogSystem;
using SkySwordKill.NextMoreCommand.Attribute;
using SkySwordKill.NextMoreCommand.Utils;

namespace SkySwordKill.NextMoreCommand.Command
{
    [RegisterCommand]
    [DialogEvent("NpcForceJiaoHu")]
    public class NpcForceJiaoHu : IDialogEvent
    {
      

        public void Execute(DialogCommand command, DialogEnvironment env, Action callback)
        {
            var npc = NPCEx.NPCIDToNew(command.GetInt(0, -1));
            if (npc >= 0)
            {
                var npcData = new UINPCData(npc);
                npcData.RefreshData();
            
                UINPCJiaoHu.Inst.HideJiaoHuPop();
                UINPCJiaoHu.Inst.NowJiaoHuNPC = npcData;
                UINPCJiaoHu.Inst.ShowJiaoHuPop();
            }
         
            callback?.Invoke();
        }

    }
}