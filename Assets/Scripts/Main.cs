using System.Collections;
using System.Collections.Generic;
using com.unity.cloudbase;
using com.unity.mgobe;
using com.unity.mgobe.src.Util;
using UnityEngine;

public class Main : MonoBehaviour {
    // Start is called before the first frame update
    void Start () {
        Debug.Log ("helloworld");
        initSDK ();
    }

    // Update is called once per frame
    void Update () {

    }

    // 初始化 mgobe SDK
    public void initSDK () {
        // Global.GameId = "XXXXXXXXXXXXXXX";
        // Global.SecretKey = "XXXXXXXXXXXXXXXXXXXXXXXXXX";
        // Global.Server = "XXXXXXXXXXXXXX.wxlagame.com";
        Global.GameId = "obg-8xwt9z7a";
        Global.SecretKey = "f857f0dc320e77ed46081407645b3a9fcfd40983";
        Global.Server = "8xwt9z7a.wxlagame.com";

        GameInfoPara gameInfo = new GameInfoPara {
            GameId = Global.GameId,
            SecretKey = Global.SecretKey,
            OpenId = "Global.OpenId"
        };
        ConfigPara config = new ConfigPara {
            Url = Global.Server,
            ReconnectMaxTimes = 5,
            ReconnectInterval = 4000,
            ResendInterval = 2000,
            ResendTimeout = 20000,
            IsAutoRequestFrame = true,
        };

        // 初始化监听器 Listener
        Listener.Init (gameInfo, config, (ResponseEvent eve) => {
            if (eve.Code == ErrCode.EcOk) {
                Debugger.Log ("init sdk");
                CreateRoom();
            }
            // 初始化广播回调事件
        });
    }
    void CreateRoom () {
        CreateTeamRoomPara para = new CreateTeamRoomPara {
            RoomName = "",
            MaxPlayers = 2,
            RoomType = "",
            CustomProperties = "0",
            IsPrivate = false,
            PlayerInfo = new PlayerInfoPara { },
            TeamNumber = 2
        };

        // 创建团队房间
        Global.Room.CreateTeamRoom (para, eve => {
            if (eve.Code == 0) {
                Debugger.Log ("create Team Room Success: {0}", eve.Code);
            } else {
                Debugger.Log ("create Team Room Fail: {0}", eve.Code);
            }

        });
    }
}