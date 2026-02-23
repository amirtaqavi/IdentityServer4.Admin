var gulp = require("gulp");
var concat = require("gulp-concat");
var uglify = require("gulp-uglify");
var sass = require("gulp-sass")(require("sass"));
var minifyCSS = require("gulp-clean-css");
var del = require("del");

var distFolder = "./wwwroot/dist/";
var jsFolder = `${distFolder}js/`;
var cssFolder = `${distFolder}css/`;
var cssThemeFolder = `${distFolder}css/themes/`;

function processClean() {
  return del(`${distFolder}**`, { force: true });
}

function processScripts() {
  // For Bootstrap 5 + Popper v2
  return gulp
    .src(
      [
        "./node_modules/jquery/dist/jquery.js",
        "./node_modules/jquery-validation/dist/jquery.validate.js",
        "./node_modules/jquery-validation-unobtrusive/dist/jquery.validate.unobtrusive.js",
        // Popper v2 path (used by Bootstrap 5)
        "./node_modules/@popperjs/core/dist/umd/popper.js",
        // Bootstrap 5 JS
        "./node_modules/bootstrap/dist/js/bootstrap.js",
        "./node_modules/cookieconsent/src/cookieconsent.js",
        "./node_modules/holderjs/holder.js",
        "./Scripts/App/components/Menu.js",
        "./Scripts/App/components/Language.js",
        "./Scripts/App/components/Theme.js",
        "./Scripts/App/components/CookieConsent.js",
      ],
      { allowEmpty: true }
    ) // Add allowEmpty to prevent errors if files are missing
    .pipe(concat("bundle.min.js"))
    .pipe(uglify())
    .on("error", function (err) {
      console.log("Uglify error: ", err.toString());
      this.emit("end");
    })
    .pipe(gulp.dest(jsFolder));
}

function processFonts() {
  return gulp
    .src([
      "./node_modules/font-awesome/fonts/**",
      "./node_modules/open-iconic/font/fonts/**",
    ])
    .pipe(gulp.dest(`${distFolder}fonts/`));
}

function processSass() {
  return gulp
    .src("Styles/web.scss")
    .pipe(sass())
    .on("error", sass.logError)
    .pipe(gulp.dest(cssFolder));
}

function processSassMin() {
  return gulp
    .src("Styles/web.scss")
    .pipe(sass())
    .on("error", sass.logError)
    .pipe(minifyCSS())
    .pipe(concat("web.min.css"))
    .pipe(gulp.dest(cssFolder));
}

function processStyles() {
  // Bootstrap 5 doesn't have bootstrap.css in the same path
  // It's now in dist/css/bootstrap.css
  return gulp
    .src(
      [
        "./node_modules/bootstrap/dist/css/bootstrap.css",
        "./node_modules/open-iconic/font/css/open-iconic-bootstrap.css",
        "./node_modules/font-awesome/css/font-awesome.css",
        "./node_modules/cookieconsent/build/cookieconsent.min.css",
      ],
      { allowEmpty: true }
    )
    .pipe(minifyCSS())
    .pipe(concat("bundle.min.css"))
    .pipe(gulp.dest(cssFolder));
}

function processTheme() {
  // Bootswatch 5+ structure
  return gulp
    .src("node_modules/bootswatch/dist/**/bootstrap.min.css")
    .pipe(gulp.dest(cssThemeFolder));
}

var buildStyles = gulp.series(
  processFonts,
  processStyles,
  processTheme,
  processSass,
  processSassMin
);
var build = gulp.parallel(buildStyles, processScripts);

gulp.task("clean", processClean);
gulp.task("styles", buildStyles);
gulp.task("sass", processSass);
gulp.task("sass:min", processSassMin);
gulp.task("fonts", processFonts);
gulp.task("scripts", processScripts);
gulp.task("build", build);
gulp.task("default", build);

// watch
function processWatch() {
  gulp.watch(["Styles/**/*.scss"], buildStyles);
}
gulp.task("watch", processWatch);
exports.default = processWatch;
