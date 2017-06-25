console.log("test");

$(document).ready(function () {

    $('.carousel-depts').slick({
        dots: false,
        infinite: false,
        arrows: false,
        speed: 300,
        slidesToShow: 3,
        slidesToScroll: 3,
        responsive: [
          { breakpoint: 1225, settings: { slidesToShow: 3, slidesToScroll: 3, infinite: false, dots: false } },
          { breakpoint: 1000, settings: { slidesToShow: 2, slidesToScroll: 2, infinite: false, dots: false } },
          { breakpoint: 650, settings: { slidesToShow: 1, slidesToScroll: 1, infinite: false, dots: false } }
        ],
      
    });

    //initializing the employees carousel
    $('.carousel-emps').slick({
        dots: false,
        infinite: false,
        arrows: false,
        speed: 300,
        slidesToShow: 2,
        slidesToScroll: 2,
        responsive: [
          { breakpoint: 1225, settings: { slidesToShow: 2, slidesToScroll: 2, infinite: false, dots: false } },
          { breakpoint: 1000, settings: { slidesToShow: 2, slidesToScroll: 2, infinite: false, dots: false } },
          { breakpoint: 650, settings: { slidesToShow: 1, slidesToScroll: 1, infinite: false, dots: false } }
        ],
      
    });


    //set employee of the month
    InitializeEmployeeOfMonth();

    //Initialize Project piecharts
    InitializeProjectPiecharts();

    //Initialize Expand Collapse Sections
    $(".main_expand_collapse").click(function () {

        MainExpandCollapse(this, 'auto');

    });
});

//
function InitializeEmployeeOfMonth() {
    //mask
    $(".employee-of-the-month .mask").prepend("<h4 class='employee-o-month-header'>Employee of the Month</h4>");
}

function InitializeProjectPiecharts() {

    //unserializing the projects
    var projects = JSON.parse(projectStatistics);

    if (projects && projects.length)
    {
        for(var i=0;i <projects.length;i++)
        {
            var pjID = projects[i].ProjectID;
            var manHours = projects[i].ManHoursEffort;
            var participants = [];
            for (var j = 0; j < projects[i].participants.length; j++)
            {
                var participant = {};
                participant.label = projects[i].participants[j].EmployeeName;
                participant.legendText = projects[i].participants[j].EmployeeName;
                participant.y = projects[i].participants[j].EffortInHours / manHours * 100;

                participants.push(participant);
            }

            var chart = new $("#piechart-"+pjID).CanvasJSChart({
                title: {
                    text: "Total effort: " + projects[i].ManHoursEffort + " hours",
                    fontSize: 20
                },
                height: 300,
                width:370,
                axisY: {
                    title: "Products in %",
                    valueFormatString: "#,##0.##"
                },
                axisX: {
                    valueFormatString: "#,##0.##",
                },
                legend: {
                    verticalAlign: "center",
                    horizontalAlign: "right"
                },
                data: [
                {
                    type: "pie",
                    showInLegend: true,
                    toolTipContent: "{label} <br/> #percent %",
                    
                    indexLabel: "#percent%",
                    indexLabelPlacement: "inside",
                    indexLabelFontSize: 14,
                    indexLabelLineThickness: 2,
                    indexLabelFontWeight: "bold",
                    indexLabelFontColor: "#fff",
                    percentFormatString: "#0.##",
                    
                    dataPoints: participants
                }
                ]
            });
        }
    }
}