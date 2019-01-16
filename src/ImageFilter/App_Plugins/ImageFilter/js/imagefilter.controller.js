(function () {
    'use strict';

    function ImageFilter ($scope, $http, editorState) {
        var vm = this;
        var apiUrl;
        var mediaUrl;

        function init() {
            mediaUrl = "";            

            apiUrl = Umbraco.Sys.ServerVariables["PJSealImageFilter"]["ImageFilterApiUrl"];

            $scope.availableImageProcessorOptions = [];

            $scope.model = {
                selectedOption: {}
            };

            $http.get(apiUrl + 'GetImageProccessorOptions').then(function (response) {
                console.log(response.data);
                $scope.availableImageProcessorOptions = response.data;
            });

            if (editorState.current.contentType.alias === "Image") {
                mediaUrl = editorState.current.mediaLink;
                vm.mediaUrl = mediaUrl;
            }

            vm.selectedProcessorChanged = selectedProcessorChanged;
        }

        function selectedProcessorChanged(option) {
            //$scope.model.selectedOption = option;
        }

        

        

        init();

    }

    angular.module('umbraco').controller('ImageFilter', ImageFilter);

})();