﻿namespace GameServer.Game.Net.Messaging
{
    public enum PacketId : byte
    {
        FAILURE,
        CREATESUCCESS,
        CREATE,
        PLAYERSHOOT,
        MOVE,
        PLAYERTEXT,
        TEXT,
        SERVERPLAYERSHOOT,
        DAMAGE,
        UPDATE,
        NOTIFICATION,
        NEWTICK,
        INVSWAP,
        USEITEM,
        SHOWEFFECT,
        HELLO,
        GOTO,
        INVDROP,
        INVRESULT,
        RECONNECT,
        MAPINFO,
        LOAD,
        TELEPORT,
        USEPORTAL,
        DEATH,
        BUY,
        BUYRESULT,
        AOE,
        PLAYERHIT,
        ENEMYHIT,
        AOEACK,
        SHOOTACK,
        SQUAREHIT,
        EDITACCOUNTLIST,
        ACCOUNTLIST,
        QUESTOBJID,
        CREATEGUILD,
        GUILDRESULT,
        GUILDREMOVE,
        GUILDINVITE,
        ALLYSHOOT,
        ENEMYSHOOT,
        ESCAPE,
        INVITEDTOGUILD,
        JOINGUILD,
        CHANGEGUILDRANK,
        PLAYSOUND,
        RESKIN,
        GOTOACK
    }
}