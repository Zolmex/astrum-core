package kabam.rotmg.servers.api {
public class Server {

    public var name:String;
    public var address:String;
    public var port:int;
    public var players:int;
    public var maxPlayers:int;
    public var usage:Number;
    public var adminOnly:Boolean;

    public function Server() {
        super();
    }

    public function setName(name:String):Server {
        this.name = name;
        return this;
    }

    public function setAddress(address:String):Server {
        this.address = address;
        return this;
    }

    public function setPort(port:int):Server {
        this.port = port;
        return this;
    }

    public function setUsage(players:int, maxPlayers:int):Server {
        this.players = players;
        this.maxPlayers = maxPlayers;
        this.usage = Number(players) / Number(maxPlayers);
        return this;
    }

    public function setIsAdminOnly(str:String):Server {
        this.adminOnly = str == "true";
        return this;
    }

    public function priority():int {
        if (this.isCrowded()) {
            return 1;
        }
        return 0;
    }

    public function get isAdminOnly():Boolean {
        return this.adminOnly;
    }

    public function isCrowded():Boolean {
        return this.usage >= 0.66;
    }

    public function isFull():Boolean {
        return this.usage >= 1;
    }

    public function toString():String {
        return "[" + this.name + ": " + this.address + ":" + this.port + ":" + this.usage + ":" + this.isAdminOnly + "]";
    }
}
}
