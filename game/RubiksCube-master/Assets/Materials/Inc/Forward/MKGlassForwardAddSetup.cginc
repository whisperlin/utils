//forward add setup
#ifndef MK_GLASS_FORWARD_ADD_SETUP
	#define MK_GLASS_FORWARD_ADD_SETUP

	#ifndef MK_GLASS_FWD_ADD_PASS
		#define MK_GLASS_FWD_ADD_PASS 1 
	#endif

	#include "UnityGlobalIllumination.cginc"
	
	#include "UnityCG.cginc"
	#include "AutoLight.cginc"

	#include "../Common/MKGlassDef.cginc"
	#include "../Common/MKGlassV.cginc"
	#include "../Common/MKGlassInc.cginc"
	#include "../Forward/MKGlassForwardIO.cginc"
	#include "../Surface/MKGlassSurfaceIO.cginc"
	#include "../Common/MKGlassLight.cginc"
	#include "../Surface/MKGlassSurface.cginc"
#endif