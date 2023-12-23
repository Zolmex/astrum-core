package kabam.rotmg.messaging.impl.outgoing
{
   import flash.utils.IDataOutput;
   
   public class OptionsChanged extends OutgoingMessage
   {
      public var allyShots:Boolean;
      public var allyDamage:Boolean;
      public var allyNotifs:Boolean;
      public var allyParticles:Boolean;
      public var allyEntities:Boolean;
      
      public function OptionsChanged(id:uint, callback:Function)
      {
         super(id,callback);
      }
      
      override public function writeToOutput(data:IDataOutput) : void
      {
         data.writeBoolean(this.allyShots);
         data.writeBoolean(this.allyDamage);
         data.writeBoolean(this.allyNotifs);
         data.writeBoolean(this.allyParticles);
         data.writeBoolean(this.allyEntities);
      }
      
      override public function toString() : String
      {
         return formatToString("OPTIONS_CHANGED","accountListId_","add_","objectId_");
      }
   }
}
