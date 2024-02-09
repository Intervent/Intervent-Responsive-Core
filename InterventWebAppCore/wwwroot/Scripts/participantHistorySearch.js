var startIndex = 1;
var pageSize = 20;
var totalRecords = 0;
var currentPage = 0;
var totalPages = 0;

function GotoPage(page) {
    currentPage = page;
    startIndex = page * pageSize + 1;
    participantHistorySearch.searchHistory();
}
var participantHistorySearch = {
    $startDate: $('#startDate'),
    $endDate: $('#endDate'),
    $userHistoryCategoryId: $('#UserHistoryCategoryId'),
    $userId: $('#ParticipantId'),
    $searchHistoryContainer : $("#search-results-container"),

    resetPager : function(){
            startIndex = 0;
            pageSize = 20;
            totalRecords = 0;
            currentPage = 0;
            totalPages = 0;
    },
    init: function () {
        participantHistorySearch.$startDate.fdatepicker({
            endDate: new Date()
        }).on('change', function (selected) {
            
            var endDate = new Date(participantHistorySearch.$endDate.val());
            var minDate = new Date(participantHistorySearch.$startDate.val());
            if (endDate < minDate) {
                participantHistorySearch.$startDate.fdatepicker('setDate', minDate);
            }
            participantHistorySearch.$endDate.fdatepicker('setStartDate', minDate);
        });
        participantHistorySearch.$endDate.fdatepicker({
            endDate: new Date()
        });
        $("#filter").on('click', function (ev) {
            participantHistorySearch.resetPager();
            participantHistorySearch.searchHistory();
            ev.preventDefault();
        });
        participantHistorySearch.$searchHistoryContainer.hide();
        $(document).on('click', '.item', function () {
            if ($(this).find('.control').hasClass('active')) {
                $(this).parent().find('.detailed-info').addClass('hide');
                $(this).find('.control').removeClass('active');
            } else {
                var $detailedInfo = $(this).parent().find('.detailed-info');
                $.ajax({
                    type: "GET",
                   // dataType: 'json',
                    url: detailUrl + '/' + $detailedInfo.prevObject.prevObject[0].dataset.id,
                    cache: false,
                    data: null,
                    success: function (data) {
                        $detailedInfo.html(data);
                        $detailedInfo.removeClass('hide');

                    }
                }).fail(function (jqXHR, textStatus, errorThrown) {
                        RedirectToErrorPage(jqXHR.status);
                    });

                $(this).find('.control').addClass('active');
            }
        });
    },
    searchHistory: function () {
        var startDate = participantHistorySearch.$startDate.val();
        var endDate = participantHistorySearch.$endDate.val();
        var userHistoryCategoryid = participantHistorySearch.$userHistoryCategoryId.val();
        var participantId = participantHistorySearch.$userId.val();
        $.ajax({
            type: "POST",
            dataType: 'json',
            async: true,
            cache: false,
            data: {
                participantId: participantId, userHistoryCategoryid: userHistoryCategoryid, startDate: startDate, endDate: endDate, page: currentPage, pageSize: pageSize, totalRecords: totalRecords
            },
            success: function (data) {
               // participantHistorySearch.totalRecords = 100;
               // participantHistorySearch.totalPages = parseInt((participantHistorySearch.totalRecords + participantHistorySearch.pageSize - 1) / participantHistorySearch.pageSize);
               // alert(data.TotalRecords);
                totalRecords = data.TotalRecords;
                totalPages = parseInt((totalRecords + pageSize - 1) / pageSize);
                if (data.Records != null && data.TotalRecords > 0) {
                    participantHistorySearch.displayRecords(data.Records);
                    participantHistorySearch.$searchHistoryContainer.show();
                    AddPager();

                }
                else {
                    participantHistorySearch.$searchHistoryContainer.hide();
                }

                //AddPagerNew(firstPageDOMId, previousPageDOMId, nextPageDOMId, lastPageDOMId, numericPageDOMId, startIndex, pageSize, totalRecords, currentPage, totalPages, gotoPageCallback) {

            }
        }).fail(function (jqXHR, textStatus, errorThrown) {
                RedirectToErrorPage(jqXHR.status);
            });
    },

    displayRecords: function (records) {
        $('#search-results').html('');
        var html = "";
        html = html + "<div class='grid-x grid-margin-x title'><div class='name small-11 medium-5 cell'>Log Date</div><div class='medium-2 cell hide-for-small-only text-left'>Category</div>" +
                
                "<div class='small-1 medium-1 cell'></div></div>";
        for (i in records) {
            var record = records[i];
          
           
       
            html = html + "<div class='item-container'>" +
                            "<div class='grid-x grid-margin-x item' data-id='" + record.Id + "'>" +
                            "<div class='logdate small-11 medium-5 cell'>" + record.LogDate + "</div>" +
                            "<div class='category medium-2 cell hide-for-small-only'>" + record.Category + "</div>" +
                            "<div class='control small-1 medium-1 cell'><i class='fa fa-chevron-right is-inactive'></i><i class='fa fa-chevron-down is-active'></i></div>" +
                           "</div>" +
                           "<div class='detailed-info hide'>test</div></div>";
    }
    $('#search-results').append(html);
    }
}