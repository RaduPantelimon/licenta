$(window).ready(InitializeSearchComponent);

function InitializeSearchComponent() {
    //initializing the search launcher
    $(".search-anchor").click(showSearchBox);

    //redirecting to the main search page
    $("#process-search").click(GoToMainSearch);
}

function showSearchBox()
{
    //we stop the default navigation
    event.preventDefault();

    //hide this element; show the search box
    $(this).hide();
    $(".search-input-main").animate({ width: 'toggle' }, 350,
       function() {
           $("#process-search").animate({ width: 'toggle' }, 350);
    });


    $(".search-input").typeahead({
        onSelect: function (item) {
            console.log(item);
            if (item.value)
            {
                window.location.href = item.value;
            }
            
        },
        ajax: {
            url: '/Search/GetPartial',
            timeout: 500,
            displayField: "name",
            valueField: "url",
            triggerLength: 1,
            method: "post",
            dataType: "JSON",
            preDispatch: function (query) {
                //showLoadingMask(true);

                //these are the values sent to the server
                return {
                    'query': query
                }
            },
            preProcess: function (data) {

                if (!data || data.success === false) {
                    return false;
                } else {
                    return data;
                }
            }
        }
    });

}

function GoToMainSearch()
{
    //we stop the default navigation
    event.preventDefault();

    var query = $(".search-input").val();
    location.href = "/Search?query=" + query;
    return;
}