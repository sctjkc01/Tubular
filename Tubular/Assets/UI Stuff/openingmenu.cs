using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class openingmenu : MonoBehaviour 
{
	public GameObject title_panel;
	public GameObject login_panel;
	public GameObject instructions_panel;
	public GameObject credits_panel;
	private List<GameObject> somepanels = new List<GameObject>();

	// Use this for initialization
	void Start () 
	{
		somepanels.Add ((GameObject)login_panel);
		somepanels.Add ((GameObject)instructions_panel);
		somepanels.Add ((GameObject)credits_panel);
	}

	public void loadtitle()
	{
		title_panel.SetActive (true);
		for(var i=0; i < somepanels.Count; i++)
		{
			somepanels[i].SetActive(false);
		}
	}
	public void loadlogin()
	{
		login_panel.SetActive (true);
		title_panel.SetActive (false);
	}
	public void loadinstructions()
	{
		instructions_panel.SetActive(true);
		title_panel.SetActive(false);
	}
	public void loadcredits()
	{
		credits_panel.SetActive (true);
		title_panel.SetActive (false);
	}
}
