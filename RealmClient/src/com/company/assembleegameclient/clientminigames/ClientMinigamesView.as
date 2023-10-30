package com.company.assembleegameclient.clientminigames {
import com.company.assembleegameclient.clientminigames.zolmexclicker.*;
import com.company.assembleegameclient.game.GameSprite;
import com.company.assembleegameclient.screens.*;
import com.company.assembleegameclient.ui.Scrollbar;
import com.company.assembleegameclient.util.RandomUtil;
import com.company.rotmg.graphics.ScreenGraphic;
import com.company.ui.SimpleText;
import com.company.util.Random;
import com.gskinner.motion.GTween;

import flash.display.DisplayObject;

import flash.display.Graphics;
import flash.display.Shape;
import flash.display.Sprite;
import flash.events.Event;
import flash.events.MouseEvent;
import flash.filters.DropShadowFilter;
import flash.text.TextFieldAutoSize;
import flash.utils.getTimer;

import kabam.rotmg.servers.api.Server;
import kabam.rotmg.ui.view.components.ScreenBase;

import org.osflash.signals.Signal;

public class ClientMinigamesView extends Sprite {

    public const close:Signal = new Signal();
    public var zolmexClicker:Signal;
    public var uadaPong:Signal;
    public var lastUpdate:int = 0;

    private var backButton:TitleMenuOption;
    private var zolmexClickerButton:TitleMenuOption;
    private var uadaPongButton:TitleMenuOption;

    public function ClientMinigamesView() {
        super();
        this.backButton = new TitleMenuOption("go back", 22, false);
        this.backButton.x = 750 - backButton.width;
        this.backButton.y = 550 - backButton.height;
        this.backButton.addEventListener(MouseEvent.MOUSE_UP, goBack);
        addChild(backButton);
        this.zolmexClickerButton = new TitleMenuOption("Zolmex Clicker", 50, false);
        this.zolmexClickerButton.x = 400 - this.zolmexClickerButton.width / 2;
        this.zolmexClickerButton.y = 300 - this.zolmexClickerButton.height / 2;
        this.zolmexClicker = this.zolmexClickerButton.clicked;
        addChild(zolmexClickerButton);
        this.uadaPongButton = new TitleMenuOption("Uada Pong", 50, false);
        this.uadaPongButton.x = 400 - this.uadaPongButton.width / 2;
        this.uadaPongButton.y = 250 - this.uadaPongButton.height / 2;
        this.uadaPong = this.uadaPongButton.clicked;
        addChild(uadaPongButton);

        this.lastUpdate = getTimer();
        addEventListener(Event.ENTER_FRAME, this.onEnterFrame);
    }

    public function onEnterFrame(e:Event):void {
        var time:int = getTimer();
        var dt:int = time - this.lastUpdate;

        this.lastUpdate = time;
    }

    public function exit():void {
        removeEventListener(Event.ENTER_FRAME, this.onEnterFrame);
    }

    public function goBack(e:Event):void {
        this.close.dispatch();
    }
}
}
