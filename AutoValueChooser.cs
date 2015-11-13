using UnityEngine;
using System.Collections;

public class AutoValueChooser : MonoBehaviour {

	// UI elements for updating user while choosing
	public UnityEngine.UI.Text textBox;
	public static float[] trueValues;


	void Start () {
		textBox.text = "";
		trueValues = new float[3];

		// pick value for first object
		StartCoroutine (PickValueAuto(0));
	}


	/**
	 * 	Choose random value and paint child object with that value
	 */
	IEnumerator PickValueAuto (int index) {
		// timed display for user
		yield return new WaitForSeconds (1f);
		textBox.text = "Choosing a random value!";
		yield return new WaitForSeconds (2.6f);
		textBox.text = "";

		// randomize value and set object to that value
		float thisValue = Random.Range (0f,1f);
		trueValues[index] = thisValue;			// add this value to global list for answer checking
		this.transform.GetChild(index).GetComponent<MeshRenderer>().material.color = new Color (thisValue, thisValue, thisValue);

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
		textBox.text = "Try to match my values!";
		yield return new WaitForSeconds (1.5f);
		textBox.text = "";
		PlayerPaint.myTurn = true;
		yield return null;
	}

}
