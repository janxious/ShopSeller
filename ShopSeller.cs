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
            private static int numToBuyOrSell = 0;
            private static SG_Shop_Screen shopScreen;
            const int BigNumberForApproxInfinity = 1_000_000_000;

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
                var isBuySellButton = button == "Capitalism";
                if (!isBuySellButton || !selectedControllerIsPresent)
                {
                    Logger.Debug("Different button, no need to do anything");
                    ResetVariables();
                    return true;
                }

                var amounts = new List<int>
                {
                    ModSettings.CtrlAndShiftKeyCombinationModifierActive && ctrlAndShiftHeld ? ModSettings.CtrlAndShiftKeyCombinationAmount : 0,
                    ModSettings.CtrlKeyModifierActive && ctrlHeld ? ModSettings.CtrlKeyAmount : 0,
                    ModSettings.ShiftKeyModifierActive && shiftHeld ? ModSettings.ShiftKeyAmount : 0,
                    1 // no modifier keys
                };
                var maxModifierAmount = amounts.Max();
                selectedController.RefreshInfo();
                var shopDefItem = selectedController.shopDefItem;
                var quantity = shopDefItem.IsInfinite ? BigNumberForApproxInfinity : selectedController.quantity;
                Logger.Debug($"raw count: {selectedController.quantity}");
                Logger.Debug($"count: {quantity}");
                var numberToTrade = new List<int> {maxModifierAmount, quantity}.Min();
                Logger.Debug($"how many? {numberToTrade}");
                numToBuyOrSell = numberToTrade;

                var simGameState = Traverse.Create(__instance).Field("simState").GetValue<SimGameState>();
                var minimumThreshold = simGameState.Constants.Finances.ShopWarnBeforeSellingPriceMinimum;
                amounts.ForEach(delegate(int amount) { Logger.Debug($"{amount}"); });
                Logger.Debug($"max: {numToBuyOrSell}");
                Logger.Debug($"threshold: {minimumThreshold}");

                if (isInBuyingState)
                {
                    var cbillValue = simGameState.CurSystem.Shop.GetPrice(shopDefItem, Shop.PurchaseType.Normal);
                    var cbillTotal = cbillValue * numToBuyOrSell;
                    Logger.Debug($"item value: {cbillValue}");
                    Logger.Debug($"item total: {cbillTotal}");
                    if (cbillTotal > minimumThreshold && ModSettings.WarnWhenBuyingAbovePriceMinimum)
                    {
                        GenericPopupBuilder.
                            Create("Confirm?", $"Purchase {numToBuyOrSell} for {SimGameState.GetCBillString(cbillTotal)}?").
                            AddButton("Cancel", null, true, (PlayerAction) null).
                            AddButton("Accept", (Action) BuyCurrent, true, (PlayerAction) null).
                            CancelOnEscape().
                            AddFader(LazySingletonBehavior<UIManager>.Instance.UILookAndColorConstants.PopupBackfill, 0.0f, true).
                            Render();
                    }
                    else
                    {
                        BuyCurrent();
                    }
                    return false;
                } else {
                    var cbillValue = selectedController.GetCBillValue();
                    var cbillTotal = cbillValue * numToBuyOrSell;
                    Logger.Debug($"item value: {cbillValue}");
                    Logger.Debug($"item total: {cbillTotal}");
                    if (cbillTotal > minimumThreshold && ModSettings.WarnWhenSellingAbovePriceMinimum)
                    {
                        GenericPopupBuilder.
                            Create("Confirm?", $"Sell {numToBuyOrSell} for {SimGameState.GetCBillString(cbillTotal)}?").
                            AddButton("Cancel", (Action) null, true, (PlayerAction) null).
                            AddButton("Accept", (Action) SellCurrent, true, (PlayerAction) null).
                            CancelOnEscape().
                            AddFader(LazySingletonBehavior<UIManager>.Instance.UILookAndColorConstants.PopupBackfill, 0.0f, true).
                            Render();
                    }
                    else
                    {
                        SellCurrent();
                    }
                    return false;
                }
            }

            static void ResetVariables()
            {
                numToBuyOrSell = 0;
                shopScreen = null; // clear ref for GC
            }

            static void BuyCurrent()
            {
                Logger.Debug($"buying: {numToBuyOrSell}");
                while (numToBuyOrSell > 0)
                {
                    Logger.Debug($"{numToBuyOrSell}");
                    numToBuyOrSell--;
                    shopScreen.BuyCurrentSelection();
                }
                ResetVariables();
            }

            static void SellCurrent()
            {
                Logger.Debug($"selling: {numToBuyOrSell}");
                while (numToBuyOrSell > 0)
                {
                    Logger.Debug($"{numToBuyOrSell}");
                    numToBuyOrSell--;
                    shopScreen.SellCurrentSelection();
                }
                ResetVariables();
            }
        }
    }
}