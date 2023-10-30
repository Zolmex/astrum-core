package kabam.rotmg.messaging.impl.outgoing
{
   import flash.utils.IDataOutput;
   import kabam.rotmg.messaging.impl.data.SlotObjectData;
   import kabam.rotmg.messaging.impl.data.WorldPosData;
   
   public class UseItem extends OutgoingMessage
   {
      public var time_:int;
      
      public var slotObject_:SlotObjectData;
      
      public var itemUsePos_:WorldPosData;
      
      public function UseItem(id:uint, callback:Function)
      {
         this.slotObject_ = new SlotObjectData();
         this.itemUsePos_ = new WorldPosData();
         super(id,callback);
      }
      
      override public function writeToOutput(data:IDataOutput) : void
      {
         data.writeInt(this.time_);
         this.slotObject_.writeToOutput(data);
         this.itemUsePos_.writeToOutput(data);
      }
      
      override public function toString() : String
      {
         return formatToString("USEITEM","slotObject_","itemUsePos_","useType_");
      }
   }
}
