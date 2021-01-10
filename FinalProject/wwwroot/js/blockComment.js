var allBlockC = document.querySelectorAll('input[id=blockC]');


for (var i = 0; i < allBlockC.length; i++) {

    allBlockC[i].addEventListener('click', function () {

        var commentId = $(this).attr("commentId")
        var pageNumber = parseInt($(this).attr("pageNumber").toString())
        var numberbtnC = $(this).attr("numberbtnC")
        var fail_delete = $(this).attr("fail_delete")

        $.ajax({
            url: "Auto_Delete?commentId=" + commentId + "&pageNumber=" + pageNumber,
            method: "get",
            success: function (respon) {

                if (fail_delete == "true") {
                    allBlockC[numberbtnC].classList.add('style')
                    $(allBlockC[numberbtnC]).prop('disabled', true);

                }


            }


        });

    });


}

