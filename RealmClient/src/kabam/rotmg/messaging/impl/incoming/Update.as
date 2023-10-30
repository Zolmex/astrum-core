package kabam.rotmg.messaging.impl.incoming {
import com.company.assembleegameclient.util.FreeList;

import flash.utils.IDataInput;

import kabam.rotmg.messaging.impl.data.GroundTileData;
import kabam.rotmg.messaging.impl.data.ObjectData;
import kabam.rotmg.messaging.impl.data.ObjectDropData;

public class Update extends IncomingMessage {

    public var tiles_:Vector.<GroundTileData>;
    public var newObjs_:Vector.<ObjectData>;
    public var drops_:Vector.<ObjectDropData>;

    public function Update(id:uint, callback:Function) {
        this.tiles_ = new Vector.<GroundTileData>();
        this.newObjs_ = new Vector.<ObjectData>();
        this.drops_ = new Vector.<ObjectDropData>();
        super(id, callback);
    }

    override public function parseFromInput(data:IDataInput):void {
        var o:Object = null;
        var i:int;

        var len:int = data.readShort();
        for (i = 0; i < this.tiles_.length; i++) {
            FreeList.deleteObject(this.tiles_[i]);
        }
        this.tiles_.length = 0;
        while (this.tiles_.length < len) {
            this.tiles_.push(FreeList.newObject(GroundTileData) as GroundTileData);
            this.tiles_[this.tiles_.length - 1].parseFromInput(data);
        }

        len = data.readShort();
        for (i = 0; i < this.newObjs_.length; i++) {
            FreeList.deleteObject(this.newObjs_[i]);
        }
        this.newObjs_.length = 0;
        while (this.newObjs_.length < len) {
            this.newObjs_.push(FreeList.newObject(ObjectData) as ObjectData);
            this.newObjs_[this.newObjs_.length - 1].parseFromInput(data);
        }

        len = data.readShort();
        for (i = 0; i < this.drops_.length; i++) {
            FreeList.deleteObject(this.drops_[i]);
        }
        this.drops_.length = 0;
        while (this.drops_.length < len) {
            this.drops_.push(FreeList.newObject(ObjectDropData) as ObjectDropData);
            this.drops_[this.drops_.length - 1].parseFromInput(data);
        }
    }

    override public function toString():String {
        return formatToString("UPDATE", "tiles_", "newObjs_", "drops_");
    }
}
}
