package kabam.rotmg.classes.model
{
import com.company.assembleegameclient.parameters.Parameters;

import kabam.rotmg.assets.model.CharacterTemplate;
   import org.osflash.signals.Signal;
   
   public class CharacterSkin
   {
      public const changed:Signal = new Signal(CharacterSkin);
      
      public var id:int = 0;
      public var name:String = "";
      public var template:CharacterTemplate;
      public var cost:int = Parameters.CHARACTER_SKIN_PRICE;
      private var state:CharacterSkinState;
      private var isSelected:Boolean;
      
      public function CharacterSkin()
      {
         this.state = CharacterSkinState.NULL;
         super();
      }
      
      public function getIsSelected() : Boolean
      {
         return this.isSelected;
      }
      
      public function setIsSelected(value:Boolean) : void
      {
         if(this.isSelected != value)
         {
            this.isSelected = value;
            this.changed.dispatch(this);
         }
      }
      
      public function getState() : CharacterSkinState
      {
         return this.state;
      }
      
      public function setState(value:CharacterSkinState) : void
      {
         if(this.state != value)
         {
            this.state = value;
            this.changed.dispatch(this);
         }
      }
   }
}
