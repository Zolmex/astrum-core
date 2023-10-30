package com.company.assembleegameclient.clientminigames.zolmexclicker {
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

public class ZolmexClickerView extends Sprite {

    private var rand:Random = new Random(4125215);
    public const close:Signal = new Signal();
    public var lastUpdate:int = 0;

    private var zolmexImage:ZolmexImage;
    private var backButton:TitleMenuOption;
    private var zolmexButton:TitleMenuOption;
    private var incButton:TitleMenuOption;
    private var clickerButton:TitleMenuOption;
    private var zolmexCoins:SimpleText;

    private var coins:int;
    private var coinsInc:int = 1;
    private var incPrice:int = 100;
    private var clickerPrice:int = 50;
    private var clickersAmt:int;
    private var clickerCd:int = 1000;

    public function ZolmexClickerView() {
        super();
        this.zolmexImage = new ZolmexImage();
        this.zolmexImage.x = 400 - this.zolmexImage.width / 2;
        this.zolmexImage.y = 300 - this.zolmexImage.height / 2;
        addChild(this.zolmexImage);
        this.backButton = new TitleMenuOption("go back", 22, false);
        this.backButton.x = 750 - backButton.width;
        this.backButton.y = 550 - backButton.height;
        this.backButton.addEventListener(MouseEvent.MOUSE_UP, goBack);
        addChild(backButton);
        this.zolmexButton = new TitleMenuOption("Zolmex Button", 35, true);
        this.zolmexButton.x = 400 - zolmexButton.width / 2;
        this.zolmexButton.y = 500;
        this.zolmexButton.addEventListener(MouseEvent.CLICK, onZolmexClick);
        addChild(zolmexButton);
        this.incButton = new TitleMenuOption("Coins +1 - Cost: " + incPrice, 22, true);
        this.incButton.x = 400 - incButton.width / 2;
        this.incButton.y = 50;
        this.incButton.addEventListener(MouseEvent.CLICK, onIncClick);
        addChild(incButton);
        this.clickerButton = new TitleMenuOption("Zolmex Clicker - Cost: " + clickerPrice, 22, true);
        this.clickerButton.x = 400 - clickerButton.width / 2;
        this.clickerButton.y = 100;
        this.clickerButton.addEventListener(MouseEvent.CLICK, onClickerClick);
        addChild(clickerButton);
        this.zolmexCoins = createSimpleText("Zolmex Coins: 0", 20, 0xFFFFFF);
        this.zolmexCoins.x = 700 - zolmexCoins.width;
        this.zolmexCoins.y = 50;
        addChild(zolmexCoins);

        this.lastUpdate = getTimer();
        addEventListener(Event.ENTER_FRAME, this.onEnterFrame);
    }

    public function onEnterFrame(e:Event):void {
        var time:int = getTimer();
        var dt:int = time - this.lastUpdate;

        if (clickersAmt > 0) {
            if (clickerCd <= 0) {
                clickerCd = 1000;
                incrementZolmexCoins(clickersAmt);
            } else {
                clickerCd -= dt;
            }
        }

        this.lastUpdate = time;
    }

    public function onZolmexClick(e:Event):void {
        incrementZolmexCoins(coinsInc);
    }

    public function onClickerClick(e:Event):void {
        if (coins - clickerPrice > 0) {
            incrementZolmexCoins(-clickerPrice, false);
            clickersAmt += 1; clickerPrice *= 1.25;
            this.reloadTexts();
            insOrSucText(clickerButton.x + clickerButton.width / 2, clickerButton.y, true);
        } else
            insOrSucText(clickerButton.x + clickerButton.width / 2, clickerButton.y);
    }

    public function onIncClick(e:Event):void {
        if (coins - incPrice > 0) {
            incrementZolmexCoins(-incPrice, false);
            coinsInc += 1; incPrice *= 1.4;
            this.reloadTexts();
            insOrSucText(incButton.x + incButton.width / 2, incButton.y, true);
        } else
            insOrSucText(incButton.x + incButton.width / 2, incButton.y);
    }

    public function incrementZolmexCoins(amount:int, pos:Boolean = true):void {
        this.coins += amount;
        this.zolmexCoins.setText("Zolmex Coins: " + coins);
        createFloatingText((pos ? "+" : "") + amount, pos ? 0x00FF00 : 0xFF0000, 22,
                rand.nextIntRange(pos ? zolmexButton.x : zolmexCoins.x, pos ? (zolmexButton.x + zolmexButton.width) : (zolmexCoins.x + zolmexCoins.width))
                , pos ? zolmexButton.y : zolmexCoins.y);
    }

    public function createFloatingText(text:String, color:uint, size:int, x:int, y:int):void {
        var floatText:SimpleText = createSimpleText(text, size, color);
        floatText.x = x; floatText.y = y;
        addChild(floatText);
        var tween:GTween = new GTween(floatText, 1.5, {"alpha": 0, "y": y - 80});
        tween.onComplete = this.onFloatComplete;
    }

    public function createSimpleText(text:String, size:int, color:uint):SimpleText {
        var simpleText:SimpleText = new SimpleText(size, color);
        simpleText.setAutoSize(TextFieldAutoSize.CENTER);
        simpleText.autoResize = true;
        simpleText.setText(text);
        return simpleText;
    }

    public function reloadTexts():void {
        this.clickerButton.setText("Zolmex Clicker - Cost: " + clickerPrice);
        this.incButton.setText("Coins +1 - Cost: " + incPrice);
        this.zolmexCoins.setText("Zolmex Coins: " + coins);
    }

    public function insOrSucText(x:int, y:int, suc:Boolean = false):void {
        createFloatingText(suc ? "Success!" : "Insufficient", suc ? 0x00FF00 : 0xFF0000, 22, x, y);
    }

    public function exit():void {
        removeEventListener(Event.ENTER_FRAME, this.onEnterFrame);
    }

    public function onFloatComplete(tween:GTween):void {
        removeChild(SimpleText(tween.target));
    }

    public function goBack(e:Event):void {
        this.close.dispatch();
    }
}
}
