using UnityEngine;
using System.Collections;
using UnityEditor;

public class DemoRunnerBuild {
	[MenuItem("Easy/Scene/Build and Run")]
	public static void Build() {
		string apk = "AndroidBuild.apk";

		// build
		BuildPipeline.BuildPlayer(new string[0], apk, BuildTarget.Android, BuildOptions.None);

		// install
		ExeCmd("adb install -r " + apk);

		// run
		ExeCmd("adb shell monkey -p " + PlayerSettings.bundleIdentifier + " -c android.intent.category.LAUNCHER 1");
	}

	static void ExeCmd(string cmd) {
		System.Diagnostics.ProcessStartInfo info = new System.Diagnostics.ProcessStartInfo();
		info.FileName = "cmd.exe";
		info.Arguments = "/c " + cmd;
		info.WorkingDirectory = Application.dataPath + "/../";
		System.Diagnostics.Process p = System.Diagnostics.Process.Start(info);
		p.WaitForExit();
	}
}
