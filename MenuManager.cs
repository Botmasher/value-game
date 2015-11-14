using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class MenuManager : MonoBehaviour {

	// UI objects
	private List<Text> texts;		// hold both child text objects
	private Color changingColor;	// color to lerp texts over time
	public Image screenFader;		// overlay image

	// raycast check for user interaction
	private RaycastHit hit;
	private Ray ray;

	// buttons
	public GameObject startButton;
	public GameObject aboutButton;

	// control flow
	private bool isUsingMenu;


	void Start () {

		// fade in from overlay image
		screenFader.CrossFadeAlpha (0f, 3f, false);

		// store all menu text items
		texts = new List<Text>();
		for (int i=0; i < this.gameObject.GetComponentsInChildren<Text>().Length; i++) {
			texts.Add (gameObject.GetComponentsInChildren<Text>()[i]);
		}

		isUsingMenu = true;

	}
	

	void Update () {

		// fx fade texts in and out over time
		SlowlyOscillateAlpha<Text>(texts[0]);
		SlowlyOscillateAlpha<Text>(texts[1]);

		// raycast to select buttons
		ray = Camera.main.ScreenPointToRay (Input.mousePosition);
		if (isUsingMenu && Physics.Raycast (ray, out hit)) {

			// hover over start
			if (hit.collider.gameObject == startButton) {
				texts[0].fontSize = 35;
				texts[1].fontSize = 24;
				// select start
				if (Input.GetMouseButtonDown (0)) {
					StartCoroutine ("StartGame");
				}
			
			// hover over about
			} else if (hit.collider.gameObject == aboutButton) {
				texts[1].fontSize = 30;
				texts[0].fontSize = 30;
				// select about
				if (Input.GetMouseButtonDown(0)) {
					Application.OpenURL("https://www.youtube.com/watch?v=J5zVFlMQZEs");
				}
			}

		// not interacting with menu
		} else {
			texts[0].fontSize = 30;
			texts[1].fontSize = 24;
		}
	
	}


	/**
	 * 	Text FX - fade in and out slowly over time
	 */
	void SlowlyOscillateAlpha <T> (T alphable) where T:Text {

		// fade towards transparent
		if (alphable.color.a >= 0.9f) {
			changingColor = new Color (alphable.color.r, alphable.color.g, alphable.color.b, 0f);

		// fade towards opaque
		} else if (alphable.color.a <= 0.3f) {
			changingColor = new Color (alphable.color.r, alphable.color.g, alphable.color.b, 1f);
		}

		// apply chosen transparency
		alphable.color = Color.Lerp (alphable.color, changingColor, 0.34f*Time.deltaTime);
	}


	/**
	 *	Go from this scene to the main game scene
	 */ 
	IEnumerator StartGame () {

		// turn off menu - partially to avoid calling this routine twice
		isUsingMenu = false;

		// fade in overlay image
		screenFader.CrossFadeAlpha (1f, 1.1f, false);;
		yield return new WaitForSeconds (1.3f);
		Application.LoadLevel ("scene-main");

	}

}