using System;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using RimWorld;
using RimWorld.Planet;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace GlobalMapSettlementFilter;

[SuppressMessage("ReSharper", "ClassNeverInstantiated.Global")]
public class GlobalMapSettlementFilter : Mod
{
    private const string ModID = "johnson1893.global.map.settlement.filter";

    private static bool showAlly = true;
    private static bool showNeutral = true;
    private static bool showHostile = true;
    private static bool hideEverVisited;
    private static bool hideVisitedRecently;

    private static string labelTitle;
    private static string labelShowAlly;
    private static string labelShowNeutral;
    private static string labelShowHostile;
    private static string labelHideEverVisited;
    private static string labelHideVisitedRecently;
    private static float widgetWidth = 300f;

    public GlobalMapSettlementFilter(ModContentPack content) : base(content)
    {
        // Init the Harmony
        var harmonyInstance = new HarmonyLib.Harmony(ModID);
        harmonyInstance.PatchAll(Assembly.GetExecutingAssembly());
    }

    public static void OnWorldGlobalControls()
    {
        Text.Font = GameFont.Tiny;
        Text.Anchor = TextAnchor.UpperLeft;
        Text.WordWrap = false;
        GUI.color = Color.white;

        GenerateLabels();

        var checkboxesCount = 5;
        var checkboxTotalHeight = checkboxesCount * 24f + 18f + Text.LineHeight;

        var widgetRect = new Rect(10f, UI.screenHeight / 2f - checkboxTotalHeight / 2f, widgetWidth, checkboxTotalHeight);

        GUI.DrawTexture(widgetRect, TexUI.HighlightTex);
        Widgets.DrawBox(widgetRect);
        Widgets.Label(widgetRect, labelTitle);

        widgetRect = widgetRect.ContractedBy(4f);
        var rect = new Rect(widgetRect.x, widgetRect.y + Text.LineHeight, widgetRect.width, 20f);
        DrawCheckbox(ref rect, labelShowAlly, ref showAlly);
        DrawCheckbox(ref rect, labelShowNeutral, ref showNeutral);
        DrawCheckbox(ref rect, labelShowHostile, ref showHostile);
        rect.y += 10;
        DrawCheckbox(ref rect, labelHideEverVisited, ref hideEverVisited);
        DrawCheckbox(ref rect, labelHideVisitedRecently, ref hideVisitedRecently);

        Text.Font = GameFont.Small;
        Text.Anchor = TextAnchor.UpperLeft;
        Text.WordWrap = true;
    }

    public static void GenerateLabels()
    {
        var shouldRecalcWidth = labelShowAlly == default || labelShowNeutral == default || labelShowHostile == default || labelHideEverVisited == default || labelHideVisitedRecently == default;
        labelTitle ??= "GlobalMapSettlementFilter.Widget.Label".Translate();
        labelShowAlly ??= "GlobalMapSettlementFilter.Widget.ShowAlly".Translate();
        labelShowNeutral ??= "GlobalMapSettlementFilter.Widget.ShowNeutral".Translate();
        labelShowHostile ??= "GlobalMapSettlementFilter.Widget.ShowHostile".Translate();
        labelHideEverVisited ??= "GlobalMapSettlementFilter.Widget.HideEverVisited".Translate();
        labelHideVisitedRecently ??= "GlobalMapSettlementFilter.Widget.HideVisitedRecently".Translate();
        if (shouldRecalcWidth)
        {
            widgetWidth = 0f;
            widgetWidth = Math.Max(widgetWidth, Text.CalcSize(labelShowAlly).x);
            widgetWidth = Math.Max(widgetWidth, Text.CalcSize(labelShowNeutral).x);
            widgetWidth = Math.Max(widgetWidth, Text.CalcSize(labelShowHostile).x);
            widgetWidth = Math.Max(widgetWidth, Text.CalcSize(labelHideEverVisited).x);
            widgetWidth = Math.Max(widgetWidth, Text.CalcSize(labelHideVisitedRecently).x);
            widgetWidth += 36f;
        }
    }

    public static void DrawCheckbox(ref Rect inRect, string label, ref bool value)
    {
        if (Widgets.ButtonInvisible(inRect))
        {
            if (value)
                SoundDefOf.Checkbox_TurnedOff.PlayOneShotOnCamera();
            else
                SoundDefOf.Checkbox_TurnedOn.PlayOneShotOnCamera();
            value = !value;
        }

        Widgets.CheckboxDraw(inRect.x, inRect.y, value, false, 20f);
        Widgets.Label(inRect with { xMin = inRect.xMin + 24f }, label);
        inRect.y += 24f;
    }

    public static bool ShouldDrawSettlement(Settlement settlement)
    {
        if (settlement.Faction == Faction.OfPlayer) return true;

        var relationKind = settlement.Faction.RelationKindWith(Faction.OfPlayer);
        if (!showHostile && relationKind == FactionRelationKind.Hostile) return false;
        if (!showNeutral && relationKind == FactionRelationKind.Neutral) return false;
        if (!showAlly && relationKind == FactionRelationKind.Ally) return false;

        if (hideEverVisited && settlement.trader.EverVisited) return false;
        if (hideVisitedRecently && settlement.trader.EverVisited && !settlement.trader.RestockedSinceLastVisit) return false;

        return true;
    }
}