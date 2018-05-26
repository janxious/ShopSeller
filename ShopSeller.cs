using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using BattleTech;
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
                var ctrlHeld = Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl);
                var ctrlAndShiftHeld = shiftHeld && ctrlHeld;
                Logger.LogLine($"shift held: {shiftHeld}\nctrl held: {ctrlHeld}\nctrlshift held: {ctrlAndShiftHeld}");
                var selectedController = Traverse.Create(__instance).Field("selectedController")
                    .GetValue<ListElementController_BASE>();
                var selectedControllerIsPresent = selectedController != null;
                var isInSellingState =
                    ! Traverse.Create(__instance).Field("isInBuyingState").GetValue<bool>();
                if (button == "Capitalism" && selectedControllerIsPresent && isInSellingState && (shiftHeld || ctrlHeld))
                {
                    var sellAmounts = new List<int>
                    {
                        ModSettings.CtrlAndShiftKeyCombinationModifierActive && ctrlAndShiftHeld ? ModSettings.CtrlAndShiftKeyCombinationSellAmount : 0,
                        ModSettings.CtrlKeyModifierActive && ctrlHeld ? ModSettings.CtrlKeySellAmount : 0,
                        ModSettings.ShiftKeyModifierActive && shiftHeld ? ModSettings.ShiftKeySellAmount : 0
                    };
                    var maxToSell = sellAmounts.Max();
                    Logger.LogLine($"ctrlshift active: {ModSettings.CtrlAndShiftKeyCombinationModifierActive}\namount: {ModSettings.CtrlAndShiftKeyCombinationSellAmount}");
                    Logger.LogLine($"ctrl active: {ModSettings.CtrlKeyModifierActive}\namount: {ModSettings.CtrlKeySellAmount}");
                    Logger.LogLine($"shift active: {ModSettings.ShiftKeyModifierActive}\namount: {ModSettings.ShiftKeySellAmount}");
                    Logger.LogLine($"sell amount: {sellAmounts.ToArray()}");
                    Logger.LogLine($"max: {maxToSell}");
                    var shopDefItem = selectedController.shopDefItem;
                    for (var i = 0; i < shopDefItem.Count && i < maxToSell; i++)
                    {
                        Logger.LogLine($"selling 1 of {shopDefItem.ID}");
                        __instance.SellCurrentSelection();
                    }
                    return false;
                }
                return true;
            }
        }
    }
}