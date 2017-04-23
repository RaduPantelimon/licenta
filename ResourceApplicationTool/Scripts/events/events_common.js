function initializeEmployeeSearchBox() {
    //we stop the default navigation
    event.preventDefault();




    $("#employees-search-box").typeahead({
        onSelect: function (item) {
            console.log(item);
            if (item.value) {
                //window.location.href = item.value;
            }

        },
        ajax: {
            url: '/Search/GetPartialEmployees',
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