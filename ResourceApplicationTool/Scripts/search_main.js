$(window).ready(InitializeSearchComponent);


function InitializeSearchComponent() {

    //deploying the expand collapse functionality
    $(".main_expand_collapse").click(function () {

        MainExpandCollapse(this, 'auto');

    });

    //Initializing arrow filters
    $(".arrow-search").click(function () {
        SetFilterOrder($(this), $("[name='secondaryFilter']"))
    });

    //initialize the Carousels
    InitializeCarousel('.carousel-depts');
}


function SetFilterOrder(arrowInput,filterInput)
{
    //arranging the arrow filter display
    $(".arrow-search").removeClass("selected-arrow");
    $(arrowInput).addClass("selected-arrow");

    //setting the filter
    var filter = $(arrowInput).attr("data-filter");
    var form = $(".query-container form");
    filterInput.val(filter);

    form.submit();
}