var dataTable;

$(document).ready(function () {
    loadDataTable();
})

function loadDataTable() {
    dataTable = $("#tblData").DataTable({
        "ajax": {
            "url": "/Admin/Companies/GetAll"
        },
        "columns": [
            { "data": "name", "width": "20%" },
            { "data": "streetAddress", "width": "10%" },
            { "data": "city", "width": "10%" },
            { "data": "state", "width": "10%" },
            { "data": "postalCode", "width": "10%" },
            { "data": "phoneNumber", "width": "10%" },
            { "data": "isAuthorizedCompany", "width": "10%" },
            {
                "data": "id",
                "render": function(data) {
                    return `
                        <div class="text-center">
                            <a href="/Admin/Companies/Upsert/${data}" class="btn btn-success text-white fas fa-edit">
                                <i style="cursor:pointer"></i>
                            </a>
                            <a class="btn btn-danger text-white fas fa-trash-alt"
                                onclick=Delete('/Admin/Companies/Delete/${data}')>
                                <i style="cursor:pointer"></i>
                            </a>
                        </div>
                    `
                },
                "width": "20%"
            }
        ]
    });
}

function Delete(url) {
    swal({
        title: "Are you sure?",
        text: "Once deleted, you will not be able to recover",
        icon: "warning",
        buttons: true,
        dangerMode: true
    }).then((willDelete) => {
        if (willDelete) {
            $.ajax({
                type: "DELETE",
                url: url,
                success: function (data) {
                    if (data.success) {
                        toastr.success(data.message);
                        dataTable.ajax.reload();
                    }
                    else {
                        toastr.error(data.message);
                    }
                }
            });
        }
    });
}