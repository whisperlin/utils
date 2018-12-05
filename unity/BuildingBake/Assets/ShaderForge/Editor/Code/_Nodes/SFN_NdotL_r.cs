using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ShaderForge
{
	public class SFN_NDotL_r : SF_Node
	{
		//SFN_DiffWarp
		public SFN_NDotL_r()
		{

		}

		public override void Initialize()
		{
			base.Initialize("NdotL rich");

			base.showColor = true;
			base.shaderGenMode = ShaderGenerationMode.CustomFunction;
			UseLowerReadonlyValues(true);
	 
			connectors = new SF_NodeConnector[]{
				SF_NodeConnector.Create(this,"OUT","",ConType.cOutput,ValueType.VTv3,false)
			};
		}
			
		public override int GetEvaluatedComponentCount()
		{
			return 3;
		}

		static string GetDefaultString(string val, string defVal)
		{
			return string.IsNullOrEmpty(val) ? defVal : val;
		}

		public override string Evaluate(OutChannel channel = OutChannel.All)
		{
			return @" saturate(  dot(normalDirection ,  DirectionLightDir0 )  )  ";

		}
	}
}