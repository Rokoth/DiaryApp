// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

function getList(page, currTab, filter) {
    let url = null;
    //alert("currTab:" + currTab);
    $('#content').html('Loading...');
    switch (currTab) {
        case "List":
            url = "/Diary/List?size=10&page=" + page + filter; break;
        case "Daily":
            url = "/Diary/Daily?offset=" + page; break;
        case "Weekly":
            url = "/Diary/Weekly?offset=" + page; break;
        case "Monthly":
            url = "/Diary/Monthly?offset=" + page; break;
        default:
            url = "/Diary/List?size=10&page=" + page; break;           
    }
    //alert("url:" + url);
    if (url !== null) {
        $.ajax({
            type: "GET",
            url: url,
            contentType: 'text/plain; charset=utf-8',
            success: function (data, textStatus, request) {
                $('#content').html(data);
                if (request.getResponseHeader('X-Header-IsFirstPage') !== undefined) {
                    $('#prevIndex').attr('enabled', !request.getResponseHeader('X-Header-IsFirstPage'));
                }
                if (request.getResponseHeader('X-Header-IsLastPage') !== undefined) {
                    $('#nextIndex').attr('enabled', !request.getResponseHeader('X-Header-IsLastPage'));
                }
                if (request.getResponseHeader('X-Header-AllCount') !== undefined) {
                    $('#allCount').html(request.getResponseHeader('X-Header-AllCount'));
                }                
                if (request.getResponseHeader('X-Header-BeginNumber') !== undefined) {
                    $('#beginNumber').html(request.getResponseHeader('X-Header-BeginNumber'));
                }
                if (request.getResponseHeader('X-Header-EndNumber') !== undefined) {
                    $('#lastNumber').html(request.getResponseHeader('X-Header-EndNumber'));
                }
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
    let filter = '';

    getList(currPage, currTab, filter);

    //$('.filterItem').change(function () {
    //    filter =
    //        '&search=' + $('#SearchFilter').val() +
    //        '&beginDate=' + $('#BeginDateFilter').val() +
    //        '&endDate=' + $('#EndDateFilter').val() +
    //        '&entryType=' + $("input[name='typeChoiceFilter']:checked").val();
    //    getList(currPage, currTab, filter);
    //});

    $('#filterButton').click(function () {
        filter =
            '&search=' + $('#SearchFilter').val() +
            '&beginDate=' + $('#BeginDateFilter').val() +
            '&endDate=' + $('#EndDateFilter').val() +
            '&entryType=' + $("input[name='typeChoiceFilter']:checked").val();
        getList(currPage, currTab, filter);
    });

    $('.tabButton').click(function () {
        if (currTab !== this.getAttribute("id")) {
            //alert(this.getAttribute("id"));
            currTab = this.getAttribute("id");   
            if (currTab === 'List') {
                $('#filterBlock').css('display', 'block');
            }
            else {
                $('#filterBlock').css('display', 'none');
            }                    
            currPage = 0;
            getList(currPage, currTab, filter);
        }
    });

    $('#prevIndex').click(function () {
        if ($('#prevIndex').attr('enabled')==='True') {
            currPage--;
            getList(currPage, currTab, filter);
        }
    });

    $('#nextIndex').click(function () {
        if ($('#nextIndex').attr('enabled') === 'True') {
            currPage++;
            getList(currPage, currTab, filter);
        }
    });
});
