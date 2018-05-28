# ShopSeller

Make the BattleTech shops less hard/faster to use

##
BattleTech Mod (using [BTML](https://github.com/Mpstark/BattleTechModLoader) and [ModTek](https://github.com/Mpstark/ModTek)) that adds fast selling to the shop.

You know how you have like a million jump jets and you want to sell some of them. Well, here we are.

## Features

- Sell more than one thing at once, and the amount is configured in `mod.json`
- - shift + sell button
- - ctrl + sell button
- - shift+ctrl + sell button

## Download
Downloads can be found on [Github](https://github.com/janxious/ShopSeller/releases).

## Install
- [Install BTML and Modtek](https://github.com/Mpstark/ModTek/wiki/The-Drop-Dead-Simple-Guide-to-Installing-BTML-&-ModTek-&-ModTek-mods).
- Put the `ShopSeller.dll` and `mod.json` files into `\BATTLETECH\Mods\ShopSeller` folder.
- If you want to change the settings do so in the mod.json.
- Start the game.

## Settings

Setting | Type | Default | Description
--- | --- | --- | ---
`shiftKeyModifierActive` | `bool` | `true` | shift key modifier is enabled when clicking the sell button in shop
`shiftKeySellAmount` | `int` | 10 | sell this many of the selected item when shift key is held and sell button pressed
`ctrlKeyModifierActive` | `bool` | `false` | ctrl key modifier is enabled when clicking the sell button in shop
`ctrlKeySellAmount` | `int` | 25 | sell this many of the selected item when ctrl key is held and sell button pressed (disabled by default)
`ctrlAndShiftKeyCombinationModifierActive` | `bool` | `false` | ctrl+shift keys modifier is enabled when clicking the sell button in shop
`ctrlAndShiftKeyCombinationSellAmount` | `int` | 1000 | sell this many of the selected item when shift+ctrl keys are held and sell button pressed (disabled by default)
`warnWhenBuyingAbovePriceMinimum` | `bool` | true | popup a warning when buying multiple items and total bove your set minimum cbill threshold 
`warnWhenSellingAbovePriceMinimum` | `bool` | false | popup a warning when selling multiple items and total above your set minimum cbill threshold 

## Special Thanks

HBS, @Mpstark, @Morphyum


## Maintainer Notes: New HBS Patch Instructions

* pop open VS
* grab the latest version of the assembly
* copy the new version of the methods in `original_src` over the existing ones
* see if anything important changed via git
