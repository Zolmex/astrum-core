package kabam.rotmg.ui.view {
import com.company.assembleegameclient.constants.ScreenTypes;
import com.company.assembleegameclient.parameters.Parameters;
import com.company.assembleegameclient.screens.AccountScreen;
import com.company.assembleegameclient.screens.TitleMenuOption;
import com.company.assembleegameclient.ui.SoundIcon;
import com.company.rotmg.graphics.ScreenGraphic;
import com.company.ui.SimpleText;

import flash.display.Sprite;
import flash.filters.DropShadowFilter;

import kabam.rotmg.ui.view.components.ScreenBase;

import org.osflash.signals.Signal;

public class TitleView extends Sprite {

    private static const COPYRIGHT:String = "© 2010, 2011 by Wild Shadow Studios, Inc.";

    public var playClicked:Signal;
    public var serversClicked:Signal;
    public var accountClicked:Signal;
    public var legendsClicked:Signal;
    public var editorClicked:Signal;
    public var minigamesClicked:Signal;

    private var container:Sprite;

    private var playButton:TitleMenuOption;
    private var serversButton:TitleMenuOption;
    private var accountButton:TitleMenuOption;
    private var legendsButton:TitleMenuOption;
    private var editorButton:TitleMenuOption;
    private var minigamesButton:TitleMenuOption;

    private var versionText:SimpleText;
    private var copyrightText:SimpleText;

    public function TitleView() {
        super();
        addChild(new ScreenBase());
        addChild(new AccountScreen());
        addChild(new ScreenGraphic());
        this.makeChildren();
    }

    private function makeChildren():void {
        this.container = new Sprite();
        this.playButton = new TitleMenuOption(ScreenTypes.PLAY, 36, true);
        this.playClicked = this.playButton.clicked;
        this.container.addChild(this.playButton);
        this.serversButton = new TitleMenuOption(ScreenTypes.SERVERS, 22, false);
        this.serversClicked = this.serversButton.clicked;
        this.container.addChild(this.serversButton);
        this.accountButton = new TitleMenuOption(ScreenTypes.ACCOUNT, 22, false);
        this.accountClicked = this.accountButton.clicked;
        this.container.addChild(this.accountButton);
        this.legendsButton = new TitleMenuOption(ScreenTypes.LEGENDS, 22, false);
        this.legendsClicked = this.legendsButton.clicked;
        this.container.addChild(this.legendsButton);
        this.editorButton = new TitleMenuOption(ScreenTypes.EDITOR, 22, false);
        this.editorClicked = this.editorButton.clicked;
        this.container.addChild(editorButton);
        this.minigamesButton = new TitleMenuOption("minigames", 22, true);
        this.minigamesClicked = this.minigamesButton.clicked;
        this.container.addChild(minigamesButton);
        this.versionText = new SimpleText(12, 8355711, false, 0, 0);
        this.versionText.filters = [new DropShadowFilter(0, 0, 0)];
        this.container.addChild(this.versionText);
        this.copyrightText = new SimpleText(12, 8355711, false, 0, 0);
        this.copyrightText.text = COPYRIGHT;
        this.copyrightText.updateMetrics();
        this.copyrightText.filters = [new DropShadowFilter(0, 0, 0)];
        this.container.addChild(this.copyrightText);
    }

    public function initialize():void {
        this.updateVersionText();
        this.positionButtons();
        this.addChildren();
    }

    private function updateVersionText():void {
        this.versionText.htmlText = "RotMG " + Parameters.BUILD_VERSION;
        this.versionText.updateMetrics();
    }

    private function addChildren():void {
        addChild(this.container);
    }

    private function positionButtons():void {
        this.playButton.x = 800 / 2 - this.playButton.width / 2;
        this.playButton.y = 525;
        this.serversButton.x = 800 / 2 - this.serversButton.width / 2 - 94;
        this.serversButton.y = 532;
        this.accountButton.x = 800 / 2 - this.accountButton.width / 2 + 96;
        this.accountButton.y = 532;
        this.legendsButton.x = 550;
        this.legendsButton.y = 532;
        this.editorButton.x = 180;
        this.editorButton.y = 532;
        this.minigamesButton.x = 770 - this.minigamesButton.width;
        this.minigamesButton.y = 532;
        this.versionText.y = stage.stageHeight - this.versionText.height;
        this.copyrightText.x = stage.stageWidth - this.copyrightText.width;
        this.copyrightText.y = stage.stageHeight - this.copyrightText.height;
    }
}
}
