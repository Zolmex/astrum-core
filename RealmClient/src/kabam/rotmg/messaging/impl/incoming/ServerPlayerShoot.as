package kabam.rotmg.messaging.impl.incoming
{
   import flash.utils.IDataInput;
   import kabam.rotmg.messaging.impl.data.WorldPosData;
   
   public class ServerPlayerShoot extends IncomingMessage
   {
       
      
      public var bulletId_:uint;
      
      public var ownerId_:int;
      
      public var containerType_:int;
      
      public var startingPos_:WorldPosData;
      
      public var angle_:Number;

      public var angleInc_:Number;
      
      public var damageList_:Vector.<uint>;
      
      public function ServerPlayerShoot(id:uint, callback:Function)
      {
         this.startingPos_ = new WorldPosData();
         this.damageList_ = new Vector.<uint>();
         super(id,callback);
      }
      
      override public function parseFromInput(data:IDataInput) : void
      {
         this.bulletId_ = data.readInt();
         this.ownerId_ = data.readInt();
         this.containerType_ = data.readShort();
         this.startingPos_.parseFromInput(data);
         this.angle_ = data.readFloat();
         this.angleInc_ = data.readFloat();
         this.damageList_.length = 0;
         var len:int = data.readUnsignedByte();
         for (var i:int = 0; i < len; i++) {
            this.damageList_.push(data.readShort());
         }
      }
      
      override public function toString() : String
      {
         return formatToString("SHOOT","bulletId_","ownerId_","containerType_","startingPos_","angle_","damage_");
      }
   }
}
