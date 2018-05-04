const gulp = require('gulp'),
    sass = require('gulp-sass'),
    autoprefixer = require('gulp-autoprefixer');

gulp.task("styles", function () {
   return gulp.src([
       "./node_modules/bootstrap/scss/bootstrap.scss",
            "./temp/styles/**/*.scss"
        ]).pipe(sass().on("error", sass.logError))
        .pipe(autoprefixer())
        .pipe(gulp.dest("./wwwroot/assets/styles"));
});