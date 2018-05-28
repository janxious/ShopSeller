using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using BattleTech;
using BattleTech.UI;
using Harmony;
using HBS;
using Newtonsoft.Json;
using UnityEngine;
using InControl;

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
                Logger.Error(ex);
                ModSettings = new Settings();
            }
        }

        // The following code doesn't work in the sense that
        // the the only key that seems to be held down is the enter
        // when we get in the code. The why of that is somewhat a mystery
        // to me.
//        [HarmonyPatch(typeof(SG_Shop_Screen), "HandleEnterKeypress")]
//        public static class SG_Shop_HandleEnterKeypressPatch
//        {
//            static bool Prefix(
//                ref bool __result,
//                SG_Shop_Screen __instance
//            )
//            {
//                Logger.Debug("We hit the handle enter patch");
//                var shiftHeld = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);
//                var ctrlHeld = Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl);
//                var ctrlAndShiftHeld = shiftHeld && ctrlHeld;
//                Logger.Debug($"shift held: {shiftHeld}\nctrl held: {ctrlHeld}\nctrlshift held: {ctrlAndShiftHeld}");
//                var btn = Traverse.Create(__instance).Field("BuyBotton").GetValue<HBSButton>();
//                var btnNotVisible = !(btn != null) || 
//                                    !__instance.Visible ||
//                                    !btn.gameObject.activeInHierarchy || 
//                                    btn.State != ButtonState.Enabled;
//                if (btnNotVisible)
//                {
//                    __result = false;
//                    return false;
//                }
//                __result = true;
//                __instance.ReceiveButtonPress(btn.name);
//                return false;
//            }
//        }

        [HarmonyPatch(typeof(SG_Shop_Screen), "ReceiveButtonPress")]
        public static class SG_Shop_ReceiveButtonPressPatch
        {
            private static int numToSell = 0;
            //private static int numToBuy = 0;
            private static SG_Shop_Screen shopScreen;

            static bool Prefix(
                string button,
                SG_Shop_Screen __instance)
            {
                shopScreen = __instance;
                var shiftHeld = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);
                var ctrlHeld = Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl);
                var ctrlAndShiftHeld = shiftHeld && ctrlHeld;
                Logger.Debug($"shift held: {shiftHeld}\nctrl held: {ctrlHeld}\nctrlshift held: {ctrlAndShiftHeld}");
                var selectedController = Traverse.Create(__instance).Field("selectedController").GetValue<ListElementController_BASE>();
                var selectedControllerIsPresent = selectedController != null;
                var isInBuyingState = Traverse.Create(__instance).Field("isInBuyingState").GetValue<bool>();
                var isInSellingState = !isInBuyingState;
                if (button == "Capitalism" && selectedControllerIsPresent && isInSellingState && (shiftHeld || ctrlHeld))
                {
                    var sellAmounts = new List<int>
                    {
                        ModSettings.CtrlAndShiftKeyCombinationModifierActive && ctrlAndShiftHeld ? ModSettings.CtrlAndShiftKeyCombinationSellAmount : 0,
                        ModSettings.CtrlKeyModifierActive && ctrlHeld ? ModSettings.CtrlKeySellAmount : 0,
                        ModSettings.ShiftKeyModifierActive && shiftHeld ? ModSettings.ShiftKeySellAmount : 0
                    };
                    var maxModifierSellAmount = sellAmounts.Max();
                    var shopDefItem = selectedController.shopDefItem;
                    var maxToSell = new List<int> {maxModifierSellAmount, shopDefItem.Count}.Min();
                    var cbillValue = selectedController.GetCBillValue();
                    var minimumThreshold = Traverse.Create(__instance).Field("simState").GetValue<SimGameState>().Constants.Finances.ShopWarnBeforeSellingPriceMinimum;;
                    var cbillTotal = cbillValue * maxToSell;
                    Logger.Debug($"sell amount: {sellAmounts.ToArray()}");
                    Logger.Debug($"max: {maxToSell}");
                    Logger.Debug($"item value: {cbillValue}");
                    Logger.Debug($"item total: {cbillTotal}");
                    Logger.Debug($"threshold: {minimumThreshold}");
                    numToSell = maxToSell;
                    if (cbillTotal > minimumThreshold && ModSettings.WarnWhenSellingAbovePriceMinimum)
                    {
                        GenericPopupBuilder.Create(
                            "Confirm?",
                            string.Format("Sell {0} of {1} for {2}?", numToSell, ,(object) SimGameState.GetCBillString(cbillTotal)))
                            .AddButton("Cancel", (Action) null, true, (PlayerAction) null)
                            .AddButton("Accept", new Action(SellCurrent), true, (PlayerAction) null).CancelOnEscape()
                            .AddFader(LazySingletonBehavior<UIManager>.Instance.UILookAndColorConstants.PopupBackfill,
                                0.0f,
                                true).Render();
                    }
                    else
                    {
                        SellCurrent();
                    }
                    ResetVariables();
                    return false;
                }
                ResetVariables();
                return true;
            }

            static void ResetVariables()
            {
                numToSell = 0;
                //numToBuy = 0;
                shopScreen = null; // clear ref for GC
            }
            static void SellCurrent()
            {
                while (numToSell > 0)
                {
                    numToSell--;
                }
                for (var i = 0; i < numToSell; i++)
                {
                    shopScreen.SellCurrentSelection();
                }
            }
        }
    }
}