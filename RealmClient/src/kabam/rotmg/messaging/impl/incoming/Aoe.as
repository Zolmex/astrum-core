package kabam.rotmg.messaging.impl.incoming
{
   import flash.utils.IDataInput;
   import kabam.rotmg.messaging.impl.data.WorldPosData;
   
   public class Aoe extends IncomingMessage
   {
       
      
      public var pos_:WorldPosData;
      
      public var radius_:Number;
      
      public var damage_:int;
      
      public var effect_:int;

      public var color_:int;
      
      public function Aoe(id:uint, callback:Function)
      {
         this.pos_ = new WorldPosData();
         super(id,callback);
      }
      
      override public function parseFromInput(data:IDataInput) : void
      {
         this.pos_.parseFromInput(data);
         this.radius_ = data.readFloat();
         this.damage_ = data.readUnsignedShort();
         this.effect_ = data.readUnsignedByte();
         this.color_ = data.readInt();
      }
      
      override public function toString() : String
      {
         return formatToString("AOE","pos_","radius_","damage_","effect_", "color_");
      }
   }
}
