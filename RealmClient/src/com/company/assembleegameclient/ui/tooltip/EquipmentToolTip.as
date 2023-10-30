package com.company.assembleegameclient.ui.tooltip {
import com.company.assembleegameclient.itemData.ActivateEffect;
import com.company.assembleegameclient.itemData.CondEffect;
import com.company.assembleegameclient.itemData.CustomToolTipData;
import com.company.assembleegameclient.itemData.ItemData;
import com.company.assembleegameclient.itemData.ProjectileDesc;
import com.company.assembleegameclient.itemData.StatBoost;
import com.company.assembleegameclient.objects.ObjectLibrary;
import com.company.assembleegameclient.objects.Player;
import com.company.assembleegameclient.parameters.Parameters;
import com.company.assembleegameclient.ui.Stats;
import com.company.assembleegameclient.util.FilterUtil;
import com.company.assembleegameclient.util.TextureRedrawer;
import com.company.assembleegameclient.util.TierUtil;
import com.company.assembleegameclient.util.UILabel;
import com.company.ui.SimpleText;
import com.company.util.AssetLibrary;
import com.company.util.BitmapUtil;
import com.company.util.KeyCodes;
import com.company.util.MathUtil2;

import flash.display.Bitmap;
import flash.display.BitmapData;
import flash.display.DisplayObject;
import flash.display.Sprite;
import flash.events.TimerEvent;
import flash.text.StyleSheet;
import flash.text.TextFieldAutoSize;
import flash.utils.Timer;

import kabam.rotmg.constants.ActivationType;

import kabam.rotmg.constants.ItemConstants;

public class EquipmentToolTip extends ToolTip {

    private static const MAX_WISMOD:int = 195;
    private static const MIN_WISMOD:int = 50;
    private static const WIDTH:int = 230;
    private static const IGNORE_AE:Array = [ActivationType.SHOOT, ActivationType.PET, ActivationType.PERMA_PET, ActivationType.CREATE, ActivationType.UNLOCK_PORTAL, ActivationType.SHURIKEN, ActivationType.DAZE_BLAST];
    private static const CSS_TEXT:String =
            ".aeIn { margin-left:14px; text-indent:-4px; }" +
            ".ieIn { margin-left:10px; text-indent:-10px; }";

    private var player:Player;
    private var equipData:ItemData;
    private var itemData:ItemData;
    private var icon:Bitmap;
    private var displayText:SimpleText;
    private var specialityText:SimpleText;
    private var tierLabel:UILabel;
    private var descText:SimpleText;
    private var line1:Sprite;
    private var line2:Sprite;
    private var bagIcon:Bitmap;
    private var customText:SimpleText;
    private var attributes:String;
    private var attributesText:SimpleText;
    private var specifications:String;
    private var specificationsText:SimpleText;
    private var usableBy:Boolean;
    private var specialityColor:uint;

    public function EquipmentToolTip(itemData:ItemData, owner:Player = null) {
        this.itemData = itemData;
        if (owner) {
            this.player = owner;
            var equipId:int = GetEquipIndex(itemData.SlotType, owner.equipment_);
            if (equipId != -1)
                this.equipData = owner.equipment_[equipId];
        }
        this.usableBy = IsUsableBy(owner, itemData.SlotType);
        this.specialityColor = TooltipHelper.getSpecialityColor(this.itemData);
        var backColor:uint = this.usableBy ? 0x363636 : 6036765;
        var outlineColor:uint = this.usableBy ? 0x9B9B9B : 10965039;
        super(backColor, 1, outlineColor, 1);
        this.drawIcon();
        this.drawDisplayName();
        this.drawSpeciality();
        this.drawTier();
        this.drawDesc();
        this.drawBagIcon();
        this.drawCustomData();
        this.makeAttributes();
        this.drawAttributes();
        this.makeSpecifications();
        this.drawSpecifications();
    }

    private function drawIcon():void {
        var tex:BitmapData = this.itemData.Texture.getRedrawnTexture(70, 0, true);
        tex = BitmapUtil.cropToBitmapData(tex, 4, 4, tex.width - 8, tex.height - 8);
        this.icon = new Bitmap(tex);
        addChild(this.icon);
    }

    private function drawDisplayName():void {
        var text:String = TooltipHelper.getSpecialityText(this.itemData);
        var color:uint = text != "Untiered" ? this.specialityColor : 0xB3B3B3;
        this.displayText = new SimpleText(17, color, false, WIDTH - (this.icon.width + 30));
        this.displayText.text = this.itemData.DisplayName;
        this.displayText.setAutoSize(TextFieldAutoSize.LEFT);
        this.displayText.setBold(true);
        this.displayText.useTextDimensions();
        if (Parameters.data_.toolTipOutline)
            this.displayText.filters = FilterUtil.getTextOutlineFilter();
        else
            this.displayText.filters = FilterUtil.getTextShadowFilter();
        this.displayText.x = this.icon.width;
        this.displayText.y = ((this.icon.height - this.displayText.height) / 4) - 3;
        addChild(this.displayText);
    }

    private function drawSpeciality():void {
        var text:String = TooltipHelper.getSpecialityText(this.itemData);
        this.specialityText = new SimpleText(14, this.specialityColor);
        this.specialityText.text = text;
        this.specialityText.useTextDimensions();
        if (Parameters.data_.toolTipOutline)
            this.specialityText.filters = FilterUtil.getTextOutlineFilter();
        else
            this.specialityText.filters = FilterUtil.getTextShadowFilter();
        this.specialityText.x = this.displayText.x;
        this.specialityText.y = this.displayText.y + this.displayText.height - 4;
        addChild(this.specialityText);
    }

    private function drawTier():void {
        this.tierLabel = TierUtil.getTierTag(this.itemData, 16);
        if (this.tierLabel) {
            if (Parameters.data_.toolTipOutline)
                this.tierLabel.filters = FilterUtil.getTextOutlineFilter();
            else
                this.tierLabel.filters = FilterUtil.getTextShadowFilter();
            this.tierLabel.x = WIDTH - this.tierLabel.width;
            this.tierLabel.y = this.displayText.y;
            addChild(this.tierLabel);
        }
    }

    private function drawDesc():void {
        this.descText = new SimpleText(14, 0xB3B3B3, false, WIDTH - 10);
        this.descText.htmlText = this.itemData.Description;
        this.descText.wordWrap = true;
        this.descText.useTextDimensions();
        if (Parameters.data_.toolTipOutline)
            this.descText.filters = FilterUtil.getTextOutlineFilter();
        else
            this.descText.filters = FilterUtil.getTextShadowFilter();
        this.descText.x = 5;
        this.descText.y = this.icon.height + 3;
        addChild(this.descText);
    }

    private function drawBagIcon():void {
        var tex:BitmapData = GetBagTexture(this.itemData.BagType, 40);
        tex = BitmapUtil.cropToBitmapData(tex, 4, 4, tex.width - 8, tex.height - 8);
        this.bagIcon = new Bitmap(tex);
        if (this.tierLabel) {
            this.bagIcon.x = this.tierLabel.x - ((this.bagIcon.width - this.tierLabel.width) / 2);
            this.bagIcon.y = this.tierLabel.y + this.tierLabel.height - 9;
        }
        else {
            this.bagIcon.x = WIDTH - this.bagIcon.width;
            this.bagIcon.y = (this.icon.height - this.bagIcon.height) / 2;
        }
        addChild(this.bagIcon);
    }

    private function drawCustomData():void {
        if (!this.itemData.CustomToolTipDataList || this.itemData.CustomToolTipDataList.length < 1)
            return;

        var str:String = "";
        for (var i:int = 0; i < this.itemData.CustomToolTipDataList.length; i++) {
            var data:CustomToolTipData = this.itemData.CustomToolTipDataList[i];
            if (data.Name != "") {
                str += data.Name + ": ";
            }
            str += TooltipHelper.wrapInFontTag(data.Description, TooltipHelper.NO_DIFF_COLOR) + "\n";
        }

        this.drawLine1();
        this.customText = new SimpleText(14, 0xB3B3B3, false, WIDTH - 10);
        this.customText.wordWrap = true;
        this.customText.htmlText = str;
        this.customText.useTextDimensions();
        if (Parameters.data_.toolTipOutline)
            this.customText.filters = FilterUtil.getTextOutlineFilter();
        else
            this.customText.filters = FilterUtil.getTextShadowFilter();
        this.customText.x = this.descText.x;
        this.customText.y = this.line1.y + this.line1.height + 3;
        addChild(this.customText);
    }

    private function makeAttributes():void {
        this.attributes = "";
        this.makeProjAttributes();
        this.makeActivateEffects();
        this.makeStatBoosts();
        this.makeGlobalAttributes();
    }

    private function makeProjAttributes():void {
        var proj:ProjectileDesc = this.itemData.Projectile;
        var proj2:ProjectileDesc = this.equipData ? this.equipData.Projectile : null;
        if (!proj)
            return;
        this.makeProjCount(proj, proj2);
        this.makeProjEffects(proj, proj2);
        this.makeProjDamage(proj, proj2);
        this.makeProjRange(proj, proj2);
        this.makeProjRoF(proj, proj2);
        this.makeProjArcGap(proj, proj2);
        this.makeProjProperties(proj, proj2);
    }

    private function makeProjCount(proj:ProjectileDesc, proj2:ProjectileDesc):void {
        var count:int = this.itemData.NumProjectiles;
        var color:String = TooltipHelper.NO_DIFF_COLOR;
        if (this.equipData) {
            var count2:int = this.equipData.NumProjectiles;
            color = TooltipHelper.getTextColor(count - count2);
        }
        else if (this.usableBy)
            color = TooltipHelper.BETTER_COLOR;
        this.attributes += "Shots: " + TooltipHelper.wrapInFontTag(String(count), color) + "\n";
    }

    private function makeProjEffects(proj:ProjectileDesc, proj2:ProjectileDesc):void {
        if (!proj.Effects || proj.Effects.length < 1)
            return;

        this.attributes += "Shot Effect(s):\n";
        for (var i:int = 0; i < proj.Effects.length; i++) {
            var condEff:CondEffect = proj.Effects[i];
            var duration:Number = MathUtil2.roundTo(condEff.DurationMS / 1000.0, 2);
            var color:String = TooltipHelper.NO_DIFF_COLOR;
            if (proj2 && proj.Effects && proj.Effects.length > 0) {
                var i2:int = GetConditionEffectIndex(condEff.Effect, proj2.Effects);
                if (i2 != -1) {
                    var duration2:Number = MathUtil2.roundTo(proj2.Effects[i2].DurationMS / 1000.0, 2);
                    color = TooltipHelper.getTextColor(duration - duration2);
                }
                else color = TooltipHelper.BETTER_COLOR;
            }
            else if (this.usableBy)
                color = TooltipHelper.BETTER_COLOR;
            this.attributes += "    Inflicts " + TooltipHelper.wrapInFontTag(condEff.EffectName, color) + " for " + TooltipHelper.wrapInFontTag(String(duration), color) + " secs" + "\n";
        }
    }

    private function makeProjDamage(proj:ProjectileDesc, proj2:ProjectileDesc):void {
        var minD:int = proj.MinDamage;
        var maxD:int = proj.MaxDamage;
        var color:String = TooltipHelper.NO_DIFF_COLOR;
        if (proj2) {
            var minD2:int = proj2.MinDamage;
            var maxD2:int = proj2.MaxDamage;
            var avg1:Number = (minD + maxD) / 2.0;
            var avg2:Number = (minD2 + maxD2) / 2.0;
            color = TooltipHelper.getTextColor(avg1 - avg2);
        }
        else if (this.usableBy)
            color = TooltipHelper.BETTER_COLOR;
        this.attributes += "Damage: " + TooltipHelper.wrapInFontTag(minD == maxD ? String(minD) : minD + " - " + maxD, color) + "\n";
    }

    private function makeProjRange(proj:ProjectileDesc, proj2:ProjectileDesc):void {
        var range:Number = TooltipHelper.getFormattedRangeString(proj.Speed * proj.LifetimeMS / 10000.0);
        var color:String = TooltipHelper.NO_DIFF_COLOR;
        if (proj2) {
            var range2:Number = TooltipHelper.getFormattedRangeString(proj2.Speed * proj2.LifetimeMS / 10000.0);
            color = TooltipHelper.getTextColor(range - range2);
        }
        else if (this.usableBy)
            color = TooltipHelper.BETTER_COLOR;
        this.attributes += "Range: " + TooltipHelper.wrapInFontTag(String(range), color) + "\n";
    }

    private function makeProjRoF(proj:ProjectileDesc, proj2:ProjectileDesc):void {
        if (this.itemData.RateOfFire == -1)
            return;

        var rof:Number = this.itemData.RateOfFire * 100;
        var color:String = TooltipHelper.NO_DIFF_COLOR;
        if (this.equipData) {
            var rof2:Number = this.equipData.RateOfFire * 100;
            color = TooltipHelper.getTextColor(rof - rof2);
        }
        else if (this.usableBy)
            color = TooltipHelper.BETTER_COLOR;
        this.attributes += "Rate of Fire: " + TooltipHelper.wrapInFontTag(rof.toFixed(0) + "%", color) + "\n";
    }

    private function makeProjArcGap(proj:ProjectileDesc, proj2:ProjectileDesc):void {
        if (this.itemData.ArcGap <= 0)
            return;
        var arc:Number = this.itemData.ArcGap;
        var color:String = TooltipHelper.NO_DIFF_COLOR;
        if (this.equipData && this.equipData.ArcGap > 0) {
            var arc2:Number = this.equipData.ArcGap;
            color = TooltipHelper.getTextColor(arc2 - arc);
        }
        else if (this.usableBy)
            color = TooltipHelper.BETTER_COLOR;
        this.attributes += "Arc Gap: " + TooltipHelper.wrapInFontTag(String(arc), color) + "\n";
    }

    private function makeProjProperties(proj:ProjectileDesc, proj2:ProjectileDesc):void {
        if (proj.ArmorPiercing)
            this.attributes += TooltipHelper.wrapInFontTag("Shots ignore defense of target", TooltipHelper.SPECIAL_COLOR) + "\n";
        if (proj.Boomerang)
            this.attributes += TooltipHelper.wrapInFontTag("Shots boomerang", TooltipHelper.NO_DIFF_COLOR) + "\n";
        if (proj.MultiHit)
            this.attributes += TooltipHelper.wrapInFontTag("Shots hit multiple targets", TooltipHelper.NO_DIFF_COLOR) + "\n";
        if (proj.PassesCover)
            this.attributes += TooltipHelper.wrapInFontTag("Shots pass through obstacles", TooltipHelper.NO_DIFF_COLOR) + "\n";
        if (proj.Parametric)
            this.attributes += TooltipHelper.wrapInFontTag("Shots are parametric", TooltipHelper.NO_DIFF_COLOR) + "\n";
    }

    private function makeActivateEffects():void {
        var effs:Vector.<ActivateEffect> = new Vector.<ActivateEffect>();
        for each (var ae:ActivateEffect in this.itemData.ActivateEffects) {
            if (!ae.EffectName || ae.EffectName == "" || IGNORE_AE.indexOf(ae.EffectName) != -1)
                continue;
            effs.push(ae);
        }

        if (effs.length < 1)
            return;

        this.attributes += "On Use:\n";
        this.attributes += "<span class=\'aeIn\'>";
        for each (ae in effs) {
            if (ae.DosesReq > 0)
                this.attributes += TooltipHelper.wrapInFontTag("(Requires at least " + ae.DosesReq + " doses)", "#E0761E");
            if (ae.NodeReq != -1)
                this.attributes += TooltipHelper.wrapInFontTag("(Requires blessing)", "#E0761E");
            if (ae.DosesReq > 0 || ae.NodeReq != -1)
                this.attributes += "\n";
            this.attributes += "-";
            var statColor:String = TooltipHelper.NO_DIFF_COLOR;
            var amountColor:String = TooltipHelper.NO_DIFF_COLOR;
            var rangeColor:String = TooltipHelper.NO_DIFF_COLOR;
            var durationColor:String = TooltipHelper.NO_DIFF_COLOR;
            var conditionColor:String = TooltipHelper.NO_DIFF_COLOR;
            var totalDmgColor:String = TooltipHelper.NO_DIFF_COLOR;
            var radiusColor:String = TooltipHelper.NO_DIFF_COLOR;
            var impactDmgColor:String = TooltipHelper.NO_DIFF_COLOR;
            var condDurationColor:String = TooltipHelper.NO_DIFF_COLOR;
            var maxTargetsColor:String = TooltipHelper.NO_DIFF_COLOR;
            var healAmountColor:String = TooltipHelper.NO_DIFF_COLOR;
            var stat:String = Stats.fromId(ae.Stats);
            var amount:int = ae.Amount;
            var range:Number = ae.Range;
            var duration:Number = ae.DurationSec;
            var condition:String = ae.ConditionEffect;
            var totalDamage:int = ae.TotalDamage;
            var radius:Number = ae.Radius;
            var impactDmg:int = ae.ImpactDmg;
            var condDuration:Number = ae.EffectDuration;
            var maxTargets:int = ae.MaxTargets;
            var healAmount:int = ae.HealAmount;
            var wisModAmount:int = ApplyWisMod(amount, this.player, 0);
            var wisModRange:Number = ApplyWisMod(range, this.player);
            var wisModDuration:Number = ApplyWisMod(duration, this.player);
            var wisModTotalDamage:Number = ApplyWisMod(totalDamage, this.player, 0);
            var wisModRadius:Number = ApplyWisMod(radius, this.player);
            var wisModCondDuration:Number = ApplyWisMod(condDuration, this.player);
            var wisModMaxTargets:Number = ApplyWisMod(maxTargets, this.player);
            var wisModHealAmount:Number = ApplyWisMod(healAmount, this.player);
            var ae2:ActivateEffect;
            var stat2:String;
            var amount2:int;
            var range2:Number;
            var duration2:Number;
            var totalDamage2:int;
            var radius2:Number;
            var impactDmg2:int;
            var condDuration2:Number;
            var maxTargets2:int;
            var healAmount2:int;
            var wisModAmount2:int;
            var wisModRange2:Number;
            var wisModDuration2:Number;
            var wisModTotalDamage2:Number;
            var wisModRadius2:Number;
            var wisModCondDuration2:Number;
            var wisModMaxTargets2:Number;
            var wisModHealAmount2:Number;
            if (this.equipData && this.equipData.ActivateEffects && this.equipData.ActivateEffects.length > 0) {
                var matchId:int = GetMatchId(ae, this.itemData.ActivateEffects);
                ae2 = GetAE(ae.EffectName, matchId, this.equipData.ActivateEffects);
                if (ae2) {
                    stat2 = Stats.fromId(ae2.Stats);
                    amount2 = ae2.Amount;
                    range2 = ae2.Range;
                    duration2 = ae2.DurationSec;
                    totalDamage2 = ae2.TotalDamage;
                    radius2 = ae2.Radius;
                    impactDmg2 = ae2.ImpactDmg;
                    condDuration2 = ae2.EffectDuration;
                    maxTargets2 = ae2.MaxTargets;
                    healAmount2 = ae2.HealAmount;
                    wisModAmount2 = ApplyWisMod(amount2, this.player, 0);
                    wisModRange2 = ApplyWisMod(range2, this.player);
                    wisModDuration2 = ApplyWisMod(duration2, this.player);
                    wisModTotalDamage2 = ApplyWisMod(totalDamage2, this.player, 0);
                    wisModRadius2 = ApplyWisMod(radius2, this.player);
                    wisModCondDuration2 = ApplyWisMod(condDuration2, this.player);
                    wisModMaxTargets2 = ApplyWisMod(maxTargets2, this.player);
                    wisModHealAmount2 = ApplyWisMod(healAmount2, this.player);
                    if (!HasAEStat(stat, ae.EffectName, this.equipData.ActivateEffects)) {
                        statColor = TooltipHelper.BETTER_COLOR;
                    }
                    if (!HasAECondition(condition, ae.EffectName, this.equipData.ActivateEffects)) {
                        conditionColor = TooltipHelper.BETTER_COLOR;
                    }
                    if (ae.UseWisMod) {
                        if (ae2.UseWisMod) {
                            amountColor = TooltipHelper.getTextColor(wisModAmount - wisModAmount2);
                            rangeColor = TooltipHelper.getTextColor(wisModRange - wisModRange2);
                            durationColor = TooltipHelper.getTextColor(wisModDuration - wisModDuration2);
                            totalDmgColor = TooltipHelper.getTextColor(wisModTotalDamage - wisModTotalDamage2);
                            radiusColor = TooltipHelper.getTextColor(wisModRadius - wisModRadius2);
                            impactDmgColor = TooltipHelper.getTextColor(impactDmg - impactDmg2);
                            condDurationColor = TooltipHelper.getTextColor(wisModCondDuration - wisModCondDuration2);
                            maxTargetsColor = TooltipHelper.getTextColor(wisModMaxTargets - wisModMaxTargets2);
                            healAmountColor = TooltipHelper.getTextColor(wisModHealAmount - wisModHealAmount2);
                        }
                        else {
                            amountColor = TooltipHelper.getTextColor(wisModAmount - amount2);
                            rangeColor = TooltipHelper.getTextColor(wisModRange - range2);
                            durationColor = TooltipHelper.getTextColor(wisModDuration - duration2);
                            totalDmgColor = TooltipHelper.getTextColor(wisModTotalDamage - totalDamage2);
                            radiusColor = TooltipHelper.getTextColor(wisModRadius - radius2);
                            impactDmgColor = TooltipHelper.getTextColor(impactDmg - impactDmg2);
                            condDurationColor = TooltipHelper.getTextColor(wisModCondDuration - condDuration2);
                            maxTargetsColor = TooltipHelper.getTextColor(wisModMaxTargets - maxTargets2);
                            healAmountColor = TooltipHelper.getTextColor(wisModHealAmount - healAmount2);
                        }
                    }
                    else {
                        if (ae2.UseWisMod) {
                            amountColor = TooltipHelper.getTextColor(amount - wisModAmount2);
                            rangeColor = TooltipHelper.getTextColor(range - wisModRange2);
                            durationColor = TooltipHelper.getTextColor(duration - wisModDuration2);
                            totalDmgColor = TooltipHelper.getTextColor(totalDamage - wisModTotalDamage2);
                            radiusColor = TooltipHelper.getTextColor(radius - wisModRadius2);
                            impactDmgColor = TooltipHelper.getTextColor(impactDmg - impactDmg2);
                            condDurationColor = TooltipHelper.getTextColor(condDuration - wisModCondDuration2);
                            maxTargetsColor = TooltipHelper.getTextColor(maxTargets - wisModMaxTargets2);
                            healAmountColor = TooltipHelper.getTextColor(healAmount - wisModHealAmount2);
                        }
                        else {
                            amountColor = TooltipHelper.getTextColor(amount - amount2);
                            rangeColor = TooltipHelper.getTextColor(range - range2);
                            durationColor = TooltipHelper.getTextColor(duration - duration2);
                            totalDmgColor = TooltipHelper.getTextColor(totalDamage - totalDamage2);
                            radiusColor = TooltipHelper.getTextColor(radius - radius2);
                            impactDmgColor = TooltipHelper.getTextColor(impactDmg - impactDmg2);
                            condDurationColor = TooltipHelper.getTextColor(condDuration - condDuration2);
                            maxTargetsColor = TooltipHelper.getTextColor(maxTargets - maxTargets2);
                            healAmountColor = TooltipHelper.getTextColor(healAmount - healAmount2);
                        }
                    }
                }
            }
            if (!this.usableBy) {
                statColor = TooltipHelper.NO_DIFF_COLOR;
                amountColor = TooltipHelper.NO_DIFF_COLOR;
                rangeColor = TooltipHelper.NO_DIFF_COLOR;
                durationColor = TooltipHelper.NO_DIFF_COLOR;
                conditionColor = TooltipHelper.NO_DIFF_COLOR;
                totalDmgColor = TooltipHelper.NO_DIFF_COLOR;
                radiusColor = TooltipHelper.NO_DIFF_COLOR;
                impactDmgColor = TooltipHelper.NO_DIFF_COLOR;
                condDurationColor = TooltipHelper.NO_DIFF_COLOR;
                maxTargetsColor = TooltipHelper.NO_DIFF_COLOR;
                healAmountColor = TooltipHelper.NO_DIFF_COLOR;
            }
            else if (!ae2) {
                statColor = TooltipHelper.BETTER_COLOR;
                amountColor = TooltipHelper.BETTER_COLOR;
                rangeColor = TooltipHelper.BETTER_COLOR;
                durationColor = TooltipHelper.BETTER_COLOR;
                conditionColor = TooltipHelper.BETTER_COLOR;
                totalDmgColor = TooltipHelper.BETTER_COLOR;
                radiusColor = TooltipHelper.BETTER_COLOR;
                impactDmgColor = TooltipHelper.BETTER_COLOR;
                condDurationColor = TooltipHelper.BETTER_COLOR;
                maxTargetsColor = TooltipHelper.BETTER_COLOR;
                healAmountColor = TooltipHelper.BETTER_COLOR;
            }
            switch (ae.EffectName) {
                case ActivationType.GENERIC_ACTIVATE:
                    this.attributes += BuildGenericAE(
                            ae, ae.UseWisMod,
                            duration, wisModDuration, durationColor,
                            range, wisModRange, rangeColor,
                            condition, conditionColor
                    );
                    break;
                case ActivationType.INCREMENT_STAT:
                    this.attributes += "Increases " + TooltipHelper.wrapInFontTag(stat, TooltipHelper.NO_DIFF_COLOR) + " by " + TooltipHelper.wrapInFontTag(String(amount), TooltipHelper.NO_DIFF_COLOR);
                    break;
                case ActivationType.HEAL:
                    if (ae.UseWisMod && wisModAmount != amount)
                        this.attributes += "Heals " + GetWisModText(amount, wisModAmount, amountColor) + " HP";
                    else
                        this.attributes += "Heals " + TooltipHelper.wrapInFontTag(String(amount), amountColor) + " HP";
                    break;
                case ActivationType.MAGIC:
                    if (ae.UseWisMod && wisModAmount != amount)
                        this.attributes += "Heals " + GetWisModText(amount, wisModAmount, amountColor) + " MP";
                    else
                        this.attributes += "Heals " + TooltipHelper.wrapInFontTag(String(amount), amountColor) + " MP";
                    break;
                case ActivationType.HEAL_NOVA:
                    if (ae.UseWisMod && (wisModAmount != amount || wisModRange != range))
                        this.attributes += "Heals " + GetWisModText(amount, wisModAmount, amountColor) + " in " + GetWisModText(range, wisModRange, rangeColor) + " sqrs";
                    else
                        this.attributes += "Heals " + TooltipHelper.wrapInFontTag(String(amount), amountColor) + " HP in " + TooltipHelper.wrapInFontTag(String(range), rangeColor) + " sqrs";
                    break;
                case ActivationType.STAT_BOOST_SELF:
                    if (ae.UseWisMod && (wisModAmount != amount || wisModDuration != duration))
                        this.attributes += "On Self: " + GetSign(amount) + GetWisModText(amount, wisModAmount, amountColor) + " " + TooltipHelper.wrapInFontTag(stat, statColor) + " for " + GetWisModText(duration, wisModDuration, durationColor) + " secs";
                    else
                        this.attributes += "On Self: " + GetSign(amount) + TooltipHelper.wrapInFontTag(String(amount), amountColor) + " " + TooltipHelper.wrapInFontTag(stat, statColor) + " for " + TooltipHelper.wrapInFontTag(String(duration), durationColor) + " secs";
                    break;
                case ActivationType.STAT_BOOST_AURA:
                    if (ae.UseWisMod && (wisModAmount != amount || wisModDuration != duration || wisModRange != range))
                        this.attributes += "On Allies: " + GetSign(amount) + GetWisModText(amount, wisModAmount, amountColor) + " " + TooltipHelper.wrapInFontTag(stat, statColor) + " in " + GetWisModText(range, wisModRange, rangeColor) + " sqrs for " + GetWisModText(duration, wisModDuration, durationColor) + " secs";
                    else
                        this.attributes += "On Allies: " + GetSign(amount) + TooltipHelper.wrapInFontTag(String(amount), amountColor) + " " + TooltipHelper.wrapInFontTag(stat, statColor) + " in " + TooltipHelper.wrapInFontTag(String(range), rangeColor) + " sqrs for " + TooltipHelper.wrapInFontTag(String(duration), durationColor) + " secs";
                    break;
                case ActivationType.BULLET_NOVA:
                    this.attributes += TooltipHelper.wrapInFontTag("Spell: ", TooltipHelper.SPECIAL_COLOR) + TooltipHelper.wrapInFontTag("20", TooltipHelper.NO_DIFF_COLOR) + " shots";
                    break;
                case ActivationType.COND_EFFECT_SELF:
                    if (ae.UseWisMod && wisModDuration != duration)
                        this.attributes += "On Self: " + TooltipHelper.wrapInFontTag(condition, conditionColor) + " for " + GetWisModText(duration, wisModDuration, durationColor) + " secs";
                    else
                        this.attributes += "On Self: " + TooltipHelper.wrapInFontTag(condition, conditionColor) + " for " + TooltipHelper.wrapInFontTag(String(duration), durationColor) + " secs";
                    break;
                case ActivationType.COND_EFFECT_AURA:
                    if (ae.UseWisMod && (wisModDuration != duration || wisModRange != range))
                        this.attributes += "On Allies: " + TooltipHelper.wrapInFontTag(condition, conditionColor) + " in " + GetWisModText(range, wisModRange, rangeColor) + " sqrs for " + GetWisModText(duration, wisModDuration, durationColor) + " secs";
                    else
                        this.attributes += "On Allies: " + TooltipHelper.wrapInFontTag(condition, conditionColor) + " in " + TooltipHelper.wrapInFontTag(String(range), rangeColor) + " sqrs for " + TooltipHelper.wrapInFontTag(String(duration), durationColor) + " secs";
                    break;
                case ActivationType.TELEPORT:
                    this.attributes += "Teleports to cursor";
                    break;
                case ActivationType.POISON_GRENADE:
                    if (ae.UseWisMod && (wisModTotalDamage != totalDamage))
                        this.attributes += "Poison: Deals " + GetWisModText(totalDamage, wisModTotalDamage, totalDmgColor) + " damage (" + TooltipHelper.wrapInFontTag(String(impactDmg), impactDmgColor) + " on impact) in " + TooltipHelper.wrapInFontTag(String(radius), radiusColor) + " sqrs for " + TooltipHelper.wrapInFontTag(String(duration), durationColor) + " secs";
                    else
                        this.attributes += "Poison: Deals " + TooltipHelper.wrapInFontTag(String(totalDamage), totalDmgColor) + " damage (" + TooltipHelper.wrapInFontTag(String(impactDmg), impactDmgColor) + " on impact) in " + TooltipHelper.wrapInFontTag(String(radius), radiusColor) + " sqrs for " + TooltipHelper.wrapInFontTag(String(duration), durationColor) + " secs";
                    break;
                case ActivationType.VAMPIRE_BLAST:
                    if (ae.UseWisMod && (wisModTotalDamage != totalDamage || wisModRadius != radius))
                        this.attributes += "Skull: Heals " + TooltipHelper.wrapInFontTag(String(healAmount), healAmountColor) + " HP dealing " + GetWisModText(totalDamage, wisModTotalDamage, totalDmgColor) + " damage in " + GetWisModText(radius, wisModRadius, radiusColor) + " sqrs";
                    else
                        this.attributes += "Skull: Heals " + TooltipHelper.wrapInFontTag(String(healAmount), healAmountColor) + " HP dealing " + TooltipHelper.wrapInFontTag(String(totalDamage), totalDmgColor) + " damage in " + TooltipHelper.wrapInFontTag(String(radius), radiusColor) + " sqrs";
                    break;
                case ActivationType.TRAP:
                    if (ae.UseWisMod && (wisModTotalDamage != totalDamage || wisModRadius != radius || wisModCondDuration != condDuration)) {
                        this.attributes += "Trap: Deals " + GetWisModText(totalDamage, wisModTotalDamage, totalDmgColor) + " damage in " + GetWisModText(radius, wisModRadius, radiusColor) + " sqrs\n";
                        this.attributes += "    Applies " + TooltipHelper.wrapInFontTag(!condition ? "Slowed" : condition, conditionColor) + " for " + GetWisModText(condDuration, wisModCondDuration, condDurationColor) + " secs";
                    }
                    else {
                        this.attributes += "Trap: Deals " + TooltipHelper.wrapInFontTag(String(totalDamage), totalDmgColor) + " damage in " + TooltipHelper.wrapInFontTag(String(radius), radiusColor) + " sqrs\n";
                        this.attributes += "    Applies " + TooltipHelper.wrapInFontTag(!condition ? "Slowed" : condition, conditionColor) + " for " + TooltipHelper.wrapInFontTag(String(condDuration), condDurationColor) + " secs";
                    }
                    break;
                case ActivationType.STASIS_BLAST:
                    if (ae.UseWisMod && (wisModDuration != duration))
                        this.attributes += "Stasies enemies within 3 sqrs for " + GetWisModText(duration, wisModDuration, durationColor) + " secs";
                    else
                        this.attributes += "Stasies enemies within 3 sqrs for " + TooltipHelper.wrapInFontTag(String(duration), durationColor) + " secs";
                    break;
                case ActivationType.DECOY:
                    this.attributes += "Decoy: Lasts for " + TooltipHelper.wrapInFontTag(String(duration), durationColor) + " secs";
                    break;
                case ActivationType.LIGHTNING:
                    if (ae.UseWisMod && (wisModMaxTargets != maxTargets || wisModTotalDamage != totalDamage)) {
                        this.attributes += "Lightning: Targets " + GetWisModText(maxTargets, wisModMaxTargets, maxTargetsColor) + " enemies dealing " + GetWisModText(totalDamage, wisModTotalDamage, totalDmgColor) + " damage";
                        if (condition)
                            this.attributes += "\n    Applies " + TooltipHelper.wrapInFontTag(condition, conditionColor) + " for " + TooltipHelper.wrapInFontTag(String(condDuration), condDurationColor) + " secs";
                    }
                    else {
                        this.attributes += "Lightning: Targets " + TooltipHelper.wrapInFontTag(String(maxTargets), maxTargetsColor) + " enemies dealing " + TooltipHelper.wrapInFontTag(String(totalDamage), totalDmgColor) + " damage";
                        if (condition)
                            this.attributes += "\n    Applies " + TooltipHelper.wrapInFontTag(condition, conditionColor) + " for " + TooltipHelper.wrapInFontTag(String(condDuration), condDurationColor) + " secs";
                    }
                    break;
                case ActivationType.MAGIC_NOVA:
                    if (ae.UseWisMod && (wisModAmount != amount || wisModRange != range))
                        this.attributes += "Heals " + GetWisModText(amount, wisModAmount, amountColor) + " MP in " + GetWisModText(range, wisModRange, rangeColor) + " sqrs";
                    else
                        this.attributes += "Heals " + TooltipHelper.wrapInFontTag(String(amount), amountColor) + " MP in " + TooltipHelper.wrapInFontTag(String(range), rangeColor) + " sqrs";
                    break;
                case ActivationType.CLEAR_COND_EFFECT_AURA:
                    this.attributes += "Removes all condition effects from allies in " + TooltipHelper.wrapInFontTag(String(range), rangeColor) + " sqrs";
                    break;
                case ActivationType.REMOVE_NEG_COND:
                    this.attributes += "Removes all negative condition effects from allies in " + TooltipHelper.wrapInFontTag(String(range), rangeColor) + " sqrs";
                    break;
                case ActivationType.CLEAR_COND_EFFECT_SELF:
                    this.attributes += "Removes all condition effects";
                    break;
                case ActivationType.REMOVE_NEG_COND_SELF:
                    this.attributes += "Removes all negative condition effects";
                    break;
            }
            if (!LastElement(ae, effs))
                this.attributes += "\n";
        }
        this.attributes += "</span>\n";
    }

    private function makeStatBoosts():void {
        if (!this.itemData.StatsBoosts || this.itemData.StatsBoosts.length < 1)
            return;

        this.attributes += "On Equip:\n";
        var amountColor:String = TooltipHelper.NO_DIFF_COLOR;
        if (this.usableBy) {
            amountColor = TooltipHelper.BETTER_COLOR;
        }
        for each (var statBoost:StatBoost in this.itemData.StatsBoosts) {
            var stat:String = Stats.fromId(statBoost.Stat);
            var amount:int = statBoost.Amount;
            if (this.usableBy) {
                amountColor = TooltipHelper.BETTER_COLOR;
            }
            else amountColor = TooltipHelper.NO_DIFF_COLOR;
            if (this.equipData && this.equipData.StatsBoosts && this.equipData.StatsBoosts.length > 0) {
                var statBoost2:StatBoost = GetStatBoost(this.equipData.StatsBoosts, statBoost.Stat);
                if (statBoost2) {
                    var amount2:int = statBoost2.Amount;
                    amountColor = TooltipHelper.getTextColor(amount - amount2);
                }
                else if (amount < 0) {
                    amountColor = TooltipHelper.WORSE_COLOR;
                }
            }
            else if (amount < 0) {
                amountColor = TooltipHelper.WORSE_COLOR;
            }
            this.attributes += "    ";
            this.attributes += TooltipHelper.wrapInFontTag(GetSign(amount) + amount, amountColor) + " " + TooltipHelper.wrapInFontTag(stat, amountColor) + "\n";
        }
    }

    private function makeGlobalAttributes():void {
        if (this.itemData.Doses > 0)
            this.makeItemDoses();
        if (this.itemData.FameBonus > 0)
            this.makeItemFameBonus();
        if (!this.itemData.MultiPhase && this.itemData.MpCost != -1)
            this.makeItemMpCost();
        if (this.itemData.MultiPhase)
            this.makeItemMpEndCost();
        if (this.itemData.Usable)
            this.makeItemCooldown();
        if (this.itemData.Resurrects)
            this.attributes += "This item resurrects you from death\n";
    }

    private function makeItemDoses():void {
        var maxDoses:int = this.itemData.MaxDoses;
        var doses:int = this.itemData.Doses;
        var color:String = TooltipHelper.NO_DIFF_COLOR;
        if (this.equipData && this.equipData.Doses > 0 && this.equipData.DisplayId == this.itemData.DisplayId) {
            var doses2:int = this.equipData.Doses;
            color = TooltipHelper.getTextColor(doses - doses2);
        }
        else if (this.usableBy)
            color = TooltipHelper.BETTER_COLOR;
        this.attributes += "Max Doses: " + TooltipHelper.wrapInFontTag(String(maxDoses), TooltipHelper.NO_DIFF_COLOR) + "\n";
        this.attributes += "Doses: " + TooltipHelper.wrapInFontTag(String(doses), color) + "\n";
    }

    private function makeItemFameBonus():void {
        var fame:int = this.itemData.FameBonus;
        var color:String = TooltipHelper.NO_DIFF_COLOR;
        if (this.equipData && this.equipData.FameBonus > 0) {
            var fame2:int = this.equipData.FameBonus;
            color = TooltipHelper.getTextColor(fame - fame2);
        }
        else if (this.usableBy)
            color = TooltipHelper.BETTER_COLOR;
        this.attributes += "Fame Bonus: " + TooltipHelper.wrapInFontTag(fame + "%", color) + "\n";
    }

    private function makeItemMpCost():void {
        if (!this.itemData.Usable)
            return;
        var cost:int = this.itemData.MpCost;
        var color:String = TooltipHelper.NO_DIFF_COLOR;
        if (this.equipData && this.equipData.MpCost > 0) {
            var cost2:int = this.equipData.MpCost;
            color = TooltipHelper.getTextColor(cost2 - cost);
        }
        else if (this.usableBy)
            color = TooltipHelper.BETTER_COLOR;
        this.attributes += "MP Cost: " + TooltipHelper.wrapInFontTag(String(cost), color) + "\n";
    }

    private function makeItemMpEndCost():void {
        if (!this.itemData.Usable)
            return;
        var cost:int = this.itemData.MpEndCost;
        var color:String = TooltipHelper.NO_DIFF_COLOR;
        if (this.equipData && this.equipData.MpEndCost > 0) {
            var cost2:int = this.equipData.MpEndCost;
            color = TooltipHelper.getTextColor(cost2 - cost);
        }
        else if (this.usableBy)
            color = TooltipHelper.BETTER_COLOR;
        this.attributes += "MP Cost: " + TooltipHelper.wrapInFontTag(String(cost), color) + "\n";
    }

    private function makeItemCooldown():void {
        var cd:Number = this.itemData.Cooldown;
        var color:String = TooltipHelper.NO_DIFF_COLOR;
        if (this.equipData && this.equipData.Usable) {
            var cd2:Number = this.equipData.Cooldown;
            color = TooltipHelper.getTextColor(cd2 - cd);
        }
        else if (this.usableBy)
            color = TooltipHelper.BETTER_COLOR;
        this.attributes += "Cooldown: " + TooltipHelper.wrapInFontTag(cd + " secs", color) + "\n";
    }

    private function drawAttributes():void {
        if (this.attributes.length <= 0)
            return;

        if (!this.line1)
            this.drawLine1();
        var sheet:StyleSheet = new StyleSheet();
        sheet.parseCSS(CSS_TEXT);
        this.attributesText = new SimpleText(14, 0xB3B3B3, false, WIDTH - 10);
        this.attributesText.styleSheet = sheet;
        this.attributesText.wordWrap = true;
        this.attributesText.htmlText = this.attributes;
        this.attributesText.useTextDimensions();
        if (Parameters.data_.toolTipOutline)
            this.attributesText.filters = FilterUtil.getTextOutlineFilter();
        else
            this.attributesText.filters = FilterUtil.getTextShadowFilter();
        if (!this.customText) {
            this.attributesText.x = this.descText.x;
            this.attributesText.y = this.line1.y + this.line1.height + 3;
        }
        else {
            this.attributesText.x = this.descText.x;
            this.attributesText.y = this.customText.y + this.customText.height - 3;
        }
        addChild(this.attributesText);
    }

    private function drawLine1():void {
        var color:uint = this.usableBy ? 0x828282 : 10965039;
        this.line1 = new Sprite();
        this.line1.x = 2;
        this.line1.y = this.descText.y + this.descText.height + 4;
        this.line1.graphics.lineStyle(2, color);
        this.line1.graphics.lineTo(WIDTH - 2, 0);
        this.line1.graphics.lineStyle();
        addChild(this.line1);
    }

    private function makeSpecifications():void {
        this.specifications = "";
        if (this.itemData.Soulbound)
            this.specifications += TooltipHelper.wrapInFontTag("Soulbound\n", TooltipHelper.SPECIAL_COLOR);
        this.makeUsableBy();
        if (this.itemData.Usable)
            this.specifications += TooltipHelper.wrapInFontTag("Press [" + KeyCodes.CharCodeStrings[Parameters.data_.useSpecial] + "] in world to use\n", "#DEDAEB");
        if (ItemConstants.isEquippable(this.itemData.SlotType)) {
            this.specifications += "Must be equipped to use\n";
            this.specifications += "Double-Click to equip\n";
        }
        if (this.itemData.Consumable)
            this.specifications += "Double-Click to consume\n";
    }

    private function makeUsableBy():void {
        if (!ItemConstants.isEquippable(this.itemData.SlotType))
            return;
        if (!this.usableBy) {
            this.specifications += "<b>" + TooltipHelper.wrapInFontTag("Not usable by " + ObjectLibrary.typeToDisplayId_[this.player.objectType_], "#FC8642") + "</b>\n";
        }
        var usableBy:Vector.<String> = ObjectLibrary.usableBy(this.itemData.ObjectType);
        if (usableBy) {
            this.specifications += "Usable by: " + usableBy.join(", ") + "\n";
        }
    }

    private function drawSpecifications():void {
        if (this.specifications.length <= 0)
            return;

        this.drawLine2();
        var sheet:StyleSheet = new StyleSheet();
        sheet.parseCSS(CSS_TEXT);
        this.specificationsText = new SimpleText(14, 0xB3B3B3, false, WIDTH - 10);
        this.specificationsText.styleSheet = sheet;
        this.specificationsText.wordWrap = true;
        this.specificationsText.htmlText = this.specifications;
        this.specificationsText.useTextDimensions();
        if (Parameters.data_.toolTipOutline)
            this.specificationsText.filters = FilterUtil.getTextOutlineFilter();
        else
            this.specificationsText.filters = FilterUtil.getTextShadowFilter();
        this.specificationsText.x = this.descText.x;
        this.specificationsText.y = this.line2.y + this.line2.height + 3;
        addChild(this.specificationsText);
    }

    private function drawLine2():void {
        var color:uint = this.usableBy ? 0x828282 : 10965039;
        this.line2 = new Sprite();
        this.line2.x = 2;
        if (this.attributesText)
            this.line2.y = this.attributesText.y + this.attributesText.height + 4;
        else if (this.customText)
            this.line2.y = this.customText.y + this.customText.height + 4;
        else
            this.line2.y = this.descText.y + this.descText.height + 4;
        this.line2.graphics.lineStyle(2, color);
        this.line2.graphics.lineTo(WIDTH - 2, 0);
        this.line2.graphics.lineStyle();
        addChild(this.line2);
    }

    private static function GetEquipIndex(slotType:int, items:Vector.<ItemData>):int {
        for (var i:int = 0; i < 4; i++)
            if (items[i] && items[i].SlotType == slotType)
                return i;
        return -1
    }

    private static function GetConditionEffectIndex(eff:int, effects:Vector.<CondEffect>):int {
        for (var i:int = 0; i < effects.length; i++)
            if (effects[i].Effect == eff)
                return i;
        return -1;
    }

    private static function GetBagTexture(bagType:int, size:int):BitmapData {
        switch (bagType) {
            case 0: // Brown
                return ObjectLibrary.getRedrawnTextureFromType(0x0500, size, true);
            case 1: // Pink
                return ObjectLibrary.getRedrawnTextureFromType(0x0506, size, true);
            case 2: // Purple
                return ObjectLibrary.getRedrawnTextureFromType(0x0507, size, true);
            case 3: // Cyan
                return ObjectLibrary.getRedrawnTextureFromType(0x0508, size, true);
            case 4: // Blue
                return ObjectLibrary.getRedrawnTextureFromType(0x0509, size, true);
            case 5: // White
                return ObjectLibrary.getRedrawnTextureFromType(0x0510, size, true);
        }
        return null;
    }

    private static function IsUsableBy(player:Player, slotType:int):Boolean {
        if (!player || !ItemConstants.isEquippable(slotType))
            return true;
        return player.slotTypes_.indexOf(slotType) != -1;
    }

    private static function ApplyWisMod(value:Number, player:Player, offset:int = 1):Number {
        if (!player)
            return value;

        var wisdom:Number = Math.min(player.wisdom_, MAX_WISMOD);
        if (wisdom < MIN_WISMOD)
            return value;
        else {
            var m:int = value < 0 ? -1 : 1;
            var n:Number = ((value * (wisdom - 20)) / 150) + (value * m);
            n = Math.floor(n * Math.pow(10, offset)) / Math.pow(10, offset);
            if (n - (int(n) * m) >= (1 / Math.pow(10, offset)) * m)
                return Math.round(MathUtil2.roundTo(n, 1));
            return int(n);
        }
    }

    private static function GetMatches(eff:String, effects:Vector.<ActivateEffect>):Vector.<ActivateEffect> {
        var matches:Vector.<ActivateEffect> = new Vector.<ActivateEffect>();
        for (var i:int = 0; i < effects.length; i++) {
            if (effects[i].EffectName == eff)
                matches.push(effects[i]);
        }
        return matches;
    }

    private static function GetMatchId(eff:ActivateEffect, effects:Vector.<ActivateEffect>):int {
        var matches:Vector.<ActivateEffect> = GetMatches(eff.EffectName, effects);
        for each (var match:ActivateEffect in matches)
            if (match == eff)
                return matches.indexOf(match);
        return -1;
    }

    private static function GetAE(eff:String, matchId:int, effects:Vector.<ActivateEffect>):ActivateEffect {
        var matches:Vector.<ActivateEffect> = GetMatches(eff, effects);
        if (matches.length < matchId + 1)
            return null;
        return matches[matchId];
    }

    private static function HasAEStat(stat:String, effName:String, effects:Vector.<ActivateEffect>):Boolean {
        if (!effects || effects.length < 1)
            return false;

        for each (var ae:ActivateEffect in effects) {
            if (!ae.EffectName || ae.EffectName == "" || IGNORE_AE.indexOf(ae.EffectName) != -1 || ae.EffectName != effName)
                continue;
            if (Stats.fromId(ae.Stats) == stat)
                return true;
        }
        return false;
    }

    private static function GetWisModText(val:Number, wisModVal:Number, color:String):String {
        return TooltipHelper.wrapInFontTag(String(wisModVal), color) + TooltipHelper.wrapInFontTag(" (+" + (wisModVal - val) + ")", TooltipHelper.WISMOD_COLOR);
    }

    private static function LastElement(elem:*, arr:*):Boolean {
        return arr.indexOf(elem) == arr.length - 1;
    }

    private static function GetSign(val:int):String {
        if (val > 0)
            return "+"; // YEP
        return "";
    }

    private static function GetStatBoost(boosts:Vector.<StatBoost>, stat:int):StatBoost {
        for each (var boost:StatBoost in boosts)
            if (boost.Stat == stat)
                return boost;
        return null;
    }

    private static function BuildGenericAE(eff:ActivateEffect, useWisMod:Boolean,
                                           duration:Number, wisModDuration:Number, durationColor:String,
                                           range:Number, wisModRange:Number, rangeColor:String,
                                           condition:String, conditionColor:String):String {
        var ret:String = "";
        var targetPlayer:Boolean = eff.Target == "player";
        var aimAtCursor:Boolean = eff.Center == "mouse";
        if (!targetPlayer)
            ret += "On enemies: ";
        else ret += "On allies: ";
        ret += TooltipHelper.wrapInFontTag(condition, conditionColor) + " within ";
        if (useWisMod)
            ret += GetWisModText(range, wisModRange, rangeColor);
        else ret += TooltipHelper.wrapInFontTag(String(range), rangeColor);
        ret += " sqrs";
        if (aimAtCursor)
            ret += " at cursor";
        ret += " for ";
        if (useWisMod)
            ret += GetWisModText(duration, wisModDuration, durationColor);
        else ret += TooltipHelper.wrapInFontTag(String(duration), durationColor);
        ret += " secs";
        return ret;
    }

    private static function HasAECondition(condition:String, effName:String, effects:Vector.<ActivateEffect>):Boolean {
        if (!effects || effects.length < 1)
            return false;

        for each (var ae:ActivateEffect in effects) {
            if (!ae.EffectName || ae.EffectName == "" || IGNORE_AE.indexOf(ae.EffectName) != -1 || ae.EffectName != effName)
                continue;
            if (ae.ConditionEffect == condition)
                return true;
        }
        return false;
    }
}
}
