var allDelete = document.querySelectorAll('input[id=deleteC]');

for (var i = 0; i < allDelete.length; i++) {

    allDelete[i].addEventListener('click', function () {

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
    });
}

