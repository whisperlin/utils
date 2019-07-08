using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.IO;

public class AnimaImportProcess : AssetPostprocessor {

    static int nSaveIndex = 0;

    public static void OnPostprocessAllAssets(string[] importedAsset, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths) {
        if (nSaveIndex > 0) {
            --nSaveIndex;
            return;
        }

        bool bSave = false;
        foreach (string str in importedAsset) {
            bSave = bSave || ProcessAnima(str);
        }

        if (bSave) {
            nSaveIndex = 2; //AssetDatabase.SaveAssets 会导致2次的 OnPostprocessAllAssets 回调
            AssetDatabase.SaveAssets();
        }
    }

    private static bool ProcessAnima(string path) {
        if (Path.GetExtension(path).ToLower() == ".anim") {
            EditorTool.AnimationOpt animaOpt = EditorTool.OptimizeAnimationClipTool.GetNewAOpt(path);
            animaOpt.OptimizeSize();
            return true;
        }

        return false;
    }
}
