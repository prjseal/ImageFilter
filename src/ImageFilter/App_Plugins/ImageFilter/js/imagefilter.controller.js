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
            vm.angular = angular;
        }

        function selectedProcessorChanged() {
            if (angular.equals($scope.model.selectedOption, {}) || $scope.model.selectedOption.DefaultValues === undefined) return;
            vm.previewMediaUrl = vm.mediaUrl + "?" + $scope.model.selectedOption.QueryStringEntryTemplate.format($scope.model.selectedOption.DefaultValues);
            switch ($scope.model.selectedOption.Name) {
                case "Brightness":
                    $scope.model.Brightness = $scope.model.selectedOption.DefaultValues[0];
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
            default:
                break;
            }

            vm.previewMediaUrl = vm.mediaUrl + "?" + qs;
        }

        function debounce(func, wait, immediate) {
            var timeout;
            return function () {
                var context = this, args = arguments;
                var later = function () {
                    timeout = null;
                    if (!immediate) func.apply(context, args);
                };
                var callNow = immediate && !timeout;
                clearTimeout(timeout);
                timeout = setTimeout(later, wait);
                if (callNow) func.apply(context, args);
            };
        };

        init();

    }

    angular.module('umbraco').controller('ImageFilter', ImageFilter);

})();