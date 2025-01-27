{
	"version":"LAYASCENE3D:01",
	"data":{
		"type":"Scene3D",
		"props":{
			"name":"dsds",
			"ambientColor":[
				0.212,
				0.227,
				0.259
			],
			"lightmaps":[
				{
					"constructParams":[
						1024,
						1024,
						1,
						false
					],
					"propertyParams":{
						"filterMode":1,
						"wrapModeU":1,
						"wrapModeV":1,
						"anisoLevel":3
					},
					"path":"Assets/dsds/Lightmap-0_comp_light.png"
				}
			],
			"enableFog":false,
			"fogStart":0,
			"fogRange":300,
			"fogColor":[
				0.5,
				0.5,
				0.5
			]
		},
		"child":[
			{
				"type":"Camera",
				"props":{
					"name":"Main Camera",
					"active":true,
					"isStatic":false,
					"layer":0,
					"position":[
						-47.33173,
						30.1651,
						7.361004
					],
					"rotation":[
						0.03775749,
						0.9218188,
						0.3744102,
						-0.0929601
					],
					"scale":[
						1,
						1,
						1
					],
					"clearFlag":1,
					"orthographic":false,
					"fieldOfView":60,
					"nearPlane":0.3,
					"farPlane":1000,
					"viewport":[
						0,
						0,
						1,
						1
					],
					"clearColor":[
						0.1921569,
						0.3019608,
						0.4745098,
						0
					]
				},
				"components":[],
				"child":[]
			},
			{
				"type":"DirectionLight",
				"props":{
					"name":"Directional Light",
					"active":true,
					"isStatic":false,
					"layer":0,
					"position":[
						-84.19798,
						13.07619,
						30.46575
					],
					"rotation":[
						0.2268162,
						0.6318192,
						0.200864,
						-0.7134511
					],
					"scale":[
						1,
						1,
						1
					],
					"intensity":1,
					"lightmapBakedType":2,
					"color":[
						1,
						0.9568627,
						0.8392157
					]
				},
				"components":[],
				"child":[]
			},
			{
				"type":"MeshSprite3D",
				"props":{
					"name":"Terrain",
					"active":true,
					"isStatic":true,
					"layer":0,
					"position":[
						0,
						0,
						0
					],
					"rotation":[
						0,
						0,
						0,
						-1
					],
					"scale":[
						1,
						1,
						1
					],
					"meshPath":"terrain/terrain_Terrain.lm",
					"materials":[
						{
							"type":"Laya.ExtendTerrainMaterial",
							"path":"terrain/terrain_Terrain.lmat"
						}
					],
					"lightmapIndex":0,
					"lightmapScaleOffset":[
						1,
						1,
						0,
						0
					]
				},
				"components":[],
				"child":[]
			}
		]
	}
}