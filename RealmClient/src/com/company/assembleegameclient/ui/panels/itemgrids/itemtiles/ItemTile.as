package com.company.assembleegameclient.ui.panels.itemgrids.itemtiles {
import com.company.assembleegameclient.itemData.ItemData;
import com.company.assembleegameclient.objects.ObjectLibrary;
import com.company.assembleegameclient.objects.Player;
import com.company.assembleegameclient.ui.panels.itemgrids.ItemGrid;
import com.company.util.GraphicsUtil;

import flash.display.GraphicsPath;
import flash.display.GraphicsSolidFill;
import flash.display.IGraphicsData;
import flash.display.Shape;
import flash.display.Sprite;

import kabam.rotmg.constants.ItemConstants;

public class ItemTile extends Sprite {

    public static const TILE_DOUBLE_CLICK:String = "TILE_DOUBLE_CLICK";
    public static const TILE_SINGLE_CLICK:String = "TILE_SINGLE_CLICK";
    public static const WIDTH:int = 40;
    public static const HEIGHT:int = 40;
    public static const BORDER:int = 4;


    private var fill_:GraphicsSolidFill = new GraphicsSolidFill(getBackgroundColor(), 1);
    private var path_:GraphicsPath = new GraphicsPath(new Vector.<int>(), new Vector.<Number>());
    private var graphicsData_:Vector.<IGraphicsData> = new <IGraphicsData>[fill_, path_, GraphicsUtil.END_FILL];
    private var restrictedUseIndicator:Shape;
    public var itemSprite:ItemTileSprite;
    public var tileId:int;
    public var ownerGrid:ItemGrid;

    public function ItemTile(id:int, parentGrid:ItemGrid) {
        super();
        this.tileId = id;
        this.ownerGrid = parentGrid;
        this.restrictedUseIndicator = new Shape();
        addChild(this.restrictedUseIndicator);
        this.setItemSprite(new ItemTileSprite());
    }

    public function drawBackground(cuts:Array):void {
        GraphicsUtil.clearPath(this.path_);
        GraphicsUtil.drawCutEdgeRect(0, 0, WIDTH, HEIGHT, 4, cuts, this.path_);
        graphics.clear();
        graphics.drawGraphicsData(this.graphicsData_);
        var fill:GraphicsSolidFill = new GraphicsSolidFill(6036765, 1);
        GraphicsUtil.clearPath(this.path_);
        var graphicsData:Vector.<IGraphicsData> = new <IGraphicsData>[fill, this.path_, GraphicsUtil.END_FILL];
        GraphicsUtil.drawCutEdgeRect(0, 0, WIDTH, HEIGHT, 4, cuts, this.path_);
        this.restrictedUseIndicator.graphics.drawGraphicsData(graphicsData);
        this.restrictedUseIndicator.cacheAsBitmap = true;
        this.restrictedUseIndicator.visible = false;
    }

    public function setItem(item:ItemData):Boolean {
        if (item == this.itemSprite.itemData) {
            return false;
        }
        this.itemSprite.setType(item);
        this.updateUseability(this.ownerGrid.curPlayer);
        return true;
    }

    public function setItemSprite(itemTileSprite:ItemTileSprite):void {
        this.itemSprite = itemTileSprite;
        this.itemSprite.x = WIDTH / 2;
        this.itemSprite.y = HEIGHT / 2;
        addChild(this.itemSprite);
    }

    public function updateUseability(player:Player):void {
        if (this.itemSprite.itemData != null) {
            this.restrictedUseIndicator.visible = !ObjectLibrary.isUsableByPlayer(this.itemSprite.itemData.ObjectType, player);
        }
        else {
            this.restrictedUseIndicator.visible = false;
        }
    }

    public function canHoldItem(type:int):Boolean {
        return true;
    }

    public function resetItemPosition():void {
        this.setItemSprite(this.itemSprite);
    }

    public function getItemId():int {
        if (this.itemSprite.itemData == null)
            return -1;
        return this.itemSprite.itemData.ObjectType;
    }

    public function getItemData():ItemData {
        return this.itemSprite.itemData;
    }

    protected function getBackgroundColor():int {
        return 5526612;
    }
}
}
