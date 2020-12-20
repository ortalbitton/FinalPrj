var allDelete = document.querySelectorAll('input[id=deleteC]');

for (var i = 0; i < allDelete.length; i++) {

    allDelete[i].addEventListener('click', function () {

        var userNameofComment = $(this).attr("userNameofComment")
        var userName = $(this).attr("userName")

        if (userNameofComment == userName) {

            var f = document.createElement("form");
            f.setAttribute('action', "../Comment/DeleteComment");

            var form = f
            var url = form.action;

            var CommentId = $(this).attr("CommentId")
            var pageNumber = parseInt($(this).attr("pageNumber").toString())

            var totalPages = parseInt($(this).attr("totalPages").toString())

            $.ajax({
                url: url + "?CommentId=" + CommentId + "&pageNumber=" + pageNumber,
                method: "get",
                success: function () {

                    $.ajax({
                        url: "Refresh?pageNumber=" + pageNumber + "&totalPages=" + totalPages,
                        method: "get",
                        success: function (respon) {
                            $("body").html(respon)
                        }

                    });


                }

            });
        }
        else {
            $(this).prop('disabled', true);
            $(this).css('pointer-events', 'none');
            $(this).css('background-color', 'gainsboro');
            $(this).val('no-delete');
        }

    });
}

