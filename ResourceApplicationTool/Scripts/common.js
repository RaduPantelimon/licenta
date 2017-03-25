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
            $('#dialog-modal').show();
        });
    });


    //when the save button is clicked
    $("#save-education-changes").click(function () {
        var datastring = $(".educations-form").serialize();
        $.ajax({
            type: "POST",
            url: "/Educations/CreateEducation",
            data: datastring,
            dataType: "json",
            success: function (data) {
                //var obj = jQuery.parseJSON(data); if the dataType is not specified as json uncomment this
                // do what ever you want with the server response
                console.log(data);
            },
            error: function (err) {
                console.log('error handing here: ' + err);
            }
        });
    });
   

}