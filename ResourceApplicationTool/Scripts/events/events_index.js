$(window).ready(initializePage);


function initializePage() {

    //initializing the department filter
    $("select.filter-select").click(InitializeEventsFilter);
}

function InitializeEventsFilter(item) {
    var filterValue = $(this).val();
    //showing only projects that are part of the current department
    $(".carousel-item").hide();
    if (filterValue && filterValue != "0") {

        $(".carousel-item").each(function () {
            var dept = $(this).find(".dept-information-container");
            if (filterValue == 1 && $(this).hasClass("creator-item")) {
                $(this).fadeIn(400);
            }
            else if (filterValue == 2 && $(this).hasClass("attendant-item")) {
                $(this).fadeIn(400);
            }

        });
    }
    else {
        $(".carousel-item").fadeIn(400);
    }
}