package kabam.rotmg.account.web.view {
import com.company.assembleegameclient.screens.TitleMenuOption;
import com.company.ui.SimpleText;

import flash.display.DisplayObject;
import flash.display.Sprite;
import flash.events.MouseEvent;
import flash.filters.DropShadowFilter;

import kabam.rotmg.account.core.view.AccountInfoView;

import org.osflash.signals.Signal;
import org.osflash.signals.natives.NativeMappedSignal;

public class WebAccountInfoView extends Sprite implements AccountInfoView {

    private static const LOG_IN:String = "log in";
    private static const LOG_OUT:String = "log out";
    private static const LOGGED_IN_TEXT:String = "logged in as ${userName} - ";
    private static const GUEST_ACCOUNT:String = "guest account - ";
    private static const REGISTER:String = "register";
    private static const FONT_SIZE:int = 18;

    private var _login:Signal;
    private var _register:Signal;
    private var userName:String = "";
    private var isRegistered:Boolean;
    private var accountText:SimpleText;
    private var registerButton:TitleMenuOption;
    private var loginButton:TitleMenuOption;
    private var reloadButton:TitleMenuOption;
    private var reloadCallback:Function;

    public function WebAccountInfoView() {
        super();
        this.makeUIElements();
        this.makeSignals();
    }

    public function get login():Signal {
        return this._login;
    }

    public function get register():Signal {
        return this._register;
    }

    private function makeUIElements():void {
        this.makeAccountText();
        this.makeLoginButton();
        this.makeRegisterButton();
        this.makeReloadButton();
    }

    private function makeSignals():void {
        this._login = new NativeMappedSignal(this.loginButton, MouseEvent.CLICK);
        this._register = new NativeMappedSignal(this.registerButton, MouseEvent.CLICK);
    }

    private function makeAccountText():void {
        this.accountText = new SimpleText(FONT_SIZE, 11776947, false, 0, 0);
        this.accountText.filters = [new DropShadowFilter(0, 0, 0, 1, 4, 4)];
    }

    private function makeLoginButton():void {
        this.loginButton = new TitleMenuOption("log in", FONT_SIZE, false);
    }

    private function makeRegisterButton():void {
        this.registerButton = new TitleMenuOption(REGISTER, FONT_SIZE, false);
    }

    private function makeReloadButton():void {
        this.reloadButton = new TitleMenuOption("reload", FONT_SIZE, false);
        this.reloadButton.clicked.add(this.onReloadClicked);
    }

    private static function makeDividerText():SimpleText {
        var ret:SimpleText = new SimpleText(FONT_SIZE, 11776947, false, 0, 0);
        ret.filters = [new DropShadowFilter(0, 0, 0, 1, 4, 4)];
        ret.text = " - ";
        ret.updateMetrics();
        return ret;
    }

    public function setInfo(userName:String, isRegistered:Boolean):void {
        this.userName = userName;
        this.isRegistered = isRegistered;
        this.updateUI();
    }

    private function updateUI():void {
        this.removeUIElements();
        if (this.isRegistered) {
            this.showUIForRegisteredAccount();
        }
        else {
            this.showUIForGuestAccount();
        }
    }

    private function removeUIElements():void {
        while (numChildren) {
            removeChildAt(0);
        }
    }

    private function showUIForRegisteredAccount():void {
        this.accountText.text = LOGGED_IN_TEXT.replace("${userName}", this.userName);
        this.accountText.updateMetrics();
        this.loginButton.setText(LOG_OUT);
        this.reloadButton.setText("reload");
        var divider:SimpleText = makeDividerText();
        this.addAndAlignHorizontally(this.accountText, this.loginButton, divider, this.reloadButton);
    }

    private function showUIForGuestAccount():void {
        this.accountText.text = GUEST_ACCOUNT;
        this.accountText.updateMetrics();
        this.loginButton.setText(LOG_IN);
        this.reloadButton.setText("reload");
        var divider1:SimpleText = makeDividerText();
        var divider2:SimpleText = makeDividerText();
        this.addAndAlignHorizontally(this.accountText, this.registerButton, divider1, this.loginButton, divider2, this.reloadButton);
    }

    private function addAndAlignHorizontally(...uiElements):void {
        var ui:DisplayObject = null;
        var x:int = 0;
        var i:int = uiElements.length;
        while (i--) {
            ui = uiElements[i];
            x = x - ui.width;
            ui.x = x;
            addChild(ui);
        }
    }

    public function onReload(callback:Function):void {
        this.reloadCallback = callback;
    }

    private function onReloadClicked():void {
        if (this.reloadButton)
            this.reloadCallback();
    }
}
}
