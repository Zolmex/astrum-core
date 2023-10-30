package com.company.assembleegameclient.screens
{
   import com.company.assembleegameclient.appengine.SavedCharactersList;
   import com.company.assembleegameclient.objects.ObjectLibrary;
   import com.company.rotmg.graphics.ScreenGraphic;
   import flash.display.Sprite;
   import flash.events.Event;
   import flash.events.MouseEvent;
   import kabam.rotmg.core.model.PlayerModel;
   import kabam.rotmg.game.view.CreditDisplay;
   import kabam.rotmg.ui.view.components.ScreenBase;
   import org.osflash.signals.Signal;
   
   public class NewCharacterScreen extends Sprite
   {
      private var backButton_:TitleMenuOption;
      private var creditDisplay_:CreditDisplay;
      private var boxes_:Object;
      public var tooltip:Signal;
      public var close:Signal;
      public var selected:Signal;
      
      private var isInitialized:Boolean = false;
      
      public function NewCharacterScreen()
      {
         this.boxes_ = {};
         super();
         this.tooltip = new Signal(Sprite);
         this.selected = new Signal(int);
         this.close = new Signal();
         addChild(new ScreenBase());
         addChild(new AccountScreen());
         addChild(new ScreenGraphic());
      }
      
      public function initialize(model:PlayerModel) : void
      {
         var playerXML:XML = null;
         var objectType:int = 0;
         var characterType:String = null;
         var charBox:CharacterBox = null;
         if(this.isInitialized)
         {
            return;
         }
         this.isInitialized = true;
         this.backButton_ = new TitleMenuOption("back",36,false);
         this.backButton_.addEventListener(MouseEvent.CLICK,this.onBackClick);
         addChild(this.backButton_);
         this.creditDisplay_ = new CreditDisplay();
         this.creditDisplay_.draw(model.getCredits(),model.getFame());
         addChild(this.creditDisplay_);
         for(var i:int = 0; i < ObjectLibrary.playerChars_.length; i++)
         {
            playerXML = ObjectLibrary.playerChars_[i];
            objectType = int(playerXML.@type);
            characterType = playerXML.@id;

            charBox = new CharacterBox(playerXML,model.getCharStats()[objectType],model);
            charBox.x = 50 + 140 * int(i % 5) + 70 - charBox.width / 2;
            charBox.y = 88 + 140 * int(i / 5);
            this.boxes_[objectType] = charBox;
            charBox.addEventListener(MouseEvent.ROLL_OVER,this.onCharBoxOver);
            charBox.addEventListener(MouseEvent.ROLL_OUT,this.onCharBoxOut);
            charBox.characterSelectClicked_.add(this.onCharBoxClick);
            addChild(charBox);
         }
         this.backButton_.x = stage.stageWidth / 2 - this.backButton_.width / 2;
         this.backButton_.y = 524;
         this.creditDisplay_.x = stage.stageWidth;
         this.creditDisplay_.y = 20;
      }
      
      private function onBackClick(event:Event) : void
      {
         this.close.dispatch();
      }
      
      private function onCharBoxOver(event:MouseEvent) : void
      {
         var charBox:CharacterBox = event.currentTarget as CharacterBox;
         charBox.setOver(true);
         this.tooltip.dispatch(charBox.getTooltip());
      }
      
      private function onCharBoxOut(event:MouseEvent) : void
      {
         var charBox:CharacterBox = event.currentTarget as CharacterBox;
         charBox.setOver(false);
         this.tooltip.dispatch(null);
      }
      
      private function onCharBoxClick(event:MouseEvent) : void
      {
         this.tooltip.dispatch(null);
         var charBox:CharacterBox = event.currentTarget.parent as CharacterBox;
         var objectType:int = charBox.objectType();
         var displayId:String = ObjectLibrary.typeToDisplayId_[objectType];
         this.selected.dispatch(objectType);
      }
      
      public function updateCreditsAndFame(credits:int, fame:int) : void
      {
         this.creditDisplay_.draw(credits,fame);
      }
   }
}
