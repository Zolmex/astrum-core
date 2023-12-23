package kabam.rotmg.legends
{
import com.company.assembleegameclient.clientminigames.ClientMinigamesMediator;
import com.company.assembleegameclient.clientminigames.ClientMinigamesView;
import com.company.assembleegameclient.clientminigames.uadapong.UadaPongMediator;
import com.company.assembleegameclient.clientminigames.uadapong.UadaPongView;
import com.company.assembleegameclient.clientminigames.zolmexclicker.ZolmexClickerMediator;
import com.company.assembleegameclient.clientminigames.zolmexclicker.ZolmexClickerView;
import com.company.assembleegameclient.mapeditor.EditingScreen;
import com.company.assembleegameclient.mapeditor.EditingScreenMediator;

import kabam.rotmg.legends.control.ExitLegendsCommand;
   import kabam.rotmg.legends.control.ExitLegendsSignal;
   import kabam.rotmg.legends.control.FameListUpdateSignal;
   import kabam.rotmg.legends.control.RequestFameListCommand;
   import kabam.rotmg.legends.control.RequestFameListSignal;
   import kabam.rotmg.legends.model.LegendFactory;
   import kabam.rotmg.legends.model.LegendsModel;
   import kabam.rotmg.legends.view.LegendsMediator;
   import kabam.rotmg.legends.view.LegendsView;
   import org.swiftsuspenders.Injector;
   import robotlegs.bender.extensions.mediatorMap.api.IMediatorMap;
   import robotlegs.bender.extensions.signalCommandMap.api.ISignalCommandMap;
   import robotlegs.bender.framework.api.IConfig;
   
   public class LegendsConfig implements IConfig
   {
      [Inject]
      public var injector:Injector;
      
      [Inject]
      public var mediatorMap:IMediatorMap;
      
      [Inject]
      public var commandMap:ISignalCommandMap;
      
      public function LegendsConfig()
      {
         super();
      }
      
      public function configure() : void
      {
         this.injector.map(LegendFactory).asSingleton();
         this.injector.map(LegendsModel).asSingleton();
         this.injector.map(FameListUpdateSignal).asSingleton();
         this.mediatorMap.map(LegendsView).toMediator(LegendsMediator);
         this.mediatorMap.map(ZolmexClickerView).toMediator(ZolmexClickerMediator);
         this.mediatorMap.map(UadaPongView).toMediator(UadaPongMediator);
         this.mediatorMap.map(EditingScreen).toMediator(EditingScreenMediator);
         this.mediatorMap.map(ClientMinigamesView).toMediator(ClientMinigamesMediator);
         this.commandMap.map(RequestFameListSignal).toCommand(RequestFameListCommand);
         this.commandMap.map(ExitLegendsSignal).toCommand(ExitLegendsCommand);
      }
   }
}
