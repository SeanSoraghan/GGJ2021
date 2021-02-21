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
	public bool showBegin = true;

	void OnGUI()
	{
		string beginMessage = "[Left Mouse] or [Spacebar] to begin...";
		string message1 = display ? "Match 'current' to 'target'. [Left Mouse] or [Spacebar] to confirm." : "Drawing level ...";
		string message2 = display ? "Use [A] and [D] to tilt the tile." : "";
		if (guiSkin != null)
			GUI.skin = guiSkin;

		float w = Screen.width * widthProportion;
		// I'm hacking in another label below the 'message' label by dividing the height proportion by 2, and splitting it among the two labels.
		float h = Screen.height * heightPropotion * 0.5f;
		float screenAspect = h / w;
		Vector2 beginCentre = LabelCentre * new Vector2(Screen.width, Screen.height);
		// Here I'm using 'LabelCentre.y - heightPropotion * 0.5f * 0.5f' to centre both labels vertically in the centre of the two heightProportion halves...
		Vector2 message1Centre = new Vector2 (LabelCentre.x, LabelCentre.y - heightPropotion * 0.5f * 0.5f) * new Vector2(Screen.width, Screen.height);
		Vector2 message2Centre = new Vector2 (LabelCentre.x, LabelCentre.y + heightPropotion * 0.5f * 0.5f) * new Vector2(Screen.width, Screen.height);

		float halfW = w * 0.5f;
		float halfH = h * 0.5f;

		GUIStyle style = new GUIStyle();
		style.alignment = TextAnchor.MiddleCenter;
		style.normal.textColor = Color.black;
		if (showBegin)
		{
			GUI.Label(new Rect(beginCentre.x - halfW, beginCentre.y - halfH, w, h), beginMessage, style);
		}
		else
		{
			GUI.Label(new Rect(message1Centre.x - halfW, message1Centre.y - halfH, w, h), message1, style);
			GUI.Label(new Rect(message2Centre.x - halfW, message2Centre.y - halfH, w, h), message2, style);
		}
	}
}
