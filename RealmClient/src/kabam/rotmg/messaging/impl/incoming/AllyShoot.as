package kabam.rotmg.messaging.impl.incoming
{
   import flash.utils.IDataInput;
   
   public class AllyShoot extends IncomingMessage
   {
      public var ownerId_:int;
      public var containerType_:int;
      public var angle_:Number;
      
      public function AllyShoot(id:uint, callback:Function)
      {
         super(id,callback);
      }
      
      override public function parseFromInput(data:IDataInput) : void
      {
         this.ownerId_ = data.readInt();
         this.containerType_ = data.readShort();
         this.angle_ = data.readFloat();
      }
      
      override public function toString() : String
      {
         return formatToString("ALLYSHOOT","ownerId_","containerType_","angle_");
      }
   }
}
