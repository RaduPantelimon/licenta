$(window).ready(initializePage);


function initializePage() {

    //initializing the department filter
    $("select.filter-select").click(InitializeEventsFilter);

    //preparing the calendar
    getExistingAnswers();
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




function getExistingAnswers() {
    $.ajax({
        url: "/api/restapi/GetUserEvents",
        contentType: "application/json; charset=utf-8",
        type: "GET",
        dataType: "json",
        success: SetEvents,
        error: OnFail
    });
}

function SetEvents(data) {
    console.log(data);
    //initializing the calendar
    setUpCalendar(data);
}


function OnFail(jqXHR, exception) {
    console.log("Error while performing Ajax Call" + jqXHR + " " + exception);
}

function setUpCalendar(data) {
    $("#calendar").show();
    $("#loaderImg").hide();
    $('#calendar').fullCalendar({
        header: {
            left: 'prev,next today',
            center: 'title',
            right: 'month,agendaDay,listWeek'
        },
        firstDay: 1,
        navLinks: true, // can click day/week names to navigate views
        editable: true,
        eventLimit: true, // allow "more" link when too many events
        events: data
    });

    //after the loading is finished we will show the calendar

}