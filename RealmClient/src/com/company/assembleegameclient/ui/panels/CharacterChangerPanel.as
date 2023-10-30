package com.company.assembleegameclient.ui.panels {
import com.company.assembleegameclient.game.GameSprite;

import flash.events.MouseEvent;

public class CharacterChangerPanel extends ButtonPanel {


    public function CharacterChangerPanel(gs:GameSprite) {
        super(gs, "Change Characters", "Change");
    }

    override protected function onButtonClick(event:MouseEvent):void {
        gs_.toHomeScreen = true;
        gs_.closed.dispatch();
    }
}
}
