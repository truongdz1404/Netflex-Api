$(document).ready(function () {
    let pathSegments = window.location.pathname.split("/");
    let filmId = pathSegments[pathSegments.length - 1];

    let selected = $("#selected-rating");
    let ratings = 0;
    getRating();

    function highlight(value) {
        $(".rating-stars i").each(function () {
            let starValue = parseInt($(this).data("value"));
            $(this).toggleClass("fas", starValue <= value);
            $(this).toggleClass("far", starValue > value);
        });
    }

    function bindStarEvents() {
        let stars = $(".rating-stars i");

        stars.on("mouseover", function () {
            let value = $(this).data("value");
            highlight(value);
        });

        stars.on("mouseout", function () {
            let currentValue = parseInt(selected.text().trim()) || ratings;
            highlight(currentValue);
        });

        stars.on("click", function () {
            ratings = $(this).data("value");
            selected.text(ratings);
            highlight(ratings);
            rating(ratings);
        });
    }

    function getRating() {
        $.ajax({
            url: "/Review/GetReview/" + filmId,
            type: "GET",
            contentType: "application/json",
            success: function (data) {
                $("#review-container").html(data);
                ratings = parseFloat($("#selected-rating").attr("data-rating")) || 0;
                console.log(data);
                highlight(ratings);
                bindStarEvents();
            },
            error: function (xhr, status, error) {
                console.error("Lỗi khi lấy đánh giá:", error);
            },
        });
    }

    function rating(ratingValue) {
        var reviewData = {
            Rating: ratingValue,
            FilmId: filmId,
        };

        $.ajax({
            url: "/Review/Rating/" + filmId,
            type: "POST",
            contentType: "application/json",
            data: JSON.stringify(reviewData),
            success: function (data) {
                $("#review-container").html(data);
                highlight(ratings);
                bindStarEvents();
            },
            error: function (xhr, status, error) {
                console.error("Lỗi khi gửi đánh giá:", error);
            },
        });
    }
});
