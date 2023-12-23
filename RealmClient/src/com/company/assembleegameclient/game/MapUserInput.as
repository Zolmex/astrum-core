package com.company.assembleegameclient.game {
import com.company.assembleegameclient.itemData.ItemData;
import com.company.assembleegameclient.map.Square;
import com.company.assembleegameclient.objects.ObjectLibrary;
import com.company.assembleegameclient.objects.Player;
import com.company.assembleegameclient.parameters.Parameters;
import com.company.assembleegameclient.ui.options.Options;
import com.company.util.KeyCodes;

import flash.display.DisplayObject;

import flash.display.Stage;
import flash.display.StageDisplayState;
import flash.events.Event;
import flash.events.KeyboardEvent;
import flash.events.MouseEvent;
import flash.events.TimerEvent;
import flash.geom.Point;
import flash.system.Capabilities;
import flash.utils.Timer;

import kabam.rotmg.constants.GeneralConstants;
import kabam.rotmg.core.StaticInjectorContext;
import kabam.rotmg.core.view.Layers;
import kabam.rotmg.game.model.PotionInventoryModel;
import kabam.rotmg.game.model.UsePotionVO;
import kabam.rotmg.game.signals.AddTextLineSignal;
import kabam.rotmg.game.signals.SetTextBoxVisibilitySignal;
import kabam.rotmg.game.signals.UsePotionSignal;
import kabam.rotmg.messaging.impl.GameServerConnection;
import kabam.rotmg.minimap.control.MiniMapZoomSignal;
import kabam.rotmg.ui.model.TabStripModel;
import kabam.rotmg.ui.signals.StatsTabHotKeyInputSignal;

import net.hires.debug.Stats;

import org.swiftsuspenders.Injector;

public class MapUserInput {
    private static var stats_:Stats = new Stats();

    public var gs_:GameSprite;
    private var moveLeft_:Boolean = false;
    private var moveRight_:Boolean = false;
    private var moveUp_:Boolean = false;
    private var moveDown_:Boolean = false;
    private var rotateLeft_:Boolean = false;
    private var rotateRight_:Boolean = false;
    private var mouseDown_:Boolean = false;
    private var autofire_:Boolean = false;
    private var specialKeyDown_:Boolean = false;
    private var enablePlayerInput_:Boolean = true;
    private var addTextLine:AddTextLineSignal;
    private var setTextBoxVisibility:SetTextBoxVisibilitySignal;
    private var statsTabHotKeyInputSignal:StatsTabHotKeyInputSignal;
    private var miniMapZoom:MiniMapZoomSignal;
    private var usePotionSignal:UsePotionSignal;
    private var potionInventoryModel:PotionInventoryModel;
    private var tabStripModel:TabStripModel;
    public var layers:Layers;

    public function MapUserInput(gs:GameSprite) {
        super();
        this.gs_ = gs;
        //this.mouseDownTimer = new Timer(MOUSE_DOWN_WAIT_PERIOD,1);
        //this.mouseDownTimer.addEventListener(TimerEvent.TIMER_COMPLETE,this.onMouseDownWaitPeriodOver);
        this.gs_.addEventListener(Event.ADDED_TO_STAGE, this.onAddedToStage);
        this.gs_.addEventListener(Event.REMOVED_FROM_STAGE, this.onRemovedFromStage);
        var injector:Injector = StaticInjectorContext.getInjector();
        this.addTextLine = injector.getInstance(AddTextLineSignal);
        this.setTextBoxVisibility = injector.getInstance(SetTextBoxVisibilitySignal);
        this.statsTabHotKeyInputSignal = injector.getInstance(StatsTabHotKeyInputSignal);
        this.miniMapZoom = injector.getInstance(MiniMapZoomSignal);
        this.usePotionSignal = injector.getInstance(UsePotionSignal);
        this.potionInventoryModel = injector.getInstance(PotionInventoryModel);
        this.tabStripModel = injector.getInstance(TabStripModel);
        this.layers = injector.getInstance(Layers);
        this.gs_.map.signalRenderSwitch.add(this.onRenderSwitch);
    }

    public function clearInput():void {
        this.moveLeft_ = false;
        this.moveRight_ = false;
        this.moveUp_ = false;
        this.moveDown_ = false;
        this.rotateLeft_ = false;
        this.rotateRight_ = false;
        this.mouseDown_ = false;
        this.autofire_ = false;
        this.setPlayerMovement();
    }

    public function onRenderSwitch(wasLastGpu:Boolean):void {
        if (wasLastGpu) {
            this.gs_.stage.removeEventListener(MouseEvent.MOUSE_DOWN, this.onMouseDown);
            this.gs_.stage.removeEventListener(MouseEvent.MOUSE_UP, this.onMouseUp);
            this.gs_.map.addEventListener(MouseEvent.MOUSE_DOWN, this.onMouseDown);
            this.gs_.map.addEventListener(MouseEvent.MOUSE_UP, this.onMouseUp);
        }
        else {
            this.gs_.map.removeEventListener(MouseEvent.MOUSE_DOWN, this.onMouseDown);
            this.gs_.map.removeEventListener(MouseEvent.MOUSE_UP, this.onMouseUp);
            this.gs_.stage.addEventListener(MouseEvent.MOUSE_DOWN, this.onMouseDown);
            this.gs_.stage.addEventListener(MouseEvent.MOUSE_UP, this.onMouseUp);
        }
    }

    public function setEnablePlayerInput(enable:Boolean):void {
        if (this.enablePlayerInput_ != enable) {
            this.enablePlayerInput_ = enable;
            this.clearInput();
        }
    }

    private function onAddedToStage(event:Event):void {
        var stage:Stage = this.gs_.stage;
        stage.addEventListener(Event.ACTIVATE, this.onActivate);
        stage.addEventListener(Event.DEACTIVATE, this.onDeactivate);
        stage.addEventListener(KeyboardEvent.KEY_DOWN, this.onKeyDown);
        stage.addEventListener(KeyboardEvent.KEY_UP, this.onKeyUp);
        stage.addEventListener(MouseEvent.MOUSE_WHEEL, this.onMouseWheel);
        if (Parameters.GPURenderFrame) {
            stage.addEventListener(MouseEvent.MOUSE_DOWN, this.onMouseDown);
            stage.addEventListener(MouseEvent.MOUSE_UP, this.onMouseUp);
        }
        else {
            this.gs_.map.addEventListener(MouseEvent.MOUSE_DOWN, this.onMouseDown);
            this.gs_.map.addEventListener(MouseEvent.MOUSE_UP, this.onMouseUp);
        }
        stage.addEventListener(Event.ENTER_FRAME, this.onEnterFrame);
        //stage.addEventListener(MouseEvent.RIGHT_CLICK,this.disableRightClick);
    }

    private function onRemovedFromStage(event:Event):void {
        var stage:Stage = this.gs_.stage;
        stage.removeEventListener(Event.ACTIVATE, this.onActivate);
        stage.removeEventListener(Event.DEACTIVATE, this.onDeactivate);
        stage.removeEventListener(KeyboardEvent.KEY_DOWN, this.onKeyDown);
        stage.removeEventListener(KeyboardEvent.KEY_UP, this.onKeyUp);
        stage.removeEventListener(MouseEvent.MOUSE_WHEEL, this.onMouseWheel);
        if (Parameters.GPURenderFrame) {
            stage.removeEventListener(MouseEvent.MOUSE_DOWN, this.onMouseDown);
            stage.removeEventListener(MouseEvent.MOUSE_UP, this.onMouseUp);
        }
        else {
            this.gs_.map.removeEventListener(MouseEvent.MOUSE_DOWN, this.onMouseDown);
            this.gs_.map.removeEventListener(MouseEvent.MOUSE_UP, this.onMouseUp);
        }
        stage.removeEventListener(Event.ENTER_FRAME, this.onEnterFrame);
        //stage.removeEventListener(MouseEvent.RIGHT_CLICK,this.disableRightClick);
    }

    private function onActivate(event:Event):void {
    }

    private function onDeactivate(event:Event):void {
        this.clearInput();
    }

    private function onMouseDown(event:MouseEvent):void {
        var mouseX:Number = NaN;
        var mouseY:Number = NaN;
        var angle:Number = NaN;
        var player:Player = this.gs_.map.player_;
        if (player == null)
            return;
        if (!this.enablePlayerInput_) {
            return;
        }
        if (event.shiftKey) {
            var item:ItemData = player.equipment_[1];
            if (item == null || item.MpEndCost > 0)
                return;

            mouseX = this.gs_.map.mouseX;
            mouseY = this.gs_.map.mouseY;
            if (Parameters.GPURenderFrame) {
                if (event.currentTarget == event.target || event.target == this.gs_.map || event.target == this.gs_) {
                    player.useAltWeapon(mouseX, mouseY);
                }
            }
            else {
                player.useAltWeapon(mouseX, mouseY);
            }
            return;
        }

        if (Parameters.GPURenderFrame) {
            if (event.currentTarget == event.target || event.target == this.gs_.map || event.target == this.gs_) {
                angle = Math.atan2(this.gs_.map.mouseY, this.gs_.map.mouseX);
            }
            else {
                return;
            }
        }
        else {
            angle = Math.atan2(this.gs_.map.mouseY, this.gs_.map.mouseX);
        }
        player.attemptAttackAngle(angle);
        this.mouseDown_ = true;
    }

    private function onMouseUp(event:MouseEvent):void {
        this.mouseDown_ = false;
    }

    private function onMouseWheel(event:MouseEvent) : void {
        var stage:DisplayObject = WebMain.STAGE;
        if (event.ctrlKey) {
            if(event.delta > 0) {
                Parameters.data_.mscale = Math.min(Parameters.data_.mscale + 0.1, 5);
                Parameters.save();
                stage.dispatchEvent(new Event(Event.RESIZE));
            }
            else {
                Parameters.data_.mscale = Math.max(Parameters.data_.mscale - 0.1, 0.5);
                Parameters.save();
                stage.dispatchEvent(new Event(Event.RESIZE));
            }
            return;
        }
        if(event.delta > 0) {
            this.miniMapZoom.dispatch(MiniMapZoomSignal.IN);
        }
        else {
            this.miniMapZoom.dispatch(MiniMapZoomSignal.OUT);
        }
    }

    private function onEnterFrame(event:Event):void {
        var angle:Number = NaN;
        var player:Player = null;
        if (this.enablePlayerInput_ && (this.mouseDown_ || this.autofire_)) {
            angle = Math.atan2(this.gs_.map.mouseY, this.gs_.map.mouseX);
            player = this.gs_.map.player_;
            if (player != null) {
                player.attemptAttackAngle(angle);
            }
        }
    }

    private function onKeyDown(event:KeyboardEvent):void {
        var success:Boolean = false;
        var square:Square = null;
        var stage:Stage = this.gs_.stage;
        switch (event.keyCode) {
            case KeyCodes.F1:
            case KeyCodes.F2:
            case KeyCodes.F3:
            case KeyCodes.F4:
            case KeyCodes.F5:
            case KeyCodes.F6:
            case KeyCodes.F7:
            case KeyCodes.F8:
            case KeyCodes.F9:
            case KeyCodes.F10:
            case KeyCodes.F11:
            case KeyCodes.F12:
            case KeyCodes.INSERT:
                break;
            default:
                if (stage.focus != null) {
                    return;
                }
                break;
        }
        var player:Player = this.gs_.map.player_;
        if (player == null) {
            return;
        }
        switch (event.keyCode) {
            case Parameters.data_.moveUp:
                this.moveUp_ = true;
                break;
            case Parameters.data_.moveDown:
                this.moveDown_ = true;
                break;
            case Parameters.data_.moveLeft:
                this.moveLeft_ = true;
                break;
            case Parameters.data_.moveRight:
                this.moveRight_ = true;
                break;
            case Parameters.data_.rotateLeft:
                if (!Parameters.data_.allowRotation) {
                    break;
                }
                this.rotateLeft_ = true;
                break;
            case Parameters.data_.rotateRight:
                if (!Parameters.data_.allowRotation) {
                    break;
                }
                this.rotateRight_ = true;
                break;
            case Parameters.data_.resetToDefaultCameraAngle:
                Parameters.data_.cameraAngle = Parameters.data_.defaultCameraAngle;
                Parameters.save();
                break;
            case Parameters.data_.useSpecial:
                if (!this.specialKeyDown_) {
                    success = player.useAltWeapon(this.gs_.map.mouseX, this.gs_.map.mouseY);
                    this.specialKeyDown_ = success;
                }
                break;
            case Parameters.data_.autofireToggle:
                this.autofire_ = !this.autofire_;
                break;
            case Parameters.data_.useEquipInvSlot1:
                this.useEquipItem(4);
                break;
            case Parameters.data_.useEquipInvSlot2:
                this.useEquipItem(5);
                break;
            case Parameters.data_.useEquipInvSlot3:
                this.useEquipItem(6);
                break;
            case Parameters.data_.useEquipInvSlot4:
                this.useEquipItem(7);
                break;
            case Parameters.data_.useEquipInvSlot5:
                this.useEquipItem(8);
                break;
            case Parameters.data_.useEquipInvSlot6:
                this.useEquipItem(9);
                break;
            case Parameters.data_.useEquipInvSlot7:
                this.useEquipItem(10);
                break;
            case Parameters.data_.useEquipInvSlot8:
                this.useEquipItem(11);
                break;
            case Parameters.data_.useHealthPotion:
                if (this.potionInventoryModel.getPotionModel(PotionInventoryModel.HEALTH_POTION_ID).available) {
                    this.usePotionSignal.dispatch(new UsePotionVO(PotionInventoryModel.HEALTH_POTION_ID, UsePotionVO.CONTEXTBUY));
                }
                break;
            case Parameters.data_.useMagicPotion:
                if (this.potionInventoryModel.getPotionModel(PotionInventoryModel.MAGIC_POTION_ID).available) {
                    this.usePotionSignal.dispatch(new UsePotionVO(PotionInventoryModel.MAGIC_POTION_ID, UsePotionVO.CONTEXTBUY));
                }
                break;
            case Parameters.data_.miniMapZoomOut:
                this.miniMapZoom.dispatch(MiniMapZoomSignal.OUT);
                break;
            case Parameters.data_.miniMapZoomIn:
                this.miniMapZoom.dispatch(MiniMapZoomSignal.IN);
                break;
            case Parameters.data_.togglePerformanceStats:
                this.togglePerformanceStats();
                break;
            case Parameters.data_.escapeToNexus:
            case Parameters.data_.escapeToNexus2:
                this.gs_.gsc_.escape();
                break;
            case Parameters.data_.options:
                this.clearInput();
                this.layers.overlay.addChild(new Options(this.gs_));
                break;
            case Parameters.data_.toggleCentering:
                Parameters.data_.centerOnPlayer = !Parameters.data_.centerOnPlayer;
                Parameters.save();
                break;
            case Parameters.data_.switchTabs:
                this.statsTabHotKeyInputSignal.dispatch();
                break;
            case Parameters.data_.toggleFullscreenMode:
                //doing it like this is better because then there's no additional memory allocated to the variable, and it works if say you go fullscreen, then press ESC, and go fullscreen again.
                //before if you did that, you'd need to press the key twice because the variable was not updated
                this.gs_.stage.displayState = this.gs_.stage.displayState == StageDisplayState.NORMAL ? StageDisplayState.FULL_SCREEN_INTERACTIVE : StageDisplayState.NORMAL;
                break;
            case Parameters.data_.crouchKey:
                player.crouching = true;
                break;
        }
        this.setPlayerMovement();
    }

    private function onKeyUp(event:KeyboardEvent):void {
        switch (event.keyCode) {
            case Parameters.data_.moveUp:
                this.moveUp_ = false;
                break;
            case Parameters.data_.moveDown:
                this.moveDown_ = false;
                break;
            case Parameters.data_.moveLeft:
                this.moveLeft_ = false;
                break;
            case Parameters.data_.moveRight:
                this.moveRight_ = false;
                break;
            case Parameters.data_.rotateLeft:
                this.rotateLeft_ = false;
                break;
            case Parameters.data_.rotateRight:
                this.rotateRight_ = false;
                break;
            case Parameters.data_.useSpecial:
                this.specialKeyDown_ = false;
                break;
            case Parameters.data_.crouchKey:
                var player:Player = this.gs_.map.player_;
                if (player == null) {
                    return;
                }
                if (this.enablePlayerInput_) {
                    player.crouching = false;
                }
                break;
        }
        this.setPlayerMovement();
    }

    private function setPlayerMovement():void {
        var player:Player = this.gs_.map.player_;
        if (player != null) {
            if (this.enablePlayerInput_) {
                player.setRelativeMovement((!!this.rotateRight_ ? 1 : 0) - (!!this.rotateLeft_ ? 1 : 0), (!!this.moveRight_ ? 1 : 0) - (!!this.moveLeft_ ? 1 : 0), (!!this.moveDown_ ? 1 : 0) - (!!this.moveUp_ ? 1 : 0));
            }
            else {
                player.setRelativeMovement(0, 0, 0);
            }
        }
    }

    private function useEquipItem(slotId:int):void {
        if (this.tabStripModel.currentSelection == TabStripModel.BACKPACK) {
            slotId = slotId + GeneralConstants.NUM_INVENTORY_SLOTS;
        }

        var slotIndex:int = ObjectLibrary.getMatchingSlotIndex(this.gs_.map.player_.equipment_[slotId], this.gs_.map.player_);
        if (slotIndex != -1) {
            GameServerConnection.instance.invSwap(
                    this.gs_.map.player_,
                    this.gs_.map.player_, slotId,
                    this.gs_.map.player_, slotIndex);
        }
        else {
            GameServerConnection.instance.useItem_new(this.gs_.map.player_, slotId);
        }
    }

    private function togglePerformanceStats():void {
        if (this.gs_.contains(stats_)) {
            this.gs_.removeChild(stats_);
            this.gs_.removeChild(this.gs_.gsc_.jitterWatcher_);
            this.gs_.gsc_.disableJitterWatcher();
        }
        else {
            this.gs_.addChild(stats_);
            this.gs_.gsc_.enableJitterWatcher();
            this.gs_.gsc_.jitterWatcher_.y = stats_.height;
            this.gs_.addChild(this.gs_.gsc_.jitterWatcher_);
        }
    }
}
}
