package com.company.assembleegameclient.util {
import com.company.ui.SimpleText;

import flash.text.TextFormat;

public class DefaultLabelFormat {

    public function DefaultLabelFormat() {
        super();
    }

    public static function createLabelFormat(label:UILabel, size:int = 12, color:Number = 16777215, align:String = "left", bold:Boolean = false, filters:Array = null):void {
        var format:TextFormat = createTextFormat(size, color, align, bold);
        applyTextFormat(format, label);
        if (filters) {
            label.filters = filters;
        }
    }

    public static function tierLevelLabel(label:UILabel, size:int = 12, color:Number = 16777215, align:String = "right"):void {
        createLabelFormat(label, size, color, align, true);
    }

    private static function applyTextFormat(format:TextFormat, label:UILabel):void {
        label.defaultTextFormat = format;
        label.setTextFormat(format);
    }

    public static function createTextFormat(size:int, color:uint, align:String, bold:Boolean):TextFormat {
        var format:TextFormat = new TextFormat();
        format.color = color;
        format.bold = bold;
        format.font = SimpleText._Font.fontName;
        format.size = size;
        format.align = align;
        return format;
    }
}
}
