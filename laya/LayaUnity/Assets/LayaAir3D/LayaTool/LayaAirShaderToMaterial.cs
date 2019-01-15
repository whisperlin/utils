using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
public class LayaAirShaderToMaterial  {

	public class ParamData
	{
		public string name;
		public ShaderUtil.ShaderPropertyType type;
		public ParamData(string n, ShaderUtil.ShaderPropertyType t)
		{
			name = n;
			type = t;
		}
	}
	[MenuItem("LayaAir3D/Shader To Mat")]
	static void shaderToMat () {
 
		if (Selection.activeObject == null)
			return;
		Shader shader = Selection.activeObject  as Shader;
		if (shader == null)
			return;
		Debug.Log (shader.name);
		string[] names = shader.name.Split (new char[]{ '/' });
		if (names.Length != 3)
			return;
		if (names [0] != "LayaAir3D")
			return;
		if (names [1] != "Addition")
			return;
	 
		string path = EditorUtility.SaveFolderPanel("保存文件夹","","");
		if (path.Length != 0) 
		{
			buildMat (shader,names[2],path);
		 
		}
	}
	static string txt;
	 
	static void buildMat (Shader shader,string name,string path)
	{
		int propertyCount = ShaderUtil.GetPropertyCount (shader);
		string matName = name+"Material";
		txt = "";
	 

		appln ("package laya.d3.core.material {");

		appln ("\timport laya.d3.math.Vector4;");
		appln ("\timport laya.d3.shader.ShaderDefines;");
		appln ("\timport laya.webgl.resource.BaseTexture;");

		app ("\tpublic class ");
		app (matName);
		appln (" extends BaseMaterial{");

		List<ParamData> _params = new List<ParamData> ();
		for (int i = 0; i < propertyCount; i++) {
			string pname = ShaderUtil.GetPropertyName (shader, i);
			var ptype = ShaderUtil.GetPropertyType (shader,i);
			switch (ptype) {
			case ShaderUtil.ShaderPropertyType.Color:
				{
					_params.Add(new ParamData(pname,ShaderUtil.ShaderPropertyType.Vector));
				}
				break;
			case ShaderUtil.ShaderPropertyType.Float:
				{
					_params.Add(new ParamData(pname,ptype));
				}
				break;
			case ShaderUtil.ShaderPropertyType.Range:
				{
					_params.Add(new ParamData(pname,ShaderUtil.ShaderPropertyType.Float));
				}
				break;
			case ShaderUtil.ShaderPropertyType.TexEnv:
				{
					_params.Add(new ParamData(pname,ptype));
					_params.Add(new ParamData(pname+"_ST",ShaderUtil.ShaderPropertyType.Vector));

				}
				break;
			case ShaderUtil.ShaderPropertyType.Vector:
				{
					_params.Add(new ParamData(pname,ptype));
				}
				break;
			}	
		}
		propertyCount = _params.Count;
		for (int i = 0; i < propertyCount; i++) {
			ParamData p = _params[i];
			appln2 ("public static const "+p.name.ToUpper()+":int = "+i+";");
		}

		appln ("");
		appln ("");
		appln ("");

		appln2 ("public static const defaultMaterial:"+matName+" = new "+matName+"();");
 
		appln2 ("public static var shaderDefines:ShaderDefines = new ShaderDefines(BaseMaterial.shaderDefines);");

		appln2 ("public static function __init__():void {");
		appln2 ("}");

		appln ("");
		appln ("");
		appln ("");

		for (int i = 0; i < propertyCount; i++) {
			ParamData p = _params[i];
			var ptype = p.type;
			switch (ptype) {
	 
			case ShaderUtil.ShaderPropertyType.Float:
				{

					appln2 ("public function get "+p.name+"():Number {");
					appln3 ("return _shaderValues.getNumber("+matName+"."+p.name.ToUpper()+") as Number;");
					appln2 ("}");
					appln ("");
					appln2 ("public function set "+p.name+"(value:Number):void {");
					appln3 ("_shaderValues.setNumber("+matName+"."+p.name.ToUpper()+" value);");
					appln2 ("}");
					appln ("");
				}
				break;
		 
			case ShaderUtil.ShaderPropertyType.TexEnv:
				{
					appln2 ("public function get "+p.name+"():BaseTexture {");
					appln3 ("return _shaderValues.getTexture("+matName+"."+p.name.ToUpper()+") as BaseTexture;");
					appln2 ("}");
					appln ("");
					appln2 ("public function set "+p.name+"(value:BaseTexture):void {");
					appln3 ("_shaderValues.setTexture("+matName+"."+p.name.ToUpper()+", value);");
					appln2 ("}");
					appln ("");
				}
				break;
			case ShaderUtil.ShaderPropertyType.Vector:
				{
					appln2 ("public function get "+p.name+"():Vector4 {");
					appln3 ("return _shaderValues.getVector("+matName+"."+p.name.ToUpper()+") as Vector4;");
					appln2 ("}");
					appln ("");
					appln2 ("public function set "+p.name+"(value:Vector4):void {");
					appln3 ("_shaderValues.setVector("+matName+"."+p.name.ToUpper()+", value);");
					appln2 ("}");
					appln ("");

				}
				break;
			}	
		}


		appln2 ("public function "+matName+"() {");
 
		//propertyCount
		//super(48);
		appln3 ("super("+propertyCount+");");
		appln3 ("setShaderName(\""+name+"\");");
		appln2 ("}");

		 

		 
		appln ("\t}");
		appln ("}");

		appln ("/*");
		appln4 ("attributeMap = {");
		appln5 ("'_glesVertex': VertexMesh.MESH_POSITION0, ");
		appln5 ("'_glesNormal': VertexMesh.MESH_NORMAL0,");
		appln5 ("'_glesMultiTexCoord0': VertexMesh.MESH_TEXTURECOORDINATE0,");
		appln5 ("'_glesTANGENT': VertexMesh.MESH_TANGENT0");
		appln4 ("}");

		appln4 ("uniformMap = {");

		for (int i = 0; i < propertyCount; i++) {
			ParamData p = _params [i];
			appln5 ("'"+p.name+"' : ["+matName+"."+p.name.ToUpper()+", Shader3D.PERIOD_MATERIAL],");
		}
		appln5 ("");
		appln5 ("");

		appln5 ("'unity_MatrixVP': [Sprite3D.VPMATRIX, Shader3D.PERIOD_SPRITE], ");
		appln5 ("'unity_ObjectToWorld': [Sprite3D.WORLDMATRIX, Shader3D.PERIOD_SPRITE],");
		appln5 ("'unity_WorldToObject': [Sprite3D.LOCALMATRIX, Shader3D.PERIOD_SPRITE],");
		appln5 ("'_WorldSpaceCameraPos': [BaseCamera.CAMERAPOS, Shader3D.PERIOD_CAMERA],");
		appln5 ("'_Time': [Scene3D.UNITY_TIME, Shader3D.PERIOD_SCENE] ");
		appln4 ("};");

		appln4 ("vs = __INCLUDESTR__(\"files/"+name+".vs\");");
		appln4 ("ps = __INCLUDESTR__(\"files/"+name+".ps\");");
		appln4 ("shader = Shader3D.add(\""+name+"\", attributeMap, uniformMap, null, "+matName+".shaderDefines);");
		appln4 ("shader.addShaderPass(vs, ps);");

		appln ("*/");
		System.IO.File.WriteAllText (path+"/"+matName+".as",txt);
	}
	static void app(string t)
	{
		txt += t;	
	}
	static void app2(string t)
	{
		txt += "\t\t"+t ;	
	}
	static void app3(string t)
	{
		txt += "\t\t\t"+t ;	
	}
	static void appln(string t)
	{
		txt += t + "\n";	
	}
	static void appln2(string t)
	{
		txt += "\t\t"+t + "\n";	
	}
	static void appln3(string t)
	{
		txt += "\t\t\t"+t + "\n";	
	}
	static void appln4(string t)
	{
		txt += "\t\t\t\t"+t + "\n";	
	}

	static void appln5(string t)
	{
		txt += "\t\t\t\t\t"+t + "\n";	
	}




	 
}
