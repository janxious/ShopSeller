using UnityEngine;

namespace ShopSeller
{
    // the properties in in here are to be a deserializer step from JSON format
    // into the C# variables used by the mod.
    public class Settings
    {
        #region log control
        public bool debug = false;
        #endregion

        #region shift key
        public bool shiftKeyModifierActive = true;
        public bool ShiftKeyModifierActive => shiftKeyModifierActive;

        public int shiftKeyAmount = 10;
        public int ShiftKeyAmount => shiftKeyAmount;
        #endregion shift key

        #region ctrl key
        public bool ctrlKeyModifierActive = false;
        public bool CtrlKeyModifierActive => ctrlKeyModifierActive;

        public int ctrlKeyAmount = 0;
        public int CtrlKeyAmount => ctrlKeyAmount;
        #endregion ctrl key

        #region shift and ctrl keys together
        public bool ctrlAndShiftKeyCombinationModifierActive = false;
        public bool CtrlAndShiftKeyCombinationModifierActive => ctrlAndShiftKeyCombinationModifierActive;

        public int ctrlAndShiftKeyCombinationAmount = 0;
        public int CtrlAndShiftKeyCombinationAmount => ctrlAndShiftKeyCombinationAmount;
        #endregion shift and ctrl keys together
        
        #region warnings for buy/sell cbill amounts

        public bool warnWhenBuyingAbovePriceMinimum = false;
        public bool WarnWhenBuyingAbovePriceMinimum => warnWhenBuyingAbovePriceMinimum;

        public bool warnWhenSellingAbovePriceMinimum = false;
        public bool WarnWhenSellingAbovePriceMinimum => warnWhenSellingAbovePriceMinimum;

        #endregion warnings for buy/sell cbill amounts
    }
}