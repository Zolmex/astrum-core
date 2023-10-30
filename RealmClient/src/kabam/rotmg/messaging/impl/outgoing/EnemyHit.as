package kabam.rotmg.messaging.impl.outgoing {
import flash.utils.IDataOutput;

public class EnemyHit extends OutgoingMessage {

    public var bulletId_:int;
    public var targetId_:int;

    public function EnemyHit(id:uint, callback:Function) {
        super(id, callback);
    }

    override public function writeToOutput(data:IDataOutput):void {
        data.writeByte(this.bulletId_);
        data.writeInt(this.targetId_);
    }

    override public function toString():String {
        return formatToString("ENEMYHIT", "time_", "hitPos", "bulletId_", "targetId_");
    }
}
}
