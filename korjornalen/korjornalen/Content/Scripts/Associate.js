$(document).ready(function () {
    var projects = new Bloodhound({
        datumTokenizer: Bloodhound.tokenizers.obj.whitespace("projectInfo"),
        queryTokenizer: Bloodhound.tokenizers.whitespace,
        remote: {
            url : "/Project/Suggest?q=%QUERY"
        }   
    });

    projects.initialize();


    $("#searchProject").typeahead({
            highlight: true
        }, {
            displayKey: 'ProjectInfo',
            source: projects.ttAdapter()
        }
    ).on("typeahead:selected", function (obj, project) {
        $("#selectedProjectId").val(project.ProjectId);
    });
});