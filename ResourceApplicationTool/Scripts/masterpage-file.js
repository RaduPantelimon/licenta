$(document).ready(InitializeMasterPage);

function InitializeMasterPage() {
    $("#cbp-spmenu-button").click(onShowLeftClick);
}



function onShowLeftClick(sender) {
    $(sender).toggleClass('active');
    $('#cbp-spmenu-s1').toggleClass('cbp-spmenu-open');
    disableOther('showLeft');
};

function closeLeftMenu() {
    $(".cbp-spmenu").removeClass('cbp-spmenu-open');
}
function disableOther(button) {
    if (button !== 'showLeft') {
        popover
        $('#showLeft').toggleClass('disabled');
    }
}