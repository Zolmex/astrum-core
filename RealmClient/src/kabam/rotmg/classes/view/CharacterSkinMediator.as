package kabam.rotmg.classes.view {
import com.company.assembleegameclient.screens.NewCharacterScreen;
import com.company.assembleegameclient.ui.dialogs.Dialog;

import flash.events.Event;

import kabam.rotmg.core.model.PlayerModel;
import kabam.rotmg.core.signals.SetScreenSignal;
import kabam.rotmg.dialogs.control.CloseDialogsSignal;
import kabam.rotmg.dialogs.control.OpenDialogSignal;
import kabam.rotmg.game.model.GameInitData;
import kabam.rotmg.game.signals.PlayGameSignal;
import kabam.rotmg.servers.api.ServerModel;
import kabam.rotmg.ui.view.NoServersDialogFactory;

import robotlegs.bender.bundles.mvcs.Mediator;

public class CharacterSkinMediator extends Mediator {

    [Inject]
    public var view:CharacterSkinView;

    [Inject]
    public var model:PlayerModel;

    [Inject]
    public var setScreen:SetScreenSignal;

    [Inject]
    public var play:PlayGameSignal;

    [Inject]
    public var openDialog:OpenDialogSignal;

    [Inject]
    public var servers:ServerModel;

    [Inject]
    public var closeDialog:CloseDialogsSignal;

    [Inject]
    public var noServersDialogFactory:NoServersDialogFactory;

    public function CharacterSkinMediator() {
        super();
    }

    override public function initialize():void {
        var hasSlot:Boolean = this.model.hasAvailableCharSlot();
        this.view.setPlayButtonEnabled(hasSlot);
        if (hasSlot) {
            this.view.play.addOnce(this.onPlay);
        }
        this.view.back.addOnce(this.onBack);
    }

    override public function destroy():void {
        this.view.back.remove(this.onBack);
        this.view.play.remove(this.onPlay);
    }

    private function onBack():void {
        this.setScreen.dispatch(new NewCharacterScreen());
    }

    private function onPlay():void {
        if (!this.servers.isServerAvailable()){
            this.showNoServers();
            return;
        }
        var game:GameInitData = new GameInitData();
        game.createCharacter = true;
        game.charId = this.model.getNextCharId();
        game.isNewGame = true;
        this.play.dispatch(game);
    }

    private function showNoServers():void{
        var dialog:Dialog = this.noServersDialogFactory.makeDialog();
        dialog.addEventListener(Dialog.BUTTON1_EVENT, this.onOk);
        this.openDialog.dispatch(dialog);
    }

    private function onOk(e:Event):void{
        this.closeDialog.dispatch();
    }
}
}
