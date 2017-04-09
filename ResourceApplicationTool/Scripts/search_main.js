$(window).ready(InitializeSearchComponent);




function InitializeSearchComponent() {
    //initializing the search launcher
    $(".main-search-input").typeahead({
        onSelect: function (item) {
            console.log(item);
            if (item.value) {
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

                if (data.success === false) {
                    return false;
                }
                else {
                    return data;
                }
            }
        }
    });
}

