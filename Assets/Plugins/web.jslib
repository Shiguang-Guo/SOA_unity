mergeInto(LibraryManager.library, {

    NotImplenmation: function () {
        window.alert("此功能正在开发中！");
    },

    getRequest: function () {
        let result;
        $.ajax({
            type: "POST",
            url: "http://166.111.7.106:17394/predict",
            async: false,
            data: JSON.stringify({
                "model": "bert",
                "question": "The theory of relativity was developed by [MASK] .",
            }),
            dataType: "json",
            contentType: 'application/json;charset=gb2312;'
        }).success(function (res) {
            console.log(res)
            result = res.answer;
        }).error(function (xhr, status) {
            console.log(xhr, status);
        });
        let returnStr = result;
        let bufferSize = lengthBytesUTF8(returnStr) + 1;
        let buffer = _malloc(bufferSize);
        stringToUTF8(returnStr, buffer, bufferSize);
        return buffer;
    }

});