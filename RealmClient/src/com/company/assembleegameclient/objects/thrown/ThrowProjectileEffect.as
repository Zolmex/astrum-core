package com.company.assembleegameclient.objects.thrown
{
   import com.company.assembleegameclient.objects.particles.ParticleEffect;
   import flash.geom.Point;
   
   public class ThrowProjectileEffect extends ParticleEffect
   {
       
      
      public var start_:Point;
      
      public var end_:Point;
      
      public var id_:uint;
      
      public function ThrowProjectileEffect(objectId:int, start:Point, end:Point)
      {
         super();
         this.start_ = start;
         this.end_ = end;
         this.id_ = objectId;
      }
      
      override public function update(time:int, dt:int) : Boolean
      {
         x_ = this.start_.x;
         y_ = this.start_.y;
         var size:int = 10000;
         var projectile:ThrownProjectile = new ThrownProjectile(this.id_,1500,this.start_,this.end_);
         map_.addObj(projectile,x_,y_);
         return false;
      }
   }
}
