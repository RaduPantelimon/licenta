$(window).ready(initializePage);


function initializePage() {

    //deploying the expand collapse functionality
    $(".main_expand_collapse").click(function () {

        MainExpandCollapse(this, 'auto');

    });

    //initialize the Carousel
    InitializeCarousel('.carousel-depts');

    //department pop-up
    LoadModalXLSXDepartmentDownloadView()
}

//used in order to view and initialize pop-up page - download project Status -XML
function LoadModalXLSXDepartmentDownloadView() {
    $('#generate-report').click(function () {
        $('#dialog-modal').modal('show');
        $('#dialog-modal').show();

        $("#excel-month-input").val(new Date().getMonth() + 1);
        $("#excel-year-input").val(new Date().getFullYear());

        $("#get-report").click(function () {
            //projectID
            var monthValue = $("#excel-month-input").val();
            var yearValue = $("#excel-year-input").val();

            location.href = "/Employees/GenerateCV?departmentID=" + departmentID + "&month=" + monthValue + "&year=" + yearValue;

            //hide the modal window
            $('#dialog-modal').modal('hide');
        });

    });

}