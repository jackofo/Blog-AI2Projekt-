// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

//loadCategories();
//loadTags();
//pops();

function enableEditMode() {
    richText.document.designMode = 'on';
}

function execCmd(cmd) {
    richText.document.execCommand(cmd);
}

function execCmdArgs(cmd, args) {
    richText.document.execCommand(cmd, false, args);
}

function beforeSubmit() {
    var rt = richText;
    var str = richText.document.body.innerHTML;
    console.log(str);
    $('#richtextvalue').val(str);
}

$(function loadCategories() {
    console.log("cats");
    var xmlHttp = new XMLHttpRequest();
    xmlHttp.overrideMimeType("application/json");
    xmlHttp.onreadystatechange = function () {
        if (xmlHttp.readyState == 4 && xmlHttp.status == 200) {
            var cats = JSON.parse(xmlHttp.response);
            for (var i = 0; i < cats.length; i++) {
                document.getElementById("cats").innerHTML += "<li><a href='/Categories/Posts/" + cats[i].id + "'>" + cats[i].name + "</a></li>";
            }
        }
    }
    xmlHttp.open("GET", "/Categories/List", true); // false for synchronous request
    xmlHttp.send(null);
})

$(function loadTags() {
    console.log("tags");
    var xmlHttpT = new XMLHttpRequest();
    xmlHttpT.overrideMimeType("application/json");
    xmlHttpT.onreadystatechange = function () {
        if (xmlHttpT.readyState == 4 && xmlHttpT.status == 200) {
            var cats = JSON.parse(xmlHttpT.response);
            for (var i = 0; i < cats.length; i++) {
                document.getElementById("tags").innerHTML += "<a href='/Tags/Posts/" + cats[i].id + "'>" + "#" + cats[i].name + "</a> ";
            }
        }
    }
    xmlHttpT.open("GET", "/Tags/List", true); // false for synchronous request
    xmlHttpT.send(null);
})

function resizeIframe(obj) {
    obj.style.height = obj.contentWindow.document.documentElement.scrollHeight + 'px';
}

$(function pops () {
    $('[data-toggle="popover"]').popover()
})