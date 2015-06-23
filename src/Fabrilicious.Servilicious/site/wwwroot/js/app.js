(function () {
    angular.module("app", ["ngRoute", "ngSanitize", "ngAnimate", "ui.bootstrap"])
        .config([
            "$routeProvider",
            function ($routeProvider) {
                $routeProvider
                    .when("/map", {
                        templateUrl: "views/mapView.html",
                        controller: "MapController",
                        controllerAs: "ctrl"
                    })
                    .when("/details", {
                        templateUrl: "views/detailsView.html",
                        controller: "DetailsController",
                        controllerAs: "ctrl"
                    })
                    .otherwise({
                        redirectTo: "/map"
                    });
            }
        ]);
})();