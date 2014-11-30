using UnityEngine;
using System.Collections.Generic;
using Sharpduino;
using Sharpduino.Constants;

public class UnityFirmataTest : MonoBehaviour {

	private ArduinoUno arduino;
	private bool ledOn = false;

	// Use this for initialization
	void Start () {
		arduino = new ArduinoUno("/dev/tty.usbmodemfd121");
	}
	
	// Update is called once per frame
	void Update () {
	}
				
    void OnGUI() {
		// LED Update
		var ledRect = new Rect(10, 50, 50, 50);
		var tmp = GUI.Toggle(ledRect, ledOn, "LED");
		if(ledOn != tmp) {
			Debug.Log(tmp);
			ledOn = tmp;
			arduino.SetPinMode(ArduinoUnoPins.D13,PinModes.Output);
			arduino.SetDO (ArduinoUnoPins.D13, ledOn);
		}
    }
	
	void OnApplicationQuit() 
    {
		if(arduino != null) {
			arduino.Dispose ();
		}
    }
}
