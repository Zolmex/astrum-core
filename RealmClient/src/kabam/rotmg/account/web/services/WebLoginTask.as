package kabam.rotmg.account.web.services
{
   import kabam.lib.tasks.BaseTask;
   import kabam.rotmg.account.core.Account;
   import kabam.rotmg.account.core.services.LoginTask;
   import kabam.rotmg.account.web.model.AccountData;
   import kabam.rotmg.appengine.api.AppEngineClient;
   
   public class WebLoginTask extends BaseTask implements LoginTask
   {
       
      
      [Inject]
      public var account:Account;
      
      [Inject]
      public var data:AccountData;
      
      [Inject]
      public var client:AppEngineClient;
      
      public function WebLoginTask()
      {
         super();
      }
      
      override protected function startTask() : void
      {
         this.client.complete.addOnce(this.onComplete);
         this.client.sendRequest("/account/verify",{
            "username":this.data.username,
            "password":this.data.password
         });
      }
      
      private function onComplete(isOK:Boolean, data:*) : void
      {
         if(isOK)
         {
            this.updateUser(data);
         }
         completeTask(isOK, data);
      }
      
      private function updateUser(response:String) : void
      {
         var xml:XML = XML(response);
         this.account.updateUser(xml.Name, this.data.password);
      }
   }
}
