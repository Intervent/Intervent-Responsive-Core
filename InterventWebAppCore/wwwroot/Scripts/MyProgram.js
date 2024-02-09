function resource_slider() {
    $('.resource-slider').slick({
        dots: false,
        infinite: false,
        speed: 300,
        slidesToShow: 1,
        slidesToScroll: 1,
        responsive: [
            {
                breakpoint: 991,
                settings: {
                    infinite: true,
                    dots: true,
                    arrows: false
                }
            }
        ]
    });

}
function slider_num(num, element) {
    var $status = $(num);
    var $slickElement = $(element);
    $slickElement.on('init reInit afterChange', function (event, slick, currentSlide, nextSlide) {
        var i = (currentSlide ? currentSlide : 0) + 1;
        $status.text(i + '/' + slick.slideCount);
    });

    $slickElement.slick({
        dots: false,
        infinite: false,
        speed: 300,
        slidesToShow: 1,
        slidesToScroll: 1,
    });
}
function listenAudio() {
    $('#modal-audio').foundation();
    $('#modal-audio').foundation('open');
}