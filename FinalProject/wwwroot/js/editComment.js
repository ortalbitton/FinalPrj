var allEdit = document.querySelectorAll('input[id=commentEdit]');

var allEditComments = document.querySelectorAll('div[id=EditComment]');

var allComments = document.querySelectorAll('div[class=comment]');

var numberCommentper = 0;

for (var i = 0; i < allEdit.length; i++) {

    allEdit[i].addEventListener('click', function () {

        var numberbtnC = $(this).attr("numberbtnC")

        var userNameofComment = $(this).attr("userNameofComment")
        var userName = $(this).attr("userName")

        if (userNameofComment == userName) {

            allComments[parseInt(numberbtnC)].classList.add('hide');

            var f = document.createElement("form");
            f.setAttribute('action', "../Comment/EditComment");

            var form = f
            var url = form.action;

            var CommentId = $(this).attr("CommentId")
            var pageNumber = parseInt($(this).attr("pageNumber").toString())

            allEditComments[parseInt(numberbtnC)].classList.add('open');

            $.ajax({
                url: url + "?CommentId=" + CommentId + "&pageNumber=" + pageNumber,
                method: "get",
                success: function (respon) {
                    allEditComments[numberbtnC].innerHTML = respon;
                }

            });

            numberCommentper = numberbtnC;
        }
        else {
            $(this).prop('disabled', true);
            $(this).css('pointer-events', 'none');
            $(this).css('background-color', 'gainsboro');
            $(this).val('no-edit');
        }

    });

}

// cancel view EditComment
$(".editbuttonC").click(function (e) {
    allEditComments[parseInt(numberCommentper)].classList.remove('open');
    allComments[parseInt(numberCommentper)].classList.remove('hide');

});

