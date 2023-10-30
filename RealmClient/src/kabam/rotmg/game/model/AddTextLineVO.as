package kabam.rotmg.game.model
{
   public class AddTextLineVO
   {
       
      
      public var name:String;
      
      public var objectId:int;
      
      public var numStars:int;
      
      public var recipient:String;
      
      public var text:String;
      
      public function AddTextLineVO(name:String, text:String, objectId:int = -1, numStars:int = -1, recipient:String = "")
      {
         super();
         this.name = name;
         this.objectId = objectId;
         this.numStars = numStars;
         this.recipient = recipient;
         this.text = text;
      }
   }
}
