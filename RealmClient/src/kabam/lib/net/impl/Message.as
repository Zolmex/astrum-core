package kabam.lib.net.impl
{
   import flash.utils.IDataInput;
   import flash.utils.IDataOutput;
   
   public class Message
   {
       
      
      public var pool:MessagePool;
      
      public var prev:Message;
      
      public var next:Message;
      
      private var isCallback:Boolean;
      
      public var id:uint;
      
      public var callback:Function;
      
      public function Message(id:uint, callback:Function = null)
      {
         super();
         this.id = id;
         this.isCallback = callback != null;
         this.callback = callback;
      }
      
      public function parseFromInput(data:IDataInput) : void
      {
      }
      
      public function writeToOutput(data:IDataOutput) : void
      {
      }
      
      public function toString() : String
      {
         return this.formatToString("MESSAGE","id");
      }
      
      protected function formatToString(name:String, ... args) : String
      {
         var str:String = "[" + name;
         for(var i:int = 0; i < args.length; i++)
         {
            str = str + (" " + args[i] + "=\"" + this[args[i]] + "\"");
         }
         str = str + "]";
         return str;
      }
      
      public function consume() : void
      {
         this.isCallback && this.callback(this);
         this.prev = null;
         this.next = null;
         this.pool.append(this);
      }
   }
}
