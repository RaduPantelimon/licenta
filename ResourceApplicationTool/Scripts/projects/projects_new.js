$(window).ready(initializePage);


function initializePage() {

    //deploying the expand collapse functionality
    $(".main_expand_collapse").click(function () {

        MainExpandCollapse(this, 'auto');

    });

    //initialize date time pickers
    InitializeDatePickers('#startDate-input');
    InitializeDatePickers('#endDate-input');
}