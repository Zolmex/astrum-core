package kabam.rotmg.game.model
{
   import com.company.assembleegameclient.objects.GameObject;
   
   public class AddSpeechBalloonVO
   {
       
      
      public var go:GameObject;
      
      public var text:String;
      
      public var background:uint;
      
      public var backgroundAlpha:Number;
      
      public var outline:uint;
      
      public var outlineAlpha:uint;
      
      public var textColor:uint;
      
      public var lifetime:int;
      
      public var bold:Boolean;
      
      public var hideable:Boolean;
      
      public function AddSpeechBalloonVO(go:GameObject, text:String, background:uint, backgroundAlpha:Number, outline:uint, outlineAlpha:Number, textColor:uint, lifetime:int, bold:Boolean, hideable:Boolean)
      {
         super();
         this.go = go;
         this.text = text;
         this.background = background;
         this.backgroundAlpha = backgroundAlpha;
         this.outline = outline;
         this.outlineAlpha = outlineAlpha;
         this.textColor = textColor;
         this.lifetime = lifetime;
         this.bold = bold;
         this.hideable = hideable;
      }
   }
}
