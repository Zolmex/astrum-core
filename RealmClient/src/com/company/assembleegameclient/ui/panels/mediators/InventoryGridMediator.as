package com.company.assembleegameclient.ui.panels.mediators
{
   import com.company.assembleegameclient.objects.Player;
   import com.company.assembleegameclient.ui.panels.itemgrids.InventoryGrid;
   import kabam.rotmg.ui.signals.UpdateHUDSignal;
   import robotlegs.bender.bundles.mvcs.Mediator;
   
   public class InventoryGridMediator extends Mediator
   {
       
      
      [Inject]
      public var view:InventoryGrid;
      
      [Inject]
      public var updateHUD:UpdateHUDSignal;
      
      public function InventoryGridMediator()
      {
         super();
      }
      
      override public function initialize() : void
      {
         this.updateHUD.add(this.onUpdateHUD);
      }
      
      override public function destroy() : void
      {
         this.updateHUD.remove(this.onUpdateHUD);
      }
      
      private function onUpdateHUD(player:Player) : void
      {
         this.view.draw();
      }
   }
}
