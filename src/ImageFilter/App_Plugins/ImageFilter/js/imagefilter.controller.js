angular.module("umbraco")
    .controller("ImageFilter", function ($scope, editorState) {

        var mediaUrl = "";

        var vm = this;

        if (editorState.current.contentType.alias === "Image") {
            mediaUrl = editorState.current.mediaLink;
            vm.mediaUrl = mediaUrl;
        }

        console.log(mediaUrl);

    });