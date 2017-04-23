var currentAddedAttendant
var attendants = [];

function initializeEmployeeSearchBox() {
    //we stop the default navigation
    event.preventDefault();

    //AttendantsIDs

    $("#add-employee").click(addEmployee);


    $("#employees-search-box").keyup(function () {
        //on entering letters in 
        $("#employees-search-box").css({ "background": "#fdfda7" });

        var currentValue = $("#employees-search-box").val();

        var itemExists = false;
        for (var j = 0; j < attendants.length; j++)
        {
            if (attendants[j].name == currentValue) {
                itemExists = true;
                currentAddedAttendant = attendants[j];
            }
        }
        if (itemExists)
        {
            $("#employees-search-box").css({ "background": "#beec78" });
            currentAddedAttendant = null;
        }
        else {
            $("#employees-search-box").css({ "background": "#fdfda7" });
        }
    });

    $("#employees-search-box").typeahead({
        onSelect: function (item) {
            console.log(item);
            if (item.value) {
                //window.location.href = item.value;
                $("#employees-search-box").css({ "background": "#beec78" });
                currentAddedAttendant = item;
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

                    addSecondaryEmployees(data);
                    return data;
                    
                }
            }
        }
    });

}

function addEmployee() {
    if(currentAddedAttendant)
    {
        var values = currentAddedAttendant.value.split("/");
        var EmpId = values[values.length - 1];

        var currentAttendants = $("#AttendantsIDs").val();
        var currentAddedAttendantArray = currentAttendants.split(";");

        var itemAdded = false;
        for(var i=0;i<currentAddedAttendantArray.length;i++)
        {
            if(currentAddedAttendantArray[i]==EmpId)
            {
                itemAdded = true;
            }
        }

        //only adding an item once
        if (itemAdded == false)
        {
            $("#attendants-container").append(
           "<div class='attendant-cell' Emp-Url='" + EmpId + "'><span class='attendant-name-holder'>" + currentAddedAttendant.text + "</span>" +
           "<button type='button' class='remove-attendant' >x</button>" + "</div>");

            currentAttendants += EmpId + ";";
            $("#AttendantsIDs").val(currentAttendants);

            $(".remove-attendant").click(removeAttendant);

            //empty search box
            $("#employees-search-box").val("");
            $("#employees-search-box").css({ "background": "#ffffff" });
        }
       
    }
}

function addSecondaryEmployees(data) {

    if(data && data.length)
    {
        for(var i=0;i<data.length;i++)
        {
            var itemExists = false;
            for (var j = 0; j < attendants.length; j++)
            {
                if (attendants[j].name == data[i].name) itemExists = true;
            }
            if(!itemExists)
            {
                attendants.push(data[i]);
            }

        }
    }
}


function removeAttendant() {
    
    var removeButton = $(this);
    var attendantCell = removeButton.closest(".attendant-cell");
    var empID = attendantCell.attr("Emp-Url");

    //remove value from input
    var currentAttendants = $("#AttendantsIDs").val();
    currentAttendants = currentAttendants.replace(empID + ";", "");
    $("#AttendantsIDs").val(currentAttendants);


    attendantCell.remove();
}