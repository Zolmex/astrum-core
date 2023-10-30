package kabam.rotmg.messaging.impl.outgoing
{
   import flash.utils.ByteArray;
   import flash.utils.IDataOutput;
   
   public class Hello extends OutgoingMessage
   {
      public var buildVersion_:String;
      public var gameId_:int = 0;
      public var username_:String;
      public var password_:String;
      public var mapJSON_:String;
      
      public function Hello(id:uint, callback:Function)
      {
         this.buildVersion_ = new String();
         this.username_ = new String();
         this.password_ = new String();
         this.mapJSON_ = new String();
         super(id,callback);
      }
      
      override public function writeToOutput(data:IDataOutput) : void
      {
         data.writeUTF(this.buildVersion_);
         data.writeInt(this.gameId_);
         data.writeUTF(this.username_);
         data.writeUTF(this.password_);
         data.writeInt(this.mapJSON_.length);
         data.writeUTFBytes(this.mapJSON_);
      }
      
      override public function toString() : String
      {
         return formatToString("HELLO","buildVersion_","gameId_","username_","password_");
      }
   }
}
