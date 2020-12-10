
var allEdit = document.querySelectorAll('input[id^=edit]');

var allEditPosts = document.querySelectorAll('div[id^=EditPost]');

var allPosts = document.querySelectorAll('div[class^=post]');

var numberComment = 0;

for (var i = 0; i < allEdit.length; i++) {

    allEdit[i].addEventListener('click', function () {

        var numberbtn = $(this).attr("numberbtn")

        allPosts[parseInt(numberbtn)].classList.add('hide');

        var f = document.createElement("form");
        f.setAttribute('action', "EditPost");

        var form = f
        var url = form.action;

        var PostId = $(this).attr("PostId")
        var pageNumber = parseInt($(this).attr("pageNumber").toString())

        allEditPosts[parseInt(numberbtn)].classList.add('open');

        $.ajax({
            url: url + "?PostId=" + PostId + "&pageNumber=" + pageNumber,
            method: "get",
            success: function (respon) {
                allEditPosts[numberbtn].innerHTML = respon;
            }

        });

        numberComment = numberbtn;
    });
}

// hide view Editpost
$(document).mouseup(function (e) {
    if (allEdit.length != 0) {
        var a = allEditPosts[parseInt(numberComment)];
        if (!a.contains(e.target)) {
            allEditPosts[parseInt(numberComment)].classList.remove('open');
            allPosts[parseInt(numberComment)].classList.remove('hide');
        }
    }
});


