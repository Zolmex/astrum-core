package kabam.rotmg.dialogs.view
{
   import flash.display.DisplayObjectContainer;
   import flash.display.Graphics;
   import flash.display.Shape;
   import flash.display.Sprite;
   import flash.events.Event;
   
   public class DialogsView extends Sprite
   {
       
      
      private var background:Shape;
      
      private var container:DisplayObjectContainer;
      
      private var current:Sprite;
      
      public function DialogsView()
      {
         super();
         addChild(this.background = new Shape());
         addChild(this.container = new Sprite());
         this.background.visible = false;
      }
      
      public function showBackground(color:int = 1381653) : void
      {
         var g:Graphics = this.background.graphics;
         g.clear();
         g.beginFill(color,0.6);
         g.drawRect(0,0,800,600);
         g.endFill();
         this.background.visible = true;
      }
      
      public function show(dialog:Sprite) : void
      {
         this.removeCurrentDialog();
         this.addDialog(dialog);
      }
      
      public function hideAll() : void
      {
         this.background.visible = false;
         this.removeCurrentDialog();
      }
      
      private function addDialog(dialog:Sprite) : void
      {
         this.current = dialog;
         dialog.addEventListener(Event.REMOVED,this.onRemoved);
         this.container.addChild(dialog);
         this.showBackground();
      }
      
      private function onRemoved(event:Event) : void
      {
         var target:Sprite = event.target as Sprite;
         if(this.current == target)
         {
            this.background.visible = false;
            this.current = null;
         }
      }
      
      private function removeCurrentDialog() : void
      {
         if(this.current && this.container.contains(this.current))
         {
            this.current.removeEventListener(Event.REMOVED,this.onRemoved);
            this.container.removeChild(this.current);
            this.background.visible = false;
         }
      }
   }
}
