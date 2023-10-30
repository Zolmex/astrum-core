package kabam.rotmg.messaging.impl.incoming
{
   import com.company.assembleegameclient.util.FreeList;
   import flash.utils.IDataInput;
   import kabam.rotmg.messaging.impl.data.ObjectStatusData;
import kabam.rotmg.messaging.impl.data.StatData;

public class NewTick extends IncomingMessage
   {
      public var statuses_:Vector.<ObjectStatusData>;
      
      public function NewTick(id:uint, callback:Function)
      {
         this.statuses_ = new Vector.<ObjectStatusData>();
         super(id,callback);
      }
      
      override public function parseFromInput(data:IDataInput) : void
      {
         var i:int = 0;
         var len:int = data.readShort();
         for(i = 0; i < this.statuses_.length; i++)
         {
            FreeList.deleteObject(this.statuses_[i]);
         }
         this.statuses_.length = 0;
         while(this.statuses_.length < len)
         {
            this.statuses_.push(FreeList.newObject(ObjectStatusData) as ObjectStatusData);
            this.statuses_[this.statuses_.length - 1].parseFromInput(data);
         }
      }
      
      override public function toString() : String
      {
         return formatToString("NEW_TICK","statuses_", "playerStats_");
      }
   }
}
