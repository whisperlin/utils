//是否使用IDE自带的node环境和插件，设置false后，则使用自己环境(使用命令行方式执行)
const useIDENode = process.argv[0].indexOf("LayaAir") > -1 ? true : false;
//获取Node插件和工作路径
let ideModuleDir = useIDENode ? process.argv[1].replace("gulp\\bin\\gulp.js", "").replace("gulp/bin/gulp.js", "") : "";
let workSpaceDir = useIDENode ? process.argv[2].replace("--gulpfile=", "").replace("\\.laya\\publish.js", "").replace("/.laya/publish.js", "") + "/" : "./../";

//引用插件模块
const gulp = require(ideModuleDir + "gulp");
const fs = require("fs");
const path = require("path");
const uglify = require(ideModuleDir + "gulp-uglify");
const jsonminify = require(ideModuleDir + "gulp-jsonminify");
const image = require(ideModuleDir + "gulp-image");
const rev = require(ideModuleDir + "gulp-rev");
const revdel = require(ideModuleDir + "gulp-rev-delete-original");
const revCollector = require(ideModuleDir + 'gulp-rev-collector');
const del = require(ideModuleDir + "del");

//清理临时文件夹，加载配置
let config;
let releaseDir;
gulp.task("loadConfig", function () {
	let platform = "web"
	if (!useIDENode && process.argv.length > 5 && process.argv[4] == "--config") {
		platform = process.argv[5].replace(".json", "");
	}
	if (useIDENode && process.argv.length >= 4 && process.argv[3].startsWith("--config") && process.argv[3].endsWith(".json")) {
		platform = process.argv[3].match(/(\w+).json/)[1];
	}
	let _path;
	if (!useIDENode) {
		_path = platform + ".json";
		releaseDir = "../release/" + platform;
	}
	if (useIDENode) {
		_path = path.join(workSpaceDir, ".laya", `${platform}.json`);
		releaseDir = path.join(workSpaceDir, "release", platform).replace(/\\/g, "/");
	}
	let file = fs.readFileSync(_path, "utf-8");
	if (file) {
		file = file.replace(/\$basePath/g, releaseDir);
		config = JSON.parse(file);
	}
});

//重新编译项目
gulp.task("compile", ["loadConfig"], function () {
	if (config.compile) {
		console.log("compile");
	}
});

//清理release文件夹
gulp.task("clearReleaseDir", ["compile"], function (cb) {
	if (config.clearReleaseDir) {
		del([releaseDir, releaseDir + "_pack"], { force: true }).then(paths => {
			cb();
		});
	} else cb();
});

//copy bin文件到release文件夹
gulp.task("copyFile", ["clearReleaseDir"], function () {
	if (useIDENode) {
		config.copyFilesFilter = `${workSpaceDir}/bin/**/*.*`;
	}
	var stream = gulp.src(config.copyFilesFilter);
	return stream.pipe(gulp.dest(releaseDir));
});

//压缩json
gulp.task("compressJson", ["copyFile"], function () {
	if (config.compressJson) {
		return gulp.src(config.compressJsonFilter)
			.pipe(jsonminify())
			.pipe(gulp.dest(releaseDir));
	}
});

//压缩js
gulp.task("compressJs", ["compressJson"], function () {
	if (config.compressJs) {
		return gulp.src(config.compressJsFilter)
			.pipe(uglify())
			.on('error', function (err) {
				console.warn(err.toString());
			})
			.pipe(gulp.dest(releaseDir));
	}
});

//压缩png，jpg
gulp.task("compressImage", ["compressJs"], function () {
	if (config.compressImage) {
		return gulp.src(config.compressImageFilter)
			.pipe(image({
				pngquant: true,			//PNG优化工具
				optipng: false,			//PNG优化工具
				zopflipng: true,		//PNG优化工具
				jpegRecompress: false,	//jpg优化工具
				mozjpeg: true,			//jpg优化工具
				guetzli: false,			//jpg优化工具
				gifsicle: false,		//gif优化工具
				svgo: false,			//SVG优化工具
				concurrent: 10,			//并发线程数
				quiet: true 			//是否是静默方式
				// optipng: ['-i 1', '-strip all', '-fix', '-o7', '-force'],
				// pngquant: ['--speed=1', '--force', 256],
				// zopflipng: ['-y', '--lossy_8bit', '--lossy_transparent'],
				// jpegRecompress: ['--strip', '--quality', 'medium', '--min', 40, '--max', 80],
				// mozjpeg: ['-optimize', '-progressive'],
				// guetzli: ['--quality', 85]
			}))
			.pipe(gulp.dest(releaseDir));
	}
});

//生成版本管理信息
gulp.task("version1", ["compressImage"], function () {
	if (config.version) {
		return gulp.src(config.versionFilter)
			.pipe(rev())
			.pipe(gulp.dest(releaseDir))
			.pipe(revdel())
			.pipe(rev.manifest("version.json"))
			.pipe(gulp.dest(releaseDir));
	}
});

//替换index.js里面的变化的文件名
gulp.task("version2", ["version1"], function () {
	if (config.version) {
		//替换index.html的index.js的时间戳
		let htmlPath = releaseDir + "/index.html";
		let html;
		if (fs.existsSync(htmlPath)) {
			html = fs.readFileSync(htmlPath, "utf-8");
		}
		if (html) {
			html = html.replace(/\"index.js(.*)?\"/g, `\"index.js?v=${Date.now()}\"`);
			fs.writeFileSync(htmlPath, html, 'utf-8');
		}
		//替换index.js里面的js文件
		return gulp.src([releaseDir + "/version.json", releaseDir + "/index.js"])
			.pipe(revCollector())
			.pipe(gulp.dest(releaseDir));
	}
});

//起始任务，筛选4M包
gulp.task("default", ["version2"], function () {
	if (config.packFile) {
		return gulp.src(config.packFileFilter)
			.pipe(gulp.dest(releaseDir + "_pack"));
	}
});