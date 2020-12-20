
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



