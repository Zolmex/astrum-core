package com.company.assembleegameclient.util {
import com.company.assembleegameclient.itemData.ItemData;
import com.company.assembleegameclient.ui.tooltip.TooltipHelper;

import kabam.rotmg.constants.ItemConstants;

public class TierUtil {

    public static function getTierTag(itemData:ItemData, size:int = 12):UILabel {
        var color:Number;
        var text:String;
        if (!itemData) return null;
        if (!itemData.Consumable && !itemData.Treasure && !itemData.PermaPet) {
            var label:UILabel = new UILabel();
            if (itemData.Tier != -1) {
                color = 16777215;
                text = "T" + String(itemData.Tier);
            }
            if (text == null) {
                if (ItemConstants.isEquippable(itemData.SlotType)) {
                    color = TooltipHelper.UNTIERED_COLOR;
                    text = "UT";
                }
                if (!text)
                    return null;
            }
            label.text = text;
            DefaultLabelFormat.tierLevelLabel(label, size, color);
            return label;
        }
        return null;
    }
}
}
