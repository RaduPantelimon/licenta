$(window).ready(initializePage);
function initializePage() {

    //deploying the expand collapse functionality
    $(".main_expand_collapse").click(function () {
        MainExpandCollapse(this, 'auto');
    });
}

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