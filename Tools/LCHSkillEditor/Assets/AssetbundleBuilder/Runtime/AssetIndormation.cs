using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class AssetsData
{
    public Dictionary<string, AssetIndormation> objs = new Dictionary<string, AssetIndormation>();
}

[System.Serializable]
public class AssetIndormation  {
    public string package = "";
    public List<string> dependencies = new List<string>();
}
