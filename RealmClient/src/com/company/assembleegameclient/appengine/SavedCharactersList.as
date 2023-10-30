package com.company.assembleegameclient.appengine {
import com.company.assembleegameclient.objects.ObjectLibrary;

import flash.events.Event;

import kabam.rotmg.account.core.Account;
import kabam.rotmg.core.StaticInjectorContext;

import org.swiftsuspenders.Injector;

public class SavedCharactersList extends Event {

    public static const SAVED_CHARS_LIST:String = "SAVED_CHARS_LIST";
    public static const AVAILABLE:String = "available";
    public static const UNAVAILABLE:String = "unavailable";
    public static const UNRESTRICTED:String = "unrestricted";

    private var origData_:String;
    private var charsXML_:XML;
    public var accountId_:int;
    public var nextCharId_:int;
    public var maxNumChars_:int;
    public var numChars_:int = 0;
    public var savedChars_:Vector.<SavedCharacter>;
    public var charStats_:Object;
    public var totalFame_:int = 0;
    public var fame_:int = 0;
    public var credits_:int = 0;
    public var numStars_:int = 0;
    public var guildName_:String;
    public var guildRank_:int;
    public var name_:String = null;
    public var news_:Vector.<SavedNewsItem>;
    public var isAdmin_:Boolean;
    private var account:Account;

    public function SavedCharactersList(data:String) {
        var value:* = undefined;
        var account:Account = null;
        this.savedChars_ = new Vector.<SavedCharacter>();
        this.charStats_ = {};
        this.news_ = new Vector.<SavedNewsItem>();
        super(SAVED_CHARS_LIST);
        this.origData_ = data;
        this.charsXML_ = new XML(this.origData_);
        var accountXML:XML = XML(this.charsXML_.Account);
        this.parseUserData(accountXML);
        this.parseGuildData(accountXML);
        this.parseCharacterData();
        this.parseCharacterStatsData();
        this.parseNewsData();
        var injector:Injector = StaticInjectorContext.getInjector();
        if (injector) {
            account = injector.getInstance(Account);
            account.reportIntStat("BestLevel", this.bestOverallLevel());
            account.reportIntStat("BestFame", this.bestOverallFame());
            account.reportIntStat("NumStars", this.numStars_);
        }
    }

    public function getCharById(id:int):SavedCharacter {
        var savedChar:SavedCharacter = null;
        for each(savedChar in this.savedChars_) {
            if (savedChar.charId() == id) {
                return savedChar;
            }
        }
        return null;
    }

    private function parseUserData(accountXML:XML):void {
        this.accountId_ = accountXML.AccountId;
        this.name_ = accountXML.Name;
        this.totalFame_ = int(accountXML.Stats.TotalFame);
        this.fame_ = int(accountXML.Stats.Fame);
        this.credits_ = int(accountXML.Stats.Credits);
        this.isAdmin_ = accountXML.hasOwnProperty("Admin");
    }

    private function parseGuildData(accountXML:XML):void {
        var guildXML:XML = null;
        if (accountXML.hasOwnProperty("Guild")) {
            guildXML = XML(accountXML.Guild);
            this.guildName_ = guildXML.Name;
            this.guildRank_ = int(guildXML.Rank);
        }
    }

    private function parseCharacterData():void {
        var charXML:XML = null;
        this.nextCharId_ = int(this.charsXML_.@nextCharId);
        this.maxNumChars_ = int(this.charsXML_.@maxNumChars);
        for each(charXML in this.charsXML_.Char) {
            this.savedChars_.push(new SavedCharacter(charXML, this.name_));
            this.numChars_++;
        }
        this.savedChars_.sort(SavedCharacter.compare);
    }

    private function parseCharacterStatsData():void {
        var charStatsXML:XML = null;
        var type:int = 0;
        var charStats:CharacterStats = null;
        var statsXML:XML = XML(this.charsXML_.Account.Stats);
        for each(charStatsXML in statsXML.ClassStats) {
            type = int(charStatsXML.@objectType);
            charStats = new CharacterStats(charStatsXML);
            this.numStars_ = this.numStars_ + charStats.numStars();
            this.charStats_[type] = charStats;
        }
    }

    private function parseNewsData():void {
        var newsItemXML:XML = null;
        for each(newsItemXML in this.charsXML_.NewsItem) {
            this.news_.push(new SavedNewsItem(newsItemXML.Icon, newsItemXML.Title, newsItemXML.TagLine, newsItemXML.Link, int(newsItemXML.Date)));
        }
    }

    public function bestLevel(objectType:int):int {
        var charStats:CharacterStats = this.charStats_[objectType];
        return charStats == null ? int(0) : int(charStats.bestLevel());
    }

    public function bestOverallLevel():int {
        var charStats:CharacterStats = null;
        var bestLevel:int = 0;
        for each(charStats in this.charStats_) {
            if (charStats.bestLevel() > bestLevel) {
                bestLevel = charStats.bestLevel();
            }
        }
        return bestLevel;
    }

    public function bestFame(objectType:int):int {
        var charStats:CharacterStats = this.charStats_[objectType];
        return charStats == null ? int(0) : int(charStats.bestFame());
    }

    public function bestOverallFame():int {
        var charStats:CharacterStats = null;
        var bestFame:int = 0;
        for each(charStats in this.charStats_) {
            if (charStats.bestFame() > bestFame) {
                bestFame = charStats.bestFame();
            }
        }
        return bestFame;
    }

    public function availableCharSlots():int {
        return this.maxNumChars_ - this.numChars_;
    }

    public function hasAvailableCharSlot():Boolean {
        return this.numChars_ < this.maxNumChars_;
    }

    override public function clone():Event {
        return new SavedCharactersList(this.origData_);
    }

    override public function toString():String {
        return "[" + " numChars: " + this.numChars_ + " maxNumChars: " + this.maxNumChars_ + " ]";
    }
}
}
