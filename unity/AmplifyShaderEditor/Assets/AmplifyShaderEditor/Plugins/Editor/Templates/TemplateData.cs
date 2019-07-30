// Amplify Shader Editor - Visual Shader Editing Tool
// Copyright (c) Amplify Creations, Lda <info@amplify.pt>
using System;
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

namespace AmplifyShaderEditor
{
    [Serializable]
    public class TemplateData
    {
        [SerializeField]
        private string m_name;

        [SerializeField]
        private string m_guid;

        [SerializeField]
        private string m_templateBody = string.Empty;

        [SerializeField]
        private string m_defaultShaderName = string.Empty;

        [SerializeField]
        private string m_shaderNameId = string.Empty;

        [SerializeField]
        private int m_orderId;

        [SerializeField]
        private List<TemplateProperty> m_propertyList = new List<TemplateProperty>();
        private Dictionary<string, TemplateProperty> m_propertyDict = new Dictionary<string, TemplateProperty>();

        [SerializeField]
        private List<TemplateInputData> m_inputDataList = new List<TemplateInputData>();
        private Dictionary<int, TemplateInputData> m_inputDataDict = new Dictionary<int, TemplateInputData>();

        [SerializeField]
        private List<TemplateCodeSnippetBase> m_snippetElementsList = new List<TemplateCodeSnippetBase>();
        private Dictionary<string, TemplateCodeSnippetBase> m_snippetElementsDict = new Dictionary<string, TemplateCodeSnippetBase>();

        [SerializeField]
        private List<TemplateVertexData> m_vertexData;

        [SerializeField]
        private TemplateInterpData m_interpolatorData;

        [SerializeField]
        private List<TemplateShaderPropertyData> m_availableShaderProperties = new List<TemplateShaderPropertyData>();

        [SerializeField]
        private TemplateFunctionData m_vertexFunctionData;

        [SerializeField]
        private TemplateFunctionData m_fragmentFunctionData;

        [SerializeField]
        private string m_interpDataId = string.Empty;

        [SerializeField]
        private string m_vertexDataId = string.Empty;

        [SerializeField]
        private bool m_isValid = true;

        public TemplateData(string name)
        {
            m_name = name;
        }

        public TemplateData(string name, string guid)
        {
            if (!string.IsNullOrEmpty(guid))
            {
                string datapath = AssetDatabase.GUIDToAssetPath(guid);
                if ( string.IsNullOrEmpty( datapath ) )
                {
                    m_isValid = false;
                    return;
                }

                string body = string.Empty;
                try
                {
                    body = IOUtils.LoadTextFileFromDisk(datapath);
                    body = body.Replace("\r\n", "\n");
                }
                catch (Exception e)
                {
                    Debug.LogException(e);
                    m_isValid = false;
                }

                if (!string.IsNullOrEmpty(body))
                {
                    LoadTemplateBody(body);
                    m_name = string.IsNullOrEmpty(name) ? m_defaultShaderName : name;
                    m_guid = guid;
                }
            }
        }

        public TemplateData(string name, string guid, string body)
        {
            if (!string.IsNullOrEmpty(body))
            {
                LoadTemplateBody(body);
                m_name = string.IsNullOrEmpty(name) ? m_defaultShaderName : name;
                m_guid = guid;
            }
        }

        void LoadTemplateBody(string body)
        {

            m_templateBody = body;

            if (m_templateBody.IndexOf(TemplatesManager.TemplateShaderNameBeginTag) < 0)
            {
                m_isValid = false;
                return;
            }

            //Fetching common tags
            FetchCommonTags();

            //Fetch function code areas
            FetchCodeAreas(TemplatesManager.TemplateVertexCodeBeginArea, MasterNodePortCategory.Vertex);
            FetchCodeAreas(TemplatesManager.TemplateFragmentCodeBeginArea, MasterNodePortCategory.Fragment);

            //Fetching inputs
            FetchInputs(MasterNodePortCategory.Fragment);
            FetchInputs(MasterNodePortCategory.Vertex);
            //Fetch snippets
        }

        void FetchCommonTags()
        {
            // Name
            try
            {
                int nameBegin = m_templateBody.IndexOf(TemplatesManager.TemplateShaderNameBeginTag);
                if (nameBegin < 0)
                {
                    // Not a template
                    return;
                }

                int nameEnd = m_templateBody.IndexOf(TemplatesManager.TemplateFullEndTag, nameBegin);
                int defaultBegin = nameBegin + TemplatesManager.TemplateShaderNameBeginTag.Length;
                int defaultLength = nameEnd - defaultBegin;
                m_defaultShaderName = m_templateBody.Substring(defaultBegin, defaultLength);
                int[] nameIdx = m_defaultShaderName.AllIndexesOf("\"");
                nameIdx[0] += 1; // Ignore the " character from the string
                m_defaultShaderName = m_defaultShaderName.Substring(nameIdx[0], nameIdx[1] - nameIdx[0]);
                m_shaderNameId = m_templateBody.Substring(nameBegin, nameEnd + TemplatesManager.TemplateFullEndTag.Length - nameBegin);
                AddId(m_shaderNameId, false);

            }
            catch (Exception e)
            {
                Debug.LogException(e);
                m_isValid = false;
            }
            // Vertex Data
            {
                int vertexDataTagBegin = m_templateBody.IndexOf(TemplatesManager.TemplateVertexDataTag);
                if (vertexDataTagBegin > -1)
                {
                    int vertexDataTagEnd = m_templateBody.IndexOf(TemplatesManager.TemplateEndOfLine, vertexDataTagBegin);
                    m_vertexDataId = m_templateBody.Substring(vertexDataTagBegin, vertexDataTagEnd + TemplatesManager.TemplateEndOfLine.Length - vertexDataTagBegin);
                    int dataBeginIdx = m_templateBody.LastIndexOf('{', vertexDataTagBegin, vertexDataTagBegin);
                    string vertexData = m_templateBody.Substring(dataBeginIdx + 1, vertexDataTagBegin - dataBeginIdx);

                    int parametersBegin = vertexDataTagBegin + TemplatesManager.TemplateVertexDataTag.Length;
                    string parameters = m_templateBody.Substring(parametersBegin, vertexDataTagEnd - parametersBegin);
                    m_vertexData = TemplateHelperFunctions.CreateVertexDataList(vertexData, parameters);
                    AddId(m_vertexDataId);
                }
            }

            // Available interpolators
            try
            {
                int interpDataBegin = m_templateBody.IndexOf(TemplatesManager.TemplateInterpolatorBeginTag);
                if (interpDataBegin > -1)
                {
                    int interpDataEnd = m_templateBody.IndexOf(TemplatesManager.TemplateEndOfLine, interpDataBegin);
                    m_interpDataId = m_templateBody.Substring(interpDataBegin, interpDataEnd + TemplatesManager.TemplateEndOfLine.Length - interpDataBegin);

                    int dataBeginIdx = m_templateBody.LastIndexOf('{', interpDataBegin, interpDataBegin);
                    string interpData = m_templateBody.Substring(dataBeginIdx + 1, interpDataBegin - dataBeginIdx);

                    m_interpolatorData = TemplateHelperFunctions.CreateInterpDataList(interpData, m_interpDataId);
                    AddId(m_interpDataId);
                }
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                m_isValid = false;
            }

            try
            {
                Dictionary<string, TemplateShaderPropertyData> duplicatesHelper = new Dictionary<string, TemplateShaderPropertyData>();
                m_availableShaderProperties = new List<TemplateShaderPropertyData>();
                // Common Tags
                for (int i = 0; i < TemplatesManager.CommonTags.Length; i++)
                {
                    int idx = m_templateBody.IndexOf(TemplatesManager.CommonTags[i].Id);
                    if (idx > -1)
                    {
                        //properties
                        if (i == (int)TemplateCommonTagId.Property)
                        {
                            TemplateHelperFunctions.CreateShaderPropertiesList(m_templateBody.Substring(0, idx + TemplatesManager.CommonTags[i].Id.Length), ref m_availableShaderProperties, ref duplicatesHelper);
                        }
                        // globals
                        if (i == (int)TemplateCommonTagId.Global)
                        {
                            TemplateHelperFunctions.CreateShaderGlobalsList(m_templateBody.Substring(0, idx + TemplatesManager.CommonTags[i].Id.Length), ref m_availableShaderProperties, ref duplicatesHelper);
                        }
                        AddId(TemplatesManager.CommonTags[i]);
                        if (i == (int)TemplateCommonTagId.Tag)
                        {
                            m_propertyList[m_propertyList.Count - 1].Indentation = " ";
                        }
                    }
                }
                duplicatesHelper.Clear();
                duplicatesHelper = null;
            }
            catch( Exception e )
            {
                Debug.LogException(e);
                m_isValid = false;
            }
        }

        void FetchCodeAreas(string begin, MasterNodePortCategory category)
        {
            int areaBeginIndexes = m_templateBody.IndexOf(begin);

            if (areaBeginIndexes > -1)
            {
                int beginIdx = areaBeginIndexes + begin.Length;
                int endIdx = m_templateBody.IndexOf(TemplatesManager.TemplateEndOfLine, beginIdx);
                int length = endIdx - beginIdx;

                string parameters = m_templateBody.Substring(beginIdx, length);

                string[] parametersArr = parameters.Split(IOUtils.FIELD_SEPARATOR);

                string id = m_templateBody.Substring(areaBeginIndexes, endIdx + TemplatesManager.TemplateEndOfLine.Length - areaBeginIndexes);
                string inParameters = parametersArr[0];
                string outParameters = (parametersArr.Length > 1) ? parametersArr[1] : string.Empty;
                if (category == MasterNodePortCategory.Fragment)
                    m_fragmentFunctionData = new TemplateFunctionData(id, areaBeginIndexes, inParameters, outParameters, category);
                else
                {
                    m_vertexFunctionData = new TemplateFunctionData(id, areaBeginIndexes, inParameters, outParameters, category);
                }
                AddId(id, true);
            }
        }

        void FetchInputs(MasterNodePortCategory portCategory)
        {
            string beginTag = (portCategory == MasterNodePortCategory.Fragment) ? TemplatesManager.TemplateInputsFragBeginTag : TemplatesManager.TemplateInputsVertBeginTag;
            int[] inputBeginIndexes = m_templateBody.AllIndexesOf(beginTag);
            if (inputBeginIndexes != null && inputBeginIndexes.Length > 0)
            {
                for (int i = 0; i < inputBeginIndexes.Length; i++)
                {
                    int inputEndIdx = m_templateBody.IndexOf(TemplatesManager.TemplateEndSectionTag, inputBeginIndexes[i]);
                    int defaultValueBeginIdx = inputEndIdx + TemplatesManager.TemplateEndSectionTag.Length;
                    int endLineIdx = m_templateBody.IndexOf(TemplatesManager.TemplateFullEndTag, defaultValueBeginIdx);

                    string defaultValue = m_templateBody.Substring(defaultValueBeginIdx, endLineIdx - defaultValueBeginIdx);
                    string tagId = m_templateBody.Substring(inputBeginIndexes[i], endLineIdx + TemplatesManager.TemplateFullEndTag.Length - inputBeginIndexes[i]);

                    int beginIndex = inputBeginIndexes[i] + beginTag.Length;
                    int length = inputEndIdx - beginIndex;
                    string inputData = m_templateBody.Substring(beginIndex, length);
                    string[] inputDataArray = inputData.Split(IOUtils.FIELD_SEPARATOR);
                    if (inputDataArray != null && inputDataArray.Length > 0)
                    {
                        try
                        {
                            string portName = inputDataArray[(int)TemplatePortIds.Name];
                            WirePortDataType dataType = (WirePortDataType)Enum.Parse(typeof(WirePortDataType), inputDataArray[(int)TemplatePortIds.DataType].ToUpper());

                            int portUniqueIDArrIdx = (int)TemplatePortIds.UniqueId;
                            int portUniqueId = (portUniqueIDArrIdx < inputDataArray.Length) ? Convert.ToInt32(inputDataArray[portUniqueIDArrIdx]) : -1;
                            if (portUniqueId < 0)
                                portUniqueId = m_inputDataList.Count;

                            int portOrderArrayIdx = (int)TemplatePortIds.OrderId;
                            int portOrderId = (portOrderArrayIdx < inputDataArray.Length) ? Convert.ToInt32(inputDataArray[portOrderArrayIdx]) : -1;
                            if (portOrderId < 0)
                                portOrderId = m_inputDataList.Count;

                            AddInput(tagId, portName, defaultValue, dataType, portCategory, portUniqueId, portOrderId);
                        }
                        catch (Exception e)
                        {
                            Debug.LogException(e);
                        }
                    }
                }
            }
        }

        void FetchSnippets()
        {
            int[] codeSnippetAttribBeginIndexes = m_templateBody.AllIndexesOf(TemplatesManager.TemplateCodeSnippetAttribBegin);
            int[] codeSnippetAttribEndIndexes = m_templateBody.AllIndexesOf(TemplatesManager.TemplateCodeSnippetAttribEnd);
            int[] codeSnippetEndIndexes = m_templateBody.AllIndexesOf(TemplatesManager.TemplateCodeSnippetEnd);

            if (codeSnippetAttribBeginIndexes != null && codeSnippetAttribBeginIndexes.Length > 0 &&
                    codeSnippetAttribEndIndexes != null && codeSnippetAttribEndIndexes.Length > 0 &&
                    codeSnippetEndIndexes != null && codeSnippetEndIndexes.Length > 0 &&
                    codeSnippetEndIndexes.Length == codeSnippetAttribBeginIndexes.Length &&
                    codeSnippetAttribBeginIndexes.Length == codeSnippetAttribEndIndexes.Length)
            {
                for (int i = 0; i < codeSnippetAttribBeginIndexes.Length; i++)
                {
                    // get attributes
                    int startAttribIndex = codeSnippetAttribBeginIndexes[i] + TemplatesManager.TemplateCodeSnippetAttribBegin.Length;
                    int lengthAttrib = codeSnippetAttribEndIndexes[i] - startAttribIndex;
                    string snippetAttribs = m_templateBody.Substring(startAttribIndex, lengthAttrib);
                    string[] snippetAttribsArr = snippetAttribs.Split(IOUtils.FIELD_SEPARATOR);
                    if (snippetAttribsArr != null && snippetAttribsArr.Length > 0)
                    {
                        string attribName = snippetAttribsArr[(int)TemplateCodeSnippetInfoIdx.Name];
                        TemplateCodeSnippetType attribType = (TemplateCodeSnippetType)Enum.Parse(typeof(TemplateCodeSnippetType), snippetAttribsArr[(int)TemplateCodeSnippetInfoIdx.Type]);
                        if (m_snippetElementsDict.ContainsKey(attribName))
                        {
                            if (m_snippetElementsDict[attribName].Type != attribType)
                            {
                                if (DebugConsoleWindow.DeveloperMode)
                                    Debug.LogWarning("Found incompatible types for snippet " + attribName);
                            }
                        }
                        else
                        {
                            switch (attribType)
                            {
                                case TemplateCodeSnippetType.Toggle:
                                    {
                                        //Register must be done by first instantiang the correct type and register it on both containers
                                        //Overrides don't work if we use the container reference into the other
                                        TemplateCodeSnippetToggle newSnippet = ScriptableObject.CreateInstance<TemplateCodeSnippetToggle>();
                                        newSnippet.Init(attribName, attribType);
                                        m_snippetElementsDict.Add(attribName, newSnippet);
                                        m_snippetElementsList.Add(newSnippet);
                                    }
                                    break;
                            }

                        }
                        // Add initial tag indentation
                        int indentationIndex = codeSnippetAttribBeginIndexes[i];
                        int lengthAdjust = 0;
                        for (; indentationIndex > 0; indentationIndex--, lengthAdjust++)
                        {
                            if (m_templateBody[indentationIndex] == TemplatesManager.TemplateNewLine)
                            {
                                indentationIndex += 1;
                                lengthAdjust -= 1;
                                break;
                            }
                        }

                        if (indentationIndex > 0)
                        {
                            string snippetId = m_templateBody.Substring(indentationIndex,
                                                                         codeSnippetEndIndexes[i] + TemplatesManager.TemplateCodeSnippetEnd.Length - codeSnippetAttribBeginIndexes[i] + lengthAdjust);

                            int snippetCodeStart = codeSnippetAttribEndIndexes[i] + TemplatesManager.TemplateCodeSnippetAttribEnd.Length;
                            int snippetCodeLength = codeSnippetEndIndexes[i] - snippetCodeStart;
                            //Remove possible identation characters present between tag and last instruction
                            if (m_templateBody[snippetCodeStart + snippetCodeLength - 1] != TemplatesManager.TemplateNewLine)
                            {
                                for (; snippetCodeLength > 0; snippetCodeLength--)
                                {
                                    if (m_templateBody[snippetCodeStart + snippetCodeLength - 1] == TemplatesManager.TemplateNewLine)
                                        break;
                                }
                            }

                            if (snippetCodeLength > 0)
                            {
                                string snippetCode = m_templateBody.Substring(snippetCodeStart, snippetCodeLength);
                                TemplateCodeSnippetElement element = new TemplateCodeSnippetElement(snippetId, snippetCode);
                                m_snippetElementsDict[attribName].AddSnippet(element);
                            }
                        }
                    }
                }
            }
        }

        void RefreshSnippetInfo()
        {
            if (m_snippetElementsDict == null)
            {
                m_snippetElementsDict = new Dictionary<string, TemplateCodeSnippetBase>();
            }

            if (m_snippetElementsDict.Count != m_snippetElementsList.Count)
            {
                m_snippetElementsDict.Clear();
                for (int i = 0; i < m_snippetElementsList.Count; i++)
                {
                    m_snippetElementsDict.Add(m_snippetElementsList[i].NameId, m_snippetElementsList[i]);
                }
            }
        }

        public void DrawSnippetProperties(ParentNode owner)
        {
            for (int i = 0; i < m_snippetElementsList.Count; i++)
            {
                m_snippetElementsList[i].DrawProperties(owner);
            }
        }

        public void InsertSnippets(ref string shaderBody)
        {
            for (int i = 0; i < m_snippetElementsList.Count; i++)
            {
                m_snippetElementsList[i].InsertSnippet(ref shaderBody);
            }
        }

        public void AddId(TemplateTagData data)
        {
            AddId(data.Id, data.SearchIndentation);
        }

        public void AddId(string ID, bool searchIndentation = true)
        {
            int propertyIndex = m_templateBody.IndexOf(ID);
            if (propertyIndex > -1)
            {
                if (searchIndentation)
                {
                    int indentationIndex = -1;
                    for (int i = propertyIndex; i > 0; i--)
                    {
                        if (m_templateBody[i] == TemplatesManager.TemplateNewLine)
                        {
                            indentationIndex = i + 1;
                            break;
                        }
                    }
                    if (indentationIndex > -1)
                    {
                        int length = propertyIndex - indentationIndex;
                        string indentation = (length > 0) ? m_templateBody.Substring(indentationIndex, length) : string.Empty;
                        m_propertyList.Add(new TemplateProperty(ID, indentation));
                    }
                }
                else
                {
                    m_propertyList.Add(new TemplateProperty(ID, string.Empty));
                }
            }
        }

        void BuildInfo()
        {
            if (m_propertyDict == null)
            {
                m_propertyDict = new Dictionary<string, TemplateProperty>();
            }

            if (m_propertyList.Count != m_propertyDict.Count)
            {
                m_propertyDict.Clear();
                for (int i = 0; i < m_propertyList.Count; i++)
                {
                    m_propertyDict.Add(m_propertyList[i].Id, m_propertyList[i]);
                }
            }
        }

        public void ResetTemplateUsageData()
        {
            BuildInfo();
            for (int i = 0; i < m_propertyList.Count; i++)
            {
                m_propertyList[i].Used = false;
            }
        }

        public void AddInput(string tagId, string portName, string defaultValue, WirePortDataType dataType, MasterNodePortCategory portCategory, int portUniqueId, int portOrderId)
        {
            m_inputDataList.Add(new TemplateInputData(tagId, portName, defaultValue, dataType, portCategory, portUniqueId, portOrderId));
            AddId(tagId, false);
        }

        public void Destroy()
        {
            if (m_vertexData != null)
            {
                m_vertexData.Clear();
                m_vertexData = null;
            }

            if (m_interpolatorData != null)
            {
                m_interpolatorData.Destroy();
                m_interpolatorData = null;
            }

            if (m_availableShaderProperties != null)
            {
                m_availableShaderProperties.Clear();
                m_availableShaderProperties = null;
            }

            if (m_propertyDict != null)
            {
                m_propertyDict.Clear();
                m_propertyDict = null;
            }

            if (m_propertyList != null)
            {
                m_propertyList.Clear();
                m_propertyList = null;
            }

            if (m_inputDataDict != null)
            {
                m_inputDataDict.Clear();
                m_inputDataDict = null;
            }

            if (m_inputDataList != null)
            {
                m_inputDataList.Clear();
                m_inputDataList = null;
            }

            if (m_snippetElementsDict != null)
            {
                m_snippetElementsDict.Clear();
                m_snippetElementsDict = null;
            }

            if (m_snippetElementsList != null)
            {
                for (int i = 0; i < m_snippetElementsList.Count; i++)
                {
                    GameObject.DestroyImmediate(m_snippetElementsList[i]);
                    m_snippetElementsList[i] = null;
                }
                m_snippetElementsList.Clear();
                m_snippetElementsList = null;
            }
        }

        public void FillEmptyTags(ref string body)
        {
            for (int i = 0; i < m_propertyList.Count; i++)
            {
                if (!m_propertyList[i].Used)
                {
                    body = body.Replace(m_propertyList[i].Indentation + m_propertyList[i].Id, string.Empty);
                }
            }
        }

        public bool FillVertexInstructions(ref string body, params string[] values)
        {
            if (m_vertexFunctionData != null && !string.IsNullOrEmpty(m_vertexFunctionData.Id))
            {
                return FillTemplateBody(m_vertexFunctionData.Id, ref body, values);
            }

            if (values.Length > 0)
            {
                UIUtils.ShowMessage("Attemping to add vertex instructions on a template with no assigned vertex code area", MessageSeverity.Error);
                return false;
            }
            return true;
        }

        public bool FillFragmentInstructions(ref string body, params string[] values)
        {
            if (m_fragmentFunctionData != null && !string.IsNullOrEmpty(m_fragmentFunctionData.Id))
            {
                return FillTemplateBody(m_fragmentFunctionData.Id, ref body, values);
            }

            if (values.Length > 0)
            {
                UIUtils.ShowMessage("Attemping to add fragment instructions on a template with no assigned vertex code area", MessageSeverity.Error);
                return false;
            }
            return true;
        }

        // values must be unindented an without line feed
        public bool FillTemplateBody(string id, ref string body, params string[] values)
        {
            if (values.Length == 0)
                return true;

            BuildInfo();

            if (m_propertyDict.ContainsKey(id))
            {
                string finalValue = string.Empty;
                for (int i = 0; i < values.Length; i++)
                {

                    if (m_propertyDict[id].AutoLineFeed)
                    {
                        string[] valuesArr = values[i].Split('\n');
                        for (int j = 0; j < valuesArr.Length; j++)
                        {
                            //first value will be automatically indented by the string replace
                            finalValue += ((i == 0 && j == 0) ? string.Empty : m_propertyDict[id].Indentation) + valuesArr[j];
                            finalValue += TemplatesManager.TemplateNewLine;
                        }

                    }
                    else
                    {
                        //first value will be automatically indented by the string replace
                        finalValue += (i == 0 ? string.Empty : m_propertyDict[id].Indentation) + values[i];
                    }
                }

                body = body.Replace(id, finalValue);
                return true;
            }

            if (values.Length > 1 || !string.IsNullOrEmpty(values[0]))
            {
                UIUtils.ShowMessage(string.Format("Attempting to write data into inexistant tag {0}. Please review the template {1} body and consider adding the missing tag.", id, m_name), MessageSeverity.Error);
                return false;
            }

            return true;

        }

        public bool FillTemplateBody(string id, ref string body, List<PropertyDataCollector> values)
        {
            if (values.Count == 0)
            {
                return FillTemplateBody(id, ref body, string.Empty);
            }

            string[] array = new string[values.Count];
            for (int i = 0; i < values.Count; i++)
            {
                array[i] = values[i].PropertyName;
            }
            return FillTemplateBody(id, ref body, array);
        }

        public TemplateInputData InputDataFromId(int id)
        {
            if (m_inputDataDict == null)
                m_inputDataDict = new Dictionary<int, TemplateInputData>();

            if (m_inputDataDict.Count != m_inputDataList.Count)
            {
                m_inputDataDict.Clear();
                for (int i = 0; i < m_inputDataList.Count; i++)
                {
                    m_inputDataDict.Add(m_inputDataList[i].PortUniqueId, m_inputDataList[i]);
                }
            }

            if (m_inputDataDict.ContainsKey(id))
                return m_inputDataDict[id];

            return null;
        }

        public string GetVertexData(TemplateInfoOnSematics info)
        {
            int count = m_vertexData.Count;
            for (int i = 0; i < count; i++)
            {
                if (m_vertexData[i].DataInfo == info)
                {
                    return string.Format(TemplateHelperFunctions.TemplateVarFormat, m_vertexFunctionData.InVarName, m_vertexData[i].VarName);
                }
            }
            return string.Empty;
        }

        public string GetInterpolatedData(TemplateInfoOnSematics info)
        {
            int count = m_interpolatorData.Interpolators.Count;
            for (int i = 0; i < count; i++)
            {
                if (m_interpolatorData.Interpolators[i].DataInfo == info)
                {
                    return string.Format(TemplateHelperFunctions.TemplateVarFormat, m_fragmentFunctionData.InVarName, m_interpolatorData.Interpolators[i].VarName);
                }
            }
            return string.Empty;
        }

        public string InterpDataId { get { return m_interpDataId; } }
        public string VertexDataId { get { return m_vertexDataId; } }
        public string DefaultShaderName { get { return m_defaultShaderName; } }
        public string ShaderNameId { get { return m_shaderNameId; } }
        public int OrderId { get { return m_orderId; } set { m_orderId = value; } }
        public string Name { get { return m_name; } }
        public string TemplateBody { get { return m_templateBody; } }
        public List<TemplateInputData> InputDataList { get { return m_inputDataList; } }
        public List<TemplateVertexData> VertexDataList { get { return m_vertexData; } }
        public TemplateInterpData InterpolatorData { get { return m_interpolatorData; } }
        public TemplateFunctionData VertexFunctionData { get { return m_vertexFunctionData; } }
        public TemplateFunctionData FragFunctionData { get { return m_fragmentFunctionData; } }
        public bool IsValid { get { return m_isValid; } }
        public string GUID { get { return m_guid; } }
        public List<TemplateShaderPropertyData> AvailableShaderProperties { get { return m_availableShaderProperties; } }
    }
}
