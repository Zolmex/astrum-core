package com.company.assembleegameclient.appengine
{
   import com.company.assembleegameclient.objects.ObjectLibrary;
   import com.company.assembleegameclient.parameters.Parameters;
   import com.company.assembleegameclient.util.AnimatedChar;
   import com.company.assembleegameclient.util.AnimatedChars;
   import com.company.assembleegameclient.util.MaskedImage;
   import com.company.assembleegameclient.util.TextureRedrawer;
import com.company.assembleegameclient.util.redrawers.GlowRedrawer;
import com.company.util.CachingColorTransformer;
   import flash.display.BitmapData;
   import flash.geom.ColorTransform;
   
   public class SavedCharacter
   {
      private static const dimmedCT:ColorTransform = new ColorTransform(0, 0, 0, 0.5, 0, 0, 0, 0);
      private static const selectedCT:ColorTransform = new ColorTransform(0.75, 0.75, 0.75, 1, 0, 0, 0, 0);

      public var charXML_:XML;
      
      public var name_:String = null;
      
      public function SavedCharacter(charXML:XML, name:String)
      {
         super();
         this.charXML_ = charXML;
         this.name_ = name;
      }
      
      public static function getImage(savedChar:SavedCharacter, playerXML:XML, dir:int, action:int, p:Number, selected:Boolean, dimmed:Boolean) : BitmapData
      {
         var animatedChar:AnimatedChar = AnimatedChars.getAnimatedChar(String(playerXML.AnimatedTexture.File),int(playerXML.AnimatedTexture.Index));
         var image:MaskedImage = animatedChar.imageFromDir(dir,action,p);
         var tex1:int = savedChar != null?int(savedChar.tex1()):int(null);
         var tex2:int = savedChar != null?int(savedChar.tex2()):int(null);
         var bd:BitmapData = TextureRedrawer.resize(image.image_,image.mask_,100,false,tex1,tex2);
         bd = GlowRedrawer.outlineGlow(bd,0);
         if (dimmed)
         {
            bd = CachingColorTransformer.transformBitmapData(bd,dimmedCT);
         }
         else if(!selected)
         {
            bd = CachingColorTransformer.transformBitmapData(bd,selectedCT);
         }
         return bd;
      }
      
      public static function compare(char1:SavedCharacter, char2:SavedCharacter) : Number
      {
         var char1Use:Number = Boolean(Parameters.data_.charIdUseMap.hasOwnProperty(char1.charId().toString()))?Number(Parameters.data_.charIdUseMap[char1.charId()]):Number(0);
         var char2Use:Number = Boolean(Parameters.data_.charIdUseMap.hasOwnProperty(char2.charId().toString()))?Number(Parameters.data_.charIdUseMap[char2.charId()]):Number(0);
         if(char1Use != char2Use)
         {
            return char2Use - char1Use;
         }
         return char2.xp() - char1.xp();
      }
      
      public function charId() : int
      {
         return int(this.charXML_.@id);
      }
      
      public function name() : String
      {
         return this.name_;
      }
      
      public function objectType() : int
      {
         return int(this.charXML_.ObjectType);
      }
      
      public function skinType() : int
      {
         return int(this.charXML_.Texture);
      }
      
      public function level() : int
      {
         return int(this.charXML_.Level);
      }
      
      public function tex1() : int
      {
         return int(this.charXML_.Tex1);
      }
      
      public function tex2() : int
      {
         return int(this.charXML_.Tex2);
      }
      
      public function xp() : int
      {
         return int(this.charXML_.Exp);
      }
      
      public function fame() : int
      {
         return int(this.charXML_.CurrentFame);
      }
      
      public function displayId() : String
      {
         return ObjectLibrary.typeToDisplayId_[this.objectType()];
      }
      
      public function toString() : String
      {
         return "SavedCharacter: {" + this.charXML_ + "}";
      }
   }
}
