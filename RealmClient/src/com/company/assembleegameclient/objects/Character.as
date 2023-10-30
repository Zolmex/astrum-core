package com.company.assembleegameclient.objects
{
import com.company.assembleegameclient.objects.particles.ExplosionEffect;
import com.company.assembleegameclient.parameters.Parameters;
import com.company.assembleegameclient.sound.SoundEffectLibrary;
import com.company.assembleegameclient.util.BloodComposition;

public class Character extends GameObject
   {
       
      
      public var hurtSound_:String;
      
      public var deathSound_:String;
      
      public function Character(objectXML:XML)
      {
         super(objectXML);
         this.hurtSound_ = Boolean(objectXML.hasOwnProperty("HitSound"))?String(objectXML.HitSound):"monster/default_hit";
         SoundEffectLibrary.load(this.hurtSound_);
         this.deathSound_ = Boolean(objectXML.hasOwnProperty("DeathSound"))?String(objectXML.DeathSound):"monster/default_death";
         SoundEffectLibrary.load(this.deathSound_);
      }
      
      override public function damage(damageAmount:int, effects:Vector.<uint>, proj:Projectile) : void
      {
         super.damage(damageAmount,effects,proj);
         SoundEffectLibrary.play(this.hurtSound_);
      }

      public function explode() : void
      {
         if (Parameters.data_.particles) {
            var colors:Vector.<uint> = BloodComposition.getBloodComposition(this.objectType_, this.texture_, this.props_.bloodProb_, this.props_.bloodColor_);
            map_.addObj(new ExplosionEffect(colors, this.size_, 30), x_, y_);
         }
         SoundEffectLibrary.play(this.deathSound_);
      }
   }
}
