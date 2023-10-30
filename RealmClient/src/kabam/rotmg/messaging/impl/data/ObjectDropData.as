package kabam.rotmg.messaging.impl.data
{
import flash.utils.IDataInput;

public class ObjectDropData
{
    public var objectId_:int;
    public var explode_:Boolean;

    public function parseFromInput(data:IDataInput) : void
    {
        this.objectId_ = data.readInt();
        this.explode_ = data.readBoolean();
    }

    public function toString() : String
    {
        return "objectId_: " + this.objectId_ + " explode_: " + this.explode_;
    }
}
}
