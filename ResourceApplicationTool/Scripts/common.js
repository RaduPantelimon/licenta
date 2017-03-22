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