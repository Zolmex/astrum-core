package com.company.assembleegameclient.itemData {

public class CustomToolTipData {
    
    public var Name:String;
    public var Description:String;
    
    public function CustomToolTipData(obj:*) {
        this.Name = ItemData.GetValue(obj, null, "@name", "");
        this.Description = ItemData.GetValue(obj, null, "@description", null);
    }
}
}
