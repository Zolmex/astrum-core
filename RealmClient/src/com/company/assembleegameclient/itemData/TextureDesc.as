package com.company.assembleegameclient.itemData {
import com.company.assembleegameclient.objects.ObjectLibrary;
import com.company.assembleegameclient.util.AnimatedChar;
import com.company.assembleegameclient.util.AnimatedChars;
import com.company.assembleegameclient.util.TextureRedrawer;

import flash.display.BitmapData;

public class TextureDesc {

    public var File:String;
    public var Index:int;

    public function TextureDesc(obj:*) {
        if (obj) {
            this.File = obj.File;
            this.Index = obj.Index;
        }
    }

    public function getRedrawnTexture(size:uint, glowColor:int, scaleSize:Boolean = false):BitmapData {
        var animChar:AnimatedChar = AnimatedChars.getAnimatedChar(this.File, this.Index);
        if (animChar) {
            var tex:BitmapData = animChar.imageFromDir(AnimatedChar.RIGHT, AnimatedChar.WALK, 1).image_;
            if (scaleSize)
                size /= tex.width / 8;
            return TextureRedrawer.redraw(tex, size, true, glowColor);
        }
        return ObjectLibrary.getRedrawnTextureFromFile(this.File, this.Index, size, glowColor, scaleSize);
    }
}
}
