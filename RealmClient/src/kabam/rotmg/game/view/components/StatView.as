package kabam.rotmg.game.view.components
{
   import com.company.assembleegameclient.ui.tooltip.TextToolTip;
   import com.company.ui.SimpleText;
   import flash.display.Sprite;
   import flash.events.MouseEvent;
   import flash.filters.DropShadowFilter;
   import flash.text.TextFormat;
   import org.osflash.signals.natives.NativeSignal;
   
   public class StatView extends Sprite
   {
      public var fullName_:String;
      
      public var description_:String;
      
      public var nameText_:SimpleText;
      
      public var valText_:SimpleText;
      
      public var redOnZero_:Boolean;
      
      public var val_:int = -1;
      
      public var boost_:int = -1;
      
      public var valColor_:uint = 11776947;
      
      public function StatView(name:String, fullName:String, desc:String, redOnZero:Boolean)
      {
         super();
         this.fullName_ = fullName;
         this.description_ = desc;
         this.nameText_ = new SimpleText(13,11776947,false,0,0);
         this.nameText_.text = name + " -";
         this.nameText_.updateMetrics();
         this.nameText_.x = -this.nameText_.width;
         this.nameText_.filters = [new DropShadowFilter(0,0,0)];
         addChild(this.nameText_);
         this.valText_ = new SimpleText(13,this.valColor_,false,0,0);
         this.valText_.setBold(true);
         this.valText_.text = "-";
         this.valText_.updateMetrics();
         this.valText_.filters = [new DropShadowFilter(0,0,0)];
         addChild(this.valText_);
         this.redOnZero_ = redOnZero;
      }
      
      public function draw(val:int, boost:int, max:int) : void
      {
         var newValColor:uint = 0;
         var format:TextFormat = null;
         if(val == this.val_ && boost == this.boost_)
         {
            return;
         }
         this.val_ = val;
         this.boost_ = boost;
         if(val - boost >= max)
         {
            newValColor = 16572160;
         }
         else if(this.redOnZero_ && this.val_ <= 0 || this.boost_ < 0)
         {
            newValColor = 16726072;
         }
         else if(this.boost_ > 0)
         {
            newValColor = 6206769;
         }
         else
         {
            newValColor = 11776947;
         }
         if(this.valColor_ != newValColor)
         {
            this.valColor_ = newValColor;
            format = this.valText_.defaultTextFormat;
            format.color = this.valColor_;
            this.valText_.setTextFormat(format);
            this.valText_.defaultTextFormat = format;
         }
         this.valText_.text = this.val_.toString();
         if(this.boost_ != 0)
         {
            this.valText_.text = this.valText_.text + (" (" + (this.boost_ > 0?"+":"") + this.boost_.toString() + ")");
         }
         this.valText_.updateMetrics();
      }
   }
}
