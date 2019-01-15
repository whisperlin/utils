package laya.d3.core.material {
	import laya.d3.math.Vector4;
	import laya.d3.shader.ShaderDefines;
	import laya.webgl.resource.BaseTexture;
	
	/**
	 * ...
	 * @author
	 */
	public class SeaMaterial extends BaseMaterial{
		
		public static const SPLTEX:int = 0;
		public static const DESTEXTURE:int = 1;
		public static const NORMALTEXTURE:int = 2;
		public static const FOAMTEXTURE:int = 3;
		public static const REFLECTTEXTURE:int = 4;
		public static const WAVESTEXTURE:int = 5;
		public static const MASKWAVESTEXTURE:int = 6;
		public static const SPECNUM:int = 7;
		public static const REFINVNUM:int = 8;
		public static const REFFRENUM:int = 9;
		public static const FRESNELCOLOR:int = 10;
		public static const OBJSCALE:int = 11;
		public static const WATERCOLOR:int = 12;
		public static const DISPLACEMENTSPEED:int = 13;
		public static const DISPLACEMENTSCALE:int = 14;
		public static const DISPLACEMENTINTENSITY:int = 15;
		public static const NORMALINTENSITY:int = 16;
		public static const WAVESCALE:int = 17;
		public static const WAVESDISPLACEMENTSPEED:int = 18;
		public static const SCENEZ:int = 19;
		public static const PARTZ:int = 20;
		public static const DISPLACEMENTFOAMINTENSITY:int = 21;
		public static const SHOREFOAMINTENSITY:int = 22;
		public static const SHORELINEOPACITY:int = 23;
		public static const SAVEDISPLACEMENTFOAMINTENSITY:int = 24;
		public static const FADELEVEL:int = 25;
		public static const SHOREFOAMDISTANCE:int = 26;
		public static const REFLECTIONCOLOR:int = 27;
		public static const WAVESAMOUNT:int = 28;
		public static const WAVESSPEED:int = 29;
		public static const WAVESINTENSITY:int = 30;
		public static const RADIALWAVES:int = 31;
		public static const WATERDENSITY:int = 32;
		public static const SHOREWATEROPACITY:int = 33;
		public static const WAVEHEIGHT:int = 34;
		public static const VHEIGHT:int = 35;
		public static const FOAMSCALE:int = 36;
		public static const FOAMSPEED:int = 37;
		public static const SKYBLUR:int = 38;
		public static const REFLECTIONMAPCOLOR:int = 39;
		public static const GLOSS:int = 40;
		public static const MVDOFFSET:int = 41;
		public static const WTOFFSET:int = 42;
		public static const REFTEXOFFSET:int = 43;
		public static const FOAMTEXOFFSET:int = 44;
		public static const NORMALTEXOFFSET:int = 45;
		public static const DISPLACEOFFSET:int = 46;
		public static const SPLTEXOFFSET:int = 47;
		
		/** 默认材质，禁止修改*/
		public static const defaultMaterial:SeaMaterial = new SeaMaterial();
		
		/**@private */
		public static var shaderDefines:ShaderDefines = new ShaderDefines(BaseMaterial.shaderDefines);
		
		/**
		 * @private
		 */
		public static function __init__():void {
			 
		}
		//spltexOffset
		public function get spltex_ST():Vector4 {
			return _shaderValues.getVector(SeaMaterial.SPLTEXOFFSET) as Vector4;
		}
		public function set spltex_ST(value:Vector4):void {
			_shaderValues.setVector(SeaMaterial.SPLTEXOFFSET, value);
		}
		//displacementOffset
		public function get displacement_ST():Vector4 {
			return _shaderValues.getVector(SeaMaterial.DISPLACEOFFSET) as Vector4;
		}
		public function set displacement_ST(value:Vector4):void {
			_shaderValues.setVector(SeaMaterial.DISPLACEOFFSET, value);
		}
		//normalTextureOffset
		public function get normalTexture_ST():Vector4 {
			return _shaderValues.getVector(SeaMaterial.NORMALTEXOFFSET) as Vector4;
		}
		public function set normalTexture_ST(value:Vector4):void {
			_shaderValues.setVector(SeaMaterial.NORMALTEXOFFSET, value);
		}
		//foamTextureOffset
		public function get foamTexture_ST():Vector4 {
			return _shaderValues.getVector(SeaMaterial.FOAMTEXOFFSET) as Vector4;
		}
		public function set foamTexture_ST(value:Vector4):void {
			_shaderValues.setVector(SeaMaterial.FOAMTEXOFFSET, value);
		}
		//reflectionTexOffset
		public function get reflectionTex_ST():Vector4 {
			return _shaderValues.getVector(SeaMaterial.REFTEXOFFSET) as Vector4;
		}
		public function set reflectionTex_ST(value:Vector4):void {
			_shaderValues.setVector(SeaMaterial.REFTEXOFFSET, value);
		}
		//wavesTextureOffset
		public function get wavesTexture_ST():Vector4 {
			return _shaderValues.getVector(SeaMaterial.WTOFFSET) as Vector4;
		}
		public function set wavesTexture_ST(value:Vector4):void {
			_shaderValues.setVector(SeaMaterial.WTOFFSET, value);
		}
		//maskWavesDisplacementOffset
		public function get maskWavesDisplacement_ST():Vector4 {
			return _shaderValues.getVector(SeaMaterial.MVDOFFSET) as Vector4;
		}
		public function set maskWavesDisplacement_ST(value:Vector4):void {
			_shaderValues.setVector(SeaMaterial.MVDOFFSET, value);
		}

		public function get gloss():Number {
			return _shaderValues.getNumber(SeaMaterial.GLOSS) as Number;
		}
		public function set gloss(value:Number):void {
			_shaderValues.setNumber(SeaMaterial.GLOSS, value);
		}

		
		public function get reflectionmapColor():Vector4 {
			return _shaderValues.getVector(SeaMaterial.REFLECTIONMAPCOLOR) as Vector4;
		}
		public function set reflectionmapColor(value:Vector4):void {
			_shaderValues.setVector(SeaMaterial.REFLECTIONMAPCOLOR, value);
		}

		public function get skyBlur():Number {
			return _shaderValues.getNumber(SeaMaterial.SKYBLUR) as Number;
		}
		public function set skyBlur(value:Number):void {
			_shaderValues.setNumber(SeaMaterial.SKYBLUR, value);
		}

		public function get foamSpeed():Number {
			return _shaderValues.getNumber(SeaMaterial.FOAMSPEED) as Number;
		}
		public function set foamSpeed(value:Number):void {
			_shaderValues.setNumber(SeaMaterial.FOAMSPEED, value);
		}

		public function get foamScale():Number {
			return _shaderValues.getNumber(SeaMaterial.FOAMSCALE) as Number;
		}
		public function set foamScale(value:Number):void {
			_shaderValues.setNumber(SeaMaterial.FOAMSCALE, value);
		}
		public function get vHeight():Number {
			return _shaderValues.getNumber(SeaMaterial.VHEIGHT) as Number;
		}
		public function set vHeight(value:Number):void {
			_shaderValues.setNumber(SeaMaterial.VHEIGHT, value);
		}

		public function get waveHeight():Number {
			return _shaderValues.getNumber(SeaMaterial.WAVEHEIGHT) as Number;
		}
		public function set waveHeight(value:Number):void {
			_shaderValues.setNumber(SeaMaterial.WAVEHEIGHT, value);
		}
		public function get shoreWaterOpacity():Number {
			return _shaderValues.getNumber(SeaMaterial.SHOREWATEROPACITY) as Number;
		}
		public function set shoreWaterOpacity(value:Number):void {
			_shaderValues.setNumber(SeaMaterial.SHOREWATEROPACITY, value);
		}

		public function get waterDensity():Number {
			return _shaderValues.getNumber(SeaMaterial.WATERDENSITY) as Number;
		}
		public function set waterDensity(value:Number):void {
			_shaderValues.setNumber(SeaMaterial.WATERDENSITY, value);
		}

		public function get radialWaves():Number {
			return _shaderValues.getNumber(SeaMaterial.RADIALWAVES) as Number;
		}
		public function set radialWaves(value:Number):void {
			_shaderValues.setNumber(SeaMaterial.RADIALWAVES, value);
		}

		public function get wavesIntensity():Number {
			return _shaderValues.getNumber(SeaMaterial.WAVESINTENSITY) as Number;
		}
		public function set wavesIntensity(value:Number):void {
			_shaderValues.setNumber(SeaMaterial.WAVESINTENSITY, value);
		}
		public function get wavesSpeed():Number {
			return _shaderValues.getNumber(SeaMaterial.WAVESSPEED) as Number;
		}
		public function set wavesSpeed(value:Number):void {
			_shaderValues.setNumber(SeaMaterial.WAVESSPEED, value);
		}

		public function get wavesAmount():Number {
			return _shaderValues.getNumber(SeaMaterial.WAVESAMOUNT) as Number;
		}
		public function set wavesAmount(value:Number):void {
			_shaderValues.setNumber(SeaMaterial.WAVESAMOUNT, value);
		}

		public function get reflectionColor():Vector4 {
			return _shaderValues.getVector(SeaMaterial.REFLECTIONCOLOR) as Vector4;
		}
		public function set reflectionColor(value:Vector4):void {
			_shaderValues.setVector(SeaMaterial.REFLECTIONCOLOR, value);
		}

		public function get shoreFoamDistance():Number {
			return _shaderValues.getNumber(SeaMaterial.SHOREFOAMDISTANCE);
		}
		public function set shoreFoamDistance(value:Number):void {
			_shaderValues.setNumber(SeaMaterial.SHOREFOAMDISTANCE, value);
		}

		public function get fadeLevel():Number {
			return _shaderValues.getNumber(SeaMaterial.FADELEVEL);
		}
		public function set fadeLevel(value:Number):void {
			_shaderValues.setNumber(SeaMaterial.FADELEVEL, value);
		}

		public function get savesDisplacementFoamIntensity():Number {
			return _shaderValues.getNumber(SeaMaterial.SAVEDISPLACEMENTFOAMINTENSITY);
		}
		public function set savesDisplacementFoamIntensity(value:Number):void {
			_shaderValues.setNumber(SeaMaterial.SAVEDISPLACEMENTFOAMINTENSITY, value);
		}

		public function get shoreLineOpacity():Number {
			return _shaderValues.getNumber(SeaMaterial.SHORELINEOPACITY);
		}
		public function set shoreLineOpacity(value:Number):void {
			_shaderValues.setNumber(SeaMaterial.SHORELINEOPACITY, value);
		}

		public function get shoreFoamIntensity():Number {
			return _shaderValues.getNumber(SeaMaterial.SHOREFOAMINTENSITY);
		}
		public function set shoreFoamIntensity(value:Number):void {
			_shaderValues.setNumber(SeaMaterial.SHOREFOAMINTENSITY, value);
		}

		public function get displacementFoamIntensity():Number {
			return _shaderValues.getNumber(SeaMaterial.DISPLACEMENTFOAMINTENSITY);
		}
		public function set displacementFoamIntensity(value:Number):void {
			_shaderValues.setNumber(SeaMaterial.DISPLACEMENTFOAMINTENSITY, value);
		}

		public function get partZ():Number {
			return _shaderValues.getNumber(SeaMaterial.PARTZ);
		}
		public function set partZ(value:Number):void {
			_shaderValues.setNumber(SeaMaterial.PARTZ, value);
		}
		public function get sceneZ():Number {
			return _shaderValues.getNumber(SeaMaterial.SCENEZ);
		}
		public function set sceneZ(value:Number):void {
			_shaderValues.setNumber(SeaMaterial.SCENEZ, value);
		}

		public function get wavesDisplacementSpeed():Number {
			return _shaderValues.getNumber(SeaMaterial.WAVESDISPLACEMENTSPEED);
		}
		public function set wavesDisplacementSpeed(value:Number):void {
			_shaderValues.setNumber(SeaMaterial.WAVESDISPLACEMENTSPEED, value);
		}

		public function get wavesScale():Number {
			return _shaderValues.getNumber(SeaMaterial.WAVESCALE);
		}
		public function set wavesScale(value:Number):void {
			_shaderValues.setNumber(SeaMaterial.WAVESCALE, value);
		}

		public function get normalIntensity():Number {
			return _shaderValues.getNumber(SeaMaterial.NORMALINTENSITY);
		}
		
		public function set normalIntensity(value:Number):void {
			_shaderValues.setNumber(SeaMaterial.NORMALINTENSITY, value);
		}

		public function get displacementIntensity():Number {
			return _shaderValues.getNumber(SeaMaterial.DISPLACEMENTINTENSITY);
		}
		
		public function set displacementIntensity(value:Number):void {
			_shaderValues.setNumber(SeaMaterial.DISPLACEMENTINTENSITY, value);
		}

		public function get displacementScale():Number {
			return _shaderValues.getNumber(SeaMaterial.DISPLACEMENTSCALE);
		}
		
		public function set displacementScale(value:Number):void {
			_shaderValues.setNumber(SeaMaterial.DISPLACEMENTSCALE, value);
		}
		public function get displacementSpeed():Number {
			return _shaderValues.getNumber(SeaMaterial.DISPLACEMENTSPEED);
		}
		
		public function set displacementSpeed(value:Number):void {
			_shaderValues.setNumber(SeaMaterial.DISPLACEMENTSPEED, value);
		}
		public function get waterColor():Vector4 {
			return _shaderValues.getVector(SeaMaterial.WATERCOLOR) as Vector4;
		}
		public function set waterColor(value:Vector4):void {
			_shaderValues.setVector(SeaMaterial.WATERCOLOR, value);
		}
		public function get objScale():Vector4 {
			return _shaderValues.getVector(SeaMaterial.OBJSCALE) as Vector4;
		}
		public function set objScale(value:Vector4):void {
			_shaderValues.setVector(SeaMaterial.OBJSCALE, value);
		}

		public function get fresnelColor():Vector4 {
			return _shaderValues.getVector(SeaMaterial.FRESNELCOLOR) as Vector4;
		}
		public function set fresnelColor(value:Vector4):void {
			_shaderValues.setVector(SeaMaterial.FRESNELCOLOR, value);
		}

		public function get reflectionFresnel():Number {
			return _shaderValues.getNumber(SeaMaterial.REFFRENUM);
		}
		
		public function set reflectionFresnel(value:Number):void {
			_shaderValues.setNumber(SeaMaterial.REFFRENUM, value);
		}

		public function get reflectionIntensity():Number {
			return _shaderValues.getNumber(SeaMaterial.REFINVNUM);
		}
		
		public function set reflectionIntensity(value:Number):void {
			_shaderValues.setNumber(SeaMaterial.REFINVNUM, value);
		}

		public function get spec():Number {
			return _shaderValues.getNumber(SeaMaterial.SPECNUM);
		}
		
		public function set spec(value:Number):void {
			_shaderValues.setNumber(SeaMaterial.SPECNUM, value);
		}
		
		public function get maskWavesDisplacement():BaseTexture {
			return _shaderValues.getTexture(SeaMaterial.MASKWAVESTEXTURE) as BaseTexture;
		}
		
		public function set maskWavesDisplacement(value:BaseTexture):void {
			_shaderValues.setTexture(SeaMaterial.MASKWAVESTEXTURE, value);
		}

		public function get wavesTexture():BaseTexture {
			return _shaderValues.getTexture(SeaMaterial.WAVESTEXTURE) as BaseTexture;
		}
		
		public function set wavesTexture(value:BaseTexture):void {
			_shaderValues.setTexture(SeaMaterial.WAVESTEXTURE, value);
		}

	 	public function get reflectionTex():BaseTexture {
			return _shaderValues.getTexture(SeaMaterial.REFLECTTEXTURE) as BaseTexture;
		}
		
		public function set reflectionTex(value:BaseTexture):void {
			_shaderValues.setTexture(SeaMaterial.REFLECTTEXTURE, value);
		}

		public function get foamTexture():BaseTexture {
			return _shaderValues.getTexture(SeaMaterial.FOAMTEXTURE) as BaseTexture;
		}
		
	 
		public function set foamTexture(value:BaseTexture):void {
			_shaderValues.setTexture(SeaMaterial.FOAMTEXTURE, value);
		}


		public function get displacement():BaseTexture {
			return _shaderValues.getTexture(SeaMaterial.DESTEXTURE) as BaseTexture;
		}
		
	 
		public function set displacement(value:BaseTexture):void {
			_shaderValues.setTexture(SeaMaterial.DESTEXTURE, value);
		}
		

		public function get spltex():BaseTexture {
			return _shaderValues.getTexture( SeaMaterial.SPLTEX );
		}
		

		public function set spltex(value:BaseTexture):void {
 
			_shaderValues.setTexture(SeaMaterial.SPLTEX , value);
		}
		

		public function get normalTexture():BaseTexture {
			return _shaderValues.getTexture(SeaMaterial.NORMALTEXTURE);
		}
		
		public function set normalTexture(value:BaseTexture):void {
			_shaderValues.setTexture(SeaMaterial.NORMALTEXTURE, value);
		}
		


		 
		
		public function SeaMaterial() {
			
			super(48);
			setShaderName("Sea");
			 
		}
	}

}