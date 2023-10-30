package kabam.rotmg.messaging.impl.outgoing {
import flash.utils.IDataOutput;

import kabam.rotmg.messaging.impl.data.SlotObjectData;
import kabam.rotmg.messaging.impl.data.WorldPosData;

public class InvSwap extends OutgoingMessage {

    public var slotObject1_:SlotObjectData;
    public var slotObject2_:SlotObjectData;

    public function InvSwap(id:uint, callback:Function) {
        this.slotObject1_ = new SlotObjectData();
        this.slotObject2_ = new SlotObjectData();
        super(id, callback);
    }

    override public function writeToOutput(data:IDataOutput):void {
        this.slotObject1_.writeToOutput(data);
        this.slotObject2_.writeToOutput(data);
    }

    override public function toString():String {
        return formatToString("INVSWAP", "slotObject1_", "slotObject2_");
    }
}
}
