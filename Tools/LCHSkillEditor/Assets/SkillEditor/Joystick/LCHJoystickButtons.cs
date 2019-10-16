using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


[System.Serializable]
public class LCHJsButtonInformation
{

 
    public Sprite sprite;
    [Header("是否开启cd")]
    public bool cd  ;
    [Header("cd名字")]
    public string cdName;
    [Header("按钮位置(按屏幕高度比例)")]
    public Vector3 position;
    [Header("按钮大小(按屏幕高度比例)")]
    public Vector2 size;
    [Header("虚拟按键(cd名为空可用)")]
    public VirtualInput.KeyCode keyCode;

    public Image image;


    [System.NonSerialized]
    public Image cdImage;
    [System.NonSerialized]
    public Text txt;
 

}
/*public struct LCHJsButtonObject
{
    public Image image;
    public Image cdImage;
    public Text txt;
    public LCHJsButtonInformation information;
}*/
public class LCHJoystickButtons : MonoBehaviour {

    
    public CharacterBase character;
    public LCHJsButtonInformation [] buttons = new LCHJsButtonInformation[0];
    //public List<LCHJsButtonObject> objects = new List<LCHJsButtonObject>();

    [Header("cd 颜色")]
    public Color cdColor = new Color(0.5f, 0.5f, 1f);

    [Header("二段技能计时颜色")]
    public Color cdSecondColor = new Color(1f, 0.5f, 0f);
    [Header("cd 字体颜色")]
    public Color cdFontColor = new Color(1f, 1f, 1f);


    [Header("cd 字体阴影颜色")]
    public Color cdFontShadowColor = new Color(0.2f, 0.2f, 0.2f);
    [Header("字号")]
    
    public float fontSize = 30f;
    public Font font;

    public Material dirMat;
    public Material pointMat;
    public Material randMat;
    public Material targetMat;
    public float pointOperaSize = 0.15f;

    [Header("选人时的那个圈")]
    public GameObject Selecter;
    public void init()
    {

        if (null != Selecter)
        {
            GameObject g = GameObject.Instantiate(Selecter);
            g.name = "selecter";
            LCHSelecter ls = g.AddComponent<LCHSelecter>();
            ls.character = character.character;

        }
         
        //objects.Clear();
        for (int i = 0; i < buttons.Length; i++)
        {
            var button = buttons[i];
            GameObject g = button.image.gameObject;
            button.image.sprite = buttons[i].sprite;
 
            LCHButton lb = g.AddComponent<LCHButton>();
            

            GameObject g2 = new GameObject();
            g2.name = "button cd" + i;
            g2.transform.parent = g.transform;
            button.cdImage = g2.AddComponent<Image>();
            button.cdImage.sprite = buttons[i].sprite;
            button.cdImage.rectTransform.anchorMin = button.cdImage.rectTransform.anchorMax = button.cdImage.rectTransform.pivot = new Vector2(1, 0);
            button.cdImage.rectTransform.sizeDelta = button.image.rectTransform.sizeDelta;
            button.cdImage.rectTransform.anchoredPosition = Vector2.zero;
            button.cdImage.rectTransform.localScale = Vector3.one;

            button.cdImage.type = Image.Type.Filled;
            button.cdImage.fillMethod = Image.FillMethod.Radial360;
            button.cdImage.fillClockwise = false;
            button.cdImage.fillOrigin = 2;
 
            button.cdImage.color = cdColor;
            button.cdImage.fillAmount = 0.3f;

            GameObject g3 = new GameObject();
            g3.name = "text cd" + i;
            g3.transform.parent = g2.transform;
            button.txt = g3.AddComponent<Text>();
            Outline ol = g3.AddComponent<Outline>();
            button.txt.rectTransform.anchorMin = button.txt.rectTransform.anchorMax = button.txt.rectTransform.pivot = new Vector2(1, 0);
            button.cdImage.rectTransform.sizeDelta = button.image.rectTransform.sizeDelta;
            button.txt.rectTransform.anchoredPosition = Vector2.zero;

            button.txt.font = font;
            button.txt.alignment = TextAnchor.MiddleCenter;
            button.txt.text = "10";
            button.txt.fontStyle = FontStyle.Bold;
            button.txt.color = cdFontColor;
       ;
            button.txt.fontSize = (int)fontSize;
            button.txt.rectTransform.localScale = Vector3.one;
            ol.effectColor = cdFontShadowColor;

 

            lb.button = button;
            lb.character = character.character;
            lb.dirMat = dirMat;
            lb.pointMat = pointMat;
            lb.pointOperaSize = pointOperaSize*Screen.height;
            lb.randMat = randMat;
            lb.keyCode = buttons[i].keyCode;
            lb.targetMat = targetMat;


            
        }
    }
    // Use this for initialization
    void Start () {

        init();
        
    }
	Dictionary<int,string> intString = new Dictionary<int, string>();
    Dictionary<int, string> ZOString = new Dictionary<int, string>();
    public string getIntString(int i )
    {
        if (intString.ContainsKey(i))
            return intString[i];
        string s = i.ToString() ;
        intString[ i] = s;
        return s;
    }
    public string getZOString(int i)
    {
        if (ZOString.ContainsKey(i))
            return ZOString[i];
        string s;
        if (i < 10)
        {
            s = "0." + i.ToString() + "0";
        }
        else
        {
            s = "0." + i.ToString() ;
        }

        ZOString[i] = s;
        return s;
    }

    // Update is called once per frame
    void Update () {
        if (null == character)
            return;
        var chr = character.character;

        for (int i = 0; i < buttons.Length; i++)
        {
            var obj = buttons[i];
 
            var _param = chr.GetSkillCDSkillParams(obj.cdName );
            if (obj.cd &&_param != null && _param.GetMaxCD() > 0.0001f && _param.cd> 0.0001f)
            {
                obj.cdImage.gameObject.SetActive(true);
                obj.cdImage.fillAmount = _param.cd / _param.GetMaxCD();
                if (_param.State == 0)
                {
                    obj.cdImage.color = cdColor;
                }
                else
                {
                    obj.cdImage.color = cdSecondColor;
                }
                //cdSecondColor
                if (_param.cd > 1f)
                {


                  

                    int c = (int)(_param.cd );
                    obj.txt.text = getIntString(c);

                }
                else
                {

                    int c = (int)(_param.cd * 100);
                    obj.txt.text = getZOString(c);
                    
                   // obj.txt.text = _param.cd.ToString("2" );
                }
            }
            else
            {
                obj.cdImage.gameObject.SetActive(false);
            }
            
        }
	}
}
