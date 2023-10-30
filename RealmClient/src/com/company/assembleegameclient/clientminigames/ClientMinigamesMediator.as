package com.company.assembleegameclient.clientminigames

{
import com.company.assembleegameclient.clientminigames.ClientMinigamesView;
import com.company.assembleegameclient.clientminigames.uadapong.UadaPongView;
import com.company.assembleegameclient.clientminigames.zolmexclicker.ZolmexClickerView;

import kabam.rotmg.core.signals.SetScreenSignal;
import kabam.rotmg.legends.control.ExitLegendsSignal;
import kabam.rotmg.ui.view.TitleView;

import robotlegs.bender.bundles.mvcs.Mediator;

public class ClientMinigamesMediator extends Mediator
{
    [Inject]
    public var view:ClientMinigamesView;
    [Inject]
    public var setScreen:SetScreenSignal;

    public function ClientMinigamesMediator() {
        super();
    }

    override public function initialize():void {
        this.view.close.add(this.onClose);
        this.view.zolmexClicker.add(this.showZolmexClicker);
        this.view.uadaPong.add(this.showUadaPong);
    }

    override public function destroy():void {
        this.view.close.remove(this.onClose);
        this.view.zolmexClicker.remove(this.showZolmexClicker);
        this.view.uadaPong.remove(this.showUadaPong);
    }

    private function showZolmexClicker():void {
        this.setScreen.dispatch(new ZolmexClickerView());
    }

    private function showUadaPong():void {
        this.setScreen.dispatch(new UadaPongView());
    }

    private function onClose() : void {
        this.view.exit();
        this.setScreen.dispatch(new TitleView());
    }
}
}

