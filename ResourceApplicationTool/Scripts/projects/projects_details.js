$(window).ready(initializePage);


function initializePage() {

    //deploying the expand collapse functionality
    $(".main_expand_collapse").click(function () {

        MainExpandCollapse(this, 'auto');

    });

    //download_report
    //used in order to view and initialize pop-up page - download project Status -XML
   LoadModalXLSXDownloadView();
}