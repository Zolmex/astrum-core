package com.company.assembleegameclient.map
{
import robotlegs.bender.bundles.mvcs.Mediator;

public class MapMediator extends Mediator
{
    [Inject]
    public var view:Map;

    public function MapMediator()
    {
        super();
    }

    override public function initialize() : void
    {
    }

    override public function destroy() : void
    {
    }
}
}
