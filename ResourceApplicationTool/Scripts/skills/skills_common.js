//expand collapse behaviour for the secondary sections
function SecondaryExpandCollapse(link, action) {
    var subsection = $(link).closest(".secondary_expandable");
    var expand = true;
    var collapse = false;
    var img = subsection.find("a.secondary_expand_collapse img:visible");
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
        subsection.children(".skill-category-container").show();
        subsection.find("a.secondary_expand_collapse img.expandimg").hide();
        subsection.find("a.secondary_expand_collapse img.collapseimg").show();

    }
    else {
        // collapsing
        subsection.children(".skill-category-container").hide();
        subsection.find("a.secondary_expand_collapse img.expandimg").show();
        subsection.find("a.secondary_expand_collapse img.collapseimg").hide();

    }

}