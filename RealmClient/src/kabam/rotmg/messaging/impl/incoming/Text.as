package kabam.rotmg.messaging.impl.incoming
{
   import flash.utils.IDataInput;
   
   public class Text extends IncomingMessage
   {
       
      
      public var name_:String;
      
      public var objectId_:int;
      
      public var numStars_:int;
      
      public var bubbleTime_:uint;
      
      public var recipient_:String;
      
      public var text_:String;
      
      public function Text(id:uint, callback:Function)
      {
         this.name_ = new String();
         this.text_ = new String();
         super(id,callback);
      }
      
      override public function parseFromInput(data:IDataInput) : void
      {
         this.name_ = data.readUTF();
         this.objectId_ = data.readInt();
         this.numStars_ = data.readInt();
         this.bubbleTime_ = data.readUnsignedByte();
         this.recipient_ = data.readUTF();
         this.text_ = data.readUTF();
      }
      
      override public function toString() : String
      {
         return formatToString("TEXT","name_","objectId_","numStars_","bubbleTime_","recipient_","text_");
      }
   }
}
