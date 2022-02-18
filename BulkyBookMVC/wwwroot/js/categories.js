var dataTable;

$(document).ready(function () {
    loadDataTable();
})

function loadDataTable() {
    dataTable = $("#tblData").DataTable({
        "ajax": {
            "url": "/Admin/Categories/GetAll"
        },
        "columns": [
            { "data": "name", "width": "60%" },
            {
                "data": "id",
                "render": function(data) {
                    return `
                        <div class="text-center">
                            <a href="/Admin/Categories/Upsert/${data}" class="btn btn-success text-white fas fa-edit">
                                <i style="cursor:pointer"></i>
                            </a>
                            <a class="btn btn-danger text-white fas fa-trash-alt">
                                <i style="cursor:pointer"></i>
                            </a>
                        </div>
                    `
                },
                "width": "40%"
            }
        ]
    });
}