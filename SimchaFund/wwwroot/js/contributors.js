$(() => {
    $(".deposit-button").on('click', function () {
        $("#deposit-modal").modal();
        $("#deposit-modal").append(`<input type="hidden" name="contributorId" value="${$(this).data("id")}"/>`);
    })

    $("#new-contributor-button").on('click', function () {
        $("#new-contributor-modal").modal();
    })

    $(".edit-button").on('click', function () {
        $("#edit-modal").modal();
        const firstName = $(this).data("first-name");
        const lastName = $(this).data("last-name");
        const cellNumber = $(this).data("cell-number");
        const dateCreated = $(this).data("date-created");
        const alwaysInclude = $(this).data("always-include");
        const id = $(this).data("id");
        $("#id").val(id);
        $("#first-name").val(firstName);
        $("#last-name").val(lastName);
        $("#cell-number").val(cellNumber);
        $("#date-created").val(dateCreated);
        if (alwaysInclude == "True") {
            $("#always-include").prop('checked', true);
        } else {
            $("#always-include").prop('checked', false);
        }
    })

    

    $("#search").on('keyup', function () {
        const search = $("#search").val();
        $(".body-row").each(function () {
            const row = $(this);
            const name = $(this).data("contributor-name");
            console.log(name, search);
            if (CheckMatch(name, search)) {
                row.show();
            }
            else {
                row.hide();
            }
        })
    })

    $("#clear").on('click', function () {
        $("#search").val('');
        $(".body-row").each(function () {
            const row = $(this);
            row.show();
        })
    })

    function CheckMatch(n, s) {
        const name = n.toLowerCase();
        const search = s.toLowerCase();
        return name.includes(search);
    }
})