function UpdateStatus(type, column, id, checkbox) {
    var data = {
        type: type,
        column : column,
        id: id,
        status : $(checkbox).is(":checked") 
    },
    url = "/Admin/ChangeStatus",
    messageClasses = "alert alert-dismissable";

    $.post(url, data, function(data) {
        var responseId = "appMessage-message";
        var responseDivId = "appMessage-alert-box"
        switch (data.Type) {
            case "error":
                messageClasses = messageClasses + " alert-danger";
                break;
            case "success":
                messageClasses = messageClasses + " alert-success";
                break;
        }
                
        if (document.getElementById(responseDivId) == null) {
            // Setup dismissable bootstrap alert.
            var html = "<div id=\""+responseDivId+"\" class=\""+messageClasses+"\"><button type=\"button\" class=\"close\" data-dismiss=\"alert\" aria-label=\"Close\"><span aria-hidden=\"true\">&times;</span></button><span id=\""+responseId+"\">"+data.Message+"</span></div>";

            $("#appMessage-container").prepend(html);
        } else {
            $("#" + responseDivId).removeClass("alert alert-dismissable alert-danger alert-success alert-info alert-warning");
            $("#" + responseDivId).addClass(messageClasses);
            $("#"+responseId).html(data.Message);
        }
    });
}