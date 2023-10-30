package kabam.rotmg.account.core.view {
import flash.events.IEventDispatcher;

import org.osflash.signals.Signal;

public interface AccountInfoView extends IEventDispatcher {
      
    function setInfo(param1:String, param2:Boolean):void;
    function onReload(func:Function):void;
}
}
