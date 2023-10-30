package kabam.rotmg.account.web.view
{
   import com.company.assembleegameclient.account.ui.CheckBoxField;
   import com.company.assembleegameclient.account.ui.Frame;
   import com.company.assembleegameclient.parameters.Parameters;
import com.company.ui.SimpleText;
   import flash.events.MouseEvent;
   import flash.events.TextEvent;
   import flash.filters.DropShadowFilter;
   import kabam.rotmg.account.ui.components.DateField;
   import kabam.rotmg.account.web.model.AccountData;
   import org.osflash.signals.Signal;
   import org.osflash.signals.natives.NativeMappedSignal;
   
   public class WebRegisterDialog extends Frame
   {
      private const SIGN_IN_TEXT:String = "Already registered? <font color=\"#7777EE\"><a href=\"event:flash.events.TextEvent\">here</a></font> to sign in!";
      
      private const REGISTER_IMPERATIVE:String = "Register in order to save your progress";
      
      private const PASSWORDS_DONT_MATCH:String = "The password did not match";
      
      private const PASSWORD_TOO_SHORT:String = "Password must be at least 9 characters long";

      private const INVALID_USERNAME:String = "Username must be between 1-12 characters and no spaces";
      
      public var register:Signal;
      
      public var signIn:Signal;
      
      public var cancel:Signal;
      
      private const errors:Array = [];
      
      private var usernameInput:LabeledField;
      
      private var passwordInput:LabeledField;
      
      private var retypePasswordInput:LabeledField;
      
      private var signInText:SimpleText;
      
      public function WebRegisterDialog()
      {
         super(this.REGISTER_IMPERATIVE,"Cancel","Register", 326);
         this.makeUIElements();
         this.makeSignals();
      }
      
      private function makeUIElements() : void
      {
         this.usernameInput = new LabeledField("Username",false,275);
         this.passwordInput = new LabeledField("Password",true,275);
         this.retypePasswordInput = new LabeledField("Retype Password",true,275);
         this.signInText = new SimpleText(12,11776947,false,0,0,true);
         this.signInText.setBold(true);
         this.signInText.htmlText = this.SIGN_IN_TEXT;
         this.signInText.updateMetrics();
         this.signInText.filters = [new DropShadowFilter(0,0,0)];
         this.signInText.addEventListener(TextEvent.LINK,this.linkEvent);
         addLabeledField(this.usernameInput);
         addLabeledField(this.passwordInput);
         addLabeledField(this.retypePasswordInput);
         addSpace(8);
         addComponent(this.signInText,14);
      }
      
      private function linkEvent(event_:TextEvent) : void
      {
         this.signIn.dispatch();
      }
      
      private function makeSignals() : void
      {
         this.cancel = new NativeMappedSignal(leftButton_,MouseEvent.CLICK);
         rightButton_.addEventListener(MouseEvent.CLICK,this.onRegister);
         this.register = new Signal(AccountData);
         this.signIn = new Signal();
      }
      
      private function onRegister(event:MouseEvent) : void
      {
         var areValid:Boolean = this.areInputsValid();
         this.displayErrors();
         if(areValid)
         {
            this.sendData();
         }
      }
      
      private function areInputsValid() : Boolean
      {
         this.errors.length = 0;
         var isValid:Boolean = true;
         isValid = this.isUsernameValid() && isValid;
         isValid = this.isPasswordValid() && isValid;
         isValid = this.isPasswordVerified() && isValid;
         return isValid;
      }

      private function isUsernameValid() : Boolean
      {
         var isValid:Boolean = Boolean(usernameInput.text().match(/^[a-zA-Z0-9]+$/i)) && this.usernameInput.text().length > 0 && this.usernameInput.text().length <= 12;
         this.passwordInput.setErrorHighlight(!isValid);
         if(!isValid)
         {
            this.errors.push(this.INVALID_USERNAME);
         }
         return isValid;
      }

      private function isPasswordValid() : Boolean
      {
         var isValid:Boolean = this.passwordInput.text().length >= 9;
         this.passwordInput.setErrorHighlight(!isValid);
         if(!isValid)
         {
            this.errors.push(this.PASSWORD_TOO_SHORT);
         }
         return isValid;
      }
      
      private function isPasswordVerified() : Boolean
      {
         var isValid:Boolean = this.passwordInput.text() == this.retypePasswordInput.text();
         this.retypePasswordInput.setErrorHighlight(!isValid);
         if(!isValid)
         {
            this.errors.push(this.PASSWORDS_DONT_MATCH);
         }
         return isValid;
      }
      
      public function displayErrors() : void
      {
         if(this.errors.length == 0)
         {
            this.clearErrors();
         }
         else
         {
            this.displayErrorText(this.errors[0]);
         }
      }
      
      public function displayServerError(value:String) : void
      {
         this.displayErrorText(value);
      }
      
      private function clearErrors() : void
      {
         titleText_.text = this.REGISTER_IMPERATIVE;
         titleText_.updateMetrics();
         titleText_.setColor(11776947);
      }
      
      private function displayErrorText(value:String) : void
      {
         titleText_.text = value;
         titleText_.updateMetrics();
         titleText_.setColor(16549442);
      }
      
      private function sendData() : void
      {
         var data:AccountData = new AccountData();
         data.username = this.usernameInput.text();
         data.password = this.passwordInput.text();
         this.register.dispatch(data);
      }
   }
}
