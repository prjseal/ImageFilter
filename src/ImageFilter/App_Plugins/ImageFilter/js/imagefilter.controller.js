(function () {
    'use strict';

    // First, checks if it isn't implemented yet.
    if (!String.prototype.format) {
        String.prototype.format = function () {
            var args = arguments;
            return this.replace(/{(\d+)}/g, function (match, number) {
                return typeof args[number] !== 'undefined'
                    ? args[number]
                    : match
                    ;
            });
        };
    }

    function ImageFilter($scope, $http, editorState) {
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
                vm.previewMediaUrl = mediaUrl;
            }

            vm.selectedProcessorChanged = selectedProcessorChanged;
            vm.setQueryString = setQueryString;
            vm.debounce = 0;
            vm.angular = angular;
        }

        function selectedProcessorChanged() {
            if (angular.equals($scope.model.selectedOption, {}) || $scope.model.selectedOption.DefaultValues === undefined) return;
            vm.previewMediaUrl = vm.mediaUrl + "?" + $scope.model.selectedOption.QueryStringEntryTemplate.format($scope.model.selectedOption.DefaultValues);
            switch ($scope.model.selectedOption.Name) {
                case "Brightness":
                    $scope.model.Brightness = $scope.model.selectedOption.DefaultValues[0];
                    break;
                case "Contrast":
                    $scope.model.Brightness = $scope.model.selectedOption.DefaultValues[0];
                    break;
                case "Filter":
                    $scope.model.Options = ["none", "blackwhite", "comic", "gotham", "greyscale", "hisatch", "invert", "lomograph", "losatch", "polaroid", "sepia" ];
                    $scope.model.Filter = $scope.model.selectedOption.DefaultValues[0];
                    break;
                case "Flip":
                    $scope.model.Options = ["none", "horizontal", "vertical", "both"];
                    $scope.model.Flip = $scope.model.selectedOption.DefaultValues[0];
                    break;
                case "Rotate":
                    $scope.model.Options = [0, 90, 180, 270];
                    $scope.model.Rotate = $scope.model.selectedOption.DefaultValues[0];
                    break;
                default:
                    break;
            }
        }

        function setQueryString() {
            var qs;
            switch ($scope.model.selectedOption.Name) {
                case "Brightness":
                    qs = $scope.model.selectedOption.QueryStringEntryTemplate.format($scope.model.Brightness);
                    break;
                case "Contrast":
                    qs = $scope.model.selectedOption.QueryStringEntryTemplate.format($scope.model.Contrast);
                    break;
                case "Filter":
                    qs = $scope.model.selectedOption.QueryStringEntryTemplate.format($scope.model.Filter);
                    break;
                case "Flip":
                    qs = $scope.model.selectedOption.QueryStringEntryTemplate.format($scope.model.Flip);
                    break;
                case "Rotate":
                    qs = $scope.model.selectedOption.QueryStringEntryTemplate.format($scope.model.Rotate);
                    break;
                default:
                    return;
            }

            vm.previewMediaUrl = vm.mediaUrl + "?" + qs;
        }

        init();

    }

    angular.module('umbraco').controller('ImageFilter', ImageFilter);

})();