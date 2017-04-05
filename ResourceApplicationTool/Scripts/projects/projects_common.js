function InitializeDepartmentFilter(filter) {
    $(filter).on("change", DepartmentFilterSelection);
}

function DepartmentFilterSelection() {
    var filterValue = $(this).val();
    //showing only projects that are part of the current department
    $(".carousel-item").hide();
    if (filterValue && filterValue != "0") {

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