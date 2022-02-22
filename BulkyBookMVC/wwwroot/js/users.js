var dataTable;

$(document).ready(function () {
    loadDataTable();
})

function loadDataTable() {
    dataTable = $("#tblData").DataTable({
        "ajax": {
            "url": "/Admin/Users/GetAll"
        },
        "columns": [
            { "data": "name", "width": "15%" },
            { "data": "email", "width": "15%" },
            { "data": "phoneNumber", "width": "15%" },
            { "data": "company.name", "width": "15%" },
            { "data": "role", "width": "15%" },
        //    {
        //        "data": "id",
        //        "render": function(data) {
        //            return `
        //                <div class="text-center">
        //                    <a class="btn btn-danger text-white fas fa-trash-alt"
        //                        onclick=Delete('/Admin/Users/Delete/${data}')>
        //                        <i style="cursor:pointer"></i>
        //                    </a>
        //                </div>
        //            `
        //        },
        //        "width": "25%"
        //    }
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