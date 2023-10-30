package kabam.rotmg.account.web.view
{
   import com.company.assembleegameclient.account.ui.Frame;
   import com.company.assembleegameclient.account.ui.TextInputField;
   import com.company.assembleegameclient.ui.ClickableText;
   import flash.events.MouseEvent;
   import kabam.rotmg.account.web.model.AccountData;
   import org.osflash.signals.Signal;
   import org.osflash.signals.natives.NativeMappedSignal;
   
   public class WebLoginDialog extends Frame
   {
       
      
      public var cancel:Signal;
      
      public var signIn:Signal;
      
      public var register:Signal;
      
      private var usernameInput:TextInputField;
      
      private var passwordInput:TextInputField;
      
      private var registerText:ClickableText;
      
      public function WebLoginDialog()
      {
         super("Sign in","Cancel","Sign in");
         this.makeUI();
         this.register = new NativeMappedSignal(this.registerText,MouseEvent.CLICK);
         this.cancel = new NativeMappedSignal(leftButton_,MouseEvent.CLICK);
         this.signIn = new Signal(AccountData);
      }
      
      private function makeUI() : void
      {
         this.usernameInput = new TextInputField("Username",false,"");
         addTextInputField(this.usernameInput);
         this.passwordInput = new TextInputField("Password",true,"");
         addTextInputField(this.passwordInput);
         this.registerText = new ClickableText(12,false,"New user?  Click here to Register");
         addNavigationText(this.registerText);
         rightButton_.addEventListener(MouseEvent.CLICK,this.onSignIn);
      }
      
      private function onCancel(event:MouseEvent) : void
      {
         this.cancel.dispatch();
      }
      
      private function onSignIn(event:MouseEvent) : void
      {
         var data:AccountData = null;
         if(this.isUsernameValid() && this.isPasswordValid())
         {
            data = new AccountData();
            data.username = this.usernameInput.text();
            data.password = this.passwordInput.text();
            this.signIn.dispatch(data);
         }
      }
      private function isUsernameValid() : Boolean
      {
         var isValid:Boolean = Boolean(usernameInput.text().match(/^[a-zA-Z0-9]+$/i)) && this.usernameInput.text().length > 0 && this.usernameInput.text().length <= 12;
         if(!isValid)
         {
            this.usernameInput.setError("Invalid username");
         }
         return isValid;
      }

      private function isPasswordValid() : Boolean
      {
         var isValid:Boolean = this.passwordInput.text().length >= 9;
         if(!isValid)
         {
            this.passwordInput.setError("Invalid password");
         }
         return isValid;
      }
      
      public function setError(error:String) : void
      {
         this.passwordInput.setError(error);
      }
   }
}
