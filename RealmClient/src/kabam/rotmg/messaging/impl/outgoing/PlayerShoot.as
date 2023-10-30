package kabam.rotmg.messaging.impl.outgoing {
import flash.utils.IDataOutput;

import kabam.rotmg.messaging.impl.data.WorldPosData;

public class PlayerShoot extends OutgoingMessage {
    
    public var angle_:Number;
    public var ability_:Boolean;
    public var numShots:int;

    public function PlayerShoot(id:uint, callback:Function) {
        super(id, callback);
    }

    override public function writeToOutput(data:IDataOutput):void {
        data.writeFloat(this.angle_);
        data.writeBoolean(ability_);
        data.writeByte(this.numShots);
    }

    override public function toString():String {
        return formatToString("PLAYERSHOOT", "time_", "startingPos_", "angle_", "ability_");
    }
}
}
