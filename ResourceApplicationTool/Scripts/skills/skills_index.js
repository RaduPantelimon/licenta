$(window).ready(initializePage);


function initializePage() {

    //deploying the expand collapse functionality for the secondary sections
    $(".secondary_expand_collapse").click(function () {

        SecondaryExpandCollapse(this, 'auto');

    });

    //once the event is added, we collapse all sections
    $(".secondary_expand_collapse").trigger("click");

    //page is loaded, we can show it
    $(".skills-container").show();
}