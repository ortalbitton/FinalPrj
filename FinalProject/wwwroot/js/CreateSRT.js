
$("#createSRT").click(function () {

    $("#selectkey").removeClass('open');
    $("#inputfile").show();

});



$("#VideoFile").change(function () {

    document.getElementById("namefile").innerHTML = this.files[0].name;

    //this.files[0].size gets the size of your file.
    if (this.files[0].size > 2147483647) {
        document.getElementById("sizelimit").innerHTML = "the file is too large";
    }
    //if (this.files[0].size > 20971520000) {
    //    document.getElementById("sizelimit").innerHTML = "the file is too large";
    //}


});

$("#next").click(function () {
    $("#selectkey").addClass('open');
    $("#inputfile").hide();
    $("#next").hide();
});

//the user can not selectkey more than three key(CategoryName)
$(function () {
    var list1 = $('#selectkey > h2 > #checkBox');

    list1.on('change', function (event) {
        var numList1 = $(list1).filter(':checked').length;
        if (numList1 >= 3) {
            $(list1).filter(':not(:checked)').prop('disabled', true);
        } else {
            $(list1).prop('disabled', false);
        }
    });
});