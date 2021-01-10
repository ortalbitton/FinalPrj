var allBlock = document.querySelectorAll('input[id=block]');

for (var i = 0; i < allBlock.length; i++) {

    allBlock[i].addEventListener('click', function () {

        var PostId = $(this).attr("PostId")
        var pageNumber = parseInt($(this).attr("pageNumber").toString())
        var numberbtn = $(this).attr("numberbtn")
        var fail_delete = $(this).attr("fail_delete")

        $.ajax({
            url: "Auto_Delete?PostId=" + PostId + "&pageNumber=" + pageNumber,
            method: "get",
            success: function (respon) {

                if (fail_delete == "true") {
                    allBlock[numberbtn].classList.add('style')
                    $(allBlock[numberbtn]).prop('disabled', true);

                }


            }


        });

    });


}


