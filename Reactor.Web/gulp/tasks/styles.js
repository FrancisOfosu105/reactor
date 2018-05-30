const gulp = require('gulp'),
    sass = require('gulp-sass'),
    autoprefixer = require('gulp-autoprefixer');

gulp.task("styles", function () {
   return gulp.src([
       // "./node_modules/bootstrap/scss/bootstrap.scss",
            "./build/styles/**/*.scss"
        ]).pipe(sass().on("error", sass.logError))
        .pipe(autoprefixer())
        .pipe(gulp.dest("./wwwroot/assets/styles"));
});

gulp.task("fa", function () {
  return gulp.src("./node_modules/font-awesome/css/font-awesome.css")
      .pipe(gulp.dest("./wwwroot/assets/styles")); 
});

gulp.task("fonts", function () {
    return gulp.src("./node_modules/font-awesome/fonts/**")
        .pipe(gulp.dest("./wwwroot/assets/fonts"));
});