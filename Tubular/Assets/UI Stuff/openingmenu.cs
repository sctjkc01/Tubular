using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class openingmenu : MonoBehaviour 
{
	public GameObject rawimage_panel;
	public GameObject title_panel;
	public GameObject login_panel;
	public GameObject instructions_panel;
	public GameObject credits_panel;
	private List<GameObject> somepanels = new List<GameObject>();

	private List<Texture> arrayofimages = new List<Texture> ();
	public Texture bgimage_1;
	public Texture bgimage_2;
	private int imgindex;

	private RawImage img;

	// Use this for initialization
	void Start () 
	{
		somepanels.Add ((GameObject)login_panel);
		somepanels.Add ((GameObject)instructions_panel);
		somepanels.Add ((GameObject)credits_panel);
		arrayofimages.Add ((Texture)bgimage_1);
		arrayofimages.Add ((Texture)bgimage_2);
		imgindex = 0;
	}
	
	// Update is called once per frame
	void Update () 
	{
		img = (RawImage)rawimage_panel.GetComponent<RawImage> ();
		img.texture = (Texture)arrayofimages[imgindex];
	}

	public void loadtitle()
	{
		title_panel.SetActive (true);
		for(var i=0; i < somepanels.Count; i++)
		{
			somepanels[i].SetActive(false);
		}
		nextimage ();
	}
	public void loadlogin()
	{
		login_panel.SetActive (true);
		title_panel.SetActive (false);
		nextimage ();
	}
	public void loadinstructions()
	{
		instructions_panel.SetActive(true);
		title_panel.SetActive(false);
		nextimage();
	}
	public void loadcredits()
	{
		credits_panel.SetActive (true);
		title_panel.SetActive (false);
		nextimage ();
	}

	public void nextimage()
	{
		imgindex ++;
		if(imgindex > arrayofimages.Count-1)
		{
			imgindex = 0;
		}
	}
}
