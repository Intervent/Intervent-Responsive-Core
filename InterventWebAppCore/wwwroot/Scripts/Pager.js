function AddPager() {
        var LastIndex = parseInt(startIndex + pageSize); // this is the last displaying record
        if (LastIndex > totalRecords) { // in case that last page includes records less than the size of the page
            LastIndex = totalRecords;
        }

        //$("#pageInfo").html("Page ( <b>" + parseInt(currentPage + 1) + "</b> of <b>" + totalPages + "</b> )      Displaying <b>" + parseInt(startIndex) + "-" + LastIndex + "</b> of <b>" + totalRecords + "</b> Records."); // displaying current records interval  and currnet page infromation

        if (currentPage > 0) {
            $("#first").unbind("click"); // remove previous click events
            $("#first").removeClass("unavailable"); // remove the inactive page style
            $("#first").on('click', function () { // set goto page to first page
                GotoPage(0);
            });
            $("#previous").unbind("click");
            $("#previous").removeClass("unavailable");
            $("#previous").on('click', function () {
                GotoPage(currentPage - 1); // set the previous page next value  to current page – 1
            });
        }
        else {
            $("#first").addClass("unavailable");
            $("#first").unbind("click");
            $("#previous").addClass("unavailable");
            $("#previous").unbind("click");
        }

        if (currentPage < totalPages - 1) { // if you are not displaying the last index
            $("#next").unbind("click");
            $("#next").removeClass("unavailable");
            $("#next").on('click', function () {
                GotoPage(currentPage + 1);
            });
            $("#last").unbind("click");
            $("#last").removeClass("unavailable");
            $("#last").on('click', function () {
                GotoPage(totalPages - 1);
            });
        } else {
            $("#next").addClass("unavailable");
            $("#next").unbind("click");
            $("#last").addClass("unavailable");
            $("#last").unbind("click");
        }

        // displaying the numeric pages by default there are 10 numeric pages
        var firstPage = 0;
        var lastPage = 4;
        if (currentPage >= 3) {
            lastPage = currentPage + 2;
            firstPage = currentPage - 2
        }
        if (lastPage > totalPages) {
            lastPage = totalPages;
            firstPage = lastPage - 4;
        }
        if (firstPage < 0) {
            firstPage = 0;
        }
        var pagesString = "";
        for (var i = firstPage; i < lastPage; i++) {
            if (i == currentPage)
            {
                pagesString += "<li class='current' ><a>" + parseInt(i + 1) + "</a></li>"
            }
            else
            {
                pagesString += "<li><a onclick='GotoPage(" + i + ")'>" + parseInt(i + 1) + "</a></li>" // add goto page event
            }
        }
        $("#numeric").html(pagesString);
        window.scrollTo(500, 0);
}

function AddPagerNew(firstPageDOMId, previousPageDOMId, nextPageDOMId, lastPageDOMId, numericPageDOMId, startIndex, pageSize, totalRecords, currentPage, totalPages, gotoPageCallback, gotoPageCallbackName) {
    var LastIndex = parseInt(startIndex + pageSize); // this is the last displaying record
    if (LastIndex > totalRecords) { // in case that last page includes records less than the size of the page
        LastIndex = totalRecords;
    }

    //$("#pageInfo").html("Page ( <b>" + parseInt(currentPage + 1) + "</b> of <b>" + totalPages + "</b> )      Displaying <b>" + parseInt(startIndex) + "-" + LastIndex + "</b> of <b>" + totalRecords + "</b> Records."); // displaying current records interval  and currnet page infromation
    var $firstPage = $("#" + firstPageDOMId);
    var $previousPage = $("#" + previousPageDOMId);
    var $nextPage = $("#" + nextPageDOMId);
    var $lastPage = $("#" + lastPageDOMId);
    if (currentPage > 0) {
        $firstPage.unbind("click"); // remove previous click events
        $firstPage.removeClass("unavailable"); // remove the inactive page style
        $firstPage.on('click', function () { // set goto page to first page
            gotoPageCallback(0);
        });
        $previousPage.unbind("click");
        $previousPage.removeClass("unavailable");
        $previousPage.on('click', function () {
            gotoPageCallback(currentPage - 1); // set the previous page next value  to current page – 1
        });
    }
    else {
        $firstPage.addClass("unavailable");
        $firstPage.unbind("click");
        $previousPage.addClass("unavailable");
        $previousPage.unbind("click");
    }

    if (currentPage < totalPages - 1) { // if you are not displaying the last index
        $nextPage.unbind("click");
        $nextPage.removeClass("unavailable");
        $nextPage.on('click', function () {
            gotoPageCallback(currentPage + 1);
        });
        $lastPage.unbind("click");
        $lastPage.removeClass("unavailable");
        $lastPage.on('click', function () {
            gotoPageCallback(totalPages - 1);
        });
    } else {
        $nextPage.addClass("unavailable");
        $nextPage.unbind("click");
        $lastPage.addClass("unavailable");
        $lastPage.unbind("click");
    }

    // displaying the numeric pages by default there are 10 numeric pages
    var firstPage = 0;
    var lastPage = 4;
    if (currentPage >= 5) {
        lastPage = currentPage + 2;
        firstPage = currentPage - 2
    }
    if (lastPage > totalPages) {
        lastPage = totalPages;
        firstPage = lastPage - 4;
    }
    if (firstPage < 0) {
        firstPage = 0;
    }
    var pagesString = "";
    for (var i = firstPage; i < lastPage; i++) {
        if (i == currentPage) {
            pagesString += "<li class='current' ><a href=''>" + parseInt(i + 1) + "</a></li>"
        }
        else {
           // gotoPageCallback.name - does not has the js container. passing the function name as a separate param
            pagesString += "<li><a onclick='" + gotoPageCallbackName + "(" + i + ")'>" + parseInt(i + 1) + "</a></li>" // add goto page event
        }
    }
    $("#" + numericPageDOMId).html(pagesString);
    window.scrollTo(500, 0);
}