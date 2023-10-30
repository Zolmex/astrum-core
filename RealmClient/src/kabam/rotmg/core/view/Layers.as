package kabam.rotmg.core.view
{
   import flash.display.DisplayObject;
   import flash.display.DisplayObjectContainer;
   import flash.display.Sprite;
   import kabam.rotmg.dialogs.view.DialogsView;
   import kabam.rotmg.tooltips.view.TooltipsView;
   
   public class Layers extends Sprite
   {
       
      
      private var menu:ScreensView;
      
      public var overlay:DisplayObjectContainer;
      
      private var tooltips:TooltipsView;
      
      public var top:DisplayObjectContainer;
      
      private var dialogs:DialogsView;
      
      public var api:DisplayObjectContainer;
      
      private var console:DisplayObject;
      
      public function Layers()
      {
         super();
         addChild(this.menu = new ScreensView());
         addChild(this.overlay = new Sprite());
         addChild(this.top = new Sprite());
         addChild(this.tooltips = new TooltipsView());
         addChild(this.dialogs = new DialogsView());
         addChild(this.api = new Sprite());
      }
   }
}
