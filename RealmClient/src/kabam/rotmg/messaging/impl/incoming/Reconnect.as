package kabam.rotmg.messaging.impl.incoming
{
   import flash.utils.ByteArray;
   import flash.utils.IDataInput;
   
   public class Reconnect extends IncomingMessage
   {
      public var gameId_:int;
      
      public function Reconnect(id:uint, callback:Function)
      {
         super(id,callback);
      }
      
      override public function parseFromInput(data:IDataInput) : void
      {
         this.gameId_ = data.readInt();
      }
      
      override public function toString() : String
      {
         return formatToString("RECONNECT","gameId_");
      }
   }
}
