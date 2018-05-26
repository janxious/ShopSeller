# ShopSeller

Make the BattleTech shops less hard/faster to use

##
BattleTech Mod (using [BTML](https://github.com/Mpstark/BattleTechModLoader) and [ModTek](https://github.com/Mpstark/ModTek)) that adds fast selling to the shop.

You know how you have like a million jump jets and you want to sell some of them. Well, here we are.

WIP

## Features
- Change the color of direct line of fire indicator drawn between the currently controlled mech and enemy targets
- Change the color of indirect line of fire indicator drawn between the currently controlled mech and enemy targets
- Add dashes to the indirect line of fire indicator drawn between the currently controlled mech and enemy targets
- Change the color of the line of fire indicator for obstructed targets on the attacker and target sides of the obstruction

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
`directLineOfFireArcActive` | `bool` | `false` | change the look of the direct firing line arc
`directLineOfFireArcColor` | `float[4]` | `[0, 1, 0.5, 1]` (light blue) | the color of the direct firing line arc. The default in vanilla is `[1, 0, 0, 1]` (red).

## Special Thanks

HBS, @Mpstark, @Morphyum


## Maintainer Notes: New HBS Patch Instructions

* pop open VS
* grab the latest version of the assembly
* copy the new version of the methods in `original_src` over the existing ones
* see if anything important changed via git
