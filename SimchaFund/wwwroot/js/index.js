$(() => {
    $("#new-simcha-btn").on('click', function () {
        $("#new-simcha-modal").modal();
    })

    $("#search-box").on('keyup', function () {
        const search = $("#search-box").val();
        $(".body-row").each(function () {
            const row = $(this);
            const name = row.data('simcha-name');
            const isMatch = CheckMatch(name, search);
            if (isMatch) {
                row.show();
            } else {
                row.hide();
            }
        });
    })

    function CheckMatch(n, s) {
        const name = n.toLowerCase();
        const search = s.toLowerCase();
        return name.includes(search);
    }

    $("#clear").on('click', function () {
        $("#search-box").val('');
        $(".body-row").each(function () {
            const row = $(this);
            row.show();
        })
    })
})