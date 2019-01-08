//是否使用IDE自带的node环境和插件，设置false后，则使用自己环境(使用命令行方式执行)
let useIDENode = process.argv[0].indexOf("LayaAir") > -1 ? true : false;
//获取Node插件和工作路径
let ideModuleDir = useIDENode ? process.argv[1].replace("gulp\\bin\\gulp.js", "").replace("gulp/bin/gulp.js", "") : "";
let workSpaceDir = useIDENode ? process.argv[2].replace("--gulpfile=", "").replace("\\.laya\\gulpfile.js", "").replace("/.laya/gulpfile.js", "") + "/" : "./../";

//引用插件模块
let gulp = require(ideModuleDir + "gulp");
let browserify = require(ideModuleDir + "browserify");
let source = require(ideModuleDir + "vinyl-source-stream");
let cp = require("child_process");

//编译as为js，并输出为.laya/temp.js
gulp.task('compile', function (cb) {
	cp.exec(`"${workSpaceDir}.laya/layajs" "${workSpaceDir}asconfig.json;iflash=false;chromerun=false;quickcompile=true;out=.laya/temp.js;subpath="`,
		function (error, stdout, stderr) {
			// console.log(`\n[Info]\n${stdout}`);
			if (error !== null) {
				throw `${error}`;
			} else {
				if (stderr) console.log(`\n[Warning]\n${stderr}`);
				cb();
			}
		}
	)
});

//使用browserify，分析require引用，合并文件，如果不使用require，本步骤可不要
gulp.task('default', ["compile"], function () {
	return browserify({
		basedir: workSpaceDir,
		debug: false,
		entries: ['.laya/temp.js'],
		cache: {},
		packageCache: {}
	})
		.bundle()
		//使用source把输出文件命名为bundle.js
		.pipe(source('bundle.js'))
		//把bundle.js复制到bin/js目录
		.pipe(gulp.dest(workSpaceDir + "/bin/js"));
});