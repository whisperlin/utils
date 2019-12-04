using System.Collections;
using System.Collections.Generic;
using System.Xml;
using UnityEngine;


public class FontItem
{
    public Vector2 uv0;
    public Vector2 uv1;
    public float width;
    public float height;
}

[System.Serializable]
public class SceenFontData
{
    public TextAsset text;
    public FontMesh fontMesh;
    [System.NonSerialized]
    public Dictionary<int, FontItem> fonts = new Dictionary<int, FontItem>();
    [System.NonSerialized]
    public SimplePool<FontMesh> pools = new SimplePool<FontMesh>();

    public FontMesh CreateFontMesh()
    {
        return GameObject.Instantiate<FontMesh>(fontMesh);
    }

}
public class HitFontManager : MonoBehaviour
{

    public static HitFontManager globalMgr;
    public SceenFontData[] fonts;

    public int value = 102486597;
    public Mesh mesh;
   
    // Use this for initialization
    void Start()
    {
        globalMgr = this;
        for (int i = 0; i < fonts.Length; i++)
        {
            var font = fonts[i];
            if (font.text == null || null == font.fontMesh)
                continue;
 
            font.pools.createHandle = font.CreateFontMesh;

            float scaleW = 0f;
            float scaleH = 0f;
            string xmlData = font.text.text;
            XmlDocument xml = new XmlDocument();
            xml.LoadXml(xmlData);
            XmlNode root = xml.SelectSingleNode("font");
            XmlNodeList xmlNodeList = root.SelectSingleNode("chars").ChildNodes;

            XmlElement com = root.SelectSingleNode("common") as XmlElement;
            scaleW = System.Convert.ToSingle(com.GetAttribute("scaleW"));
            scaleH = System.Convert.ToSingle(com.GetAttribute("scaleH"));

            foreach (XmlElement xl1 in xmlNodeList)
            {
                int _id = System.Convert.ToInt32(xl1.GetAttribute("id"));
                float _x = System.Convert.ToSingle(xl1.GetAttribute("x"));
                float _y = System.Convert.ToSingle(xl1.GetAttribute("y"));
                float _width = System.Convert.ToSingle(xl1.GetAttribute("width"));
                float _height = System.Convert.ToSingle(xl1.GetAttribute("height"));

                FontItem _font = new FontItem();
                _font.uv0 = new Vector2(_x / scaleW, _y / scaleH);
                _font.uv1 = _font.uv0 + new Vector2(_width / scaleW, _height / scaleH);
                _font.width = _width;
                _font.height = _height;
                font.fonts[_id] = _font;
            }
        }

        if (mesh == null)
            mesh = new Mesh();
        else
            mesh.Clear();
        gameObject.AddComponent<MeshFilter>().mesh = mesh;

        //buildFont(mesh, value,fonts);

    }

    public void CreateFont(int type, int value,Vector3 pos)
    {
        FontMesh mf =  fonts[type].pools.Get();
        mf.SetValue(value, fonts[type].fonts);
        mf.pools = fonts[type].pools;
        mf.curTime = 0f;
 
        mf.transform.position = pos;
        mf.gameObject.SetActive(true);

    }


}
 