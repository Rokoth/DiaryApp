// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

function getList(page, currTab) {
    let url = null;
    //alert("currTab:" + currTab);
    switch (currTab) {
        case "List":
            url = "/Home/List?size=10&page=" + page; break;
        case "Daily":
            url = "/Home/Daily?offset=" + page; break;
        case "Weekly":
            url = "/Home/Weekly?offset=" + page; break;
        case "Monthly":
            url = "/Home/Monthly?offset=" + page; break;
        default:
            url = "/Home/List?size=10&page=" + page; break;           
    }
    //alert("url:" + url);
    if (url !== null) {
        $.ajax({
            type: "GET",
            url: url,
            contentType: 'text/plain; charset=utf-8',
            success: function (data) {
                $('#content').html(data);
            },
            error: function () {
                alert("Error occured!!")
            }
        });
    }
}

$(document).ready(function () {
    let currPage = 0;
    let currTab = 'List';
    getList(currPage, currTab);

    $('.tabButton').click(function () {
        if (currTab !== this.getAttribute("id")) {
            //alert(this.getAttribute("id"));
            currTab = this.getAttribute("id");
            currPage = 0;
            getList(currPage, currTab);
        }
    });

    $('#prevIndex').click(function () {
        if (currPage > 0 || currTab !== 'List') currPage--;
        getList(currPage, currTab);
    });

    $('#nextIndex').click(function () {
        currPage++;
        getList(currPage, currTab);
    });


});
