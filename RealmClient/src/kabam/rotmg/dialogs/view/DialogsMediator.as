package kabam.rotmg.dialogs.view
{
   import flash.display.Sprite;
   import kabam.rotmg.dialogs.control.CloseDialogsSignal;
   import kabam.rotmg.dialogs.control.OpenDialogSignal;
   import kabam.rotmg.dialogs.control.ShowDialogBackgroundSignal;
   import robotlegs.bender.bundles.mvcs.Mediator;
   
   public class DialogsMediator extends Mediator
   {
       
      
      [Inject]
      public var view:DialogsView;
      
      [Inject]
      public var openDialog:OpenDialogSignal;
      
      [Inject]
      public var closeDialog:CloseDialogsSignal;
      
      [Inject]
      public var showDialogBackground:ShowDialogBackgroundSignal;
      
      public function DialogsMediator()
      {
         super();
      }
      
      override public function initialize() : void
      {
         this.showDialogBackground.add(this.onShowDialogBackground);
         this.openDialog.add(this.onOpenDialog);
         this.closeDialog.add(this.onCloseDialog);
      }
      
      override public function destroy() : void
      {
         this.showDialogBackground.remove(this.onShowDialogBackground);
         this.openDialog.remove(this.onOpenDialog);
         this.closeDialog.remove(this.onCloseDialog);
      }
      
      private function onShowDialogBackground(color:int = 1381653) : void
      {
         this.view.showBackground(color);
      }
      
      private function onOpenDialog(dialog:Sprite) : void
      {
         this.view.show(dialog);
      }
      
      private function onCloseDialog() : void
      {
         this.view.stage.focus = null;
         this.view.hideAll();
      }
   }
}
