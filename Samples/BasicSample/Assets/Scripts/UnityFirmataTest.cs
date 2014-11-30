using UnityEngine;
using System.Collections.Generic;
using Sharpduino;
using Sharpduino.Constants;

public class UnityFirmataTest : MonoBehaviour {

	private ArduinoLeo arduino;
	private bool ledOn = false;

	// Use this for initialization
	void Start () {
		arduino = new ArduinoLeo("/dev/tty.usbmodemfd121");
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
			arduino.pinMode(ArduinoLeoPins.D13, PinModes.Output);
			arduino.digitalWrite (ArduinoLeoPins.D13, ledOn ? ArduinoConstants.HIGH : ArduinoConstants.LOW);
		}

		//arduino.pinMode(ArduinoLeoPins.D2, PinModes.Input);
		//int value = arduino.digitalRead(ArduinoLeoPins.D2);
		//GUI.Label(new Rect(10, 100, 300, 50), "D2: " + value); 

		arduino.pinMode(ArduinoLeoPins.A0, PinModes.Analog);
		int value = arduino.analogRead(ArduinoLeoAnalogPins.A0);
		GUI.Label(new Rect(10, 150, 300, 50), "A0: " + value); 

		arduino.pinMode (ArduinoLeoPins.D9_PWM, PinModes.PWM);
		arduino.SetPWM (ArduinoLeoPWMPins.D9_PWM, value / 4);
    }
	
	void OnApplicationQuit() 
    {
		if(arduino != null) {
			arduino.Dispose ();
		}
    }
}
