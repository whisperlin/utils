using System;
using UnityEditor;
using UnityEngine;

// Token: 0x02000005 RID: 5
[CustomEditor(typeof(Tutorial))]
public class Tutorial : EditorWindow
{
	// Token: 0x06000021 RID: 33 RVA: 0x00004244 File Offset: 0x00002444
	[MenuItem("LayaAir3D/Help/Tutorial", false)]
	private static void initTutorial()
	{
		Tutorial.tutorial = (Tutorial)EditorWindow.GetWindow(typeof(Tutorial));
		WWW www = new WWW("file://" + Application.dataPath + "/LayaAir3D/LayaTool/layabox.png");
		GUIContent titleContent = new GUIContent("LayaAir3D", www.texture);
		Tutorial.tutorial.titleContent = titleContent;
		WWW www2 = new WWW("file://" + Application.dataPath + "/LayaAir3D/LayaTool/Open.png");
		WWW www3 = new WWW("file://" + Application.dataPath + "/LayaAir3D/LayaTool/Close.png");
		Tutorial.OpenTexture = www2.texture;
		Tutorial.CloseTexture = www3.texture;
		Color textColor = EditorStyles.label.normal.textColor;
		Tutorial.titleStyle.fontSize = 15;
		Tutorial.titleStyle.fontStyle =(UnityEngine.FontStyle) 1;
		Tutorial.titleStyle.normal.textColor = textColor;
		Tutorial.contenDescStyle.fontSize = 12;
		Tutorial.contenDescStyle.normal.textColor = textColor;
		Tutorial.contentHeaderStyle.fontSize = 16;
		Tutorial.contentHeaderStyle.fontStyle = (UnityEngine.FontStyle)1;
		Tutorial.contentHeaderStyle.normal.textColor = textColor;
		Tutorial.contentStyle.fontSize = 12;
		Tutorial.contentStyle.normal.textColor = textColor;
		Tutorial.NodeButton = Tutorial.OpenTexture;
		Tutorial.TextureButton = Tutorial.OpenTexture;
		Tutorial.MaterialButton = Tutorial.OpenTexture;
		Tutorial.AnimatorButton = Tutorial.OpenTexture;
		Tutorial.PhysicleButton = Tutorial.OpenTexture;
		Tutorial.AlbodeLight = Tutorial.OpenTexture;
	}

	// Token: 0x06000022 RID: 34 RVA: 0x000043BC File Offset: 0x000025BC
	private void OnGUI()
	{
		Tutorial.ScrollPosition = GUILayout.BeginScrollView(Tutorial.ScrollPosition, new GUILayoutOption[0]);
		GUILayout.Label("", new GUILayoutOption[]
		{
			GUILayout.Height(20f)
		});
		GUILayout.Label("                            推荐使用Unity5.6版本", Tutorial.contentHeaderStyle, new GUILayoutOption[]
		{
			GUILayout.ExpandWidth(true)
		});
		GUILayout.Label("", new GUILayoutOption[]
		{
			GUILayout.Height(20f)
		});
		this.DrawButton(ref Tutorial.NodeButton, "节点");
		if (Tutorial.NodeButton == Tutorial.OpenTexture)
		{
			this.DrawContentDesc(Tutorial.first, "(1)相机");
			this.DrawContentDesc(Tutorial.second, "兼容Camera组件");
			this.DrawContentDesc(Tutorial.first, "(2)光照");
			this.DrawContentDesc(Tutorial.second, "兼容DirectionLight组件、PointLight组件、SpotLight组件,目前Mode为RealTime时仅支持三种类型灯光各一盏");
			this.DrawContent(Tutorial.third, "\u00a0Type(Directional、Point、Spot)");
			this.DrawContent(Tutorial.third, "\u00a0Color");
			this.DrawContent(Tutorial.third, " Mode(RealTime、Baked)   注：暂不支持Mixed,烘焙光照贴图需要选择Baked");
			this.DrawContent(Tutorial.third, "\u00a0Intensity");
			this.DrawContentDesc(Tutorial.first, "(3)模型");
			this.DrawContentDesc(Tutorial.second, "兼容MeshRender和MeshFilter组件");
			this.DrawContentDesc(Tutorial.first, "(4)粒子");
			this.DrawContentDesc(Tutorial.second, "兼容ParticleSystem组件部分模块");
			this.DrawContent(Tutorial.third, "\u00a0Emission");
			this.DrawContent(Tutorial.fourth, "\u00a0Rate over Time(Constant)");
			this.DrawContent(Tutorial.fourth, "\u00a0Bursts");
			this.DrawContent(Tutorial.third, " Shape(SphereShape、HemisphereShape、BoxShape、CircleShape、ConeShape)");
			this.DrawContent(Tutorial.third, "\u00a0Velocity over Lifetime");
			this.DrawContent(Tutorial.third, "\u00a0Color over Lifetime");
			this.DrawContent(Tutorial.third, " Size over Lifetime");
			this.DrawContent(Tutorial.third, "\u00a0Texture Sheet Animation");
			this.DrawContent(Tutorial.third, " Rotation Over LifeTime");
			this.DrawContentDesc(Tutorial.first, "(5)拖尾");
			this.DrawContentDesc(Tutorial.first + Tutorial.tabOffset, "兼容TrailRender组件");
			this.DrawContentDesc(Tutorial.first, "(6)地形");
			this.DrawContentDesc(Tutorial.first + Tutorial.tabOffset, "兼容Terrain组件 仅支持地表 不支持植被  导出时会转换为静态Mesh网格");
		}
		GUILayout.Label("", new GUILayoutOption[]
		{
			GUILayout.Height(5f)
		});
		this.DrawButton(ref Tutorial.TextureButton, "纹理");
		if (Tutorial.TextureButton == Tutorial.OpenTexture)
		{
			this.DrawContentDesc(Tutorial.first, "兼容纹理属性面板中的部分属性");
			this.DrawContent(Tutorial.second, " Mip Map");
			this.DrawContent(Tutorial.second, " Wrap Mode");
			this.DrawContent(Tutorial.second, " Filter Mode");
			this.DrawContent(Tutorial.second, " Aniso Level");
		}
		GUILayout.Label("", new GUILayoutOption[]
		{
			GUILayout.Height(5f)
		});
		this.DrawButton(ref Tutorial.MaterialButton, "材质");
		if (Tutorial.MaterialButton == Tutorial.OpenTexture)
		{
			this.DrawContentDesc(Tutorial.first, "支持Shader列表中LayaAir3D目录中的所有Shader");
			this.DrawContentDesc(Tutorial.first, "如使用非LayaAir3D Shader会强制转换为LayaAir3D的默认Shader");
		}
		GUILayout.Label("", new GUILayoutOption[]
		{
			GUILayout.Height(5f)
		});
		this.DrawButton(ref Tutorial.AnimatorButton, "动画");
		if (Tutorial.AnimatorButton == Tutorial.OpenTexture)
		{
			this.DrawContentDesc(Tutorial.first, "兼容Animator组件和关联的AnimatorState");
		}
		GUILayout.Label("", new GUILayoutOption[]
		{
			GUILayout.Height(5f)
		});
		this.DrawButton(ref Tutorial.PhysicleButton, "物理");
		if (Tutorial.PhysicleButton == Tutorial.OpenTexture)
		{
			this.DrawContentDesc(Tutorial.first, "兼容BoxCollider、SphereCollider、CapsuleCollider、MeshCollider组件");
			this.DrawContentDesc(Tutorial.first, "兼容Rigidbody组件");
		}
		GUILayout.Label("", new GUILayoutOption[]
		{
			GUILayout.Height(5f)
		});
		this.DrawButton(ref Tutorial.AlbodeLight, "光照配置");
		if (Tutorial.AlbodeLight == Tutorial.OpenTexture)
		{
			this.DrawContentDesc(Tutorial.first, "兼容Lighting面板中的部分属性,不支持Auto Generate,需要手动点击Generate Lighting");
			this.DrawContentDesc(Tutorial.second, "Scene:");
			this.DrawContent(Tutorial.third, " Lighting->");
			this.DrawContent(Tutorial.fourth, " Environment->");
			this.DrawContent(Tutorial.fifth, " Skybox Material(material)    注：material必须为Skybox/Cubemap材质");
			this.DrawContent(Tutorial.fourth, " Environment Lighting->");
			this.DrawContent(Tutorial.fifth, " Source(Skybox、Color)");
			this.DrawContent(Tutorial.fifth, " Ambient Color");
			this.DrawContent(Tutorial.fifth, " Ambient Mode(Realtime)");
			this.DrawContent(Tutorial.fourth, " Lightmapping Settings->");
			this.DrawContentDesc(Tutorial.fourth, "    全部支持,但不包含 Directional Mode(Directional)    注：烘焙光照贴图必须使用Non-Directional");
			this.DrawContentDesc(Tutorial.second, "Global maps:");
			this.DrawContentDesc(Tutorial.second, "可导出,效果和PC、Mac & Linux Standalone保持一致");
		}
		GUILayout.EndScrollView();
	}

	// Token: 0x06000023 RID: 35 RVA: 0x000048A4 File Offset: 0x00002AA4
	private void DrawButton(ref Texture ButtonIcon, string Labelname)
	{
		GUILayout.BeginHorizontal(new GUILayoutOption[0]);
		GUILayout.Label("", new GUILayoutOption[]
		{
			GUILayout.Width(15f)
		});
		if (GUILayout.Button(ButtonIcon, Tutorial.titleStyle, new GUILayoutOption[]
		{
			GUILayout.Width(15f),
			GUILayout.Height(15f)
		}))
		{
			if (ButtonIcon == Tutorial.OpenTexture)
			{
				ButtonIcon = Tutorial.CloseTexture;
			}
			else
			{
				ButtonIcon = Tutorial.OpenTexture;
			}
		}
		GUILayout.Label(Labelname, Tutorial.titleStyle, new GUILayoutOption[]
		{
			GUILayout.ExpandWidth(true)
		});
		GUILayout.EndHorizontal();
	}

	// Token: 0x06000024 RID: 36 RVA: 0x00004944 File Offset: 0x00002B44
	private void DrawContentDesc(int Start, string LabelString)
	{
		GUILayout.BeginHorizontal(new GUILayoutOption[0]);
		GUILayout.Label("", new GUILayoutOption[]
		{
			GUILayout.Width((float)Start)
		});
		GUILayout.Label(LabelString, Tutorial.contenDescStyle, new GUILayoutOption[]
		{
			GUILayout.ExpandWidth(true)
		});
		GUILayout.EndHorizontal();
	}

	// Token: 0x06000025 RID: 37 RVA: 0x00004998 File Offset: 0x00002B98
	private void DrawContent(int Start, string LabelString)
	{
		GUILayout.BeginHorizontal(new GUILayoutOption[0]);
		GUILayout.Label("", new GUILayoutOption[]
		{
			GUILayout.Width((float)Start)
		});
		GUILayout.Label(" · ", Tutorial.contentHeaderStyle, new GUILayoutOption[]
		{
			GUILayout.Width(10f)
		});
		GUILayout.Label(LabelString, Tutorial.contentStyle, new GUILayoutOption[]
		{
			GUILayout.ExpandWidth(true)
		});
		GUILayout.EndHorizontal();
	}

	// Token: 0x04000038 RID: 56
	private static Vector2 ScrollPosition;

	// Token: 0x04000039 RID: 57
	private static Tutorial tutorial;

	// Token: 0x0400003A RID: 58
	private static Texture OpenTexture;

	// Token: 0x0400003B RID: 59
	private static Texture CloseTexture;

	// Token: 0x0400003C RID: 60
	private static int tabOffset = 18;

	// Token: 0x0400003D RID: 61
	private static int first = 30;

	// Token: 0x0400003E RID: 62
	private static int second = Tutorial.first + Tutorial.tabOffset;

	// Token: 0x0400003F RID: 63
	private static int third = Tutorial.first + Tutorial.tabOffset * 2;

	// Token: 0x04000040 RID: 64
	private static int fourth = Tutorial.first + Tutorial.tabOffset * 3;

	// Token: 0x04000041 RID: 65
	private static int fifth = Tutorial.first + Tutorial.tabOffset * 4;

	// Token: 0x04000042 RID: 66
	private static GUIStyle titleStyle = new GUIStyle();

	// Token: 0x04000043 RID: 67
	private static GUIStyle contenDescStyle = new GUIStyle();

	// Token: 0x04000044 RID: 68
	private static GUIStyle contentHeaderStyle = new GUIStyle();

	// Token: 0x04000045 RID: 69
	private static GUIStyle contentStyle = new GUIStyle();

	// Token: 0x04000046 RID: 70
	private static Texture NodeButton;

	// Token: 0x04000047 RID: 71
	private static Texture TextureButton;

	// Token: 0x04000048 RID: 72
	private static Texture MaterialButton;

	// Token: 0x04000049 RID: 73
	private static Texture AnimatorButton;

	// Token: 0x0400004A RID: 74
	private static Texture PhysicleButton;

	// Token: 0x0400004B RID: 75
	private static Texture AlbodeLight;
}
