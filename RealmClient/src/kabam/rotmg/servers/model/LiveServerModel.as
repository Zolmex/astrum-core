package kabam.rotmg.servers.model {
import com.company.assembleegameclient.parameters.Parameters;

import kabam.rotmg.core.model.PlayerModel;
import kabam.rotmg.servers.api.Server;
import kabam.rotmg.servers.api.ServerModel;

public class LiveServerModel implements ServerModel {

    [Inject]
    public var model:PlayerModel;

    private const servers:Vector.<Server> = new Vector.<Server>(0);

    public function setServers(list:Vector.<Server>):void {
        var server:Server = null;
        var isAdmin:Boolean = this.model.isAdmin();
        this.servers.length = 0;
        for each(server in list) {
            if (server.isAdminOnly && !isAdmin)
                continue;
            this.servers.push(server);
        }
    }

    public function getServers():Vector.<Server> {
        return this.servers;
    }

    public function getServer():Server {
        var server:Server = null;
        var priority:int = 0;
        var dist:Number = NaN;
        var isAdmin:Boolean = this.model.isAdmin();
        var closestServer:Server = null;
        var minDist:Number = Number.MAX_VALUE;
        var bestPriority:int = int.MAX_VALUE;
        for each(server in this.servers) {
            if (server.isAdminOnly && !isAdmin)
                continue;
            if (server.name == Parameters.data_.preferredServer) {
                return server;
            }
            priority = server.priority();
            if (priority < bestPriority || priority == bestPriority && dist < minDist) {
                closestServer = server;
                minDist = dist;
                bestPriority = priority;
            }
        }
        return closestServer;
    }

    public function isServerAvailable():Boolean {
        return this.servers.length > 0;
    }
}
}
