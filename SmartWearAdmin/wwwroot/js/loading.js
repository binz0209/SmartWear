function DisplayProgressMessage(ctl, msg) {
    //if use the submit button, you can use event.preventDefault to prevent the default submit form behavior.
    //event.preventDefault();
    // Turn off the "Save" button and change text
    $(ctl).prop("disabled", true).val(msg);
    // Gray out background on page
    $("body").addClass("submit-progress-bg");
    // Wrap in setTimeout so the UI can update the spinners
    $(".processing-preview").removeClass("hidden");
    //submit the form.
    setTimeout(function () {
        $("form").submit();
        $(".processing-preview").addClass("hidden");
        $("body").removeClass("submit-progress-bg");
        $(ctl).prop("disabled", false).val("Cập nhật");
    }, 1000);
    return true;
}