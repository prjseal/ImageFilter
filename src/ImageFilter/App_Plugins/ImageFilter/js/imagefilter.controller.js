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

    function ImageFilter($scope, $http, editorState, navigationService, $location) {

        $scope.createNewMedia = function (overwriteExisting) {

            apiUrl = Umbraco.Sys.ServerVariables["PJSealImageFilter"]["ImageFilterApiUrl"];

            $http.post(apiUrl + "CreateNewMedia", JSON.stringify({ MediaId: parseInt($scope.mediaId), QueryString: $scope.model.queryString, OverwriteExisting: overwriteExisting }),
                {
                    headers: {
                        'Content-Type': 'application/json'
                    }
                }).then(function (response) {
                    navigationService.hideDialog();

                    if (editorState.current.id != response.data) {
                        $location.path('media/media/edit/' + response.data);
                    }
                    else {
                        window.location.reload(true);
                    }

                }, function (response) {
                    navigationService.hideDialog();
                });

        };

        $scope.reset = function() {
            $scope.model.Brightness = 0;
            $scope.model.Contrast = 0;
            $scope.model.Filter = 'none';
            $scope.model.Flip = 'none';
            $scope.model.Rotate = 0;
            $scope.model.queryString = "";
            $scope.model.qsBrightness = "";
            $scope.model.qsContrast = "";
            $scope.model.qsFilter = "";
            $scope.model.qsFlip = "";
            $scope.model.qsRotate = "";

            vm.previewMediaUrl = vm.mediaUrl + $scope.model.queryString;
        };

        var vm = this;
        var apiUrl;
        var mediaUrl;

        function init() {
            mediaUrl = "";

            apiUrl = Umbraco.Sys.ServerVariables["PJSealImageFilter"]["ImageFilterApiUrl"];
            $scope.mediaId = editorState.current.id;
            $scope.availableImageProcessorOptions = [];

            $scope.model = {
                selectedOption: {},
                queryString: "",
                qsBrightness: "",
                qsContrast: "",
                qsFilter: "",
                qsFlip: "",
                qsRotate: ""
            };

            $http.get(apiUrl + 'GetImageProccessorOptions').then(function (response) {
                $scope.availableImageProcessorOptions = response.data;
            });

            if (editorState.current.contentType.alias === "Image") {
                mediaUrl = editorState.current.mediaLink;
                vm.mediaUrl = mediaUrl;
                vm.previewMediaUrl = mediaUrl;
                vm.fileName = editorState.current.mediaLink.replace(/^.*[\\\/]/, '');
            }

            vm.selectedProcessorChanged = selectedProcessorChanged;
            vm.setQueryString = setQueryString;
            vm.debounce = 0;
            vm.angular = angular;
            vm.showQueryString = true;
            
        }

        function selectedProcessorChanged() {
            if (angular.equals($scope.model.selectedOption, {}) || $scope.model.selectedOption.DefaultValues === undefined) return;
            vm.previewMediaUrl = vm.mediaUrl + $scope.model.queryString;

            switch ($scope.model.selectedOption.Name) {
                case "Brightness":
                    $scope.model.Brightness = $scope.model.Brightness !== null && $scope.model.Brightness !== undefined ? $scope.model.Brightness : $scope.model.selectedOption.DefaultValues[0];
                    break;
                case "Contrast":
                    $scope.model.Contrast = $scope.model.Contrast !== null && $scope.model.Contrast !== undefined ? $scope.model.Contrast : $scope.model.selectedOption.DefaultValues[0];
                    break;
                case "Filter":
                    $scope.model.Options = ["none", "blackwhite", "comic", "gotham", "greyscale", "hisatch", "invert", "lomograph", "losatch", "polaroid", "sepia" ];
                    $scope.model.Filter = $scope.model.Filter !== null && $scope.model.Filter !== undefined ? $scope.model.Filter : $scope.model.selectedOption.DefaultValues[0];
                    break;
                case "Flip":
                    $scope.model.Options = ["none", "horizontal", "vertical", "both"];
                    $scope.model.Flip = $scope.model.Flip !== null && $scope.model.Flip !== undefined ? $scope.model.Flip : $scope.model.selectedOption.DefaultValues[0];
                    break;
                case "Rotate":
                    $scope.model.Options = [0, 90, 180, 270];
                    $scope.model.Rotate = $scope.model.Rotate !== null && $scope.model.Rotate !== undefined ? $scope.model.Rotate : $scope.model.selectedOption.DefaultValues[0];
                    break;
                default:
                    break;
            }
        }

        function setQueryString() {
            switch ($scope.model.selectedOption.Name) {
                case "Brightness":
                    $scope.model.qsBrightness = $scope.model.selectedOption.QueryStringEntryTemplate.format($scope.model.Brightness);
                    break;
                case "Contrast":
                    $scope.model.qsContrast = $scope.model.selectedOption.QueryStringEntryTemplate.format($scope.model.Contrast);
                    break;
                case "Filter":
                    $scope.model.qsFilter = $scope.model.selectedOption.QueryStringEntryTemplate.format($scope.model.Filter);
                    break;
                case "Flip":
                    $scope.model.qsFlip = $scope.model.selectedOption.QueryStringEntryTemplate.format($scope.model.Flip);
                    break;
                case "Rotate":
                    $scope.model.qsRotate = $scope.model.selectedOption.QueryStringEntryTemplate.format($scope.model.Rotate);
                    break;
                default:
                    return;
            }

            var qs = "";

            if ($scope.model.qsBrightness) {
                if (qs !== "") {
                    qs += "&";
                }
                qs += $scope.model.qsBrightness;
            }
            if ($scope.model.qsContrast) {
                if (qs !== "") {
                    qs += "&";
                }
                qs += $scope.model.qsContrast;
            }
            if ($scope.model.qsFilter) {
                if (qs !== "") {
                    qs += "&";
                }
                qs += $scope.model.qsFilter;
            }
            if ($scope.model.qsFlip) {
                if (qs !== "") {
                    qs += "&";
                }
                qs += $scope.model.qsFlip;
            }
            if ($scope.model.qsRotate) {
                if (qs !== "") {
                    qs += "&";
                }
                qs += $scope.model.qsRotate;
            }
            vm.previewMediaUrl = vm.mediaUrl + "?" + qs;
            $scope.model.queryString = "?" + qs;
        }

        init();

    }

    angular.module('umbraco').controller('ImageFilter', ImageFilter);

})();