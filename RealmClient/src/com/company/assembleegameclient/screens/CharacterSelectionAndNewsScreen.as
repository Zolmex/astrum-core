package com.company.assembleegameclient.screens {
import com.company.assembleegameclient.ui.ClickableText;
import com.company.assembleegameclient.ui.Scrollbar;
import com.company.rotmg.graphics.ScreenGraphic;
import com.company.ui.SimpleText;
import com.hurlant.util.asn1.parser.nulll;

import flash.display.DisplayObject;
import flash.display.Shape;
import flash.display.Sprite;
import flash.events.Event;
import flash.events.MouseEvent;
import flash.filters.DropShadowFilter;
import flash.geom.Rectangle;
import flash.text.TextFormatAlign;

import kabam.rotmg.core.model.PlayerModel;
import kabam.rotmg.game.view.CreditDisplay;
import kabam.rotmg.servers.api.ServerModel;
import kabam.rotmg.ui.view.components.ScreenBase;

import org.osflash.signals.Signal;
import org.osflash.signals.natives.NativeMappedSignal;

public class CharacterSelectionAndNewsScreen extends Sprite {

    private const SCROLLBAR_REQUIREMENT_HEIGHT:Number = 400;
    private const DROP_SHADOW:DropShadowFilter = new DropShadowFilter(0, 0, 0, 1, 8, 8);

    private var model:PlayerModel;
    private var isInitialized:Boolean;
    private var nameText:SimpleText;
    private var creditDisplay:CreditDisplay;
    private var charactersText:SimpleText;
    private var newsText:SimpleText;
    private var newsList:NewsList;
    private var characterList:CharacterList;
    private var characterListHeight:Number;
    private var playButton:TitleMenuOption;
    private var backButton:TitleMenuOption;
    private var classesButton:TitleMenuOption;
    private var lines:Shape;
    private var scrollBar:Scrollbar;
    public var close:Signal;
    public var showClasses:Signal;
    public var newCharacter:Signal;
    public var playGame:Signal;
    private var servers:ServerModel;
    public var oryxSleeping:Signal;

    public function CharacterSelectionAndNewsScreen() {
        this.playButton = new TitleMenuOption("play", 36, true);
        this.backButton = new TitleMenuOption("back", 22, false);
        this.classesButton = new TitleMenuOption("classes", 22, false);
        this.newCharacter = new Signal();
        this.playGame = new Signal();
        this.oryxSleeping = new Signal();
        super();
        addChild(new ScreenBase());
        addChild(new AccountScreen());
        this.close = new NativeMappedSignal(this.backButton, MouseEvent.CLICK);
        this.showClasses = new NativeMappedSignal(this.classesButton, MouseEvent.CLICK);
    }

    public function initialize(model:PlayerModel, servers:ServerModel):void {
        if (this.isInitialized) {
            return;
        }
        this.isInitialized = true;
        this.model = model;
        this.servers = servers;
        this.createDisplayAssets(model);
    }

    private function createDisplayAssets(model:PlayerModel):void {
        this.createNameText();
        this.createCreditDisplay();
        this.createCharactersText();
        this.createNewsText();
        this.createNewsList();
        this.createBoundaryLines();
        this.createCharacterList();

        this.createButtons();
        this.positionButtons();

        if (this.characterListHeight > this.SCROLLBAR_REQUIREMENT_HEIGHT) {
            this.createScrollbar();
        }
    }

    private function createButtons():void {
        addChild(new ScreenGraphic());
        addChild(this.playButton);
        addChild(this.classesButton);
        addChild(this.backButton);
        this.playButton.addEventListener(MouseEvent.CLICK, this.onPlayClick);
    }

    private function positionButtons():void {
        this.playButton.x = (this.getReferenceRectangle().width - this.playButton.width) / 2;
        this.playButton.y = 525;
        this.backButton.x = (this.getReferenceRectangle().width - this.backButton.width) / 2 - 94;
        this.backButton.y = 532;
        this.classesButton.x = (this.getReferenceRectangle().width - this.classesButton.width) / 2 + 96;
        this.classesButton.y = 532;
    }

    private function createScrollbar():void {
        this.scrollBar = new Scrollbar(16, 399);
        this.scrollBar.x = 375;
        this.scrollBar.y = 113;
        this.scrollBar.setIndicatorSize(399, this.characterList.height);
        this.scrollBar.addEventListener(Event.CHANGE, this.onScrollBarChange);
        addChild(this.scrollBar);
    }

    private function createCharacterList():void {
        this.characterList = new CharacterList(this.model);
        this.characterList.x = 10;
        this.characterList.y = 112;
        this.characterListHeight = this.characterList.height;
        addChild(this.characterList);
    }

    private function createNewsList():void {
        this.newsList = new NewsList(this.model);
        this.newsList.x = 400;
        this.newsList.y = 112;
        addChild(this.newsList);
    }

    private function createNewsText():void {
        this.newsText = new SimpleText(18, 11776947, false, 0, 0);
        this.newsText.setBold(true);
        this.newsText.text = "News";
        this.newsText.updateMetrics();
        this.newsText.filters = [this.DROP_SHADOW];
        this.newsText.setAlignment(TextFormatAlign.LEFT);
        this.newsText.x = 410;
        this.newsText.y = 79;
        addChild(this.newsText);
    }

    private function createCharactersText():void {
        this.charactersText = new SimpleText(18, 11776947, false, 0, 0);
        this.charactersText.setBold(true);
        this.charactersText.text = "Characters";
        this.charactersText.updateMetrics();
        this.charactersText.filters = [this.DROP_SHADOW];
        this.charactersText.setAlignment(TextFormatAlign.LEFT);
        this.charactersText.x = 10;
        this.charactersText.y = 79;
        addChild(this.charactersText);
    }

    private function createCreditDisplay():void {
        this.creditDisplay = new CreditDisplay();
        this.creditDisplay.draw(this.model.getCredits(), this.model.getFame());
        this.creditDisplay.x = this.getReferenceRectangle().width;
        this.creditDisplay.y = 20;
        addChild(this.creditDisplay);
    }

    private function createNameText():void {
        this.nameText = new SimpleText(22, 11776947, false, 0, 0);
        this.nameText.setBold(true);
        this.nameText.text = this.model.getName() || "Undefined";
        this.nameText.updateMetrics();
        this.nameText.filters = [this.DROP_SHADOW];
        this.nameText.y = 24;
        this.nameText.x = (this.getReferenceRectangle().width - this.nameText.width) / 2;
        addChild(this.nameText);
    }

    private function getReferenceRectangle():Rectangle {
        var rectangle:Rectangle = new Rectangle();
        if (stage) {
            rectangle = new Rectangle(0, 0, stage.stageWidth, stage.stageHeight);
        }
        return rectangle;
    }

    private function createBoundaryLines():void {
        this.lines = new Shape();
        this.lines.graphics.clear();
        this.lines.graphics.lineStyle(2, 5526612);
        this.lines.graphics.moveTo(0, 105);
        this.lines.graphics.lineTo(this.getReferenceRectangle().width, 105);
        this.lines.graphics.moveTo(400, 107);
        this.lines.graphics.lineTo(400, 526);
        this.lines.graphics.lineStyle();
        addChild(this.lines);
    }

    private function onScrollBarChange(event:Event):void {
        this.characterList.setPos(-this.scrollBar.pos() * (this.characterListHeight - 400));
    }

    private function removeIfAble(object:DisplayObject):void {
        if (object && contains(object)) {
            removeChild(object);
        }
    }

    private function onPlayClick(event:Event):void {
        if (!this.servers.isServerAvailable()){
            this.oryxSleeping.dispatch();
            return;
        }
        if (this.model.getCharacterCount() == 0) {
            this.newCharacter.dispatch();
        }
        else {
            this.playGame.dispatch();
        }
    }

    public function setName(name:String):void {
        this.nameText.text = name;
        this.nameText.updateMetrics();
        this.nameText.x = (this.getReferenceRectangle().width - this.nameText.width) * 0.5;
    }
}
}
