package com.company.assembleegameclient.game.events
{
   import flash.events.Event;
   import flash.utils.ByteArray;
   
   public class ReconnectEvent extends Event
   {
      public static const RECONNECT:String = "RECONNECT_EVENT";
      
      public var gameId_:int;
      
      public var createCharacter_:Boolean;
      
      public var charId_:int;
      
      public function ReconnectEvent(gameId:int, createCharacter:Boolean, charId:int)
      {
         super(RECONNECT);
         this.gameId_ = gameId;
         this.createCharacter_ = createCharacter;
         this.charId_ = charId;
      }
      
      override public function clone() : Event
      {
         return new ReconnectEvent(this.gameId_,this.createCharacter_,this.charId_);
      }
      
      override public function toString() : String
      {
         return formatToString(RECONNECT,"server_","gameId_","charId_");
      }
   }
}
