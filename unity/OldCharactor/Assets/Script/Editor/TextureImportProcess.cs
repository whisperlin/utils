using UnityEngine;
using UnityEditor;
using System.IO;

public class TextureImportProcess : AssetPostprocessor {
    void OnPreprocessTexture() {
        var importer = assetImporter as TextureImporter;
        var path = importer.assetPath;

        var dir = Path.GetDirectoryName(path);
        var fileName = Path.GetFileName(path);
        var lsName = Path.Combine(dir, "." + fileName + ".ls");
        if (File.Exists(lsName)) {
            return;
        }

        importer.textureCompression = TextureImporterCompression.Compressed;
        importer.maxTextureSize = 2048;

        if (path.StartsWith("Assets/UI/Textures")) {
            importer.textureType = TextureImporterType.Sprite;
            importer.spriteImportMode = SpriteImportMode.Multiple;
            importer.mipmapEnabled = false;
            importer.spritePackingTag = "";

            TextureImporterPlatformSettings setting = new TextureImporterPlatformSettings();
            setting.textureCompression = TextureImporterCompression.CompressedHQ;
            setting.maxTextureSize = 2048;
            setting.name = "iPhone";
            setting.format = TextureImporterFormat.ASTC_RGBA_4x4;
            setting.overridden = true;

            importer.SetPlatformTextureSettings(setting);

            TextureImporterPlatformSettings pcSetting = new TextureImporterPlatformSettings();
            pcSetting.textureCompression = TextureImporterCompression.CompressedHQ;
            pcSetting.maxTextureSize = 2048;
            pcSetting.name = "Standalone";
            pcSetting.format = TextureImporterFormat.RGBA32;
            pcSetting.overridden = true;
            
            importer.SetPlatformTextureSettings(pcSetting);

        }
        else if (path.StartsWith("Assets/Plugins")) {
            importer.textureCompression = TextureImporterCompression.CompressedHQ;
        }
        //Assets\Props\Art\Fx\_Common\Texture
            //Assets\Props\Art\Fx\Character\Skill\Texture
            //Assets\Props\Art\Fx\Scene\_Common\Texture
        else {
            var name = path.Substring(0, path.LastIndexOf('.'));
            if (name.EndsWith("_full")) {// || name.EndsWith("_mapfull")
                TextureImporterPlatformSettings setting = new TextureImporterPlatformSettings();
                setting.textureCompression = TextureImporterCompression.CompressedHQ;
                setting.maxTextureSize = 2048;
                setting.name = "iPhone";
                setting.allowsAlphaSplitting = false;
                setting.overridden = true;
                setting.format = TextureImporterFormat.RGBA32;
                importer.SetPlatformTextureSettings(setting);
            }
            else {
                 {
                        TextureImporterPlatformSettings iphonePlamt = importer.GetPlatformTextureSettings("iPhone");
                        if (iphonePlamt != null && iphonePlamt.overridden == true &&
                            (iphonePlamt.format == TextureImporterFormat.PVRTC_RGB2
                            || iphonePlamt.format == TextureImporterFormat.PVRTC_RGBA2
                            || iphonePlamt.format == TextureImporterFormat.RGBA16
                            || iphonePlamt.format == TextureImporterFormat.RGBA32))
                        {
                            return;
                        }
                 } 

                TextureImporterPlatformSettings setting = new TextureImporterPlatformSettings();
                setting.textureCompression = TextureImporterCompression.CompressedHQ;
                setting.maxTextureSize = 2048;
                setting.name = "iPhone";
                setting.allowsAlphaSplitting = false;
                setting.overridden = true;
                setting.format = TextureImporterFormat.ASTC_RGBA_8x8; 
                importer.SetPlatformTextureSettings(setting);
            }
        }
    }
}

//public class TextureImportProcess : AssetPostprocessor {
//	void OnPreprocessTexture () {
//		var importer = assetImporter as TextureImporter;
//        var path = importer.assetPath;

//		importer.textureFormat = TextureImporterFormat.AutomaticCompressed;
//		importer.maxTextureSize = 2048;

//		if(path.StartsWith("Assets/UI/Textures")) {
//			importer.textureType = TextureImporterType.Sprite;
//			importer.spriteImportMode = SpriteImportMode.Multiple;
//			importer.mipmapEnabled = false;
//			importer.spritePackingTag = "";

//			importer.SetPlatformTextureSettings("iPhone", 2048, TextureImporterFormat.AutomaticTruecolor);
//		} else if(path.StartsWith("Assets/Plugins")) {
//			importer.textureFormat = TextureImporterFormat.AutomaticTruecolor;
//		} else {
//			var name = path.Substring(0, path.LastIndexOf('.'));

//			if(name.EndsWith("_full")) {
//				importer.textureFormat = TextureImporterFormat.AutomaticTruecolor;
//			} else {
//				importer.SetPlatformTextureSettings("iPhone", 2048, TextureImporterFormat.AutomaticCompressed, 100, false);
//			}
//		}
//	}
//}