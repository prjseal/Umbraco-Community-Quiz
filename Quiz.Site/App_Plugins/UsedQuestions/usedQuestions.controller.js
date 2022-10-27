angular.module("umbraco")
    .controller("CQ.UsedQuestions", function (editorState, usedQuestionsResource) {

        var vm = this;
        vm.CurrentNodeId = editorState.current.id;
        vm.CurrentNodeAlias = editorState.current.contentTypeAlias;
     
        function getQuestions() {
            usedQuestionsResource.getAll(vm.CurrentNodeId).then(function (data) {
                if (Utilities.isArray(data)) {
                    vm.Questions = data;
                 
                }
            });
        }

        getQuestions();

       
      
    });