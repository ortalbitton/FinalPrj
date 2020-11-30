
var allReply = document.querySelectorAll('input[id^=reply]');

$('#myform').hide();

for (var i = 0; i < allReply.length; i++) {

    allReply[i].addEventListener('click', function () {

        var form = $('#myform')
        var url = form.attr('action')

        $('#myform').show();

        var PostId = $(this).attr("PostId")
        var pageNumber = parseInt($(this).attr("pageNumber").toString())

        $.ajax({
            url: url + "?PostId=" + PostId + "&pageNumber=" + pageNumber,
            method: "get",
            success: function (respon) {
                $("#NewComment").html(respon)
            }
        });

    });
}





