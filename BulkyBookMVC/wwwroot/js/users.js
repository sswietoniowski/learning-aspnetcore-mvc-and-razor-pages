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
            {
                "data": { id: "id", lockoutEnd: "lockoutEnd" },
                "render": function (data) {
                    var today = new Date().getTime();
                    var lockout = new Date(data.lockoutEnd).getTime();
                    if (lockout > today) {
                        // user is locked
                        return `
                        <div class="text-center">
                            <a class="btn btn-danger text-white fas fa-lock-open"
                                onclick=LockUnlock('${data.id}')>
                                <i style="cursor:pointer; width:100px"></i> Unlock
                            </a>
                        </div>
                        `
                    } else {
                        // user can be locked
                        return `
                        <div class="text-center">
                            <a class="btn btn-success text-white fas fa-lock"
                                onclick=LockUnlock('${data.id}')>
                                <i style="cursor:pointer; width:100px"></i> Lock
                            </a>
                        </div>
                        `
                    }
                },
                "width": "25%"
            }
        ]
    });
}

function LockUnlock(id) {
    swal({
        title: "Are you sure?",
        text: "Do you really want to do that",
        icon: "warning",
        buttons: true,
        dangerMode: true
    }).then((doIt) => {
        if (doIt) {
            $.ajax({
                type: "POST",
                url: "/Admin/Users/LockUnlock",
                data: JSON.stringify(id),
                contentType: "application/json",
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