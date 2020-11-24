$("#newPostbtn").click(function () {

    $('.tweet-body').hide();
    $('#container').hide();

    var pageNumber = $(this).attr("pageNumber")

    $.ajax({
        url: "NewPost?pageNumber=" + pageNumber,
        method: "get",
        success: function (respon) {
            $("#newPost").html(respon)
        }
    });

});

