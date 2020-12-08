$("#sizelimit").hide();

$("#VideoFile").change(function () {

    var a = document.getElementById("VideoFile");
    document.getElementById("namefile").innerHTML = this.files[0].name;

    //this.files[0].size gets the size of your file.
    if (this.files[0].size > 2147483647) {
        $("#sizelimit").show();
    }

});