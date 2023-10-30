package kabam.rotmg.ui.model
{
   import com.company.assembleegameclient.game.GameSprite;
   import com.company.assembleegameclient.parameters.Parameters;
   
   public class HUDModel
   {
       
      
      public var gameSprite:GameSprite;
      
      public function HUDModel()
      {
         super();
      }
      
      public function getPlayerName() : String
      {
         return this.gameSprite.model.getName() || "Undefined";
      }
      
      public function getButtonType() : String
      {
         return this.gameSprite.gameId_ == Parameters.NEXUS_GAMEID?"OPTIONS_BUTTON":"NEXUS_BUTTON";
      }
   }
}
