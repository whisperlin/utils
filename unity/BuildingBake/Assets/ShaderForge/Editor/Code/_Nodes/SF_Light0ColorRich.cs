using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ShaderForge
{
	public class SF_Light0ColorRich : SF_Node
	{
		public SF_Light0ColorRich()
		{

		}

		public override void Initialize()
		{
			base.Initialize(" LightColor0 x Intensity0 rich5");
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
			return @" DirectionLightColor0 *DirectionLightIntensity0  ";

		}
	}
}

 
