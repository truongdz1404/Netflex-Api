$(document).ready(function () {
    let stars = $(".rating-stars i");
    let selected = $("#selected-rating");

    let currentRating = parseInt(selected.text().trim()) || 0;
    let pathSegments = window.location.pathname.split("/");
    let filmId = pathSegments[pathSegments.length - 1]; 

    getRating();

    highlight(currentRating);
    let selectedRating = parseInt($("#selected-rating").text().trim()) || 0;

    // Sự kiện di chuột vào sao
    $("#reviews-container").on("mouseover", ".rating-stars i", function () {
        let value = $(this).data("value");
        highlight(value);
    });

    // Sự kiện rời chuột khỏi sao
    $("#reviews-container").on("mouseout", ".rating-stars i", function () {
        highlight(selectedRating); // Trả về trạng thái sao được chọn trước đó
    });

    // Sự kiện click để chọn sao
    $("#reviews-container").on("click", ".rating-stars i", function () {
        selectedRating = $(this).data("value");
        $("#selected-rating").text(selectedRating); // Cập nhật số sao đã chọn
        highlight(selectedRating)
        rating(selectedRating);
    });
    function highlight(value) {
        stars.each(function () {
            let starValue = parseInt($(this).data("value"));
            $(this).toggleClass("fas", starValue <= value);
            $(this).toggleClass("far", starValue > value);
        });
    }
    function getRating() {
        $.ajax({
            url: "/Review/GetReview/"+filmId,
            type: "GET",
            contentType: "application/json",
            success: function (data) {
                $("#reviews-container").html(data);
            },
            error: function (xhr, status, error) {
                console.error("Lỗi khi lấy đánh giá:", error);
            }
        });
    }

   

    function rating(ratingValue) {
        var reviewData = {
            Rating: ratingValue,
            FilmId: filmId
        };

        console.log("Dữ liệu gửi đi:", reviewData); // Log ra để kiểm tra
        $.ajax({
            url: "/Review/Rating/" + filmId,
            type: "POST",
            contentType: "application/json",
            data: JSON.stringify(reviewData),
            success: function (data) {
                $("#reviews-container").html(data);
            },
            error: function (xhr, status, error) {
                console.error("Lỗi khi gửi đánh giá:", error);
            }
        });
    }

    

});


