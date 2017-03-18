console.log("test");

$(document).ready(function () {
    /*$('.carousel-depts').slick({
        infinite: true,
        slidesToShow: 3,
        slidesToScroll: 3
    });*/

    $('.carousel-depts').slick({
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
});