package com.company.assembleegameclient.ui.panels
{
   import com.company.assembleegameclient.game.GameSprite;
   import com.company.assembleegameclient.ui.TextButton;
   import com.company.ui.SimpleText;
   import flash.events.MouseEvent;
   import flash.filters.DropShadowFilter;
   import flash.text.TextFieldAutoSize;
   
   public class ButtonPanel extends Panel
   {
       
      
      private var titleText_:SimpleText;
      
      protected var button_:TextButton;
      
      public function ButtonPanel(gs:GameSprite, title:String, button:String)
      {
         super(gs);
         this.titleText_ = new SimpleText(18,16777215,false,WIDTH,0);
         this.titleText_.setBold(true);
         this.titleText_.htmlText = "<p align=\"center\">" + title + "</p>";
         this.titleText_.wordWrap = true;
         this.titleText_.multiline = true;
         this.titleText_.autoSize = TextFieldAutoSize.CENTER;
         this.titleText_.filters = [new DropShadowFilter(0,0,0)];
         this.titleText_.y = 6;
         addChild(this.titleText_);
         this.button_ = new TextButton(16,button);
         this.button_.addEventListener(MouseEvent.CLICK,this.onButtonClick);
         this.button_.x = WIDTH / 2 - this.button_.width / 2;
         this.button_.y = HEIGHT - this.button_.height - 4;
         addChild(this.button_);
      }
      
      protected function onButtonClick(event:MouseEvent) : void
      {
      }
   }
}
