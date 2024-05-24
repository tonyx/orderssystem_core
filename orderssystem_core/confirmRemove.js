
function confirmRemove(uri, msg) {
    // var confirmed = confirm('Are you sure you want to remove this item?');
    var confirmed = confirm(msg);
    if (confirmed) {
        window.location = uri;
    }
}
