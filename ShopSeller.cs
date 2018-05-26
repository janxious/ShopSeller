﻿using System;
using System.Reflection;
using BattleTech.UI;
using Harmony;
using Newtonsoft.Json;
using UnityEngine;

namespace ShopSeller
{
    
    public class ShopSeller
    {
        internal static Settings ModSettings = new Settings();
        internal static string ModDirectory;

        public static void Init(string directory, string settingsJSON)
        {
            var harmony = HarmonyInstance.Create("com.joelmeador.ShopSeller");
            harmony.PatchAll(Assembly.GetExecutingAssembly());
            ModDirectory = directory;
            try
            {
                ModSettings = JsonConvert.DeserializeObject<Settings>(settingsJSON);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex);
                ModSettings = new Settings();
            }
        }

        // TODO: check why the handle enter handler doesn't get to this method unless shift is not pressed
        // TODO: sell an item
        // TODO: sell ModSettings.ShiftKeySellAmount items
        [HarmonyPatch(typeof(SG_Shop_Screen), "ReceiveButtonPress")]
        public static class SG_Shop_ReceiveButtonPressPatch
        {
            static bool Prefix(
                string button,
                SG_Shop_Screen __instance)
            {
                var shiftHeld = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);
                Logger.LogLine($"shift helf: {shiftHeld}");
                var selectedControllerIsPresent =
                    Traverse.Create(__instance).Field("selectedController").GetValue() != null;
                var isInSellingState =
                    ! Traverse.Create(__instance).Field("isInBuyingState").GetValue<bool>();
                if (button == "Capitalism" && selectedControllerIsPresent && isInSellingState && shiftHeld)
                {
                    Logger.LogLine($"active: {ModSettings.ShiftKeyModifierActive}\namount: {ModSettings.ShiftKeySellAmount}");
                    return false;
                }
                return true;
            }
        }
    }
}