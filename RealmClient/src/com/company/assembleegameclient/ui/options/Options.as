package com.company.assembleegameclient.ui.options {
import com.company.assembleegameclient.game.GameSprite;
import com.company.assembleegameclient.parameters.Parameters;
import com.company.assembleegameclient.screens.TitleMenuOption;
import com.company.assembleegameclient.sound.SFX;
import com.company.rotmg.graphics.ScreenGraphic;
import com.company.ui.SimpleText;
import com.company.ui.SimpleText;
import com.company.util.AssetLibrary;
import com.company.util.KeyCodes;

import flash.display.BitmapData;
import flash.display.Sprite;
import flash.display.StageDisplayState;
import flash.display.StageQuality;
import flash.events.Event;
import flash.events.KeyboardEvent;
import flash.events.MouseEvent;
import flash.filters.DropShadowFilter;
import flash.geom.Point;
import flash.system.Capabilities;
import flash.text.TextFieldAutoSize;
import flash.ui.Mouse;
import flash.ui.MouseCursor;
import flash.ui.MouseCursorData;

import kabam.rotmg.ui.UIUtils;

public class Options extends Sprite {

    private static const CONTROLS_TAB:String = "Controls";
    private static const HOTKEYS_TAB:String = "Hot Keys";
    private static const CHAT_TAB:String = "Chat";
    private static const GRAPHICS_TAB:String = "Graphics";
    private static const SOUND_TAB:String = "Sound";
    private static const TABS:Vector.<String> = new <String>[CONTROLS_TAB, HOTKEYS_TAB, CHAT_TAB, GRAPHICS_TAB, SOUND_TAB];

    private static var registeredCursors:Vector.<String> = new <String>[];

    private var gs_:GameSprite;
    private var title_:SimpleText;
    private var continueButton_:TitleMenuOption;
    private var resetToDefaultsButton_:TitleMenuOption;
    private var homeButton_:TitleMenuOption;
    private var tabs_:Vector.<OptionsTabTitle>;
    private var selected_:OptionsTabTitle = null;
    private var options_:Vector.<Sprite>;
    private var optionIndex_:int = 0;

    public function Options(gs:GameSprite) {
        var tab:OptionsTabTitle = null;
        this.tabs_ = new Vector.<OptionsTabTitle>();
        this.options_ = new Vector.<Sprite>();
        super();
        this.gs_ = gs;
        graphics.clear();
        graphics.beginFill(2829099, 0.8);
        graphics.drawRect(0, 0, 800, 600);
        graphics.endFill();
        graphics.lineStyle(1, 6184542);
        graphics.moveTo(0, 100);
        graphics.lineTo(800, 100);
        graphics.lineStyle();
        this.title_ = new SimpleText(36, 16777215, false, 800, 0);
        this.title_.setBold(true);
        this.title_.htmlText = "<p align=\"center\">Options</p>";
        this.title_.autoSize = TextFieldAutoSize.CENTER;
        this.title_.filters = [new DropShadowFilter(0, 0, 0)];
        this.title_.updateMetrics();
        this.title_.x = 800 / 2 - this.title_.width / 2;
        this.title_.y = 8;
        addChild(this.title_);
        addChild(new ScreenGraphic());
        this.continueButton_ = new TitleMenuOption("continue", 36, false);
        this.continueButton_.addEventListener(MouseEvent.CLICK, this.onContinueClick);
        addChild(this.continueButton_);
        this.resetToDefaultsButton_ = new TitleMenuOption("reset to defaults", 22, false);
        this.resetToDefaultsButton_.addEventListener(MouseEvent.CLICK, this.onResetToDefaultsClick);
        addChild(this.resetToDefaultsButton_);
        this.homeButton_ = new TitleMenuOption("back to home", 22, false);
        this.homeButton_.addEventListener(MouseEvent.CLICK, this.onHomeClick);
        addChild(this.homeButton_);
        var xOffset:int = 14;
        for (var i:int = 0; i < TABS.length; i++) {
            tab = new OptionsTabTitle(TABS[i]);
            tab.x = xOffset;
            tab.y = 70;
            addChild(tab);
            tab.addEventListener(MouseEvent.CLICK, this.onTabClick);
            this.tabs_.push(tab);
            xOffset = xOffset + 108;
        }
        addEventListener(Event.ADDED_TO_STAGE, this.onAddedToStage);
        addEventListener(Event.REMOVED_FROM_STAGE, this.onRemovedFromStage);
    }

    private function onContinueClick(event:MouseEvent):void {
        this.close();
    }

    private function onResetToDefaultsClick(event:MouseEvent):void {
        var option:Option = null;
        for (var i:int = 0; i < this.options_.length; i++) {
            option = this.options_[i] as Option;
            if (option != null) {
                delete Parameters.data_[option.paramName_];
            }
        }
        Parameters.setDefaults();
        Parameters.save();
        this.refresh();
    }

    private function onHomeClick(event:MouseEvent):void {
        this.close();
        this.gs_.toHomeScreen = true;
        this.gs_.closed.dispatch();
    }

    private function onTabClick(event:MouseEvent):void {
        var tab:OptionsTabTitle = event.target as OptionsTabTitle;
        this.setSelected(tab);
    }

    private function setSelected(tab:OptionsTabTitle):void {
        if (tab == this.selected_) {
            return;
        }
        if (this.selected_ != null) {
            this.selected_.setSelected(false);
        }
        this.selected_ = tab;
        this.selected_.setSelected(true);
        this.removeOptions();
        switch (this.selected_.text_) {
            case CONTROLS_TAB:
                this.addControlsOptions();
                break;
            case HOTKEYS_TAB:
                this.addHotKeysOptions();
                break;
            case CHAT_TAB:
                this.addChatOptions();
                break;
            case GRAPHICS_TAB:
                this.addGraphicsOptions();
                break;
            case SOUND_TAB:
                this.addSoundOptions();
                break;
        }
    }

    private function onAddedToStage(event:Event):void {
        this.continueButton_.x = stage.stageWidth / 2 - this.continueButton_.width / 2;
        this.continueButton_.y = 520;
        this.resetToDefaultsButton_.x = 20;
        this.resetToDefaultsButton_.y = 532;
        this.homeButton_.x = 620;
        this.homeButton_.y = 532;
        this.setSelected(this.tabs_[0]);
        stage.addEventListener(KeyboardEvent.KEY_DOWN, this.onKeyDown, false, 1);
        stage.addEventListener(KeyboardEvent.KEY_UP, this.onKeyUp, false, 1);
    }

    private function onRemovedFromStage(event:Event):void {
        stage.removeEventListener(KeyboardEvent.KEY_DOWN, this.onKeyDown, false);
        stage.removeEventListener(KeyboardEvent.KEY_UP, this.onKeyUp, false);
    }

    private function onKeyDown(event:KeyboardEvent):void {
        if (event.keyCode == Parameters.data_.options) {
            this.close();
        }
        event.stopImmediatePropagation();
    }

    private function close():void {
        stage.focus = null;
        parent.removeChild(this);
    }

    private function onKeyUp(event:KeyboardEvent):void {
        event.stopImmediatePropagation();
    }

    private function removeOptions():void {
        var option:Sprite = null;
        for each(option in this.options_) {
            removeChild(option);
        }
        this.options_.length = 0;
        this.optionIndex_ = 0;
    }

    private function addControlsOptions():void {
        this.addOption(new KeyMapper("moveUp", "Move Up", "Key to will move character up"));
        this.addOption(new KeyMapper("moveLeft", "Move Left", "Key to will move character to the left"));
        this.addOption(new KeyMapper("moveDown", "Move Down", "Key to will move character down"));
        this.addOption(new KeyMapper("moveRight", "Move Right", "Key to will move character to the right"));
        this.addOption(new ChoiceOption("allowRotation", new <String>["On", "Off"], [true, false], "Allow Camera Rotation", "Toggles whether to allow for camera rotation", this.onAllowRotationChange));
        this.addOption(new Sprite());
        this.addOption(new KeyMapper("rotateLeft", "Rotate Left", "Key to will rotate the camera to the left", !Parameters.data_.allowRotation));
        this.addOption(new KeyMapper("rotateRight", "Rotate Right", "Key to will rotate the camera to the right", !Parameters.data_.allowRotation));
        this.addOption(new KeyMapper("useSpecial", "Use Special Ability", "This key will activate your special ability"));
        this.addOption(new KeyMapper("autofireToggle", "Autofire Toggle", "This key will toggle autofire"));
        this.addOption(new KeyMapper("resetToDefaultCameraAngle", "Reset To Default Camera Angle", "This key will reset the camera angle to the default " + "position"));
        this.addOption(new KeyMapper("togglePerformanceStats", "Toggle Performance Stats", "This key will toggle a display of fps and memory usage"));
        this.addOption(new KeyMapper("toggleCentering", "Toggle Centering of Player", "This key will toggle the position between centered and " + "offset"));
        this.addOption(new KeyMapper("interact", "Interact/Buy", "This key will allow you to enter a portal or buy an item"));
    }

    private function onAllowRotationChange():void {
        var keyMapper:KeyMapper = null;
        for (var i:int = 0; i < this.options_.length; i++) {
            keyMapper = this.options_[i] as KeyMapper;
            if (keyMapper != null) {
                if (keyMapper.paramName_ == "rotateLeft" || keyMapper.paramName_ == "rotateRight") {
                    keyMapper.setDisabled(!Parameters.data_.allowRotation);
                }
            }
        }
    }

    private function addHotKeysOptions():void {
        this.addOption(new KeyMapper("useHealthPotion", "Use Health Potion", "This key will use health potions if available"));
        this.addOption(new KeyMapper("useMagicPotion", "Use Magic Potion", "This key will use magic potions if available"));
        this.addOption(new KeyMapper("useEquipInvSlot1", "Use/Equip Inventory Slot 1", "Use/Equip item in inventory slot 1"));
        this.addOption(new KeyMapper("useEquipInvSlot2", "Use/Equip Inventory Slot 2", "Use/Equip item in inventory slot 2"));
        this.addOption(new KeyMapper("useEquipInvSlot3", "Use/Equip Inventory Slot 3", "Use/Equip item in inventory slot 3"));
        this.addOption(new KeyMapper("useEquipInvSlot4", "Use/Equip Inventory Slot 4", "Use/Equip item in inventory slot 4"));
        this.addOption(new KeyMapper("useEquipInvSlot5", "Use/Equip Inventory Slot 5", "Use/Equip item in inventory slot 5"));
        this.addOption(new KeyMapper("useEquipInvSlot6", "Use/Equip Inventory Slot 6", "Use/Equip item in inventory slot 6"));
        this.addOption(new KeyMapper("useEquipInvSlot7", "Use/Equip Inventory Slot 7", "Use/Equip item in inventory slot 7"));
        this.addOption(new KeyMapper("useEquipInvSlot8", "Use/Equip Inventory Slot 8", "Use/Equip item in inventory slot 8"));
        this.addOption(new KeyMapper("miniMapZoomIn", "Mini-Map Zoom In", "This key will zoom in the minimap"));
        this.addOption(new KeyMapper("miniMapZoomOut", "Mini-Map Zoom Out", "This key will zoom out the minimap"));
        this.addOption(new KeyMapper("escapeToNexus", "Escape To Nexus", "This key will instantly escape you to the Nexus"));
        this.addOption(new KeyMapper("options", "Show Options", "This key will bring up the options screen"));
        this.addOption(new KeyMapper("switchTabs", "Switch Tabs", "This key will switch from available tabs"));
        this.addOption(new KeyMapper("toggleFullscreenMode", "Toggle Fullscreen", "This toggles whether to go fullscreen or not"));
        this.addOption(new KeyMapper("crouchKey", "Crouch", "Acts as if your speed is 15 while holding this key."));

    }

    private function addChatOptions():void {
        this.addOption(new KeyMapper("chat", "Activate Chat", "This key will bring up the chat input box"));
        this.addOption(new KeyMapper("chatCommand", "Start Chat Command", "This key will bring up the chat with a \'/\' prepended to " + "allow for commands such as /who, /ignore, etc."));
        this.addOption(new KeyMapper("tell", "Begin Tell", "This key will bring up a tell (private message) in the chat" + " input box"));
        this.addOption(new KeyMapper("guildChat", "Begin Guild Chat", "This key will bring up a guild chat in the chat" + " input box"));
        this.addOption(new KeyMapper("scrollChatUp", "Scroll Chat Up", "This key will scroll up to older messages in the chat " + "buffer"));
        this.addOption(new KeyMapper("scrollChatDown", "Scroll Chat Down", "This key will scroll down to newer messages in the chat " + "buffer"));
        this.addOption(new ChoiceOption("chatScaling", new <String>["1.0x", "0.8x", "0.6x", "0.4x"], [1, 0.8, 0.6, 0.4], "Chat Scale", "Changes the chat's scale", doResizeEvent));
    }

    private function doResizeEvent():void {
        this.gs_.stage.dispatchEvent(new Event(Event.RESIZE));
    }

    private function addGraphicsOptions():void {
        this.addOption(new ChoiceOption("defaultCameraAngle", new <String>["45°", "0°"], [7 * Math.PI / 4, 0], "Default Camera Angle", "This toggles the default camera angle", onDefaultCameraAngleChange));
        this.addOption(new ChoiceOption("centerOnPlayer", new <String>["On", "Off"], [true, false], "Center On Player", "This toggles whether the player is centered or offset", null));
        this.addOption(new ChoiceOption("showQuestPortraits", new <String>["On", "Off"], [true, false], "Show Quest Portraits", "This toggles whether quest portraits are displayed", this.onShowQuestPortraitsChange));
        this.addOption(new ChoiceOption("drawShadows", new <String>["On", "Off"], [true, false], "Draw Shadows", "This toggles whether to draw shadows", null));
        this.addOption(new ChoiceOption("textBubbles", new <String>["On", "Off"], [true, false], "Draw Text Bubbles", "This toggles whether to draw text bubbles", null));
        this.addOption(new ChoiceOption("showGuildInvitePopup", new <String>["On", "Off"], [true, false], "Show Guild Invite Panel", "This toggles whether to show guild invites in the " + "lower-right panel or just in chat.", null));
        this.addOption(new ChoiceOption("particles", new <String>["On", "Off"], [true, false], "Particles", "If enabled, particles which are not necessary for gameplay are rendered (e.g. hit/death particles).", null));
        this.addOption(new ChoiceOption("hpBars", new <String>["On", "Off"], [true, false], "Health Bars", "Enabling this will render health bars below entities.", null));
        this.addOption(new ChoiceOption("quality", new <String>["High", "Low"], [true, false], "Quality", "Enabling this will render UI elements at higher/lower quality.", onQualityToggle));
        this.addOption(new ChoiceOption("cursor", new <String>[
                    "OS", "0", "1", "2", "3", "4", "5", "6", "7", "8", "9", "10", "11", "12", "13", "14", "15"],
                [MouseCursor.AUTO, "0", "1", "2", "3", "4", "5", "6", "7", "8", "9", "10", "11", "12", "13", "14", "15"], "Cursor", "Changing this will give you a new mouse cursor.", refreshCursor));

        if (!Parameters.GPURenderError) {
            this.addOption(new ChoiceOption("GPURender", new <String>["On", "Off"], [true, false], "Hardware Acceleration", "Enables Hardware Acceleration if your system supports it", null));
        }
    }

    public static function refreshCursor():void {
        var cursorData:MouseCursorData;
        var bitmapData:Vector.<BitmapData>;
        if (((!((Parameters.data_.cursor == MouseCursor.AUTO))) && ((registeredCursors.indexOf(Parameters.data_.cursor) == -1)))) {
            cursorData = new MouseCursorData();
            cursorData.hotSpot = new Point(15, 15);
            bitmapData = new Vector.<BitmapData>(1, true);
            bitmapData[0] = AssetLibrary.getImageFromSet("cursorsEmbed", int(Parameters.data_.cursor));
            cursorData.data = bitmapData;
            Mouse.registerCursor(Parameters.data_.cursor, cursorData);
            registeredCursors.push(Parameters.data_.cursor);
        }
        Mouse.cursor = Parameters.data_.cursor;
    }

    private static function onQualityToggle():void {
        UIUtils.toggleQuality(Parameters.data_.quality);
    }

    private static function onDefaultCameraAngleChange():void {
        Parameters.data_.cameraAngle = Parameters.data_.defaultCameraAngle;
        Parameters.save();
    }

    private function onShowQuestPortraitsChange():void {
        if (this.gs_ != null && this.gs_.map != null && this.gs_.map.partyOverlay_ != null && this.gs_.map.partyOverlay_.questArrow_ != null) {
            this.gs_.map.partyOverlay_.questArrow_.refreshToolTip();
        }
    }

    private function addSoundOptions():void {
        this.addOption(new ChoiceOption("playSFX", new <String>["On", "Off"], [true, false], "Play Sound Effects", "This toggles whether sound effects are played", this.onPlaySoundEffectsChange));
        this.addOption(new Sprite());
        this.addOption(new ChoiceOption("playPewPew", new <String>["On", "Off"], [true, false], "Play Weapon Sounds", "This toggles whether weapon sounds are played", null));
    }

    private function onPlaySoundEffectsChange():void {
        SFX.setPlaySFX(Parameters.data_.playSFX);
    }

    private function addOption(option:Sprite):void {
        option.x = this.optionIndex_ % 2 == 0 ? Number(20) : Number(415);
        option.y = int(this.optionIndex_ / 2) * 44 + 122;
        addChild(option);
        option.addEventListener(Event.CHANGE, this.onChange);
        this.options_.push(option);
        this.optionIndex_++;
    }

    private function onChange(event:Event):void {
        this.refresh();
    }

    private function refresh():void {
        var option:Option = null;
        for (var i:int = 0; i < this.options_.length; i++) {
            option = this.options_[i] as Option;
            if (option != null) {
                option.refresh();
            }
        }
    }
}
}
