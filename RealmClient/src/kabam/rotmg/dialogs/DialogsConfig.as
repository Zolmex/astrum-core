package kabam.rotmg.dialogs
{
   import kabam.rotmg.dialogs.control.CloseDialogsSignal;
   import kabam.rotmg.dialogs.control.OpenDialogSignal;
   import kabam.rotmg.dialogs.control.ShowDialogBackgroundSignal;
   import kabam.rotmg.dialogs.view.DialogsMediator;
   import kabam.rotmg.dialogs.view.DialogsView;
   import org.swiftsuspenders.Injector;
   import robotlegs.bender.extensions.mediatorMap.api.IMediatorMap;
   import robotlegs.bender.framework.api.IConfig;
   
   public class DialogsConfig implements IConfig
   {
       
      
      [Inject]
      public var injector:Injector;
      
      [Inject]
      public var mediatorMap:IMediatorMap;
      
      public function DialogsConfig()
      {
         super();
      }
      
      public function configure() : void
      {
         this.injector.map(ShowDialogBackgroundSignal).asSingleton();
         this.injector.map(OpenDialogSignal).asSingleton();
         this.injector.map(CloseDialogsSignal).asSingleton();
         this.mediatorMap.map(DialogsView).toMediator(DialogsMediator);
      }
   }
}
