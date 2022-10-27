// adds the resource to umbraco.resources module:
angular.module('umbraco').factory('usedQuestionsResource',
    function ($http, umbRequestHelper) {
        // the factory object returned
        return {
            // this calls the ApiController we setup earlier
            getAll: function ($q) {
                return umbRequestHelper.resourcePromise(
                    $http.get("/Umbraco/backoffice/UsedQuestionsContentApp/UsedQuestions/GetQuestions"+"?currentNodeId="+ $q),
                    "Failed to retrieve all question data");
            }
        };
    }
);