package kabam.rotmg.servers.control {
import com.company.assembleegameclient.parameters.Parameters;

import kabam.rotmg.servers.api.Server;
import kabam.rotmg.servers.api.ServerModel;

public class ParseServerDataCommand {

    [Inject]
    public var servers:ServerModel;

    [Inject]
    public var data:XML;

    public function ParseServerDataCommand() {
        super();
    }

    public function execute():void {
        this.servers.setServers(this.makeListOfServers());
    }

    private function makeListOfServers():Vector.<Server> {
        var xml:XML = null;
        var serverList:XMLList = this.data.child("Servers").child("Server");
        var list:Vector.<Server> = new Vector.<Server>(0);
        for each(xml in serverList) {
            list.push(makeServer(xml));
        }
        return list;
    }

    private static function makeServer(xml:XML):Server {
        return new Server().setName(xml.Name).setAddress(xml.DNS).setPort(xml.Port || Parameters.GAME_PORT).setUsage(xml.Players, xml.MaxPlayers).setIsAdminOnly(xml.AdminOnly);
    }
}
}
