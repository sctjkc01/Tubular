using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ButtonColorChange : MonoBehaviour {
	
	//Reference to button to access its components
	private Button theButton;
	
	//this get the Transitions of the Button as its pressed
	private ColorBlock theColor;

	//colorCounter for color switch
	int colorCounter = 0; 
	//readyCounter for ready switch
	int readyCounter = 0;
	
	// Use this for initialization
	void Awake () {
		theButton = GetComponent<Button>();
		theColor = GetComponent<Button>().colors;
		
	}
	
	
	/// <summary>
	/// Example
	/// just add this to your Button component On Click()
	/// </summary>
	public void ButtonTransitionColors()
	{
		colorCounter++; //add colorcolorCounter to switch color
		if (colorCounter > 5) { //reset colorCounter if above 4
			colorCounter = 0;
		}

		switch (colorCounter)
		{
			case 0:
				theColor.highlightedColor = Color.blue;
				theColor.normalColor = Color.blue;
				theColor.pressedColor = Color.blue;
				
				theButton.colors = theColor;
			break;

			case 1:
				theColor.highlightedColor = Color.red;
				theColor.normalColor = Color.red;
				theColor.pressedColor = Color.red;
				
				theButton.colors = theColor;
				break;

			case 2:
				theColor.highlightedColor = Color.green;
				theColor.normalColor = Color.green;
				theColor.pressedColor = Color.green;
				
				theButton.colors = theColor;
				break;

			case 3:
				theColor.highlightedColor = Color.yellow;
				theColor.normalColor = Color.yellow;
				theColor.pressedColor = Color.yellow;
				
				theButton.colors = theColor;
				break;

			case 4:
				theColor.highlightedColor = Color.magenta;
				theColor.normalColor = Color.magenta;
				theColor.pressedColor = Color.magenta;
				
				theButton.colors = theColor;
				break;
			case 5:
				theColor.highlightedColor = Color.white;
				theColor.normalColor = Color.white;
				theColor.pressedColor = Color.white;
				
				theButton.colors = theColor;
				break;
		}
	}

	public void ReadyColorSwitch()
	{
		readyCounter++; //add colorcolorCounter to switch color
		if (readyCounter > 1) { //reset colorCounter if above 4
			readyCounter = 0;
		}
		
		switch (readyCounter)
		{
		case 0:
			theColor.highlightedColor = Color.red;
			theColor.normalColor = Color.red;
			theColor.pressedColor = Color.red;
			
			theButton.colors = theColor;
			break;
			
		case 1:
			theColor.highlightedColor = Color.green;
			theColor.normalColor = Color.green;
			theColor.pressedColor = Color.green;
			
			theButton.colors = theColor;
			break;
		}
	}
}
