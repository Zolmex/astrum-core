package kabam.rotmg.game.signals
{
   import kabam.rotmg.game.model.UsePotionVO;
   import org.osflash.signals.Signal;
   
   public class UsePotionSignal extends Signal
   {
       
      
      public function UsePotionSignal()
      {
         super(UsePotionVO);
      }
   }
}
