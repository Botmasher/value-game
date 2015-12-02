using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AutoValueChooser : MonoBehaviour {

	// UI elements for updating user while choosing
	public UnityEngine.UI.Text textBox;
	public List <UnityEngine.UI.Text> textLeft;		// three text boxes beside the auto filled objects
	public static float[] trueValues;

	// exit keys
	public KeyCode exit1;
	public KeyCode exit2;


	void Start () {
		textBox.text = "";
		trueValues = new float[3];

		// pick value for first object
		StartCoroutine (PickValueAuto(0));
	}


	void Update () {

		// go back to the main menu
		if (Input.GetKeyDown(exit1) || Input.GetKeyDown(exit2)){
			Application.LoadLevel("scene-menu");
		}

	}


	/**
	 * 	Choose random value and paint child object with that value
	 */
	IEnumerator PickValueAuto (int index) {

		// timed display for user
		yield return new WaitForSeconds (1f);
		textLeft[index].CrossFadeAlpha(0f,0f,false);
		textLeft[index].CrossFadeAlpha(1f,0.6f,false);

		// play a writing sound
		if (Random.Range (0f,1f) > 0.5f) {
			AudioManager.playSfx_InkLong1 = true;
		} else {
			AudioManager.playSfx_InkLong2 = true;
		}

		// write text to screen
		textLeft[index].text = "Picking a value...";
		yield return new WaitForSeconds (0.8f);
		textLeft[index].CrossFadeAlpha(0f,0.8f,false);
		yield return new WaitForSeconds (0.8f);

		// erase text
		textLeft[index].text = "";
		textLeft[index].CrossFadeAlpha(1f,0f,false);

		// randomize value and set object to that value
		float thisValue = Random.Range (0f,1f);
		trueValues[index] = thisValue;			// add this value to global list for answer checking
		this.transform.GetChild(index).GetComponent<MeshRenderer>().material.color = new Color (thisValue, thisValue, thisValue);

		// play value selecting sound
		AudioManager.playSfx_Tapping = true;

		// continue painting all children
		index ++;
		if (index < this.gameObject.transform.childCount) {
			StartCoroutine (PickValueAuto(index));
		} else {
			StartCoroutine ("YourTurn");
		}

		yield return null;
	}


	// switch from AI choice turn to player turn
	IEnumerator YourTurn () {

		yield return new WaitForSeconds(0.5f);

		//fade text in and display game start message
		textBox.CrossFadeAlpha(0f,0f,false);
		textBox.CrossFadeAlpha(1f,0.5f,false);
		textBox.text = "Try to match my values!";

		// play writing sound
		AudioManager.playSfx_InkLong2 = true;

		yield return new WaitForSeconds (1.3f);

		// fade text out and reset it
		textBox.CrossFadeAlpha(0f,0.5f,false);
		yield return new WaitForSeconds (0.8f);
		textBox.CrossFadeAlpha(1f,0f,false);
		textBox.text = "";

		// pass control to player
		PlayerPaint.myTurn = true;
		yield return null;
	}

}
