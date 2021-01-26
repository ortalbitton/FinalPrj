
$("#searchSRT").click(function () {
    $('#SRTList').show();
    $("#categoryNames").removeClass('open');
});

$("#chooseC").click(function () {
    $("#categoryNames").addClass('open');
});


$("#search").click(function () {
 
    var status = document.getElementById('status').value;

    $.ajax({
        url: "SearchSRT?pageNumber=" + 1 + "&status=" + status + "&isAuthenticated=" + true,
        method: "get",
        success: function (respon) {         
            $("body").html(respon)
        }
    });

});


var allSearchB = document.querySelectorAll('input[id=searchB]');

for (var i = 0; i < allSearchB.length; i++) {

    allSearchB[i].addEventListener('click', function () {

        var categoryName = $(this).attr("categoryName");


        $.ajax({
            url: "SearchSRT?pageNumber=" + 1 + "&categoryName=" + categoryName + "&isAuthenticated=" + true,
            method: "get",
            success: function (respon) {  
                    $("body").html(respon)
            }
        });

    });
}

// hide part of view of categoryNames
$(document).mouseup(function (e) {
    var a = $("#chooseC");
    if (!a.is(e.target) && a.has(e.target).length === 0 ) {
        $("#categoryNames").removeClass('open');
    }
});


