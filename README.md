# This mod is no longer supported as the functionality has roughly been duplicated in v1.2 of the BattleTech game.

# ShopSeller

Make the BattleTech shops less hard/faster to use

##
BattleTech Mod (using [BTML](https://github.com/Mpstark/BattleTechModLoader) and [ModTek](https://github.com/Mpstark/ModTek)) that adds fast selling to the shop.

You know how you have like a million jump jets and you want to sell some of them before the heat death of the universe. Well, here we are. Also you can buy more than one thing at a time. Also you can disable those popups! Also you can change the amount of spare change involved when the popups happen

## Features

- Buy or sell more than one thing at once, and the amount is configured in `mod.json`
- - shift + buy/sell button
- - ctrl + buy/sell button
- - shift+ctrl + buy/sell button
- disable popups when buying/selling
- change c-bill threshold for when buy/sell popups occur

## Download
Downloads can be found on [Github](https://github.com/janxious/ShopSeller/releases).

## Install
- [Install BTML and Modtek](https://github.com/Mpstark/ModTek/wiki/The-Drop-Dead-Simple-Guide-to-Installing-BTML-&-ModTek-&-ModTek-mods).
- Make sure the `constansts\SimGameConstants.json` (keep that directory), `ShopSeller.dll` and `mod.json` files into `\BATTLETECH\Mods\ShopSeller` folder.
- If you want to change the settings do so in the `mod.json` and `constants\SimGameConstants.json` .
- Start the game.

## Settings

Setting | Type | Default | Description
--- | --- | --- | ---
`shiftKeyModifierActive` | `bool` | `true` | shift key modifier is enabled when clicking the sell button in shop
`shiftKeyAmount` | `int` | 10 | sell this many of the selected item when shift key is held and sell button pressed
`ctrlKeyModifierActive` | `bool` | `false` | ctrl key modifier is enabled when clicking the sell button in shop
`ctrlKeyAmount` | `int` | 25 | sell this many of the selected item when ctrl key is held and sell button pressed (disabled by default)
`ctrlAndShiftKeyCombinationModifierActive` | `bool` | `false` | ctrl+shift keys modifier is enabled when clicking the sell button in shop
`ctrlAndShiftKeyCombinationAmount` | `int` | 1000 | sell this many of the selected item when shift+ctrl keys are held and sell button pressed (disabled by default)
`warnWhenBuyingAbovePriceMinimum` | `bool` | `true` | popup a warning when buying multiple items and total above your set minimum cbill threshold 
`warnWhenSellingAbovePriceMinimum` | `bool` | `false` | popup a warning when selling multiple items and total above your set minimum cbill threshold 
`debug` | `bool` | `false` | enable debugging logs, probably not useful unless you are changing the code or looking at it as you run the mod

The minimum threshold is set in the file `Mods\ShopSeller\constants\SimGameConstants.json`. Vanilla is set at 100,000 C-Bills, and I upped that to 1,000,000 in this mod.

## Special Thanks

HBS, @Mpstark, @Morphyum


## Maintainer Notes: New HBS Patch Instructions

* pop open VS
* grab the latest version of the assembly
* copy the new version of the methods in `original_src` over the existing ones
* see if anything important changed via git
