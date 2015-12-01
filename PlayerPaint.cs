using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerPaint : MonoBehaviour {

	// color picker objects
	public GameObject picker;						// for checking that hit object is gradient texture
	public GameObject pickerButton;					// object clicked to confirm value selected from gradient
	private GameObject lastSelected;				// object clicked before toggling the color picker
	private Texture2D tex;							// comparing gradient texture for extracting values
	public UnityEngine.UI.InputField inputText;		// color picker helper box for inputting values directly
	private float inputValue;						// sanitize input values from the text box

	// text boxes for updating user
	public List<UnityEngine.UI.Text> textCenter;
	public List<UnityEngine.UI.Text> textLeft;
	public List<UnityEngine.UI.Text> textRight;
	public UnityEngine.UI.Text textAnswer;

	// screen images
	public UnityEngine.UI.Image screenFader;	// UI fade to color
	public GameObject midlineImage;				// line drawn in middle of board

	// control flow
	public static bool myTurn;				// for determining if player can act
	private bool isPicking;					// for determining if player can select an object to color

	// for storing player answers
	float[] myValues;
	public GameObject gameOverButton;

	// for collecting mouse raycast
	RaycastHit hit;
	Ray ray;

	// click-picked color for painting
	public static Color brushColor;


	void Start () {
		// clear UI
		for (int i=0; i<textCenter.Count; i++) {
			textCenter[i].text = "";
			textRight[i].text = "";
			textLeft[i].text = "";
		}
		textAnswer.text = "";

		// fade in screen at beginning of game
		screenFader.enabled = true;
		screenFader.CrossFadeAlpha(0f,1f,false);

		// hide divider line and roundover button
		gameOverButton.SetActive (false);
		midlineImage.SetActive (false);

		// wait for AI to take turn
		myTurn = false;

		// setup value picker
		brushColor = Color.white;
		isPicking = false;
		picker.SetActive (false);
		inputText.gameObject.SetActive (false);
		myValues = new float[3];
	}


	void Update () {

		// only run once AI has chosen
		if (myTurn) {

			// turn on center divider line
			if (!midlineImage.activeSelf) {
				midlineImage.SetActive (true);
			}

			// turn on the end round button at the start of your turn
			if (!gameOverButton.activeSelf && !isPicking) {
				gameOverButton.SetActive (true);
			}

			// toggle value picker
			if (Input.GetMouseButtonDown(0)) {
				ray = Camera.main.ScreenPointToRay(Input.mousePosition);
				Physics.Raycast(ray, out hit);

				// hit paint target - bring up picker and remember this object
				if (hit.collider != null && hit.collider.tag == "Paint" && !isPicking) {
					// remember the clicked object
					lastSelected = hit.collider.gameObject;
					// calibrate and toggle the value picker-slider
					isPicking = true;
					picker.SetActive (true);
					inputText.gameObject.SetActive (true);
					gameOverButton.SetActive (false);
					brushColor = lastSelected.GetComponent<MeshRenderer>().material.color;
					inputText.text = Mathf.RoundToInt(brushColor.r*100f).ToString();
				
				// hit okay button - confirm value and hide picker
				} else if (hit.collider != null && hit.collider.gameObject.tag != "Picker" && hit.collider.gameObject != gameOverButton) {
					isPicking = false;
					picker.SetActive (false);
					inputText.gameObject.SetActive (false);
					lastSelected.GetComponent<MeshRenderer>().material.color = brushColor;
//				} else if (hit.collider != null && hit.collider.gameObject == pickerButton) {
//					isPicking = false;
//					picker.SetActive (false);
//					inputText.gameObject.SetActive (false);
//					lastSelected.GetComponent<MeshRenderer>().material.color = brushColor;
				
				// hit gameover button - end round and tally score
				} else if (hit.collider != null && hit.collider.gameObject == gameOverButton) {
					StartCoroutine ("EndRound");
				}

//			// drag mouse along value picker - update this color based on exact value texture pixel
//			} else if (Input.GetMouseButton(0)) {
//				ray = Camera.main.ScreenPointToRay(Input.mousePosition);
//				Physics.Raycast(ray, out hit);
//
//				// choose and store the player picked color along this gradient
//				if (hit.collider != null && hit.collider.gameObject == picker) {
//					// analyze the texture pixels on this object for the value at this mouse position
//					tex = hit.collider.GetComponent<MeshRenderer>().material.mainTexture as Texture2D;
//					brushColor = tex.GetPixel ((int)(hit.textureCoord.x*tex.width), (int)(hit.textureCoord.y*tex.height));
//					// set the clicked object to this value
//					//inputText.text = Mathf.RoundToInt(brushColor.r*100f).ToString();
//					lastSelected.GetComponent<MeshRenderer>().material.color = brushColor;
//				}

			// pick a value from slider or input field
			} else if (isPicking) {

				// input field text - parse the integer
				int this_integer;
				if (inputText.text == "") {
					// do nothing
				} else if (int.TryParse (inputText.text, out this_integer)) {
					this_integer = Mathf.Clamp (this_integer, 0, 100);
					// set the brush value to this integer
					inputValue = this_integer/100f;
					brushColor = new Color (inputValue, inputValue, inputValue);
				}

				// slider bar texture - determine the tex value at this input position
				if (Input.GetMouseButton(0)) {
					ray = Camera.main.ScreenPointToRay(Input.mousePosition);
					Physics.Raycast(ray, out hit);

					// choose and store the player picked color along this gradient
					if (hit.collider != null && hit.collider.gameObject == picker) {
						// analyze the texture pixels on this object for the value at this mouse position
						tex = hit.collider.GetComponent<MeshRenderer>().material.mainTexture as Texture2D;
						brushColor = tex.GetPixel ((int)(hit.textureCoord.x*tex.width), (int)(hit.textureCoord.y*tex.height));
					}
				}
				// update the object and text to reflect the chosen value
				lastSelected.GetComponent<MeshRenderer>().material.color = brushColor;
				inputText.text = Mathf.RoundToInt(brushColor.r*100f).ToString();
			}

		}

	}


	/**
	 * 	Finish round, compare player values to AI and tally score
	 */
	IEnumerator EndRound () {
		// turn off gameplay elements
		myTurn = false;
		gameOverButton.SetActive (false);
		midlineImage.SetActive (false);

		// message user - tallying score
		textCenter[1].CrossFadeAlpha (0f, 0f, false);
		textCenter[1].text = "Tallying your score";
		textCenter[1].CrossFadeAlpha (1f, 0.6f, false);
		yield return new WaitForSeconds (1f);
		textCenter[1].CrossFadeAlpha (0f, 0.5f, false);
		yield return new WaitForSeconds (0.5f);
		textCenter[1].text = "";
		textCenter[1].CrossFadeAlpha (1f, 0f, false);

		// compile list of answers for final check
		for (int i=0; i<this.transform.childCount; i++) {
			myValues[i] = this.transform.GetChild(i).GetComponent<MeshRenderer>().material.color.r;
		}

		// compare your values versus true values and show error diff
		int howMuchOff = 0;
		yield return new WaitForSeconds (0.2f);
		for (int i=0; i<AutoValueChooser.trueValues.Length; i++) {

			// display your value then wait
			textRight[i].text = string.Format("{0}", Mathf.RoundToInt(myValues[i]*100f));
			yield return new WaitForSeconds(0.5f);

			// display true value then wait
			textLeft[i].text = string.Format("{0}", Mathf.RoundToInt(AutoValueChooser.trueValues[i]*100f));
			yield return new WaitForSeconds(0.5f);

			// display difference
//			textRight[i].text = "";
//			textLeft[i].text = "";
			howMuchOff += Mathf.RoundToInt(Mathf.Abs (AutoValueChooser.trueValues[i] - myValues[i])*100f);
			textCenter[i].text = string.Format("{0}", Mathf.RoundToInt(Mathf.Abs(AutoValueChooser.trueValues[i] - myValues[i])*100f));
			yield return new WaitForSeconds(1.5f);
		}

		// display final score
		textAnswer.text = string.Format("{0}", howMuchOff);

		yield return new WaitForSeconds (1.2f);
		screenFader.CrossFadeAlpha (1f, 2f, false);
		yield return new WaitForSeconds (2f);
		textAnswer.CrossFadeAlpha (0f, 1.8f, false);
		yield return new WaitForSeconds (2f);
		Application.LoadLevel ("scene-menu");
	}


}
