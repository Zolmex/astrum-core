package kabam.rotmg.appengine.impl
{
import com.company.assembleegameclient.parameters.Parameters;

import flash.net.URLLoaderDataFormat;
   import kabam.rotmg.appengine.api.AppEngineClient;
   import kabam.rotmg.appengine.api.RetryLoader;
   import org.osflash.signals.OnceSignal;
   
   public class SimpleAppEngineClient implements AppEngineClient
   {
       
      
      [Inject]
      public var loader:RetryLoader;
      
      private var isEncrypted:Boolean;
      
      private var maxRetries:int;
      
      private var dataFormat:String;
      
      public function SimpleAppEngineClient()
      {
         super();
         this.isEncrypted = true;
         this.maxRetries = 0;
         this.dataFormat = URLLoaderDataFormat.TEXT;
      }
      
      public function get complete() : OnceSignal
      {
         return this.loader.complete;
      }
      
      public function setDataFormat(dataFormat:String) : void
      {
         this.loader.setDataFormat(dataFormat);
      }
      
      public function setSendEncrypted(value:Boolean) : void
      {
         this.isEncrypted = value;
      }
      
      public function setMaxRetries(maxRetries:int) : void
      {
         this.loader.setMaxRetries(maxRetries);
      }
      
      public function sendRequest(target:String, params:Object) : void
      {
         this.loader.sendRequest(this.makeURL(target),params);
      }
      
      private function makeURL(target:String) : String
      {
         if(target.charAt(0) != "/")
         {
            target = "/" + target;
         }
         return Parameters.appServerAddress() + target;
      }
   }
}
