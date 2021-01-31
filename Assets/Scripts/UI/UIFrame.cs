using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIFrame : MonoBehaviour
{
	public float panelThickness = 5.0f;
	public float sideLengthProportion = 0.2f;
	public GUISkin guiSkin;

	public Vector2 panelFrameCentre = new Vector2(0.5f, 0.5f); 


	void OnGUI()
	{
		DrawPanel();
	}

	void DrawPanel()
	{
		if (guiSkin != null)
			GUI.skin = guiSkin;

		DrawPanelWithFrameCenter(panelFrameCentre);
	}

	void DrawPanelWithFrameCenter(Vector2 frameCenter)
	{
		float w = Screen.width;
		float h = Screen.height;
		float screenAspect = h / w;
		Vector2 centre = frameCenter * new Vector2(w, h);

		float frameRectHeight = sideLengthProportion * h;
		float frameRectWidth = frameRectHeight;
		float halfW = frameRectWidth * 0.5f;
		float halfH = frameRectHeight * 0.5f;

		Vector2 leftBarXY = new Vector2(centre.x - halfW, centre.y - halfH);
		GUI.Box(new Rect(leftBarXY.x, leftBarXY.y, panelThickness, frameRectHeight), "");

		Vector2 rightBarXY = new Vector2(centre.x + halfW - panelThickness, centre.y - halfH);
		GUI.Box(new Rect(rightBarXY.x, rightBarXY.y, panelThickness, frameRectHeight), "");

		Vector2 topBarXY = new Vector2(centre.x - halfW, centre.y - halfH);
		GUI.Box(new Rect(topBarXY.x, topBarXY.y, frameRectWidth, panelThickness), "");

		Vector2 bottomBarXY = new Vector2(centre.x - halfW, centre.y + halfH - panelThickness);
		GUI.Box(new Rect(bottomBarXY.x, bottomBarXY.y, frameRectWidth, panelThickness), "");
	}
}
