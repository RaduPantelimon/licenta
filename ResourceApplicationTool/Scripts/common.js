function displayLoadedImage(fileInput,img, isBanner){
    $(fileInput).change(function () {
        if (this.files && this.files[0]) {
            var reader = new FileReader();
            reader.onload = function(e){ 
                imageIsLoaded(e,img,isBanner)};
            reader.readAsDataURL(this.files[0]);
        }
    });
}
function imageIsLoaded(e,img,isBanner) {
    //$(img).attr('src', e.target.result);
    $(img).css("background-image", "url(" + e.target.result + ")");
    if (isBanner == true)
    {
        $(img).css("background-position","top center;")
    }
};

function DisableSliders(sliderSelector){
    $(sliderSelector).slider('disable');
}

function InitializeSliders(sliderInput,disabled) {
    $(sliderInput).slider();
    $(sliderInput).on("change", changeSlider);

    //triggering the initialization for these items
    $(sliderInput).change();

    if(disabled == true)
    {
        $(sliderInput).slider('disable');
    }
    
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

    //
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

//used in order to view and initialize pop-up page - download project Status -XML
function LoadModalXLSXDownloadView() {
    $('#generate-report').click(function () {
        $('#dialog-modal').modal('show');
        $('#dialog-modal').show();
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


function InitializeCarousel(carouselContainer) {
    $(carouselContainer).slick({
        dots: false,
        infinite: true,
        speed: 300,
        slidesToShow: 3,
        slidesToScroll: 3,
        responsive: [
          { breakpoint: 1225, settings: { slidesToShow: 3, slidesToScroll: 3, infinite: true, dots: true } },
          { breakpoint: 1000, settings: { slidesToShow: 2, slidesToScroll: 2 } },
          { breakpoint: 650, settings: { slidesToShow: 1, slidesToScroll: 1 } }
        ],

    });
}