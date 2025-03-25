$((function(){
    var url;
    var redirectUrl;
    var target;

    $('body').append(`
            <div class="modal fade" id="deleteModal" tabindex="-1" role="dialog" aria-labelledby="myModalLabel" aria-hidden="true">
            <div class="modal-dialog text-white" role="document">
                <div class="modal-content">
                    <div class="modal-header pt-2">
                        <h5 class="modal-title" id="myModalLabel">Warning</h4>
                        <button class="btn-transparent p-0 fw-bold fs-3 close" type="button" data-bs-dismiss="modal" aria-label="Close">
                            <span aria-hidden="true" class="text-white-50">&times;</span>
                        </button>
                    </div>
                    <div class="modal-body delete-modal-body">  
                        
                    </div>
                    <div class="modal-footer">
                        <button type="button" class="btn btn-secondary" data-bs-dismiss="modal" id="cancel-delete">Cancel</button>
                        <button type="button" class="btn btn-danger" id="confirm-delete">Delete</button>
                    </div>
                </div>
            </div>
            </div>`);

    $(".delete").on('click',(e)=>{
        e.preventDefault();

        target = e.target;
        var Id = $(target).data('id');
        var link = $(target).data('url');
        var bodyMessage = $(target).data('body-message');
        redirectUrl = $(target).data('redirect-url');
        url = link + "/" + Id; 

        $(".delete-modal-body").text(bodyMessage);
        $("#deleteModal").modal('show');
        console.log(bodyMessage);
    });

    $("#confirm-delete").on('click',()=>{
        $.ajax({
            url: url,
            type: 'DELETE',
            success: (result) => {
                if (!redirectUrl) {
                    $(target).closest('tr').fadeOut("slow");
                } else {
                    window.location.href = redirectUrl;
                }
            },
            error: (error) => {
                console.error("error:", error);
                if (redirectUrl) {
                    window.location.href = redirectUrl;
                }
            },
            complete: () => {
                $("#deleteModal").modal('hide');
            }
        });
    });

}()));