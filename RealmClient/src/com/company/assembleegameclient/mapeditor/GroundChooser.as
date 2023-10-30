package com.company.assembleegameclient.mapeditor
{
   import com.company.assembleegameclient.map.GroundLibrary;
   import com.company.util.MoreStringUtil;
   
   public class GroundChooser extends Chooser
   {
       
      
      function GroundChooser()
      {
         var id:* = null;
         var type:int = 0;
         var tileElement:GroundElement = null;
         super(Layer.GROUND);
         var ids:Vector.<String> = new Vector.<String>();
         for(id in GroundLibrary.idToType_)
         {
            ids.push(id);
         }
         ids.sort(MoreStringUtil.cmp);
         for each(id in ids)
         {
            type = GroundLibrary.idToType_[id];
            tileElement = new GroundElement(GroundLibrary.xmlLibrary_[type]);
            addElement(tileElement);
         }
      }
   }
}
