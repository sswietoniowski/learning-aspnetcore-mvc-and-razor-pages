var dataTable;

$(document).ready(function () {
    var url = window.location.search;
    if (url.includes("Processing")) {
        loadDataTable("GetAll?status=Processing");
    } else if (url.includes("Pending")) {
        loadDataTable("GetAll?status=Pending");
    } else if (url.includes("Completed")) {
        loadDataTable("GetAll?status=Completed");
    } else if (url.includes("Rejected")) {
        loadDataTable("GetAll?status=Rejected");
    } else {
        loadDataTable("GetAll?status=All");
    }
})

function loadDataTable(url) {
    dataTable = $("#tblData").DataTable({
        "ajax": {
            "url": "/Admin/Orders/" + url
        },
        "columns": [
            { "data": "id", "width": "10%" },
            { "data": "name", "width": "15%" },
            { "data": "phoneNumber", "width": "15%" },
            { "data": "applicationUser.email", "width": "15%" },
            { "data": "orderStatus", "width": "15%" },
            { "data": "orderTotal", "width": "15%" },
            {
                "data": "id",
                "render": function(data) {
                    return `
                        <div class="text-center">
                            <a href="/Admin/Orders/Details/${data}" class="btn btn-success text-white fas fa-edit">
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
