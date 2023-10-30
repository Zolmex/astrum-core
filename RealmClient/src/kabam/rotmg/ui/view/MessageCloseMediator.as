package kabam.rotmg.ui.view
{
   import kabam.rotmg.account.core.Account;
   import kabam.rotmg.dialogs.control.CloseDialogsSignal;
   import robotlegs.bender.bundles.mvcs.Mediator;
   
   public class MessageCloseMediator extends Mediator
   {
      [Inject]
      public var account:Account;
      
      [Inject]
      public var view:MessageCloseDialog;
      
      [Inject]
      public var closeDialogs:CloseDialogsSignal;
      
      public function MessageCloseMediator()
      {
         super();
      }
      
      override public function initialize() : void
      {
         this.view.cancel.add(this.onCancel);
      }
      
      override public function destroy() : void
      {
         this.view.cancel.remove(this.onCancel);
      }
      
      public function onCancel() : void
      {
         this.closeDialogs.dispatch();
      }
   }
}
