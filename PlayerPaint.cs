using UnityEngine;
using System.Collections;

public class PlayerPaint : MonoBehaviour {

	// color picker objects
	public GameObject picker;
	public GameObject pickerButton;
	private GameObject lastSelected;
	private Texture2D tex;

	// for determining if player can act
	public static bool myTurn;

	// for storing player answers
	float[] myValues;
	public GameObject gameOverButton;

	// for collecting mouse raycast
	RaycastHit hit;
	Ray ray;

	// for painting
	public static Color brushColor;


	void Start () {
		myTurn = false;
		brushColor = Color.white;
		picker.SetActive (false);
		myValues = new float[3];
	}


	void Update () {
		if (myTurn) {
			// toggle value picker
			if (Input.GetMouseButtonDown(0)) {
				ray = Camera.main.ScreenPointToRay(Input.mousePosition);
				Physics.Raycast(ray, out hit);
				// hit paint target - bring up picker and remember this object
				if (hit.collider.tag == "Paint") {
					lastSelected = hit.collider.gameObject;
					picker.SetActive (true);
				// hit okay button - confirm value and hide picker
				} else if (hit.collider.gameObject == pickerButton) {
					picker.SetActive (false);
					lastSelected.GetComponent<MeshRenderer>().material.color = brushColor;
					// add to list of answers for final checks
					for (int i=0; i<this.transform.childCount; i++) {
						if (this.transform.GetChild(i) == lastSelected) {
							myValues[i] = brushColor[0];
						}
					}
				// hit gameover button - end round and tally score
				} else if (hit.collider.gameObject == gameOverButton) {
					StartCoroutine ("EndRound");
				}
			// drag mouse along value picker - update this color based on exact value texture pixel
			} else if (Input.GetMouseButton(0)) {
				ray = Camera.main.ScreenPointToRay(Input.mousePosition);
				Physics.Raycast(ray, out hit);
				if (hit.collider.gameObject == picker) {
					// analyze the texture pixels on this object for the value at this mouse position
					tex = hit.collider.GetComponent<MeshRenderer>().material.mainTexture as Texture2D;
					brushColor = tex.GetPixel ((int)(hit.textureCoord.x*tex.width), (int)(hit.textureCoord.y*tex.height));
					// set the clicked object to this value
					lastSelected.GetComponent<MeshRenderer>().material.color = brushColor;
				}
			}
		}
	}


	IEnumerator EndRound () {
		Debug.Log ("ending round!");
		float howMuchOff = 0f;
		yield return new WaitForSeconds (2f);
		for (int i=0; i<AutoValueChooser.trueValues.Length; i++) {
			howMuchOff += Mathf.Abs (AutoValueChooser.trueValues[i] - myValues[i]);
		}
		Debug.Log (howMuchOff);
	}


}
