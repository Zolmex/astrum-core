package kabam.rotmg.messaging.impl.incoming
{
   import flash.utils.IDataInput;
   import kabam.rotmg.messaging.impl.data.WorldPosData;
   
   public class Goto extends IncomingMessage
   {
       
      
      public var pos_:WorldPosData;
      
      public function Goto(id:uint, callback:Function)
      {
         this.pos_ = new WorldPosData();
         super(id,callback);
      }
      
      override public function parseFromInput(data:IDataInput) : void
      {
         this.pos_.parseFromInput(data);
      }
      
      override public function toString() : String
      {
         return formatToString("GOTO","pos_");
      }
   }
}
