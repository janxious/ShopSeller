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
            var harmony = HarmonyInstance.Create("com.joelmeador.ShopSeller");
            harmony.PatchAll(Assembly.GetExecutingAssembly());
        }

        // By default, the game doesn't mind any key combos with Enter, so
        // we need to keep the old binding and add some new ones to handle:
        // enter
        // shift+enter
        // ctrl+enter
        // ctrl+shift+enter
        [HarmonyPatch(typeof(StaticActions), "CreateWithDefaultBindings")]
        public static class StaticActions_Patch_CreateWDB
        {
            public static void Postfix(ref StaticActions __result)
            {
                Logger.Debug("Clearing Default Bindings for enter Key");
                __result.Return.ClearBindings();
                Logger.Debug("Setting enter and others key as defaults for enter Key");
                __result.Return.AddDefaultBinding(new Key[]
                {
                    Key.Return
                });
                __result.Return.AddDefaultBinding(new Key[]
                {
                    Key.Shift,
                    Key.Return
                });
                __result.Return.AddDefaultBinding(new Key[]
                {
                    Key.RightShift,
                    Key.Return
                });
                __result.Return.AddDefaultBinding(new Key[]
                {
                    Key.LeftShift,
                    Key.Return
                });
                Logger.Debug("Success setting enter and others key as defaults for enter Key");
            }
        }

        // Override how the game deals with the "Captialism" button press.
        // That button is the Buy/Sell button (in english).
        // The enter key presses above route to this as well.
        // Also this fixes a bug in the base game wherein enter would get
        // around travel restrictions for buying and selling.
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
                var selectedController = Traverse.Create(shopScreen).Field("selectedController").GetValue<InventoryDataObject_BASE>();
                var selectedControllerIsPresent = selectedController != null;
                var isBuySellButton = button == "Capitalism";
                if (!isBuySellButton || !selectedControllerIsPresent)
                {
                    Logger.Debug($"buysellbutton: {isBuySellButton}\nselectedController: {selectedControllerIsPresent}");
                    ResetVariables();
                    return true;
                }
                var simGameState = Traverse.Create(shopScreen).Field("simState").GetValue<SimGameState>();
                var isTraveling = simGameState.TravelState != SimGameTravelStatus.IN_SYSTEM;
                if (isTraveling)
                {
                    Logger.Debug("traveling, so block buy/sell");
                    ResetVariables();
                    return false;
                }
                var shiftHeld = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);
                var ctrlHeld = Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl);
                var ctrlAndShiftHeld = shiftHeld && ctrlHeld;
                Logger.Debug($"shift held: {shiftHeld}\nctrl held: {ctrlHeld}\nctrlshift held: {ctrlAndShiftHeld}");
                var isInBuyingState = Traverse.Create(shopScreen).Field("isInBuyingState").GetValue<bool>();
                var activeModifierAmounts = new List<int>
                {
                    ModSettings.CtrlAndShiftKeyCombinationModifierActive && ctrlAndShiftHeld ? ModSettings.CtrlAndShiftKeyCombinationAmount : 0,
                    ModSettings.CtrlKeyModifierActive && ctrlHeld ? ModSettings.CtrlKeyAmount : 0,
                    ModSettings.ShiftKeyModifierActive && shiftHeld ? ModSettings.ShiftKeyAmount : 0,
                    1 // no modifier keys
                };
                var maxModifierAmount = activeModifierAmounts.Max();
                // selectedController.RefreshInfo();
                var shopDefItem = selectedController.shopDefItem;
                var quantity = shopDefItem.IsInfinite ? BigNumberForApproxInfinity : selectedController.quantity;
                Logger.Debug($"raw count: {selectedController.quantity}");
                Logger.Debug($"count: {quantity}");
                var numberToTrade = new List<int> {maxModifierAmount, quantity}.Min();
                Logger.Debug($"how many? {numberToTrade}");
                numToBuyOrSell = numberToTrade;


                var minimumThreshold = simGameState.Constants.Finances.ShopWarnBeforeSellingPriceMinimum;
                activeModifierAmounts.ForEach(delegate(int amount) { Logger.Debug($"{amount}"); });
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

            private static void BuyCurrent()
            {
                Logger.Debug($"buying: {numToBuyOrSell}");
                while (numToBuyOrSell > 0)
                {
                    numToBuyOrSell--;
                    shopScreen.BuyCurrentSelection();
                }
                ResetVariables();
            }

            private static void SellCurrent()
            {
                Logger.Debug($"selling: {numToBuyOrSell}");
                while (numToBuyOrSell > 0)
                {
                    numToBuyOrSell--;
                    shopScreen.SellCurrentSelection();
                }
                ResetVariables();
            }

            private static void ResetVariables()
            {
                numToBuyOrSell = 0;
                shopScreen = null; // clear ref for GC
            }
        }
    }
}