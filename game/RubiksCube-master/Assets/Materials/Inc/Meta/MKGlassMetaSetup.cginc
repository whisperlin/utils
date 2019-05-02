//Meta setup
#ifndef MK_GLASS_META_SETUP
	#define MK_GLASS_META_SETUP

	#ifndef _EMISSION
		#define _EMISSION 1
	#endif

	#ifndef MK_GLASS_META_PASS
		#define MK_GLASS_META_PASS 1
	#endif

	#include "UnityCG.cginc"
	#include "../Common/MKGlassDef.cginc"
	#ifndef MKGLASS_TC
		#define MKGLASS_TC 1
	#endif
	#include "../Common/MKGlassV.cginc"
	#include "../Common/MKGlassInc.cginc"
	#include "../Surface/MKGlassSurfaceIO.cginc"
	#include "../Surface/MKGlassSurface.cginc"
	#include "MKGlassMetaIO.cginc"
#endif