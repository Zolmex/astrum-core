package kabam.rotmg.messaging.impl.outgoing {
import flash.utils.IDataOutput;

import kabam.rotmg.messaging.impl.data.WorldPosData;

public class Move extends OutgoingMessage {
    
    public var newPosition_:WorldPosData;

    public function Move(id:uint, callback:Function) {
        this.newPosition_ = new WorldPosData();
        super(id, callback);
    }

    override public function writeToOutput(data:IDataOutput):void {
        this.newPosition_.writeToOutput(data);
    }

    override public function toString():String {
        return formatToString("MOVE", "tickId_", "time_", "newPosition_");
    }
}
}
