function displayLoadedImage(fileInput,img){
    $(fileInput).change(function () {
        if (this.files && this.files[0]) {
            var reader = new FileReader();
            reader.onload = function(e){ 
                imageIsLoaded(e,img)};
            reader.readAsDataURL(this.files[0]);
        }
    });
}
function imageIsLoaded(e,img) {
    //$(img).attr('src', e.target.result);
    $(img).css("background-image", "url(" + e.target.result + ")");
};

function InitializeSliders(sliderInput) {
    $(sliderInput).slider();
    $(sliderInput).on("change", changeSlider);

    //triggering the initialization for these items
    $(sliderInput).change();
}

function changeSlider(sliderValue){

    var sliderContainer = $(sliderValue.target).parent();
    var value = $(sliderValue.target).val();
    var sliderSelection = sliderContainer.find(".slider-track .slider-selection");
    var skillDescriptionsParent = sliderContainer.find(".skill-level-description");
    //set the background color classes
    sliderSelection.removeClass("skill-level5 skill-level4 skill-level3 skill-level2 skill-level1 skill-level0")
    sliderSelection.addClass("skill-level" + value);

    //set the description
    skillDescriptionsParent.children().addClass("skill-level-hidden");
    skillDescriptionsParent.children().removeClass("skill-level-active");
    skillDescriptionsParent.children(".skill-level" + value).addClass("skill-level-active");
    skillDescriptionsParent.children(".skill-level" + value).removeClass("skill-level-hidden");
}

//used in order to initialize and use pop-up pages
function LoadModalView(button, container, body)
{
    $('#add-education').click(function () {
        var url = $('#dialog-modal').data('url');

        $.get(url, function (data) {
            $('#modal-body-content').html(data);

            $('#dialog-modal').modal('show');
            $('#dialog-modal').find(".modal-title").html("Add Education");
            $('#dialog-modal').find("#save-education-changes").text("Save Changes");
            $('#dialog-modal').show();
        });
        //when the save button is clicked
        $("#save-education-changes").off();
        $("#save-education-changes").on("click",function (event) {
            event.stopImmediatePropagation();



            var datastring = $(".educations-form").serialize();
            $.ajax({
                type: "POST",
                url: "/Educations/CreateEducation",
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

//behaviour used to initialize the hiredate pickers
function InitializeDatePickers(selector) {
    var container = $("#main-form-container > form").length > 0 ? $('"#main-form-container > form').parent() : "body";
    var options = {
        format: 'mm/dd/yyyy',
        container: container,
        todayHighlight: true,
        autoclose: true,
    };
    

    var date_input = $(selector);
    date_input.datepicker(/*options*/);
}