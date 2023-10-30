package com.company.assembleegameclient.map.partyoverlay
{
   import com.company.assembleegameclient.map.Camera;
   import com.company.assembleegameclient.objects.GameObject;
   import com.company.assembleegameclient.ui.menu.Menu;
   import com.company.assembleegameclient.ui.tooltip.ToolTip;
   import com.company.util.RectangleUtil;
   import com.company.util.Trig;
   import flash.display.Graphics;
   import flash.display.Shape;
   import flash.display.Sprite;
   import flash.events.MouseEvent;
   import flash.filters.DropShadowFilter;
   import flash.geom.Point;
   import flash.geom.Rectangle;
   
   public class GameObjectArrow extends Sprite
   {
      
      public static const SMALL_SIZE:int = 8;
      
      public static const BIG_SIZE:int = 11;
      
      public static const DIST:int = 3;
      
      private static var menu_:Menu = null;
       
      
      public var lineColor_:uint;
      
      public var fillColor_:uint;
      
      public var go_:GameObject = null;
      
      public var extraGOs_:Vector.<GameObject>;
      
      public var mouseOver_:Boolean = false;
      
      private var big_:Boolean;
      
      private var arrow_:Shape;
      
      protected var tooltip_:ToolTip = null;
      
      private var tempPoint:Point;
      
      public function GameObjectArrow(lineColor:uint, fillColor:uint, big:Boolean)
      {
         this.extraGOs_ = new Vector.<GameObject>();
         this.arrow_ = new Shape();
         this.tempPoint = new Point();
         super();
         this.lineColor_ = lineColor;
         this.fillColor_ = fillColor;
         this.big_ = big;
         addChild(this.arrow_);
         this.drawArrow();
         addEventListener(MouseEvent.MOUSE_OVER,this.onMouseOver);
         addEventListener(MouseEvent.MOUSE_OUT,this.onMouseOut);
         addEventListener(MouseEvent.MOUSE_DOWN,this.onMouseDown);
         filters = [new DropShadowFilter(0,0,0,1,8,8)];
         visible = false;
      }
      
      public static function removeMenu() : void
      {
         if(menu_ != null)
         {
            if(menu_.parent != null)
            {
               menu_.parent.removeChild(menu_);
            }
            menu_ = null;
         }
      }
      
      protected function onMouseOver(event:MouseEvent) : void
      {
         this.mouseOver_ = true;
         this.drawArrow();
      }
      
      protected function onMouseOut(event:MouseEvent) : void
      {
         this.mouseOver_ = false;
         this.drawArrow();
      }
      
      protected function onMouseDown(event:MouseEvent) : void
      {
         event.stopImmediatePropagation();
      }
      
      protected function setToolTip(toolTip:ToolTip) : void
      {
         this.removeTooltip();
         this.tooltip_ = toolTip;
         if(this.tooltip_ != null)
         {
            addChild(this.tooltip_);
            this.positionTooltip(this.tooltip_);
         }
      }
      
      protected function removeTooltip() : void
      {
         if(this.tooltip_ != null)
         {
            if(this.tooltip_.parent != null)
            {
               this.tooltip_.parent.removeChild(this.tooltip_);
            }
            this.tooltip_ = null;
         }
      }
      
      protected function setMenu(menu:Menu) : void
      {
         this.removeTooltip();
         menu_ = menu;
         stage.addChild(menu_);
      }
      
      public function setGameObject(go:GameObject) : void
      {
         if(this.go_ != go)
         {
            this.go_ = go;
         }
         this.extraGOs_.length = 0;
         if(this.go_ == null)
         {
            visible = false;
         }
      }
      
      public function addGameObject(go:GameObject) : void
      {
         this.extraGOs_.push(go);
      }
      
      public function draw(time:int, camera:Camera) : void
      {
         if(this.go_ == null)
         {
            visible = false;
            return;
         }
         this.go_.computeSortVal(camera);
         var clipRect:Rectangle = camera.clipRect_;
         var playerXS:Number = this.go_.posS_[0];
         var playerYS:Number = this.go_.posS_[1];
         if(!RectangleUtil.lineSegmentIntersectXY(camera.clipRect_,0,0,playerXS,playerYS,this.tempPoint))
         {
            this.go_ = null;
            visible = false;
            return;
         }
         x = this.tempPoint.x;
         y = this.tempPoint.y;
         var rot:Number = Trig.boundTo180(270 - Trig.toDegrees * Math.atan2(playerXS,playerYS));
         if(this.tempPoint.x < clipRect.left + 5)
         {
            if(rot > 45)
            {
               rot = 45;
            }
            if(rot < -45)
            {
               rot = -45;
            }
         }
         else if(this.tempPoint.x > clipRect.right - 5)
         {
            if(rot > 0)
            {
               if(rot < 135)
               {
                  rot = 135;
               }
            }
            else if(rot > -135)
            {
               rot = -135;
            }
         }
         if(this.tempPoint.y < clipRect.top + 5)
         {
            if(rot < 45)
            {
               rot = 45;
            }
            if(rot > 135)
            {
               rot = 135;
            }
         }
         else if(this.tempPoint.y > clipRect.bottom - 5)
         {
            if(rot > -45)
            {
               rot = -45;
            }
            if(rot < -135)
            {
               rot = -135;
            }
         }
         this.arrow_.rotation = rot;
         if(this.tooltip_ != null)
         {
            this.positionTooltip(this.tooltip_);
         }
         visible = true;
      }
      
      private function positionTooltip(tooltip:ToolTip) : void
      {
         var x1:Number = NaN;
         var y1:Number = NaN;
         var rot:Number = this.arrow_.rotation;
         var d:int = DIST + BIG_SIZE + 12;
         var x0:Number = d * Math.cos(rot * Trig.toRadians);
         var y0:Number = d * Math.sin(rot * Trig.toRadians);
         var w:Number = tooltip.contentWidth_;
         var h:Number = tooltip.contentHeight_;
         if(rot >= 45 && rot <= 135)
         {
            x1 = x0 + w / Math.tan(rot * Trig.toRadians);
            tooltip.x = (x0 + x1) / 2 - w / 2;
            tooltip.y = y0;
         }
         else if(rot <= -45 && rot >= -135)
         {
            x1 = x0 - w / Math.tan(rot * Trig.toRadians);
            tooltip.x = (x0 + x1) / 2 - w / 2;
            tooltip.y = y0 - h;
         }
         else if(rot < 45 && rot > -45)
         {
            tooltip.x = x0;
            y1 = y0 + h * Math.tan(rot * Trig.toRadians);
            tooltip.y = (y0 + y1) / 2 - h / 2;
         }
         else
         {
            tooltip.x = x0 - w;
            y1 = y0 - h * Math.tan(rot * Trig.toRadians);
            tooltip.y = (y0 + y1) / 2 - h / 2;
         }
      }
      
      private function drawArrow() : void
      {
         var g:Graphics = this.arrow_.graphics;
         g.clear();
         var size:int = this.big_ || this.mouseOver_?int(BIG_SIZE):int(SMALL_SIZE);
         g.lineStyle(1,this.lineColor_);
         g.beginFill(this.fillColor_);
         g.moveTo(DIST,0);
         g.lineTo(size + DIST,size);
         g.lineTo(size + DIST,-size);
         g.lineTo(DIST,0);
         g.endFill();
         g.lineStyle();
      }
   }
}
