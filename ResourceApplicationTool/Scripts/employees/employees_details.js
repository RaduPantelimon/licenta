$(window).ready(initializePage);


function initializePage() {

    //deploying the expand collapse functionality
    $(".main_expand_collapse").click(function () {

        MainExpandCollapse(this, 'auto');

    });
    //deploying the expand collapse functionality for the secondary sections
    $(".secondary_expand_collapse").click(function () {

        SecondaryExpandCollapse(this, 'auto');

    });

    // initialization of the sliders
    //this is a display page, so the sliders need to be disabled
    InitializeSliders('.skill-slider-input', true);


    //initialize educations
    InitializeEducationsDisplay($("#educations-table"), EmployeeID);
}


