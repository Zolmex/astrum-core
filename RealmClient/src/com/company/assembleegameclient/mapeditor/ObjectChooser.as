package com.company.assembleegameclient.mapeditor
{
   import com.company.assembleegameclient.objects.ObjectLibrary;
   import com.company.util.MoreStringUtil;

   public class ObjectChooser extends Chooser
   {
       
      
      function ObjectChooser()
      {
         var id:* = null;
         var type:int = 0;
         var objXML:XML = null;
         var objectElement:ObjectElement = null;
         super(Layer.OBJECT);
         var ids:Vector.<String> = new Vector.<String>();
         for(id in ObjectLibrary.idToType_)
         {
            ids.push(id);
         }
         ids.sort(MoreStringUtil.cmp);
         for each(id in ids)
         {
            type = ObjectLibrary.idToType_[id];
            objXML = ObjectLibrary.xmlLibrary_[type];
            if(!(objXML.hasOwnProperty("Item") || objXML.hasOwnProperty("Player") || objXML.Class == "Projectile"))
            {
               objectElement = new ObjectElement(objXML);
               addElement(objectElement);
            }
         }
      }
   }
}
