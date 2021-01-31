using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Instructions : MonoBehaviour
{
	public Vector2 LabelCentre = new Vector2(0.5f, 0.9f);
	public float widthProportion = 0.5f;
	public float heightPropotion = 0.1f;
	public GUISkin guiSkin;
	public bool display = true;

	void OnGUI()
	{
		string message = display ? "Match 'current' to 'target'.Click to confirm." : "Drawing level ...";
		if (guiSkin != null)
			GUI.skin = guiSkin;

		float w = Screen.width * widthProportion;
		float h = Screen.height * heightPropotion;
		float screenAspect = h / w;
		Vector2 centre = LabelCentre * new Vector2(Screen.width, Screen.height);

		float halfW = w * 0.5f;
		float halfH = h * 0.5f;

		GUIStyle style = new GUIStyle();
		style.alignment = TextAnchor.MiddleCenter;
		style.normal.textColor = Color.white;
		GUI.Label(new Rect(centre.x - halfW, centre.y - halfH, w, h), message, style);
	}
}
