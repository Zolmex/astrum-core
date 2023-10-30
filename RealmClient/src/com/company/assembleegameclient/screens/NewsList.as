package com.company.assembleegameclient.screens {
import com.company.assembleegameclient.appengine.SavedNewsItem;

import flash.display.Sprite;

import kabam.rotmg.core.model.PlayerModel;

public class NewsList extends  Sprite {

    private var lines_:Vector.<NewsLine>;

    public function NewsList(model:PlayerModel) {
        super();
        this.lines_ = new Vector.<NewsLine>();
        var news:SavedNewsItem;
        for each (news in model.getNews()){
            this.addLine(new NewsLine(news.getIcon(), news.title_, news.tagline_, news.link_, news.date_, model.getAccountId()));
        }
    }

    public function addLine(newsLine:NewsLine) : void {
        newsLine.y = (4 + (this.lines_.length * (NewsLine.HEIGHT + 4)));
        this.lines_.push(newsLine);
        addChild(newsLine);
    }
}
}
