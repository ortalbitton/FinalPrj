
$("#newPostbtn").click(function () {

    $('#PostsAndComments').hide();
    $("#newPost").addClass('open');


    $.ajax({
        url: "NewPost",
        method: "get",
        success: function (respon) {
            $("#newPost").html(respon)
        }
    });

});

// hide view NewPost
$(document).mouseup(function (e) {
    var a = $("#newPost");
    if (!a.is(e.target) && a.has(e.target).length == 0) {
        $("#newPost").removeClass('open');
        $('#PostsAndComments').show();
    }
});

