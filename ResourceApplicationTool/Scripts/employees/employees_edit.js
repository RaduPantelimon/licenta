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
    IntializeDialogModal();

    //initialize educations
    InitializeEducations($("#educations-table"), EmployeeID);
}

function OpenDialog() {
    $('#dialog-modal').dialog('open');
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


function IntializeDialogModal() {
    $('#dialog-modal').on('show.bs.modal', function (e) {
        setTimeout(function () {
            InitializeDatePickers('#startDate-input');
            InitializeDatePickers('#endDate-input');
        }, 300);
    });

}