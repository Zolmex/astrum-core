package com.company.assembleegameclient.mapeditor

{
import com.company.assembleegameclient.clientminigames.uadapong.*;
import com.company.assembleegameclient.clientminigames.ClientMinigamesView;
import com.company.assembleegameclient.clientminigames.zolmexclicker.*;

import kabam.rotmg.core.signals.SetScreenSignal;
import kabam.rotmg.legends.control.ExitLegendsSignal;
import kabam.rotmg.ui.view.TitleView;

import robotlegs.bender.bundles.mvcs.Mediator;

public class EditingScreenMediator extends Mediator
{
    [Inject]
    public var view:EditingScreen;
    [Inject]
    public var setScreen:SetScreenSignal;

    public function EditingScreenMediator() {
        super();
    }

    override public function initialize() : void {
        this.view.close.add(this.onClose);
    }

    private function onClose() : void {
        this.setScreen.dispatch(new TitleView());
    }

    override public function destroy() : void {
        this.view.close.remove(this.onClose);
    }
}
}
