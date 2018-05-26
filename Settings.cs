using UnityEngine;

namespace ShopSeller
{
    // the properties in in here are to be a deserializer step from JSON format
    // into the C# variables used by the mod.
    public class Settings
    {
        public bool shiftKeyModifierActive = false;
        public bool ShiftKeyModifierActive => shiftKeyModifierActive;

        public int shiftKeySellAmount = 1;
        public int ShiftKeySellAmount => shiftKeySellAmount;
    }
}