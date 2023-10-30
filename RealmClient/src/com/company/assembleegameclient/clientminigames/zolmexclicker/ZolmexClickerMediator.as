package com.company.assembleegameclient.clientminigames.zolmexclicker

{
import com.company.assembleegameclient.clientminigames.ClientMinigamesView;

import kabam.rotmg.core.signals.SetScreenSignal;
import kabam.rotmg.legends.control.ExitLegendsSignal;
import kabam.rotmg.ui.view.TitleView;

import robotlegs.bender.bundles.mvcs.Mediator;

public class ZolmexClickerMediator extends Mediator
{
    [Inject]
    public var view:ZolmexClickerView;
    [Inject]
    public var setScreen:SetScreenSignal;

    public function ZolmexClickerMediator() {
        super();
    }

    override public function initialize() : void {
        this.view.close.add(this.onClose);
    }

    private function onClose() : void {
        this.view.exit();
        this.setScreen.dispatch(new ClientMinigamesView());
    }

    override public function destroy() : void {
        this.view.close.remove(this.onClose);
    }
}
}

