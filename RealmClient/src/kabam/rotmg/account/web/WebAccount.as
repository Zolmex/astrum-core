package kabam.rotmg.account.web
{
   import com.company.assembleegameclient.parameters.Parameters;
   import flash.external.ExternalInterface;
   import flash.net.SharedObject;
   import kabam.rotmg.account.core.Account;
   
   public class WebAccount implements Account
   {
      private var username:String = "";
      private var password:String;
      
      public function WebAccount()
      {
         super();
      }
      
      public function getUserName() : String
      {
         return this.username;
      }
      
      public function getUsername() : String
      {
         return this.username = this.username || null;
      }
      
      public function getPassword() : String
      {
         return this.password || "";
      }
      
      public function getCredentials() : Object
      {
         return {
            "username":this.getUsername(),
            "password":this.getPassword()
         };
      }
      
      public function isRegistered() : Boolean
      {
         return this.getPassword() != "";
      }
      
      public function updateUser(userId:String, password:String) : void
      {
         var rotmg:SharedObject = null;
         this.username = userId;
         this.password = password;
         try
         {
            rotmg = SharedObject.getLocal("OWRotMG","/");
            rotmg.data["Username"] = userId;
            rotmg.data["Password"] = password;
            rotmg.flush();
         }
         catch(error:Error)
         {
         }
      }
      
      public function clear() : void
      {
         this.updateUser(null,null);
         Parameters.data_.charIdUseMap = {};
         Parameters.save();
      }
      
      public function reportIntStat(name:String, value:int) : void
      {
         trace("Setting int stat \"" + name + "\" to \"" + value + "\"");
      }
   }
}
