package com.company.assembleegameclient.util
{
import com.company.assembleegameclient.util.redrawers.GlowRedrawer;
import com.company.util.AssetLibrary;
   import com.company.util.PointUtil;
   import flash.display.BitmapData;
   import flash.filters.BitmapFilterQuality;
   import flash.filters.GlowFilter;
   import flash.geom.Matrix;
   
   public class ConditionEffect
   {
      public static const NOTHING:uint = 0;
      
      public static const QUIET:uint = 1;
      
      public static const WEAK:uint = 2;
      
      public static const SLOWED:uint = 3;
      
      public static const SICK:uint = 4;
      
      public static const DAZED:uint = 5;
      
      public static const STUNNED:uint = 6;

      public static const BLIND:uint = 7;
      
      public static const HALLUCINATING:uint = 8;
      
      public static const DRUNK:uint = 9;
      
      public static const CONFUSED:uint = 10;
      
      public static const STUN_IMMUNE:uint = 11;
      
      public static const INVISIBLE:uint = 12;
      
      public static const PARALYZED:uint = 13;
      
      public static const SPEEDY:uint = 14;
      
      public static const BLEEDING:uint = 15;
      
      public static const HEALING:uint = 16;
      
      public static const DAMAGING:uint = 17;
      
      public static const BERSERK:uint = 18;
      
      public static const STASIS:uint = 19;
      
      public static const STASIS_IMMUNE:uint = 20;
      
      public static const INVINCIBLE:uint = 21;
      
      public static const INVULNERABLE:uint = 22;
      
      public static const ARMORED:uint = 23;
      
      public static const ARMORBROKEN:uint = 24;
      
      public static const HEXED:uint = 25;
      
      public static const QUIET_BIT:uint = 1 << QUIET;
      
      public static const WEAK_BIT:uint = 1 << WEAK;
      
      public static const SLOWED_BIT:uint = 1 << SLOWED;
      
      public static const SICK_BIT:uint = 1 << SICK;
      
      public static const DAZED_BIT:uint = 1 << DAZED;
      
      public static const STUNNED_BIT:uint = 1 << STUNNED;
      
      public static const BLIND_BIT:uint = 1 << BLIND;
      
      public static const HALLUCINATING_BIT:uint = 1 << HALLUCINATING;
      
      public static const DRUNK_BIT:uint = 1 << DRUNK;
      
      public static const CONFUSED_BIT:uint = 1 << CONFUSED;
      
      public static const STUN_IMMUNE_BIT:uint = 1 << STUN_IMMUNE;
      
      public static const INVISIBLE_BIT:uint = 1 << INVISIBLE;
      
      public static const PARALYZED_BIT:uint = 1 << PARALYZED;
      
      public static const SPEEDY_BIT:uint = 1 << SPEEDY;
      
      public static const BLEEDING_BIT:uint = 1 << BLEEDING;
      
      public static const HEALING_BIT:uint = 1 << HEALING;
      
      public static const DAMAGING_BIT:uint = 1 << DAMAGING;
      
      public static const BERSERK_BIT:uint = 1 << BERSERK;
      
      public static const STASIS_BIT:uint = 1 << STASIS;
      
      public static const STASIS_IMMUNE_BIT:uint = 1 << STASIS_IMMUNE;
      
      public static const INVINCIBLE_BIT:uint = 1 << INVINCIBLE;
      
      public static const INVULNERABLE_BIT:uint = 1 << INVULNERABLE;
      
      public static const ARMORED_BIT:uint = 1 << ARMORED;
      
      public static const ARMORBROKEN_BIT:uint = 1 << ARMORBROKEN;
      
      public static const HEXED_BIT:uint = 1 << HEXED;
      
      public static const MAP_FILTER_BITMASK:uint = DRUNK_BIT | BLIND_BIT;
      
      public static var effects_:Vector.<ConditionEffect> = new <ConditionEffect>[new ConditionEffect("Nothing",0,null),new ConditionEffect("Quiet",QUIET_BIT,[32]),new ConditionEffect("Weak",WEAK_BIT,[34,35,36,37]),new ConditionEffect("Slowed",SLOWED_BIT,[1]),new ConditionEffect("Sick",SICK_BIT,[39]),new ConditionEffect("Dazed",DAZED_BIT,[44]),new ConditionEffect("Stunned",STUNNED_BIT,[45]),new ConditionEffect("Blind",BLIND_BIT,[41]),new ConditionEffect("Hallucinating",HALLUCINATING_BIT,[42]),new ConditionEffect("Drunk",DRUNK_BIT,[43]),new ConditionEffect("Confused",CONFUSED_BIT,[2]),new ConditionEffect("Stun Immune",STUN_IMMUNE_BIT,null),new ConditionEffect("Invisible",INVISIBLE_BIT,null),new ConditionEffect("Paralyzed",PARALYZED_BIT,[53,54]),new ConditionEffect("Speedy",SPEEDY_BIT,[0]),new ConditionEffect("Bleeding",BLEEDING_BIT,[46]),new ConditionEffect("Healing",HEALING_BIT,[47]),new ConditionEffect("Damaging",DAMAGING_BIT,[49]),new ConditionEffect("Berserk",BERSERK_BIT,[50]),new ConditionEffect("Stasis",STASIS_BIT,null),new ConditionEffect("Stasis Immune",STASIS_IMMUNE_BIT,null),new ConditionEffect("Invincible",INVINCIBLE_BIT,null),new ConditionEffect("Invulnerable",INVULNERABLE_BIT,[17]),new ConditionEffect("Armored",ARMORED_BIT,[16]),new ConditionEffect("Armor Broken",ARMORBROKEN_BIT,[55]),new ConditionEffect("Hexed",HEXED_BIT,[42])];
      
      private static var conditionEffectFromName_:Object = null;
      
      private static var bitToIcon_:Object = null;
      
      private static const GLOW_FILTER:GlowFilter = new GlowFilter(0,0.3,6,6,2,BitmapFilterQuality.LOW,false,false);
       
      
      public var name_:String;
      
      public var bit_:uint;
      
      public var iconOffsets_:Array;
      
      public function ConditionEffect(name:String, bit:uint, iconOffsets:Array)
      {
         super();
         this.name_ = name;
         this.bit_ = bit;
         this.iconOffsets_ = iconOffsets;
      }
      
      public static function getConditionEffectFromName(name:String) : uint
      {
         var ce:uint = 0;
         if(conditionEffectFromName_ == null)
         {
            conditionEffectFromName_ = new Object();
            for(ce = 0; ce < effects_.length; ce++)
            {
               conditionEffectFromName_[effects_[ce].name_] = ce;
            }
         }
         return conditionEffectFromName_[name];
      }
      
      public static function getConditionEffectIcons(condition:uint, icons:Vector.<BitmapData>, index:int) : void
      {
         var newCondition:uint = 0;
         var bit:uint = 0;
         var iconList:Vector.<BitmapData> = null;
         while(condition != 0)
         {
            newCondition = condition & condition - 1;
            bit = condition ^ newCondition;
            iconList = getIconsFromBit(bit);
            if(iconList != null)
            {
               icons.push(iconList[index % iconList.length]);
            }
            condition = newCondition;
         }
      }
      
      private static function getIconsFromBit(bit:uint) : Vector.<BitmapData>
      {
         var drawMatrix:Matrix = null;
         var ce:uint = 0;
         var icons:Vector.<BitmapData> = null;
         var i:int = 0;
         var icon:BitmapData = null;
         if(bitToIcon_ == null)
         {
            bitToIcon_ = new Object();
            drawMatrix = new Matrix();
            drawMatrix.translate(4,4);
            for(ce = 0; ce < effects_.length; ce++)
            {
               icons = null;
               if(effects_[ce].iconOffsets_ != null)
               {
                  icons = new Vector.<BitmapData>();
                  for(i = 0; i < effects_[ce].iconOffsets_.length; i++)
                  {
                     icon = new BitmapData(16,16,true,0);
                     icon.draw(AssetLibrary.getImageFromSet("lofiInterface2",effects_[ce].iconOffsets_[i]),drawMatrix);
                     icon = GlowRedrawer.outlineGlow(icon,4294967295);
                     icon.applyFilter(icon,icon.rect,PointUtil.ORIGIN,GLOW_FILTER);
                     icons.push(icon);
                  }
               }
               bitToIcon_[effects_[ce].bit_] = icons;
            }
         }
         return bitToIcon_[bit];
      }
   }
}
