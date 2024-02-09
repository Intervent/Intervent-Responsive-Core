function AddPager() {
    currentPage = currentPage + 1;
    var adjacents = 1, pagination = "", start, prev, next, lastpage1, counter;
    if (currentPage > 0)
        start = (currentPage - 1) * pageSize;
    else
        start = 0;
    if (currentPage == 0) currentPage = 1;
    prev = currentPage - 1;
    next = currentPage + 1;
    lastpage1 = totalPages;

    if (lastpage1 > 1) {
        if (lastpage1 < 6 + (adjacents * 2)) {
            for (counter = 1; counter <= lastpage1; counter++) {
                if (counter == currentPage)
                    pagination += "<li class='current' ><a>" + counter + "</a></li>";
                else
                    pagination += "<li><a onclick='GotoPage(" + (counter - 1) + ")'>" + parseInt(counter) + "</a></li>";
            }
        }
        else if (lastpage1 > 4 + (adjacents * 2)) {
            if (currentPage < 1 + (adjacents * 2)) {
                for (counter = 1; counter < 4 + (adjacents * 2); counter++) {
                    if (counter == currentPage)
                        pagination += "<li class='current' ><a>" + counter + "</a></li>";
                    else
                        pagination += "<li><a onclick='GotoPage(" + (counter - 1) + ")'>" + parseInt(counter) + "</a></li>";
                }
                pagination += "... ";
                pagination += "<li><a onclick='GotoPage(" + (lastpage1 - 1) + ")'>" + parseInt(lastpage1) + "</a></li>";
            }
            else if (lastpage1 - (adjacents * 2) > currentPage && currentPage > (adjacents * 2)) {
                pagination += "<li><a onclick='GotoPage(0)'>1</a></li>";
                pagination += "... ";
                for (counter = currentPage - adjacents; counter <= currentPage + adjacents; counter++) {
                    if (counter == currentPage)
                        pagination += "<li class='current' ><a>" + counter + "</a></li>";
                    else
                        pagination += "<li><a onclick='GotoPage(" + (counter - 1) + ")'>" + parseInt(counter) + "</a></li>";
                }
                pagination += "... ";
                pagination += "<li><a onclick='GotoPage(" + (lastpage1 - 1) + ")'>" + parseInt(lastpage1) + "</a></li>";
            }
            else {
                pagination += "<li><a onclick='GotoPage(0)'>1</a></li>";
                pagination += "... ";
                for (counter = lastpage1 - (2 + (adjacents * 2)); counter <= lastpage1; counter++) {
                    if (counter == currentPage)
                        pagination += "<li class='current' ><a>" + counter + "</a></li>";
                    else
                        pagination += "<li><a onclick='GotoPage(" + (counter - 1) + ")'>" + parseInt(counter) + "</a></li>";
                }
            }
        }
        $(".pagination-area").removeClass('hide');
        $(".page-link").removeClass('hide');
    }
    else {
        $(".pagination-area").addClass('hide');
        $(".page-link").addClass('hide');
    }
    $("#numeric").html(pagination);
    window.scrollTo(500, 0);
}


$("#gotoPage_submit").on('click', function (e) {
    var page = $('#jumptoPageNo').val();
    if (totalPages > page - 1 && page != '' && page != 0)
        GotoPage(page - 1)
    e.preventDefault();
    $('#jumptoPageNo').val('');
});
