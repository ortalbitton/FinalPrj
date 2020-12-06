
var allReply = document.querySelectorAll('input[id^=reply]');

var allNewComments = document.querySelectorAll('div[id^=NewComment]');

var numberComment = 0;

for (var i = 0; i < allReply.length; i++) {

    allReply[i].addEventListener('click', function () {

        $('.comments').hide();

        var f = document.createElement("form");
        f.setAttribute('action', "../Comment/NewComment");

        var form = f
        var url = form.action;

        var PostId = $(this).attr("PostId")
        var pageNumber = parseInt($(this).attr("pageNumber").toString())
        var numberbtn = $(this).attr("numberbtn")

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
    var a = allNewComments[parseInt(numberComment)];
    if (!a.contains(e.target)) {
        allNewComments[parseInt(numberComment)].classList.remove('open');
        $('.comments').show();
   }
});









