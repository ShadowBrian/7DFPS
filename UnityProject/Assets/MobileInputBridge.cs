using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MobileInputBridge : MonoBehaviour
{
    public static MobileInputBridge instance;

    public RightJoystick lookJoystick;

    public AimScript aimScript_ref;

    public List<string> ButtonsPressed = new List<string>();
    public List<string> ButtonsPressedDown = new List<string>();
    public List<string> ButtonsPressedUp = new List<string>();

    public GameObject GyroEnabledText;

    Vector3 GyroDelta, LastGyro;

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

    bool GyroAimAdd = false;

    private void Update() {
        GyroDelta = Vector3.Lerp(GyroDelta,(Input.gyro.rotationRateUnbiased/36f) * 50f,0.5f);

        

        if (ButtonsPressedDown.Contains("gyro toggle")) {
            GyroAimAdd = !GyroAimAdd;
            
            GyroEnabledText.SetActive(GyroAimAdd);
            if (!SystemInfo.supportsGyroscope) {
                GyroEnabledText.GetComponent<Text>().text = "Gyroscope not supported by device.";
            }
            else {
                
                Input.gyro.enabled = true;
                //LastGyro = (Input.gyro.attitude * Vector3.forward * 100f);
            }
        }

        //LastGyro = (Input.gyro.attitude * Vector3.forward * 100f);
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

        float MouseX = lookJoystick.GetInputDirection().magnitude > 2.5f ? lookJoystick.GetInputDirection().x / 10f : 0;

        float MouseY = lookJoystick.GetInputDirection().magnitude > 2.5f ? lookJoystick.GetInputDirection().y / 10f : 0;

        if (GyroAimAdd) {
            MouseX += -GyroDelta.y;
            MouseY += GyroDelta.x;
        }

        switch (input_str) {
            case "Mouse ScrollWheel":
                return 0f;
            case "Mouse X":
                return MouseX;
            case "Mouse Y":
                return MouseY;
            case "Vertical":
                return ForwBackw;
            case "Horizontal":
                return LeftRight;
            default:
                return Input.GetAxis(input_str);
        }
        
    }
}
