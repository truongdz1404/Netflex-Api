$(document).ready(function () {
    let pathSegments = window.location.pathname.split("/");
    let filmId = pathSegments[pathSegments.length - 1];
    let selected = $("#selected-rating");
    let ratings = 10;

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
        let averageRating = parseFloat($("#average-rating").text());
        highlight(averageRating);

        stars.on("mouseover", function () {
            let value = $(this).data("value");
            highlight(value);
        });

        stars.on("mouseout", function () {
            let currentValue = selected.text().trim() === "0" ? ratings : selected.text();
            highlight(currentValue);
        });

        stars.on("click", function () {
            let ratingValue = $(this).data("value"); 
            selected.text(ratingValue);
            rating(ratingValue);
        });
    }

    function getRating() {
        $.ajax({
            url: "/Review/GetReview/" + filmId,
            type: "GET",
            contentType: "application/json",
            success: function (data) {
                $("#review-container").html(data);
                bindStarEvents();
            },
            error: function (xhr, status, error) {
                console.error("Error at GetReview:", error);
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
                bindStarEvents();
            },
            error: function (xhr, status, error) {
                console.error("Lỗi khi gửi đánh giá:", error);
            },
        });
    }
});