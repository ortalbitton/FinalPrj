
var allReply = document.querySelectorAll('input[id^=reply]');

var allNewComments = document.querySelectorAll('div[id^=NewComment]');

var allComments = document.querySelectorAll('div[class^=comments]');

var numberComment = 0;

for (var i = 0; i < allReply.length; i++) {

    allReply[i].addEventListener('click', function () {

        var numberbtn = $(this).attr("numberbtn")

        allComments[parseInt(numberbtn)].classList.add('hide');

        var f = document.createElement("form");
        f.setAttribute('action', "../Comment/NewComment");

        var form = f
        var url = form.action;

        var PostId = $(this).attr("PostId")
        var pageNumber = parseInt($(this).attr("pageNumber").toString())

        allNewComments[parseInt(numberbtn)].classList.add('open');

        $.ajax({
            url: url + "?PostId=" + PostId + "&pageNumber=" + pageNumber,
            method: "get",
            success: function (respon) {
                allNewComments[numberbtn].innerHTML = respon;
            }

        });

        numberComment = numberbtn;
    });
}

// hide view NewComment
$(document).mouseup(function (e) {
    if (allReply.length != 0) {
        var a = allNewComments[parseInt(numberComment)];
        if (!a.contains(e.target)) {
            allNewComments[parseInt(numberComment)].classList.remove('open');
            allComments[parseInt(numberComment)].classList.remove('hide');
        }
    }
});









