package com.company.assembleegameclient.clientminigames.uadapong {
import com.company.assembleegameclient.screens.*;
import com.company.ui.SimpleText;
import com.company.util.Random;

import flash.display.Sprite;
import flash.events.Event;
import flash.events.KeyboardEvent;
import flash.events.MouseEvent;
import flash.geom.Rectangle;
import flash.text.TextFieldAutoSize;
import flash.ui.Keyboard;
import flash.utils.getTimer;

import org.osflash.signals.Signal;

public class UadaPongView extends Sprite {

    public const close:Signal = new Signal();
    public var lastUpdate:int = 0;
    public var rand:Random = new Random(8008135);

    private var backButton:TitleMenuOption;
    private var tutorialText:SimpleText;
    private var paddleHits:SimpleText;
    private var ballSpeedText:SimpleText;
    private var player1Score:SimpleText;
    private var player2Score:SimpleText;
    private var paddle1:Sprite;
    private var paddle2:Sprite;
    private var ball:Sprite;
    private var uadaImage:UadaImage;

    private var rotationSpeed:Number = 0;
    private var rotationFriction:Number = 0.994;
    private var predictedY:Number = -1;
    private var timesPaddlesHit:int = 0;
    private var ballSpeedX:int = 0;
    private var ballSpeedY:int = 0;
    private var score1:int = 0;
    private var score2:int = 0;
    private var upArrowDown:Boolean = false;
    private var downArrowDown:Boolean = false;
    private var holdingShift:Boolean = false;

    public function UadaPongView() {
        super();
        this.tutorialText = createSimpleText("Hold Shift to Speed Up!", 18, 0xFFFF00);
        this.tutorialText.y = 600 - this.tutorialText.height;
        addChild(tutorialText);
        this.paddleHits = createSimpleText("Uada Combo: " + timesPaddlesHit, 22, 0xFFFFFF);
        this.paddleHits.x = 400 - this.paddleHits.width / 2;
        addChild(paddleHits);
        this.ballSpeedText = createSimpleText("Ball Speed: " + (Number)(1.0 + timesPaddlesHit / 20.0) + "x", 22, 0xFFFFFF);
        this.ballSpeedText.x = 400 - this.ballSpeedText.width / 2;
        this.ballSpeedText.y = 600 - this.ballSpeedText.height;
        addChild(ballSpeedText);
        this.player1Score = createSimpleText("Player 1 Score : " + score1, 22, 0xFFFFFF);
        addChild(player1Score);
        this.player2Score = createSimpleText("CPU Score : " + score2, 22, 0xFFFFFF);
        this.player2Score.x = 800 - this.player2Score.width;
        addChild(player2Score);
        this.paddle1 = new Sprite();
        this.paddle1.graphics.beginFill(0xFFFFFF);
        this.paddle1.graphics.drawRect(0, 0, 10, 60);
        this.paddle1.graphics.endFill();
        this.paddle1.x = 10;
        this.paddle1.y = 270;
        addChild(paddle1);
        this.paddle2 = new Sprite();
        this.paddle2.graphics.beginFill(0xFFFFFF);
        this.paddle2.graphics.drawRect(0, 0, 10, 60);
        this.paddle2.graphics.endFill();
        this.paddle2.x = 780;
        this.paddle2.y = 270;
        addChild(paddle2);
        this.ball = new Sprite();
        this.ball.graphics.beginFill(0xFFFFFF);
        this.ball.graphics.drawCircle(0, 0, 5);
        this.ball.graphics.endFill();
        this.ball.x = 400;
        this.ball.y = 300;
        this.uadaImage = new UadaImage();
        this.uadaImage.scaleX = 0.2; this.uadaImage.scaleY = 0.2;
        this.ball.addChild(uadaImage);
        this.uadaImage.x -= this.uadaImage.width / 2;
        this.uadaImage.y -= this.uadaImage.height / 2;
        addChild(ball);
        this.backButton = new TitleMenuOption("go back", 22, false);
        this.backButton.x = 750 - backButton.width;
        this.backButton.y = 550 - backButton.height;
        this.backButton.addEventListener(MouseEvent.MOUSE_UP, goBack);
        addChild(backButton);

        gameOver();

        this.lastUpdate = getTimer();
        addEventListener(Event.ENTER_FRAME, onEnterFrame);
        addEventListener(Event.ADDED_TO_STAGE, onAddedToStage);
    }

    public function reloadText():void {
        this.paddleHits.setText("Uada Combo: " + timesPaddlesHit);
        this.ballSpeedText.setText("Ball Speed: " + (Number)(1.0 + timesPaddlesHit / 20.0) + "x");
        this.player1Score.setText("Player 1 Score : " + score1);
        this.player2Score.setText("CPU Score : " + score2);
        this.player2Score.x = 800 - this.player2Score.width;
    }

    public function onKeyDown(e:KeyboardEvent):void {
        if (e.keyCode == Keyboard.UP) {
            upArrowDown = true;
        } else if (e.keyCode == Keyboard.DOWN) {
            downArrowDown = true;
        } else if (e.keyCode == Keyboard.SHIFT) {
            holdingShift = true;
        }
    }

    public function onKeyUp(e:KeyboardEvent):void {
        if (e.keyCode == Keyboard.UP) {
            upArrowDown = false;
        } else if (e.keyCode == Keyboard.DOWN) {
            downArrowDown = false;
        } else if (e.keyCode == Keyboard.SHIFT) {
            holdingShift = false;
        }
    }

    public function onEnterFrame(e:Event):void {
        var time:int = getTimer();
        var dt:int = time - this.lastUpdate;
        var paddleSpeed:int = holdingShift ? 200 : 100;
        var ballSpeedMult:Number = 1 + timesPaddlesHit / 20;

        rotationSpeed *= rotationFriction;
        this.ball.rotation += rotationSpeed * (dt / 1000.0);

        ball.x += (ballSpeedX * ballSpeedMult) * (dt / 1000.0);
        ball.y += (ballSpeedY * ballSpeedMult) * (dt / 1000.0);

        var paddle1Rect:Rectangle = paddle1.getBounds(this);
        if (ball.x - ball.width / 2 <= paddle1Rect.right &&
                ball.x + ball.width / 2 >= paddle1Rect.left &&
                ball.y + ball.height / 2 >= paddle1Rect.top &&
                ball.y - ball.height / 2 <= paddle1Rect.bottom) {

            ball.x = paddle1Rect.right + ball.width / 2;
            hitPaddle();
            reloadText();
        }
        var paddle2Rect:Rectangle = paddle2.getBounds(this);
        if (ball.x - ball.width / 2 <= paddle2Rect.right &&
                ball.x + ball.width / 2 >= paddle2Rect.left &&
                ball.y + ball.height / 2 >= paddle2Rect.top &&
                ball.y - ball.height / 2 <= paddle2Rect.bottom) {

            ball.x = paddle2Rect.left - ball.width / 2;
            hitPaddle();
            reloadText();
            predictedY = -1;
        }
        if (upArrowDown && (paddle1.y) > 0) {
            paddle1.y -= paddleSpeed * (dt / 1000.0);
        }
        if (downArrowDown && (paddle1.y + paddle1.height) < 600) {
            paddle1.y += paddleSpeed * (dt / 1000.0);
        }
        if (ball.y <= 0 || ball.y >= 600) {
            ball.y = (ball.y <= 0) ? ball.height / 2 : 600 - ball.height / 2;
            ballSpeedY = -ballSpeedY;
        }
        if (ball.x <= 0) {
            score2++;
            gameOver();
        } else if (ball.x >= 800) {
            score1++;
            gameOver();
        }
        var paddle2y:Number = paddle2.y + paddle2.height / 2;
        var vertDist:Number = Math.abs(ball.y - paddle2y);
        var horzDist:Number = Math.abs(ball.x - paddle2.x);
        var diff:Number = 0;

        /*if (horzDist < 750 && horzDist > 550 && ballSpeedX > 0) {
            if (vertDist < 0) {
                paddle2.y -= 100 * (dt / 1000.0);
            } else {
                paddle2.y += 100 * (dt / 1000.0);
            }
        } else */if (horzDist < 750 && ballSpeedX > 0) {
            if (predictedY == -1) {
                predictedY = predictBallLanding(ball.x, ball.y, ballSpeedX, ballSpeedY, paddle2.x);
            }
            diff = predictedY - paddle2y;

            if (Math.abs(diff) > 5 && diff < 0) {
                paddle2.y -= 300 * (dt / 1000.0);
            } else if (Math.abs(diff) > 5) {
                paddle2.y += 300 * (dt / 1000.0);
            }
        } else {
            predictedY = -1;
            var midDist:Number = 300 - paddle2y;
            if (Math.abs(midDist) > 5) {
                if (midDist < 0) {
                    paddle2.y -= 100 * (dt / 1000.0);
                } else {
                    paddle2.y += 100 * (dt / 1000.0);
                }
            }
        }
        this.lastUpdate = time;
    }

    public function predictBallLanding(ballX:Number, ballY:Number, ballSpeedX:int, ballSpeedY:int, paddleX:Number):Number {
        var timeToPaddle:Number = (paddleX - ballX) / ballSpeedX;
        var predictedY:Number = ballY + (ballSpeedY * timeToPaddle);

        while (predictedY < 0 || predictedY > 600) {
            if (predictedY < 0) {
                predictedY = -predictedY;
                ballSpeedY = -ballSpeedY;
            } else if (predictedY > 600) {
                predictedY = 1200 - predictedY;
                ballSpeedY = -ballSpeedY;
            }
        }
        return predictedY;
    }

    public function hitPaddle():void {
        ballSpeedX = -ballSpeedX;
        timesPaddlesHit++;
        rotationSpeed += 720;
    }

    public function gameOver():void {
        ball.x = 400;
        ball.y = 300;
        ball.rotation = 0;
        rotationSpeed = 0;
        timesPaddlesHit = 0;
        ballSpeedX = rand.nextIntRange(1, 3) == 1 ? -200 : 200;
        ballSpeedY = rand.nextIntRange(1, 3) == 1 ? -200 : 200;
        reloadText();
    }

    public function createSimpleText(text:String, size:int, color:uint):SimpleText {
        var simpleText:SimpleText = new SimpleText(size, color);
        simpleText.setAutoSize(TextFieldAutoSize.LEFT);
        simpleText.setText(text);
        return simpleText;
    }

    private function onAddedToStage(e:Event):void {
        removeEventListener(Event.ADDED_TO_STAGE, onAddedToStage);
        addEventListener(Event.REMOVED_FROM_STAGE, onRemovedFromStage);
        stage.addEventListener(KeyboardEvent.KEY_DOWN, onKeyDown);
        stage.addEventListener(KeyboardEvent.KEY_UP, onKeyUp);
    }

    public function onRemovedFromStage(e:Event):void {
        removeEventListener(Event.ENTER_FRAME, this.onEnterFrame);
        stage.removeEventListener(KeyboardEvent.KEY_DOWN, onKeyDown);
        stage.removeEventListener(KeyboardEvent.KEY_UP, onKeyUp);
    }

    public function goBack(e:Event):void {
        this.close.dispatch();
    }
}
}
