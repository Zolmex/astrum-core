package com.company.assembleegameclient.game
{
import com.company.assembleegameclient.map.Camera;
import com.company.assembleegameclient.map.HurtOverlay;
import com.company.assembleegameclient.map.Map;
import com.company.assembleegameclient.objects.GameObject;
import com.company.assembleegameclient.objects.IInteractiveObject;
import com.company.assembleegameclient.objects.Player;
import com.company.assembleegameclient.parameters.Parameters;
import com.company.assembleegameclient.ui.GuildText;
import com.company.assembleegameclient.ui.RankText;
import com.company.assembleegameclient.ui.TextBox;
import com.company.assembleegameclient.ui.tooltip.ToolTip;
import com.company.assembleegameclient.util.TextureRedrawer;
import com.company.util.CachingColorTransformer;
import com.company.util.PointUtil;
import flash.display.Sprite;
import flash.display.StageScaleMode;
import flash.events.Event;
import flash.utils.getTimer;
import kabam.lib.loopedprocs.LoopedCallback;
import kabam.lib.loopedprocs.LoopedProcess;
import kabam.rotmg.constants.GeneralConstants;
import kabam.rotmg.core.model.MapModel;
import kabam.rotmg.core.model.PlayerModel;
import kabam.rotmg.game.view.CreditDisplay;
import kabam.rotmg.messaging.impl.GameServerConnection;
import kabam.rotmg.messaging.impl.incoming.MapInfo;
import kabam.rotmg.servers.api.Server;
import kabam.rotmg.stage3D.Renderer;
import kabam.rotmg.ui.UIUtils;
import kabam.rotmg.ui.view.HUDView;
import org.osflash.signals.Signal;

public class GameSprite extends Sprite
{
    public const closed:Signal = new Signal();
    public const modelInitialized:Signal = new Signal();
    public const drawCharacterWindow:Signal = new Signal(Player);
    public var map:Map;
    public var camera_:Camera;
    public var gsc_:GameServerConnection;
    public var mui_:MapUserInput;
    public var textBox_:TextBox;
    public var isNexus_:Boolean = false;
    public var hudView:HUDView;
    public var rankText_:RankText;
    public var guildText_:GuildText;
    public var creditDisplay_:CreditDisplay;
    public var isEditor:Boolean;
    public var hurtOverlay_:HurtOverlay = new HurtOverlay();
    public var lastUpdate_:int = 0;
    public var firstUpdate:Boolean = true;
    public var mapModel:MapModel;
    public var model:PlayerModel;
    private var focus:GameObject;
    private var isGameStarted:Boolean;
    private var displaysPosY:uint = 4;
    public var gameId_:int;
    public var createCharacter_:Boolean;
    public var charId_:int;
    public var mapJSON_:String;
    public var toHomeScreen:Boolean;

    public function GameSprite(server:Server, gameId:int, createCharacter:Boolean, charId:int, model:PlayerModel, mapJSON:String) {
        this.camera_ = new Camera();
        super();
        this.model = model;
        this.map = new Map(this);
        addChild(this.map);
        this.gsc_ = GameServerConnection.instance != null ? GameServerConnection.instance : new GameServerConnection(this);
        this.gsc_.setServer(server);
        this.gsc_.gs_ = this;
        this.mui_ = new MapUserInput(this);
        this.textBox_ = new TextBox(this, 600, 600);
        addChild(this.textBox_);
        this.gameId_ = gameId;
        this.createCharacter_ = createCharacter;
        this.charId_ = charId;
        this.mapJSON_ = mapJSON;
    }

    public function setFocus(focus:GameObject) : void
    {
        focus = focus || this.map.player_;
        this.focus = focus;
    }

    public function applyMapInfo(mapInfo:MapInfo) : void
    {
        this.map.setProps(mapInfo.width_,mapInfo.height_,mapInfo.name_,mapInfo.background_,mapInfo.allowPlayerTeleport_,mapInfo.showDisplays_);
    }

    public function hudModelInitialized() : void
    {
        this.hudView = new HUDView();
        this.hudView.x = 600;
        addChild(this.hudView);
    }

    public function initialize() : void
    {
        this.map.initialize();
        this.creditDisplay_ = new CreditDisplay(this);
        this.creditDisplay_.x = 594;
        this.creditDisplay_.y = 0;
        addChild(this.creditDisplay_);
        this.modelInitialized.dispatch();
        this.focus = null;

        if(this.map.showDisplays_)
        {
            this.showSafeAreaDisplays();
        }

        if (this.map.name_ == "Nexus")
        {
            isNexus_ = true;
        }
        this.textBox_.addText(Parameters.CLIENT_CHAT_NAME, "Connected to " + this.map.name_ + "!");

        addChild(this.hurtOverlay_);
    }

    public function onScreenResize(event:Event):void
    {
        var scaleW:Number = (800 / stage.stageWidth);
        var scaleH:Number = (600 / stage.stageHeight);
        var mscale:Number = Parameters.data_.mscale;

        if (this.map != null)
        {
            this.map.scaleX = scaleW * mscale;
            this.map.scaleY = scaleH * mscale;
        }
        if (this.textBox_ != null)
        {
            this.textBox_.scaleX = scaleW / scaleH;
            this.textBox_.scaleY = 1;
        }
        if (this.hudView != null)
        {
            this.hudView.scaleX = scaleW / scaleH;
            this.hudView.scaleY = 1;
            this.hudView.y = 0;
            this.hudView.x = (800 - (200 * this.hudView.scaleX));
            //this.hudView.filters = [GameObject.PAUSED_FILTER];
            if (this.creditDisplay_ != null)
            {
                this.creditDisplay_.x = (this.hudView.x - (6 * this.creditDisplay_.scaleX));
            }
        }
        if (this.rankText_ != null)
        {
            this.rankText_.scaleX = scaleW;
            this.rankText_.scaleY = scaleH;
            this.rankText_.x = 8 * this.rankText_.scaleX;
            this.rankText_.y = 2 * this.rankText_.scaleY;
        }
        if (this.guildText_ != null)
        {
            this.guildText_.scaleX = scaleW;
            this.guildText_.scaleY = scaleH;
            this.guildText_.x = 64 * this.guildText_.scaleX;
            this.guildText_.y = 2 * this.guildText_.scaleY;
        }
        if (this.creditDisplay_ != null)
        {
            this.creditDisplay_.scaleX = scaleW;
            this.creditDisplay_.scaleY = scaleH;
        }
        if (this.constellationsView != null) {
            this.constellationsView.scaleX = scaleW / scaleH;
            this.constellationsView.scaleY = 1;
            this.constellationsView.x = 400 * (1 - this.constellationsView.scaleX);
        }
    }

    private function showSafeAreaDisplays() : void
    {
        this.showRankText();
        this.showGuildText();
    }

    private function showGuildText() : void
    {
        this.guildText_ = new GuildText("",-1);
        this.guildText_.x = 64;
        this.guildText_.y = 6;
        addChild(this.guildText_);
    }

    private function showRankText() : void
    {
        this.rankText_ = new RankText(-1,true,false);
        this.rankText_.x = 8;
        this.rankText_.y = this.displaysPosY;
        this.displaysPosY = this.displaysPosY + UIUtils.NOTIFICATION_SPACE;
        addChild(this.rankText_);
    }

    private function updateNearestInteractive() : void
    {
        var dist:Number = NaN;
        var go:GameObject = null;
        var iObj:IInteractiveObject = null;
        if(!this.map || !this.map.player_)
        {
            return;
        }
        var player:Player = this.map.player_;
        var minDist:Number = GeneralConstants.MAXIMUM_INTERACTION_DISTANCE;
        var closestInteractive:IInteractiveObject = null;
        var playerX:Number = player.x_;
        var playerY:Number = player.y_;
        for each(go in this.map.goDict_)
        {
            iObj = go as IInteractiveObject;
            if(iObj)
            {
                if(Math.abs(playerX - go.x_) < GeneralConstants.MAXIMUM_INTERACTION_DISTANCE || Math.abs(playerY - go.y_) < GeneralConstants.MAXIMUM_INTERACTION_DISTANCE)
                {
                    dist = PointUtil.distanceXY(go.x_,go.y_,playerX,playerY);
                    if(dist < GeneralConstants.MAXIMUM_INTERACTION_DISTANCE && dist < minDist)
                    {
                        minDist = dist;
                        closestInteractive = iObj;
                    }
                }
            }
        }
        this.mapModel.currentInteractiveTarget = closestInteractive;
    }

    public function connect() : void
    {
        if(!this.isGameStarted)
        {

            this.isGameStarted = true;
            Renderer.inGame = true;
            !this.gsc_.conReady && this.gsc_.connect();
            this.gsc_.conReady && this.gsc_.sendHello();
            this.lastUpdate_ = getTimer();

            if (Parameters.data_.mscale == undefined)
            {
                Parameters.data_.mscale = 1;
            }
            if (Parameters.data_.stageScale == undefined)
            {
                Parameters.data_.stageScale = StageScaleMode.NO_SCALE;
            }
            if (Parameters.data_.uiscale == undefined)
            {
                Parameters.data_.uiscale = true;
            }
            Parameters.save();

            stage.scaleMode = StageScaleMode.NO_SCALE;
            stage.addEventListener(Event.ENTER_FRAME,this.onEnterFrame);
            stage.addEventListener(Event.RESIZE, this.onScreenResize);
            stage.dispatchEvent(new Event(Event.RESIZE));
            LoopedProcess.addProcess(new LoopedCallback(100,this.updateNearestInteractive));
        }
    }

    public function disconnect() : void
    {
        if(this.isGameStarted)
        {
            this.isGameStarted = false;
            Renderer.inGame = false;
            this.toHomeScreen && this.gsc_.disconnect();
            stage.removeEventListener(Event.ENTER_FRAME,this.onEnterFrame);
            stage.removeEventListener(Event.RESIZE, this.onScreenResize);
            stage.scaleMode = StageScaleMode.EXACT_FIT;
            stage.dispatchEvent(new Event(Event.RESIZE));
            LoopedProcess.destroyAll();
            contains(this.map) && removeChild(this.map);
            this.map.dispose();
            CachingColorTransformer.clear();
            TextureRedrawer.clearCache();
        }
    }

    private function onEnterFrame(event:Event) : void
    {
        var time:int = getTimer();
        var player:Player = this.map.player_;
        if(player != null)
        {
            var dt:int = time - this.lastUpdate_;
            LoopedProcess.runProcesses(time);
            this.map.update(time, dt);
            this.camera_.update(dt);

            if(this.focus)
            {
                this.camera_.configureCamera(this.focus);
                this.map.draw(this.camera_,time);
            }
            this.creditDisplay_.draw(player.credits_,player.fame_);
            this.drawCharacterWindow.dispatch(player);
            if(this.map.showDisplays_)
            {
                this.rankText_.draw(player.numStars_);
                this.guildText_.draw(player.guildName_,player.guildRank_);
            }
            stage.dispatchEvent(new Event(Event.RESIZE));
        }
        this.lastUpdate_ = time;
    }
}
}
