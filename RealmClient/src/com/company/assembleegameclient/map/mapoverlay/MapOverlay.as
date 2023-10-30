package com.company.assembleegameclient.map.mapoverlay
{
   import com.company.assembleegameclient.map.Camera;
   import flash.display.Sprite;
   
   public class MapOverlay extends Sprite
   {
      private const speechBalloons:Object = {};
      
      public function MapOverlay()
      {
         super();
         mouseEnabled = false;
         mouseChildren = false;
      }
      
      public function addSpeechBalloon(sb:SpeechBalloon) : void
      {
         var id:int = sb.go_.objectId_;
         var currentBalloon:SpeechBalloon = this.speechBalloons[id];
         if(currentBalloon && contains(currentBalloon))
         {
            removeChild(currentBalloon);
         }
         this.speechBalloons[id] = sb;
         addChild(sb);
      }
      
      public function addStatusText(text:CharacterStatusText) : void
      {
         addChild(text);
      }
      
      public function draw(camera:Camera, time:int) : void
      {
         var elem:IMapOverlayElement = null;
         for(var i:int = 0; i < numChildren; )
         {
            elem = getChildAt(i) as IMapOverlayElement;
            if(!elem || elem.draw(camera,time))
            {
               i++;
            }
            else
            {
               elem.dispose();
            }
         }
      }
   }
}
