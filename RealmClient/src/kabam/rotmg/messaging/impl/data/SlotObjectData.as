package kabam.rotmg.messaging.impl.data
{
   import flash.utils.IDataInput;
   import flash.utils.IDataOutput;
   
   public class SlotObjectData
   {
      public var objectId_:int;
      
      public var slotId_:int;
      
      public function SlotObjectData()
      {
         super();
      }
      
      public function parseFromInput(data:IDataInput) : void
      {
         this.objectId_ = data.readInt();
         this.slotId_ = data.readUnsignedByte();
      }
      
      public function writeToOutput(data:IDataOutput) : void
      {
         data.writeInt(this.objectId_);
         data.writeByte(this.slotId_);
      }
      
      public function toString() : String
      {
         return "objectId_: " + this.objectId_ + " slotId_: " + this.slotId_;
      }
   }
}
