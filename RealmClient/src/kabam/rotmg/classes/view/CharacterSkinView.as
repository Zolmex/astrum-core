package kabam.rotmg.classes.view
{
   import com.company.assembleegameclient.screens.AccountScreen;
   import com.company.assembleegameclient.screens.TitleMenuOption;
   import com.company.rotmg.graphics.ScreenGraphic;
   import flash.display.Shape;
   import flash.display.Sprite;
   import flash.events.MouseEvent;
   import kabam.rotmg.game.view.CreditDisplay;
   import kabam.rotmg.ui.view.components.ScreenBase;
   import org.osflash.signals.Signal;
   import org.osflash.signals.natives.NativeMappedSignal;
   
   public class CharacterSkinView extends Sprite
   {
      private const base:ScreenBase = makeScreenBase();
      private const account:AccountScreen = makeAccountScreen();
      private const lines:Shape = makeLines();
      private const creditsDisplay:CreditDisplay = makeCreditDisplay();
      private const graphic:ScreenGraphic = makeScreenGraphic();
      private const playBtn:TitleMenuOption = makePlayButton();
      private const backBtn:TitleMenuOption = makeBackButton();
      private const list:CharacterSkinListView = makeListView();
      private const detail:ClassDetailView = makeClassDetailView();
      public const play:Signal = new NativeMappedSignal(playBtn,MouseEvent.CLICK);
      public const back:Signal = new NativeMappedSignal(backBtn,MouseEvent.CLICK);
      
      public function CharacterSkinView()
      {
         super();
      }
      
      private function makeScreenBase() : ScreenBase
      {
         var base:ScreenBase = new ScreenBase();
         addChild(base);
         return base;
      }
      
      private function makeAccountScreen() : AccountScreen
      {
         var screen:AccountScreen = new AccountScreen();
         addChild(screen);
         return screen;
      }
      
      private function makeCreditDisplay() : CreditDisplay
      {
         var display:CreditDisplay = new CreditDisplay();
         display.x = 800;
         display.y = 20;
         addChild(display);
         return display;
      }
      
      private function makeLines() : Shape
      {
         var shape:Shape = new Shape();
         shape.graphics.clear();
         shape.graphics.lineStyle(2,5526612);
         shape.graphics.moveTo(0,105);
         shape.graphics.lineTo(800,105);
         shape.graphics.moveTo(346,105);
         shape.graphics.lineTo(346,526);
         addChild(shape);
         return shape;
      }
      
      private function makeScreenGraphic() : ScreenGraphic
      {
         var graphic:ScreenGraphic = new ScreenGraphic();
         addChild(graphic);
         return graphic;
      }
      
      private function makePlayButton() : TitleMenuOption
      {
         var option:TitleMenuOption = null;
         option = new TitleMenuOption("play",36,false);
         option.x = 400 - option.width / 2;
         option.y = 520;
         addChild(option);
         return option;
      }
      
      private function makeBackButton() : TitleMenuOption
      {
         var option:TitleMenuOption = new TitleMenuOption("back",22,false);
         option.x = 30;
         option.y = 534;
         addChild(option);
         return option;
      }
      
      private function makeListView() : CharacterSkinListView
      {
         var view:CharacterSkinListView = new CharacterSkinListView();
         view.x = 351;
         view.y = 110;
         addChild(view);
         return view;
      }
      
      private function makeClassDetailView() : ClassDetailView
      {
         var view:ClassDetailView = new ClassDetailView();
         view.x = 5;
         view.y = 110;
         addChild(view);
         return view;
      }

      public function setPlayButtonEnabled(activate:Boolean):void {
         if (!activate) {
            this.playBtn.deactivate();
         }
      }
   }
}
