package com.company.assembleegameclient.ui.panels
{
   import com.company.assembleegameclient.game.GameSprite;
   import com.company.assembleegameclient.ui.guild.GuildChronicleScreen;
   import flash.events.MouseEvent;
   
   public class GuildChroniclePanel extends ButtonPanel
   {
       
      
      public function GuildChroniclePanel(gs:GameSprite)
      {
         super(gs,"Guild Chronicle","View");
      }
      
      override protected function onButtonClick(event:MouseEvent) : void
      {
         gs_.mui_.clearInput();
         gs_.addChild(new GuildChronicleScreen(gs_));
      }
   }
}
