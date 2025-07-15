function sendsub(ga_cookie) {
    var xhr = new XMLHttpRequest();
    var url = "https://api.ydplatform.com/campaignmng/sub_notification.ashx";
    xhr.open("POST", url, true);
    xhr.setRequestHeader("Content-Type", "application/json");
    xhr.onreadystatechange = function () {
        if (xhr.readyState === 4 && xhr.status === 200) {
            var json = JSON.parse(xhr.responseText);
            console.log(json.email + ", " + json.password);
        }
    };
    var data = JSON.stringify({
        "email": "hey@mail.com", "password": "101010", "ga_cookie": new String(ga_cookie) });
    xhr.send(data);
}