'use strict';

var app = angular.module('sentimentAnalysis', ['SignalR', 'nvd3ChartDirectives', 'ngSanitize']);

app.factory('emotionAnalysisFactory', [
    '$rootScope', 'Hub', '$timeout', function($rootScope, Hub, $timeout) {

        var Analysis = this;


        var hub = new Hub('sentimentHub', {
            listeners: {
                'socialEventReceived': function (analysedMessage) {
                    $rootScope.$broadcast('socialevents:newanalysedevent', analysedMessage);
                }
            },

            methods: ['addHashtag', 'removeHashtag'],

            errorHandler: function(error) {
                console.error(error);
            }
        });


        Analysis.addHashtag = function(hashtag) {
            hub.addHashtag(hashtag);
            console.log("ADDED HASHTAG - " + hashtag);
        };

        Analysis.removeHashtag = function (hashtag) {
            hub.removeHashtag(hashtag);
            console.log("REMOVE HASHTAG - " + hashtag);
        };

        return Analysis;
    }
]);

app.controller('emotionAnalysisController', [
    '$scope', 'emotionAnalysisFactory', function($scope, emotionAnalysisFactory) {

        $scope.hashtag = "";
        $scope.oldHashtag = "";
        $scope.hashtag2 = "";
        $scope.oldHashtag2 = "";
        $scope.xAxisLabelValue = 0;
        $scope.timeline1 = [];
        $scope.timeline2 = [];
        $scope.exampleData = [
            {
                key: $scope.hashtag,
                values: [],
                color: '#ff7f0e'
            },
            {
                key: $scope.hashtag2,
                values: [],
                color: '#2ca02c'
    }
        ];

        $scope.addHashtag = function (hashval) {
            $scope.hashtag = hashval;
            if ($scope.oldHashtag != "" && $scope.oldHashtag != $scope.hashtag && $scope.oldHashtag != $scope.hashtag2)
                emotionAnalysisFactory.removeHashtag($scope.oldHashtag);

            $scope.exampleData[0].key = $scope.hashtag;
            emotionAnalysisFactory.addHashtag($scope.hashtag);

            $scope.oldHashtag = $scope.hashtag;
        };

        // dirty copy paste - this is just for a demo
        $scope.addHashtag2 = function (hashval) {
            $scope.hashtag2 = hashval;
            if ($scope.oldHashtag2 != "" && $scope.oldHashtag2 != $scope.hashtag2 && $scope.oldHashtag2 != $scope.hashtag)
                emotionAnalysisFactory.removeHashtag($scope.oldHashtag2);

            $scope.exampleData[1].key = $scope.hashtag2;
            emotionAnalysisFactory.addHashtag($scope.hashtag2);

            $scope.oldHashtag2 = $scope.hashtag2;
        };

        $scope.$on('socialevents:newanalysedevent', function(event, data) {

            console.log("BROADCAST-ANALYSIS: " + data.Hashtag + " MESSAGE: " + data.Message + " POINTS: " + data.Sentiment);
            if (data.Hashtag == $scope.hashtag) {
                $scope.exampleData[0].values.push([$scope.xAxisLabelValue, data.Sentiment, data.EmbededHtml]);
                $scope.timeline1.unshift(data);
            }
            if (data.Hashtag == $scope.hashtag2) {
                $scope.exampleData[1].values.push([$scope.xAxisLabelValue, data.Sentiment, data.EmbededHtml]);
                var htmlsofar2 = $scope.timeline2;
                $scope.timeline2.unshift(data);
            }
            $scope.xAxisLabelValue++;
            $scope.$apply();
        });

        $scope.xFunction = function() {
            return function(d) {
                return d[0];
            };
        };

        $scope.yFunction = function() {
            return function(d) {
                return d[1];
            };
        };

        $scope.toolTipContentFunction = function() {
            return function (key, x, y, e, graph) {
                return '<div width="200"><h2>' + key + '</h2>' + e.point[2] + '</div>';
            };
        };

        $scope.hashtagNice = function() {
            if ($scope.hashtag != "")
                return $scope.hashtag;
            else {
                return "Hashtag #1";
            }
        };

        $scope.hashtag2Nice = function () {
            if ($scope.hashtag2 != "")
                return $scope.hashtag2;
            else {
                return "Hashtag #2";
            }
        };

    }
]);
