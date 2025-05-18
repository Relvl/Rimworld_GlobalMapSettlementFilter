using System.Diagnostics.CodeAnalysis;
using HarmonyLib;
using RimWorld.Planet;
using UnityEngine;
using Verse;

namespace GlobalMapSettlementFilter.Harmony;

[SuppressMessage("ReSharper", "InconsistentNaming")]
[SuppressMessage("ReSharper", "UnusedType.Global")]
[SuppressMessage("ReSharper", "UnusedMember.Global")]
[HarmonyPatch(typeof(WorldGlobalControls))]
public static class Patch_WorldGlobalControls
{
    /// <summary>
    ///     Just hooks world map OnGUI
    /// </summary>
    [HarmonyPrefix]
    [HarmonyPatch(nameof(WorldGlobalControls.WorldGlobalControlsOnGUI))]
    public static void WorldGlobalControlsOnGUI()
    {
        if (Event.current.type == EventType.Layout)
            return;
        if (Current.ProgramState != ProgramState.Playing)
            return;
        GlobalMapSettlementFilter.OnWorldGlobalControls();
    }
}