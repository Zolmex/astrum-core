package com.company.assembleegameclient.sound
{
   import com.company.assembleegameclient.parameters.Parameters;
   import flash.events.IOErrorEvent;
   import flash.media.Sound;
   import flash.media.SoundTransform;
   import flash.net.URLRequest;
   import flash.utils.Dictionary;
   
   public class SoundEffectLibrary
   {
      private static const URL_PATTERN:String = "{URLBASE}/sfx/{NAME}.mp3";
      public static var nameMap_:Dictionary = new Dictionary();
      private static var playMap_:Dictionary = new Dictionary();
      private static var clearTicks:int;
       
      
      public function SoundEffectLibrary()
      {
         super();
      }
      
      public static function load(name:String) : Sound
      {
         return nameMap_[name] = nameMap_[name] || makeSound(name);
      }
      
      public static function makeSound(name:String) : Sound
      {
         var sound:Sound = new Sound();
         sound.addEventListener(IOErrorEvent.IO_ERROR,onIOError);
         sound.load(makeSoundRequest(name));
         return sound;
      }
      
      private static function makeSoundRequest(name:String) : URLRequest
      {
         var url:String = URL_PATTERN.replace("{URLBASE}", Parameters.appServerAddress()).replace("{NAME}",name);
         return new URLRequest(url);
      }

      public static function clear(): void {
         clearTicks++;
         if (clearTicks % 5 == 0) {
            playMap_ = new Dictionary();
         }
      }
      
      public static function play(name:String, volume:Number = 1.0, isFX:Boolean = true) : void
      {
         var playFX:Boolean = Parameters.data_.playSFX && isFX || !isFX && Parameters.data_.playPewPew;
         if (!playFX) {
            return;
         }

         if (playMap_[name]) {
            return;
         }

         playMap_[name] = true;

         var actualVolume:Number = NaN;
         var trans:SoundTransform = null;
         var sound:Sound = load(name);
         try
         {
            actualVolume = Number(volume);
            trans = new SoundTransform(actualVolume);
            sound.play(0,0,trans);
         }
         catch(error:Error)
         {
            trace("ERROR playing " + name + ": " + error.message);
         }
      }
      
      public static function onIOError(event:IOErrorEvent) : void
      {
         trace("ERROR loading sound: " + event.text);
      }
   }
}
