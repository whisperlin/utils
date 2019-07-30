using System;

namespace AmplifyShaderEditor
{
	[Serializable]
	public class TemplateShaderPropertyData
	{
		public string PropertyInspectorName;
		public string PropertyName;
		public WirePortDataType PropertyDataType;
		public PropertyType PropertyType;

		public TemplateShaderPropertyData( string propertyInspectorName, string propertyName, WirePortDataType propertyDataType , PropertyType propertyType )
		{
			PropertyInspectorName = string.IsNullOrEmpty( propertyInspectorName )?propertyName: propertyInspectorName;
			PropertyName = propertyName;
			PropertyDataType = propertyDataType;
			PropertyType = propertyType;
		}
	}
}
