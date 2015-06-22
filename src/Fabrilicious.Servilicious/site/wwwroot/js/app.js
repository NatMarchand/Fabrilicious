(function () {
    angular.module("app", ["ngRoute", "ngSanitize", "ngAnimate", "ui.bootstrap"])
        .config([
            "$routeProvider",
            function ($routeProvider) {
                $routeProvider
                    .when("/home", {
                        templateUrl: "views/homeView.html",
                        controller: "HomeController",
                        controllerAs: "ctrl"
                    })
                    .when("/details", {
                        templateUrl: "views/detailsView.html",
                        controller: "DetailsController",
                        controllerAs: "ctrl"
                    })
                    .otherwise({
                        redirectTo: "/home"
                    });
            }
        ]);
})();