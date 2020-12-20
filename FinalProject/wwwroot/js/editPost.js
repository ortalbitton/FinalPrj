
var allEdit = document.querySelectorAll('input[id=edit]');

var allEditPosts = document.querySelectorAll('div[id=EditPost]');

var allPosts = document.querySelectorAll('div[class=post]');

var numberComment = 0;

for (var i = 0; i < allEdit.length; i++) {

    allEdit[i].addEventListener('click', function () {

        var numberbtn = $(this).attr("numberbtn")

        var userNameofPost = $(this).attr("userNameofPost")
        var userName = $(this).attr("userName")

        if (userNameofPost == userName) {

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
        }
        else {
            $(this).prop('disabled', true);
            $(this).css('pointer-events', 'none');
            $(this).css('background-color', 'gainsboro');
            $(this).val('no-edit');
        }
    });
}

// cancel view Editpost
$(".editbutton").click(function (e) {
    allEditPosts[parseInt(numberComment)].classList.remove('open');
    allPosts[parseInt(numberComment)].classList.remove('hide');

});


