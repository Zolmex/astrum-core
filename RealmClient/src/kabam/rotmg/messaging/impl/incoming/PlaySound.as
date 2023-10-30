package kabam.rotmg.messaging.impl.incoming
{
   import flash.utils.IDataInput;
   
   public class PlaySound extends IncomingMessage
   {
      public var sound_:String;
      
      public function PlaySound(id:uint, callback:Function)
      {
         sound_ = new String();
         super(id,callback);
      }
      
      override public function parseFromInput(data:IDataInput) : void
      {
         this.sound_ = data.readUTF();
      }
      
      override public function toString() : String
      {
         return formatToString("PLAYSOUND","sound_");
      }
   }
}
