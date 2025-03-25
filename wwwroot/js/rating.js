$(document).ready(function () {
    let pathSegments = window.location.pathname.split("/").filter(segment => segment);
    let contentId = pathSegments[pathSegments.length - 1]; // ID của phim hoặc series
    let isSeries = window.location.pathname.toLowerCase().includes("series"); // Kiểm tra nếu là series

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

        stars.off("mouseover mouseout click"); // Xóa sự kiện cũ để tránh trùng lặp
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
        let url = isSeries ? "/Review/GetSerieReview/" + contentId : "/Review/GetFilmReview/" + contentId;
        $.ajax({
            url: url,
            type: "GET",
            success: function (data) {
                $("#review-container").html(data);
                ratings = parseFloat($("#selected-rating").attr("data-rating")) || 0;
                console.log("Rating data:", data);
                highlight(ratings);
                bindStarEvents();
            },
            error: function (xhr, status, error) {
                console.error("Lỗi khi lấy đánh giá:", error, "Status:", xhr.status);
            }
        });
    }

    function rating(ratingValue) {
        let url = isSeries ? "/Review/SerieRating/" + contentId : "/Review/FilmRating/" + contentId;
        var reviewData = {
            Rating: ratingValue
        };

        $.ajax({
            url: url,
            type: "POST",
            contentType: "application/json",
            data: JSON.stringify(reviewData),
            success: function (data) {
                $("#review-container").html(data);
                ratings = parseFloat($("#selected-rating").attr("data-rating")) || 0;
                highlight(ratings);
                bindStarEvents();
            },
            error: function (xhr, status, error) {
                console.error("Lỗi khi gửi đánh giá:", error, "Status:", xhr.status);
            }
        });
    }
});