const gulp = require('gulp'),
    watch = require('gulp-watch');

gulp.task("default", ["styles"], function () {
    watch(["./temp/styles/**/*.scss", "./node_modules/bootstrap/scss/**/*.scss"], function () {

        gulp.start("styles");
    });
});