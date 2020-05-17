// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

function getList(page, filter) {
    let url = "/Contact/List?size=10&page=" + page + filter;    
    $('#content').html('Loading...');
      
    if (url !== null) {
        $.ajax({
            type: "GET",
            url: url,
            contentType: 'text/plain; charset=utf-8',
            success: function (data, textStatus, request) {
                $('#content').html(data);
                if (request.getResponseHeader('X-Header-IsFirstPage') !== undefined) {                   
                    $('#prevIndex').attr('enabled', request.getResponseHeader('X-Header-IsFirstPage')==='False');
                }
                if (request.getResponseHeader('X-Header-IsLastPage') !== undefined) {                  
                    $('#nextIndex').attr('enabled', request.getResponseHeader('X-Header-IsLastPage') ==='False');
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
    let filter = '';

    getList(currPage, filter);
    
    $('#filterButton').click(function () {
        filter =
            '&search=' + $('#SearchFilter').val() +
            '&beginDate=' + $('#BeginDateFilter').val() +
            '&endDate=' + $('#EndDateFilter').val() +
            '&entryType=' + $("input[name='typeChoiceFilter']:checked").val();
        getList(currPage, filter);
    });

    $('#prevIndex').click(function () {
        if ($('#prevIndex').attr('enabled')==='true') {
            currPage--;
            getList(currPage, filter);
        }
    });

    $('#nextIndex').click(function () {
        if ($('#nextIndex').attr('enabled') === 'true') {
            currPage++;
            getList(currPage, filter);
        }
    });   
});
