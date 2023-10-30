package kabam.rotmg.account.core
{
   public interface Account
   {
      function updateUser(guid:String, password:String) : void;
      
      function getUserName() : String;
      
      function getUsername() : String;
      
      function getPassword() : String;
      
      function getCredentials() : Object;
      
      function isRegistered() : Boolean;
      
      function clear() : void;
      
      function reportIntStat(name:String, value:int) : void;
   }
}
