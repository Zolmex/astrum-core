package kabam.rotmg.messaging.impl.incoming {
import flash.utils.IDataInput;

import kabam.rotmg.messaging.impl.data.WorldPosData;

public class EnemyShoot extends IncomingMessage {

    public var projId_:uint;
    public var ownerId_:int;
    public var bulletType_:int;
    public var startingPos_:WorldPosData;
    public var angle_:Number;
    public var damage_:uint;

    public function EnemyShoot(id:uint, callback:Function) {
        this.startingPos_ = new WorldPosData();
        super(id, callback);
    }

    override public function parseFromInput(data:IDataInput):void {
        this.projId_ = data.readUnsignedByte();
        this.ownerId_ = data.readInt();
        this.bulletType_ = data.readUnsignedByte();
        this.startingPos_.parseFromInput(data);
        this.angle_ = data.readFloat();
        this.damage_ = data.readUnsignedInt();
    }

    override public function toString():String {
        return formatToString("SHOOT", "bulletId_", "ownerId_", "bulletType_", "startingPos_", "angle_", "damage_", "numShots_", "angleInc_");
    }
}
}
