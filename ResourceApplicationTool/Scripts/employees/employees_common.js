function InitializeEducations(educationsTable, empId) {
    
    //checking if the id is valid 
    if(empId)
    {
        getEducationsForCurrentEmployee(educationsTable,empId);
    }
}



function getEducationsForCurrentEmployee(educationsTable, empId)
{
    //adding loading gif
    var tableBody = $(educationsTable).find("tbody");
    tableBody.html("<tr><td colspan='12'><img src='../../Content/Pictures/ajax-loader.gif' height='20' width='20' /></td></tr>");

    $.ajax({
        type: "GET",
        url: "/api/restapi/GetEducations/" + empId,
        dataType: "json",
        success: function (data) {

            tableBody.html("");
            if(data && data.length && data.length > 0)
            {
                for(var i=0;i< data.length;i++)
                {
                    try {
                        //set row content
                        var row = "<tr educationId='" + (data[i]["EducationID"] ? data[i]["EducationID"] : "") + "' >";
                        var startDate = new Date(data[i]["StartDate"]);
                        var endDate = new Date(data[i]["EndDate"]);
                        row += "<td>" + (data[i]["Title"] ? data[i]["Title"] : "") + "</td>";
                        row += "<td>" + (startDate ? (startDate.getMonth() + 1) + "/" + startDate.getUTCDate() + "/" + startDate.getFullYear() : "") + "</td>";
                        row += "<td>" + (endDate ? (endDate.getMonth() + 1) + "/" + endDate.getUTCDate() + "/" + endDate.getFullYear() : "") + "</td>";
                        row += "<td>" + (data[i]["Degree"] ? data[i]["Degree"] : "") + "</td>";
                        row += "<td>" +
                        "<button class='options-button edit-button'" +
                        " data-url='/Educations/Edit/" + (data[i]["EducationID"] ? data[i]["EducationID"] : "") + "?isModal=1'" +
                        " type='button' " +
                        " educationId='" + (data[i]["EducationID"] ? data[i]["EducationID"] : "") + "' ></button>" +

                        "<button class='options-button delete-button' " +
                        " data-url='/Educations/Delete/" + (data[i]["EducationID"] ? data[i]["EducationID"] : "") + "?isModal=1'" +
                        " type='button' " +
                        " educationId='" + (data[i]["EducationID"] ? data[i]["EducationID"] : "") + "' ></button>" +
                        "</td>";
                        row += "</tr>";

                        tableBody.append(row);
                    }
                    catch(ex)
                    {
                        console.log("Could not set row " + i + ", exception: " + ex);
                    }
                  
                }

                //initialize modal windows
                IntializeEditEvents();
                IntializeDeleteEvents();
            }
            else{
                tableBody.append("<tr><td colspan='12'>No information added</td></tr>");
            }
        },
        error: function (err) {
            console.log('The educations for the current employee could not be retrieved: ' + err);
        }
    });
}

function IntializeEditEvents()
{
    $('.options-button.edit-button').click(function () {
        var url = $(this).data('url');

        $.get(url, function (data) {
            $('#modal-body-content').html(data);

            $('#dialog-modal').modal('show');
            $('#dialog-modal').find(".modal-title").html("Edit Education");
            $('#dialog-modal').find("#save-education-changes").text("Save Changes");
            $('#dialog-modal').show();
        });

        //initializeDatePickers
        InitializeDatePickers('#startDate-input');
        InitializeDatePickers('#endDate-input');

        //when the save button is clicked
        $("#save-education-changes").off();
        $("#save-education-changes").on("click",function (event) {
            event.stopImmediatePropagation();

            var datastring = $(".educations-form").serialize();
            $.ajax({
                type: "POST",
                url: "/Educations/EditEducation",
                data: datastring,
                dataType: "json",
                success: function (data) {
                    console.log(data);
                    $('#dialog-modal').modal('hide');

                    //after the update, we reinitialize the educations table
                    InitializeEducations($("#educations-table"), EmployeeID);
                },
                error: function (err) {
                    console.log('error handing here: ' + err);
                    $('#dialog-modal').modal('hide');
                }
            });
        });

    });  
}

function IntializeDeleteEvents() {
    $('.options-button.delete-button').click(function () {
        var url = $(this).data('url');
        var edId = $(this).attr('educationId');
        $.get(url, function (data) {
            $('#modal-body-content').html(data);

            $('#dialog-modal').modal('show');
            $('#dialog-modal').find(".modal-title").html("Delete Education");
            $('#dialog-modal').find("#save-education-changes").text("Delete Education");
            $('#dialog-modal').show();
        });

        //when the save button is clicked
        $("#save-education-changes").off();
        $("#save-education-changes").on("click", function (event) {
            event.stopImmediatePropagation();

            var datastring = $(".educations-form").serialize();
            $.ajax({
                type: "POST",
                url: "/Educations/DeleteEducation/" + edId,
                data: datastring,
                dataType: "json",
                success: function (data) {
                    console.log(data);
                    $('#dialog-modal').modal('hide');

                    //after the update, we reinitialize the educations table
                    InitializeEducations($("#educations-table"), EmployeeID);
                },
                error: function (err) {
                    console.log('error handing here: ' + err);
                    $('#dialog-modal').modal('hide');
                }
            });
        });

    });
}


function InitializeDepartmentFilter(filter)
{
    $(filter).on("change", DepartmentFilterSelection);
}

function DepartmentFilterSelection()
{
    var filterValue = $(this).val();
    //showing only employees that are part of the current department
    $(".carousel-item").hide();
    if (filterValue && filterValue != "0")
    {
        
        $(".carousel-item").each(function () {
            var dept = $(this).find(".dept-information-container");
            if (dept && dept.attr("departmentId") == filterValue) {
                $(this).fadeIn(400);
            }

        });
    }
    else {
        $(".carousel-item").fadeIn(400);
    }
}