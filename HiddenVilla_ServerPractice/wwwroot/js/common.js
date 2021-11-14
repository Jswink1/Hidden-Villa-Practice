/* Toastr Notifications */
window.ShowToastr = (type, message) => {
    if (type === "success") {
        toastr.success(message, "Operation Successfull!");
    }
    if (type === "error") {
        toastr.error(message, "Operation Failed!");
    }
}

/* Show/Hide DeleteConfirmation.razor Modal */
function ShowDeleteConfirmationModal() {
    $('#deleteConfirmationModal').modal('show');
}
function HideDeleteConfirmationModal() {
    $('#deleteConfirmationModal').modal('hide');
}