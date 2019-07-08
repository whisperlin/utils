 using UnityEngine;
 using UnityEditor;
using UnityEngine.UI;
using System.Collections.Generic;

[CustomEditor(typeof(RectTransform))]
public class RectHelper : DecoratorEditor {
    Dictionary<string, bool> dicInitState = new Dictionary<string, bool>();
    public RectHelper() : base("RectTransformEditor") {
    }
    public override void OnInspectorGUI() {
        base.OnInspectorGUI();
        if (GUILayout.Button("/\\")) {
            var old = ((RectTransform)this.target).offsetMax;
            ((RectTransform)this.target).offsetMax = new Vector2(old.x, old.y + 1);

            old = ((RectTransform)this.target).offsetMin;
            ((RectTransform)this.target).offsetMin = new Vector2(old.x, old.y + 1);
        }
        GUILayout.BeginHorizontal();
        if (GUILayout.Button("<-")) {
            var old = ((RectTransform)this.target).offsetMax;
            ((RectTransform)this.target).offsetMax = new Vector2(old.x - 1, old.y);

            old = ((RectTransform)this.target).offsetMin;
            ((RectTransform)this.target).offsetMin = new Vector2(old.x - 1, old.y);
        }
        if (GUILayout.Button("\\/")) {
            var old = ((RectTransform)this.target).offsetMax;
            ((RectTransform)this.target).offsetMax = new Vector2(old.x, old.y - 1);

            old = ((RectTransform)this.target).offsetMin;
            ((RectTransform)this.target).offsetMin = new Vector2(old.x, old.y - 1);
        }
        if (GUILayout.Button("->")) {
            var old = ((RectTransform)this.target).offsetMax;
            ((RectTransform)this.target).offsetMax = new Vector2(old.x + 1, old.y);

            old = ((RectTransform)this.target).offsetMin;
            ((RectTransform)this.target).offsetMin = new Vector2(old.x + 1, old.y);
        }
        this.CheckImage(false);
        this.CheckText(false);
        GUILayout.EndHorizontal();
    }

    protected override void CheckImage(bool bInit) {
        if (!dicInitState.ContainsKey("Image"))
            dicInitState.Add("Image", false);
        if (bInit || !dicInitState["Image"]) {
            RectTransform goRect = (RectTransform)this.target;
            GameObject go = goRect.gameObject;
            var img = go.GetComponent<Image>();
            if (img != null) {
                dicInitState["Image"] = true;
                if (!bInit) {
                    img.raycastTarget = false;
                    EditorUtility.SetDirty(go);
                }
            }
        }
    }

    protected override void CheckText(bool bInit) {
        if (!dicInitState.ContainsKey("Text"))
            dicInitState.Add("Text", false);
        if (bInit || !dicInitState["Text"]) {
            RectTransform goRect = (RectTransform)this.target;
            GameObject go = goRect.gameObject;
            var text = go.GetComponent<Text>();
            if (text != null) {
                dicInitState["Text"] = true;
                if (!bInit) {
                    text.raycastTarget = false;
                    text.supportRichText = false;
                    EditorUtility.SetDirty(go);
                }
            }
        }
    }

}
