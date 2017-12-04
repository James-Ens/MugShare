

/*
    Function: MSU_create_submit

    Submits new record to the database
*/
function MSU_create_submit() {
    toastr.clear();
    MSU_stringCorrections();
    MSU_create_field_check();
}