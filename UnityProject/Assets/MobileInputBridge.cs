using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MobileInputBridge : MonoBehaviour
{
    public static MobileInputBridge instance;

    public RightJoystick lookJoystick;

    public AimScript aimScript_ref;

    public List<string> ButtonsPressed = new List<string>();
    public List<string> ButtonsPressedDown = new List<string>();
    public List<string> ButtonsPressedUp = new List<string>();

    void Awake()
    {
        instance = this;
    }

    private IEnumerator Start() {
        yield return null;
        StartCoroutine(ButtonClearer());
    }

    IEnumerator ButtonClearer() {
        yield return null;
        ButtonsPressedDown.Clear();
        ButtonsPressedUp.Clear();
        StartCoroutine(ButtonClearer());
    }

    public void SetButtonPressed(GameObject obj) {
        ButtonsPressed.Add(obj.name);
        ButtonsPressedDown.Add(obj.name);
    }

    public void SetButtonReleased(GameObject obj) {
        if (ButtonsPressed.Contains(obj.name)) {
            ButtonsPressed.Remove(obj.name);
        }
        ButtonsPressedUp.Add(obj.name);
    }

    public bool GetButtonDown(string input_str) {
        return ButtonsPressedDown.Contains(input_str.ToLower());
        /*
        switch (input_str) {
            case "Slide Lock":
                return false;
            case "Safety":
                return false;
            case "Auto Mod Toggle":
                return false;
            case "Pull Back Slide":
                return false;

            case "Swing Out Cylinder":
                return false;

            case "Close Cylinder":
                return false;

            case "Insert":
                return false;

            case "Eject/Drop":
                return false;
            case "Flashlight Toggle":
                return false;
            case "Inventory 1":
                    return false;

            case "Inventory 2":
                    return false;

            case "Inventory 3":
                    return false;

            case "Inventory 4":
                    return false;

            case "Inventory 5":
                    return false;
                
            case "Inventory 6":
                    return false;

            case "Inventory 7":
                    return false;

            case "Inventory 8":
                    return false;

            case "Inventory 9":
                    return false;

            case "Inventory 10":
                    return false;

            case "Tape Player":
                    return false;

            case "Holster":
                    return false;

            case "Hammer":
                return false;
            default:
                return Input.GetButtonDown(input_str);
        }*/
    }


    public bool GetButton(string input_str) {
        return ButtonsPressed.Contains(input_str.ToLower());
        /*
        switch (input_str) {
            case "Trigger":
                return false;

            case "Slide Lock":
                return false;

            case "Extractor Rod":
                return false;

            case "Hammer":
                return false;

            case "Get":
                return false;

            case "Pull Back Slide":
                    return false;

            case "Pull Back Slide Press Check":
                return false;

            default:
                return Input.GetButton(input_str);
        }*/
        
    }

    private void Update() {
        //MagOut = (aimScript_ref.magazine_instance_in_hand != null);

        //Debug.DrawLine(VRInputController.instance.RightHand.transform.position, VRInputController.instance.RightHand.transform.position + VRInputController.instance.RightHand.transform.rotation*closeDirection, Color.red);
    }

    public bool GetButtonUp(string input_str) {
        return ButtonsPressedUp.Contains(input_str.ToLower());
        /*switch (input_str) {
            case "Slide Lock":
                return false;
            case "Pull Back Slide":
                return false;
            case "Hammer":
                return false;
            default:
                return Input.GetButtonUp(input_str);
        }*/
        
    }
    public float GetAxis(string input_str) {
        float ForwBackw = ButtonsPressed.Contains("w") ? 1f : ButtonsPressed.Contains("s") ? -1f : 0f;

        float LeftRight = ButtonsPressed.Contains("a") ? -1f : ButtonsPressed.Contains("d") ? 1f : 0f;

        switch (input_str) {
            case "Mouse ScrollWheel":
                return 0f;
            case "Mouse X":
                return lookJoystick.GetInputDirection().magnitude > 2.5f? lookJoystick.GetInputDirection().x / 10f:0;
            case "Mouse Y":
                return lookJoystick.GetInputDirection().magnitude > 2.5f ? lookJoystick.GetInputDirection().y / 10f : 0;
            case "Vertical":
                return ForwBackw;
            case "Horizontal":
                return LeftRight;
            default:
                return Input.GetAxis(input_str);
        }
        
    }
}
