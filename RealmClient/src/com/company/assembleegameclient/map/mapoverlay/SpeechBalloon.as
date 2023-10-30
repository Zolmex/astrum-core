package com.company.assembleegameclient.map.mapoverlay
{
   import com.company.assembleegameclient.map.Camera;
   import com.company.assembleegameclient.objects.GameObject;
   import com.company.assembleegameclient.parameters.Parameters;
import com.company.ui.SimpleText;
import com.company.util.GraphicsUtil;
   import flash.display.CapsStyle;
   import flash.display.GraphicsPath;
   import flash.display.GraphicsPathCommand;
   import flash.display.GraphicsSolidFill;
   import flash.display.GraphicsStroke;
   import flash.display.IGraphicsData;
   import flash.display.JointStyle;
   import flash.display.LineScaleMode;
   import flash.display.Sprite;
   import flash.filters.DropShadowFilter;
   import flash.geom.Point;
   import flash.text.TextField;
   import flash.text.TextFieldAutoSize;
   import flash.text.TextFormat;
   
   public class SpeechBalloon extends Sprite implements IMapOverlayElement
   {
       
      
      public var go_:GameObject;
      
      public var lifetime_:int;
      
      public var hideable_:Boolean;
      
      public var offset_:Point;
      
      public var text_:TextField;
      
      private var backgroundFill_:GraphicsSolidFill = new GraphicsSolidFill(0,1);
      
      private var outlineFill_:GraphicsSolidFill= new GraphicsSolidFill(16777215,1);
      
      private var lineStyle_:GraphicsStroke= new GraphicsStroke(2,false,LineScaleMode.NORMAL,CapsStyle.NONE,JointStyle.ROUND,3,outlineFill_);
      
      private var path_:GraphicsPath = new GraphicsPath(new Vector.<int>(),new Vector.<Number>());
      
      private const graphicsData_:Vector.<IGraphicsData> = new <IGraphicsData>[lineStyle_,backgroundFill_,path_,GraphicsUtil.END_FILL,GraphicsUtil.END_STROKE];
      
      private var startTime_:int = 0;
      
      public function SpeechBalloon(go:GameObject, text:String, background:uint, backgroundAlpha:Number, outline:uint, outlineAlpha:Number, textColor:uint, lifetime:int, bold:Boolean, hideable:Boolean)
      {
         this.offset_ = new Point();
         super();
         mouseEnabled = false;
         mouseChildren = false;
         this.go_ = go;
         this.lifetime_ = lifetime * 1000;
         this.hideable_ = hideable;
         this.text_ = new TextField();
         this.text_.autoSize = TextFieldAutoSize.LEFT;
         this.text_.embedFonts = true;
         this.text_.width = 150;
         var format:TextFormat = new TextFormat();
         format.font = SimpleText._Font.fontName;
         format.size = 14;
         format.bold = bold;
         format.color = textColor;
         this.text_.defaultTextFormat = format;
         this.text_.selectable = false;
         this.text_.mouseEnabled = false;
         this.text_.multiline = true;
         this.text_.wordWrap = true;
         this.text_.text = text;
         addChild(this.text_);
         var w:int = this.text_.textWidth + 4;
         this.offset_.x = -w / 2;
         this.backgroundFill_.color = background;
         this.backgroundFill_.alpha = backgroundAlpha;
         this.outlineFill_.color = outline;
         this.outlineFill_.alpha = outlineAlpha;
         graphics.clear();
         GraphicsUtil.clearPath(this.path_);
         GraphicsUtil.drawCutEdgeRect(-6,-6,w + 12,height + 12,4,[1,1,1,1],this.path_);
         this.path_.commands.splice(6,0,GraphicsPathCommand.LINE_TO,GraphicsPathCommand.LINE_TO,GraphicsPathCommand.LINE_TO);
         var h:int = height;
         this.path_.data.splice(12,0,w / 2 + 8,h + 6,w / 2,h + 18,w / 2 - 8,h + 6);
         graphics.drawGraphicsData(this.graphicsData_);
         filters = [new DropShadowFilter(0,0,0,1,16,16)];
         this.offset_.y = -height - this.go_.texture_.height * (go.size_ / 100) * 5 - 2;
         visible = false;
      }
      
      public function draw(camera:Camera, time:int) : Boolean
      {
         if(this.startTime_ == 0)
         {
            this.startTime_ = time;
         }
         var dt:int = time - this.startTime_;
         if(dt > this.lifetime_ || this.go_ != null && this.go_.map_ == null)
         {
            return false;
         }
         if(this.go_ == null || !this.go_.drawn_)
         {
            visible = false;
            return true;
         }
         if(this.hideable_ && !Parameters.data_.textBubbles)
         {
            visible = false;
            return true;
         }
         visible = true;
         x = int(this.go_.posS_[0] + this.offset_.x);
         y = int(this.go_.posS_[1] + this.offset_.y);
         return true;
      }
      
      public function getGameObject() : GameObject
      {
         return this.go_;
      }
      
      public function dispose() : void
      {
         parent.removeChild(this);
      }
   }
}
