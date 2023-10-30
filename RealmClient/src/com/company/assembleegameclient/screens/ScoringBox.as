package com.company.assembleegameclient.screens
{
   import com.company.assembleegameclient.ui.Scrollbar;
   import com.company.assembleegameclient.util.FameUtil;
   import com.company.util.BitmapUtil;
   import flash.display.Bitmap;
   import flash.display.BitmapData;
   import flash.display.DisplayObject;
   import flash.display.Shape;
   import flash.display.Sprite;
   import flash.events.Event;
   import flash.geom.Rectangle;
   import flash.utils.getTimer;
   
   public class ScoringBox extends Sprite
   {
       
      
      private var rect_:Rectangle;
      
      private var mask_:Shape;
      
      private var linesSprite_:Sprite;
      
      private var scoreTextLines_:Vector.<ScoreTextLine>;
      
      private var scrollbar_:Scrollbar;
      
      private var startTime_:int;
      
      public function ScoringBox(rect:Rectangle, fameXML:XML)
      {
         var bonusXML:XML = null;
         this.mask_ = new Shape();
         this.linesSprite_ = new Sprite();
         this.scoreTextLines_ = new Vector.<ScoreTextLine>();
         super();
         this.rect_ = rect;
         graphics.lineStyle(1,4802889,2);
         graphics.drawRect(this.rect_.x + 1,this.rect_.y + 1,this.rect_.width - 2,this.rect_.height - 2);
         graphics.lineStyle();
         this.scrollbar_ = new Scrollbar(16,this.rect_.height);
         this.scrollbar_.addEventListener(Event.CHANGE,this.onScroll);
         this.mask_.graphics.beginFill(16777215,1);
         this.mask_.graphics.drawRect(this.rect_.x,this.rect_.y,this.rect_.width,this.rect_.height);
         this.mask_.graphics.endFill();
         addChild(this.mask_);
         mask = this.mask_;
         addChild(this.linesSprite_);
         this.addLine("Minutes Active",null,fameXML.MinutesActive,5746018);
         this.addLine("Shots",null,fameXML.Shots,5746018);
         this.addLine("Accuracy",null,100 * Number(fameXML.ShotsThatDamage) / Number(fameXML.Shots),5746018,"","%");
         this.addLine("Abilities Used",null,fameXML.AbilitiesUsed,5746018);
         this.addLine("Tiles Seen",null,fameXML.TilesUncovered,5746018);
         this.addLine("Monster Kills",null,fameXML.MonsterKills,5746018);
         this.addLine("God Kills",null,fameXML.GodKills,5746018);
         this.addLine("Oryx Kills",null,fameXML.OryxKills,5746018);
         this.addLine("Cube Kills",null,fameXML.CubeKills,5746018);
         this.addLine("Cyan Bags",null,fameXML.CyanBags,5746018);
         this.addLine("Blue Bags",null,fameXML.BlueBags,5746018);
         this.addLine("White Bags",null,fameXML.WhiteBags,5746018);
         this.addLine("Damage Taken",null,fameXML.DamageTaken,5746018);
         this.addLine("Damage Dealt",null,fameXML.DamageDealt,5746018);
         this.addLine("Teleports",null,fameXML.Teleports,5746018);
         this.addLine("Potions Drank",null,fameXML.PotionsDrank,5746018);
         this.addLine("Quests Completed",null,fameXML.QuestsCompleted,5746018);
         this.addLine("Dungeons Completed",null,int(fameXML.PirateCavesCompleted) + int(fameXML.UndeadLairsCompleted) + int(fameXML.AbyssOfDemonsCompleted) + int(fameXML.SnakePitsCompleted) + int(fameXML.SpiderDensCompleted) + int(fameXML.SpriteWorldsCompleted) + int(fameXML.TombsCompleted),5746018);
         this.addLine("Escapes", null, int(fameXML.Escapes), 5746018);
         this.addLine("Near Death Escapes", null, int(fameXML.NearDeathEscapes), 5746018);
         this.addLine("Party Member Level Ups",null,fameXML.LevelUpAssists,5746018);
         var fameBD:BitmapData = FameUtil.getFameIcon();
         fameBD = BitmapUtil.cropToBitmapData(fameBD,6,6,fameBD.width - 12,fameBD.height - 12);
         this.addLine("Base Fame Earned",null,fameXML.BaseFame,16762880,"","",new Bitmap(fameBD));
         for each(bonusXML in fameXML.Bonus)
         {
            this.addLine("Bonus: " + bonusXML.@id,null,int(bonusXML),16762880,"+","",new Bitmap(fameBD));
         }
      }
      
      public function showScore() : void
      {
         var textLine:ScoreTextLine = null;
         this.animateScore();
         this.startTime_ = -int.MAX_VALUE;
         for each(textLine in this.scoreTextLines_)
         {
            textLine.skip();
         }
      }
      
      public function animateScore() : void
      {
         this.startTime_ = getTimer();
         addEventListener(Event.ENTER_FRAME,this.onEnterFrame);
      }
      
      private function onScroll(event:Event) : void
      {
         var v:Number = this.scrollbar_.pos();
         this.linesSprite_.y = v * (this.rect_.height - this.linesSprite_.height - 15) + 5;
      }
      
      private function addLine(name:String, desc:String, val:int, color:uint, numberPrefix:String = "", unit:String = "", unitIcon:DisplayObject = null) : void
      {
         this.scoreTextLines_.push(new ScoreTextLine(20,11776947,color,name,desc,val,numberPrefix,unit,unitIcon));
      }
      
      private function onEnterFrame(event:Event) : void
      {
         var stl:ScoreTextLine = null;
         var endTime:Number = this.startTime_ + 1000 * (this.scoreTextLines_.length - 1) / 2;
         var time:Number = getTimer();
         var maxI:int = Math.min(this.scoreTextLines_.length,2 * (getTimer() - this.startTime_) / 1000 + 1);
         for(var i:int = 0; i < maxI; i++)
         {
            stl = this.scoreTextLines_[i];
            stl.y = 28 * i;
            this.linesSprite_.addChild(stl);
         }
         this.linesSprite_.y = this.rect_.height - this.linesSprite_.height - 10;
         if(time > endTime + 600)
         {
            this.addScrollbar();
            dispatchEvent(new Event(Event.COMPLETE));
            removeEventListener(Event.ENTER_FRAME,this.onEnterFrame);
         }
      }
      
      private function addScrollbar() : void
      {
         graphics.clear();
         graphics.lineStyle(1,4802889,2);
         graphics.drawRect(this.rect_.x + 1,this.rect_.y + 1,this.rect_.width - 26,this.rect_.height - 2);
         graphics.lineStyle();
         this.scrollbar_.x = this.rect_.width - 16;
         this.scrollbar_.setIndicatorSize(this.mask_.height,this.linesSprite_.height);
         this.scrollbar_.setPos(1);
         addChild(this.scrollbar_);
      }
   }
}
