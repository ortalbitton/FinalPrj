var allDelete = document.querySelectorAll('input[id=delete]');

for (var i = 0; i < allDelete.length; i++) {

    allDelete[i].addEventListener('click', function () {

        var userNameofPost = $(this).attr("userNameofPost")
        var userName = $(this).attr("userName")
       
        if (userNameofPost == userName) {

            var f = document.createElement("form");
            f.setAttribute('action', "DeletePost");

            var form = f
            var url = form.action;

            var PostId = $(this).attr("PostId")
            var pageNumber = parseInt($(this).attr("pageNumber").toString())

            var totalPages = parseInt($(this).attr("totalPages").toString())

            $.ajax({
                url: url + "?PostId=" + PostId + "&pageNumber=" + pageNumber,
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

