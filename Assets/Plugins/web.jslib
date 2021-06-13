mergeInto(LibraryManager.library, {

    NotImplenmation: function () {
        window.alert("此功能正在开发中！");
    },

    getRequest: function (ques, model) {
        ques_ = Pointer_stringify(ques);
        model_ = Pointer_stringify(model);
        var result;
        $.ajax({
            type: "POST",
            url: "http://166.111.7.106:17394/predict",
            async: false,
            data: JSON.stringify({
                "model": model_,
                "question": ques_,
            }),
            dataType: "json",
            contentType: 'application/json;charset=gb2312;'
        }).success(function (res) {
            console.log(res);
            result = res.answer;
        }).error(function (xhr, status) {
            console.log(xhr, status);
        });
        var returnStr = result;
        var bufferSize = lengthBytesUTF8(returnStr) + 1;
        var buffer = _malloc(bufferSize);
        stringToUTF8(returnStr, buffer, bufferSize);
        return buffer;
    },

    getLeaderBoard: function () {
        type = Pointer_stringify("leaderboard");
        var result;
        $.ajax({
            type: "POST",
            url: "http://166.111.7.106:17394/leaderboard",
            async: false,
            data: JSON.stringify({
                "type": type,
            }),
            dataType: "json",
            contentType: 'application/json;charset=gb2312;'
        }).success(function (res) {
            console.log(res);
            result = res.answer;
        }).error(function (xhr, status) {
            console.log(xhr, status);
        });
        var returnStr = result;
        var bufferSize = lengthBytesUTF8(returnStr) + 1;
        var buffer = _malloc(bufferSize);
        stringToUTF8(returnStr, buffer, bufferSize);
        return buffer;
    },

    getLeaderBoard: function () {
        type = Pointer_stringify("leaderboard");
        var result;
        $.ajax({
            type: "POST",
            url: "http://166.111.7.106:17394/leaderboard",
            async: false,
            data: JSON.stringify({
                "type": type,
            }),
            dataType: "json",
            contentType: 'application/json;charset=gb2312;'
        }).success(function (res) {
            console.log(res);
            result = res.answer;
        }).error(function (xhr, status) {
            console.log(xhr, status);
        });
        var returnStr = result;
        var bufferSize = lengthBytesUTF8(returnStr) + 1;
        var buffer = _malloc(bufferSize);
        stringToUTF8(returnStr, buffer, bufferSize);
        return buffer;
    },
    submitRanking: function (user, mode, score) {
        user_ = Pointer_stringify(user);
        mode_ = Pointer_stringify(mode);
        score_ = Pointer_stringify(score);
        var result;
        $.ajax({
            type: "POST",
            url: "http://166.111.7.106:17394/submit_ranking",
            async: false,
            data: JSON.stringify({
                "type": type,
            }),
            dataType: "json",
            contentType: 'application/json;charset=gb2312;'
        }).success(function (res) {
            console.log(res);
            result = res.answer;
        }).error(function (xhr, status) {
            console.log(xhr, status);
        });
        var returnStr = result;
        var bufferSize = lengthBytesUTF8(returnStr) + 1;
        var buffer = _malloc(bufferSize);
        stringToUTF8(returnStr, buffer, bufferSize);
        return buffer;
    }
});