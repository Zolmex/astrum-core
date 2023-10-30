package kabam.rotmg.game.model
{
   public class UsePotionVO
   {
      
      public static var SHIFTCLICK:String = "shift_click";
      
      public static var CONTEXTBUY:String = "context_buy";
       
      
      public var objectId:int;
      
      public var source:String;
      
      public function UsePotionVO(objectId:int, source:String)
      {
         super();
         this.objectId = objectId;
         this.source = source;
      }
   }
}
