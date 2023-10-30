package kabam.rotmg.messaging.impl.outgoing {
import flash.utils.IDataOutput;

public class PlayerHit extends OutgoingMessage {
    
    public var ownerId:int;
    public var bulletId_:uint;

    public function PlayerHit(id:uint, callback:Function) {
        super(id, callback);
    }

    override public function writeToOutput(data:IDataOutput):void {
        data.writeInt(this.ownerId);
        data.writeByte(this.bulletId_);
    }

    override public function toString():String {
        return formatToString("PLAYERHIT", "bulletId_", "objectId_");
    }
}
}
