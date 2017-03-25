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

    console.log("Starting form initialization");
    InitializeDepartments();
    InitializeDatePickers('#hireDate-input');
    InitializeDatePickers('#terminationDate-input');

    // initialization of the sliders
    InitializeSliders('.skill-slider-input');

    //initialize profile image
    displayLoadedImage($("#uploadProfilePicture"), $("#main-profile-picture-div"));

    //initialize CreateItemPopup
    LoadModalView('#add-education', '#dialog-modal', '#modal-body-content');
}

function OpenDialog() {
    $('#dialog-modal').dialog('open');
}


//expand collapse behaviour for the secondary sections
function SecondaryExpandCollapse(link, action) {
    var subsection = $(link).closest(".secondary_expandable");
    var expand = true;
    var collapse = false;
    var img = subsection.find("a.secondary_expand_collapse img:visible");
    if (action == 'auto') {
        if (img.hasClass("expandimg")) {
            expand = true;
            collapse = false;
        }
        else {
            expand = false;
            collapse = true;
        }
    }
    if (action == 'expand') {
        expand = true;
        collapse = false;
    }
    if (action == 'collapse') {
        expand = false;
        collapse = true;
    }
    if (expand) {
        // expanding
        subsection.children(".skill-category-container").show();
        subsection.find("a.secondary_expand_collapse img.expandimg").hide();
        subsection.find("a.secondary_expand_collapse img.collapseimg").show();

    }
    else {
        // collapsing
        subsection.children(".skill-category-container").hide();
        subsection.find("a.secondary_expand_collapse img.expandimg").show();
        subsection.find("a.secondary_expand_collapse img.collapseimg").hide();

    }

}

//expand collapse behaviour for the main sections
function MainExpandCollapse(link, action) {
    var subsection = $(link).closest(".main_expandable");
    var expand = true;
    var collapse = false;
    var img = subsection.find("a.main_expand_collapse img:visible");
    if (action == 'auto') {
        if (img.hasClass("expandimg")) {
            expand = true;
            collapse = false;
        }
        else {
            expand = false;
            collapse = true;
        }
    }
    if (action == 'expand') {
        expand = true;
        collapse = false;
    }
    if (action == 'collapse') {
        expand = false;
        collapse = true;
    }
    if (expand) {
        // expanding
        subsection.children(":not(.tvd-sub-header)").show();
        subsection.find("a.main_expand_collapse img.expandimg").hide();
        subsection.find("a.main_expand_collapse img.collapseimg").show();

    }
    else {
        // collapsing
        subsection.children(":not(.tvd-sub-header)").hide();
        subsection.find("a.main_expand_collapse img.expandimg").show();
        subsection.find("a.main_expand_collapse img.collapseimg").hide();

    }

}

//behavoiur used by the Departments/Manager cascading dropdowns
function InitializeDepartments(){
    $('#departments-input').change(function () {
        var deptID = $(this).val()

        var pickedDept;
        for (var i = 0; i < departments.length; i++)
        {
            if(departments[i].DepartmentID == deptID)
            {
                pickedDept = departments[i];
            }
        }
        if (pickedDept)
        {
            var managersInput = $("#managers-input")
            var htmlData = $.map(pickedDept.Manangers, function (emp) {
                return '<option value="' + emp.EmployeeID + '">' + emp.FirstName + " " + emp.LastName + '</option>'
            }).join('');
        }
        var managersInput = $("#managers-input");
        managersInput.html(htmlData);
    });
    $("#departments-input").trigger("change");
}

//behaviour used to initialize the hiredate pickers
function InitializeDatePickers(selector)
{
    var container = $("#main-form-container > form").length > 0 ? $('"#main-form-container > form').parent() : "body";
    var options = {
        format: 'mm/dd/yyyy',
        container: container,
        todayHighlight: true,
        autoclose: true,
    };

    var date_input = $(selector);
    date_input.datepicker(options);
}