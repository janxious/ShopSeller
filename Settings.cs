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

        public int shiftKeySellAmount = 10;
        public int ShiftKeySellAmount => shiftKeySellAmount;
        #endregion shift key

        #region ctrl key
        public bool ctrlKeyModifierActive = false;
        public bool CtrlKeyModifierActive => ctrlKeyModifierActive;

        public int ctrlKeySellAmount = 0;
        public int CtrlKeySellAmount => ctrlKeySellAmount;
        #endregion ctrl key

        #region shift and ctrl keys together
        public bool ctrlAndShiftKeyCombinationModifierActive = false;
        public bool CtrlAndShiftKeyCombinationModifierActive => ctrlAndShiftKeyCombinationModifierActive;

        public int ctrlAndShiftKeyCombinationSellAmount = 0;
        public int CtrlAndShiftKeyCombinationSellAmount => ctrlAndShiftKeyCombinationSellAmount;
        #endregion shift and ctrl keys together
    }
}