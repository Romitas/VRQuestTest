using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Button : MonoBehaviour {

    public bool local = true; // if this button belongs to this player (can be clicked)
    public bool action = true; // 1 - turn on the light; 0 - turn off the light

    public GameObject target; // the flame GameObject
    public Material matButtonOn; // green material
    public Material matButtonOff; // red material

    //private GameObject parentPlayer;
    private Client gameController;
    private bool isDown = false;

    private Material sourceMaterial;
    private Material highlightMaterial;

    // Use this for initialization
    void Start () {
        // initializing materials depending on action, setting up highlight materials
        if (this.action) sourceMaterial = new Material(matButtonOn);
        else sourceMaterial = new Material(matButtonOff);

        highlightMaterial = new Material(sourceMaterial);
        highlightMaterial.color = LightenColor(highlightMaterial.color);
        gameObject.GetComponent<Renderer>().material = sourceMaterial;

        //parentPlayer = transform.parent.gameObject;
        gameController = GameObject.FindGameObjectWithTag("GameController").GetComponent<Client>();
        if (!target) target = GameObject.FindGameObjectWithTag("Flame");
    }
	
	// Update is called once per frame
	void Update () { }

    void Click() {
        if (!local) return;

        isDown = true;
        gameObject.transform.Translate(new Vector3(0f, -0.2f, 0f));

        gameController.SendMessageSwitch(action);
    }

    void Unclick() {
        if (!local) return;

        if (isDown) {
            isDown = false;
            gameObject.transform.Translate(new Vector3(0f, 0.2f, 0f));
        }
    }

    Color LightenColor(Color color) {
        float multiplier = 2.5f;
        return new Color(color.r * multiplier, color.g * multiplier, color.b * multiplier);
    }

    void Highlight() {
        if (!local) return;
        gameObject.GetComponent<Renderer>().material = this.highlightMaterial;
    }
    void UnHighlight() {
        if (!local) return;
        gameObject.GetComponent<Renderer>().material = this.sourceMaterial; 
    }

    void OnMouseEnter() { Highlight(); }
    void OnMouseDown() { Click(); }

    void OnMouseUp() { Unclick(); }
    void OnMouseExit() { UnHighlight(); Unclick(); }
}
