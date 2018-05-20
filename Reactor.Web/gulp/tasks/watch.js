var gulp = require('gulp'),
    watch = require('gulp-watch');

gulp.task("default", ["styles","scripts" ,"fa", "fonts"], function () {
    watch([
        "./temp/styles/**/*.scss"
        // "./node_modules/bootstrap/scss/**/*.scss"
    ], function () {

        gulp.start("styles");
    });
    
    watch("./temp/scripts/**/*.{ts,js}",function () {
        gulp.start("scripts");
    })
});