package com.company.assembleegameclient.ui.tooltip
{
   import com.company.assembleegameclient.appengine.CharacterStats;
   import com.company.assembleegameclient.objects.ObjectLibrary;
   import com.company.assembleegameclient.objects.Player;
   import com.company.assembleegameclient.ui.GameObjectListItem;
   import com.company.assembleegameclient.ui.LineBreakDesign;
   import com.company.assembleegameclient.ui.StatusBar;
   import com.company.assembleegameclient.ui.panels.itemgrids.EquippedGrid;
   import com.company.assembleegameclient.ui.panels.itemgrids.InventoryGrid;
   import com.company.assembleegameclient.util.FameUtil;
   import com.company.ui.SimpleText;
   import flash.filters.DropShadowFilter;
   import kabam.rotmg.assets.services.CharacterFactory;
   import kabam.rotmg.classes.model.CharacterClass;
   import kabam.rotmg.classes.model.CharacterSkin;
   import kabam.rotmg.classes.model.ClassesModel;
   import kabam.rotmg.constants.GeneralConstants;
   import kabam.rotmg.core.StaticInjectorContext;
   
   public class MyPlayerToolTip extends ToolTip
   {
       
      
      private var factory:CharacterFactory;
      
      private var classes:ClassesModel;
      
      public var player_:Player;
      
      private var playerPanel_:GameObjectListItem;
      
      private var hpBar_:StatusBar;
      
      private var mpBar_:StatusBar;
      
      private var lineBreak_:LineBreakDesign;
      
      private var bestLevel_:SimpleText;
      
      private var nextClassQuest_:SimpleText;
      
      private var eGrid:EquippedGrid;
      
      private var iGrid:InventoryGrid;
      
      public function MyPlayerToolTip(accountName:String, charXML:XML, charStats:CharacterStats)
      {
         super(3552822,1,16777215,1);
         this.factory = StaticInjectorContext.getInjector().getInstance(CharacterFactory);
         this.classes = StaticInjectorContext.getInjector().getInstance(ClassesModel);
         var objectType:int = int(charXML.ObjectType);
         var playerXML:XML = ObjectLibrary.xmlLibrary_[objectType];
         this.player_ = Player.fromPlayerXML(accountName,charXML);
         var char:CharacterClass = this.classes.getCharacterClass(this.player_.objectType_);
         var skin:CharacterSkin = char.skins.getSkin(charXML.Texture);
         this.player_.animatedChar_ = this.factory.makeCharacter(skin.template);
         this.playerPanel_ = new GameObjectListItem(11776947,true,this.player_);
         addChild(this.playerPanel_);
         this.hpBar_ = new StatusBar(176,16,14693428,5526612,"HP");
         this.hpBar_.x = 6;
         this.hpBar_.y = 40;
         addChild(this.hpBar_);
         this.mpBar_ = new StatusBar(176,16,6325472,5526612,"MP");
         this.mpBar_.x = 6;
         this.mpBar_.y = 64;
         addChild(this.mpBar_);
         this.eGrid = new EquippedGrid(null,this.player_.slotTypes_,this.player_);
         this.eGrid.x = 8;
         this.eGrid.y = 88;
         addChild(this.eGrid);
         this.eGrid.setItems(this.player_.equipment_);
         this.iGrid = new InventoryGrid(null,this.player_,GeneralConstants.NUM_EQUIPMENT_SLOTS);
         this.iGrid.x = 8;
         this.iGrid.y = 132;
         addChild(this.iGrid);
         this.iGrid.setItems(this.player_.equipment_);
         this.lineBreak_ = new LineBreakDesign(100,1842204);
         this.lineBreak_.x = 6;
         this.lineBreak_.y = 228;
         addChild(this.lineBreak_);
         var numStars:int = charStats == null?int(0):int(charStats.numStars());
         this.bestLevel_ = new SimpleText(14,6206769,false,0,0);
         this.bestLevel_.text = numStars + " of 5 Class Quests Completed\n" + "Best Level Achieved: " + (charStats != null?charStats.bestLevel():0).toString() + "\n" + "Best Fame Achieved: " + (charStats != null?charStats.bestFame():0).toString();
         this.bestLevel_.updateMetrics();
         this.bestLevel_.filters = [new DropShadowFilter(0,0,0)];
         this.bestLevel_.x = 8;
         this.bestLevel_.y = height - 2;
         addChild(this.bestLevel_);
         var nextStarFame:int = FameUtil.nextStarFame(charStats == null?int(0):int(charStats.bestFame()),0);
         if(nextStarFame > 0)
         {
            this.nextClassQuest_ = new SimpleText(13,16549442,false,174,0);
            this.nextClassQuest_.text = "Next Goal: Earn " + nextStarFame + " Fame\n" + "  with a " + playerXML.@id;
            this.nextClassQuest_.updateMetrics();
            this.nextClassQuest_.filters = [new DropShadowFilter(0,0,0)];
            this.nextClassQuest_.x = 8;
            this.nextClassQuest_.y = height - 2;
            addChild(this.nextClassQuest_);
         }
      }
      
      override public function draw() : void
      {
         this.hpBar_.draw(this.player_.hp_,this.player_.maxHP_,this.player_.maxHPBoost_,this.player_.maxHPMax_);
         this.mpBar_.draw(this.player_.mp_,this.player_.maxMP_,this.player_.maxMPBoost_,this.player_.maxMPMax_);
         this.lineBreak_.setWidthColor(width - 10,1842204);
         super.draw();
      }
   }
}
