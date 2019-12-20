using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using Valve.VR;

public enum HandSide {
    Right,
    Left
}

public class VRInputController : MonoBehaviour
{
    public static VRInputController instance;

    public GameObject LeftHand,RightHand,Head,InventoryPos;

    public HandScript LhandScr, RhandScr;

    public float TallestHead = 0.1f;

    public OVRInput.Axis2D Locomotion;

    public OVRInput.Button ActionButton, JumpButton, CollectButton, GunInteract1Btn, GunInteract2Btn, GunInteract3Btn, GunInteractLongBtn, RotateLeft, RotateRight, ChangeHandedness;

    public GameObject LHandSphere, RHandSphere;

    public Renderer cylinderRenderer;

    //public SteamVR_Action_Pose ControllerPose;

    private void Awake() {
        instance = this;

    }

    IEnumerator Start() {
        yield return new WaitForSeconds(1f);
        if (VRInputBridge.instance.aimScript_ref.primaryHand == HandSide.Left) {
            LHandSphere.SetActive(false);
            RHandSphere.SetActive(true);
        }
        else {
            LHandSphere.SetActive(true);
            RHandSphere.SetActive(false);
        }

        OVRManager.fixedFoveatedRenderingLevel = OVRManager.FixedFoveatedRenderingLevel.High;
        OVRManager.cpuLevel = 2;
        OVRManager.gpuLevel = 2;
        //Camera.main.allowDynamicResolution = true;
        
    }

    public Vector3 GetAimPos(HandSide hand) {
        switch (hand) {
            case HandSide.Right:
                return RightHand.transform.TransformPoint(RhandScr.offsetPos.localPosition) - GetAimDir(hand)*0.02f;
            case HandSide.Left:
                return LeftHand.transform.TransformPoint(RhandScr.offsetPos.localPosition) - GetAimDir(hand) * 0.02f;
            default:
                return RightHand.transform.position;
        }
    }

    bool checkedCylinderRenderer;

    public float GetSpinSpeed(HandSide hand) {
        if(!checkedCylinderRenderer && VRInputBridge.instance.aimScript_ref != null && VRInputBridge.instance.aimScript_ref.gun_script.magazineType == MagazineType.CYLINDER) {
            cylinderRenderer = VRInputBridge.instance.aimScript_ref.gun_instance.transform.Find("yolk_pivot").Find("yolk").Find("cylinder_assembly").Find("cylinder").GetComponent<Renderer>();
            checkedCylinderRenderer = true;
        }
        if (cylinderRenderer != null) {
            switch (hand) {
                case HandSide.Right:
                    if (cylinderRenderer.bounds.Contains(RightHand.transform.TransformPoint(RhandScr.offsetPos.localPosition))) {
                        return -OVRInput.GetLocalControllerVelocity(OVRInput.Controller.RTouch).y;
                    }
                    else {
                        return 0f;
                    }
                case HandSide.Left:
                    if (cylinderRenderer.bounds.Contains(LeftHand.transform.TransformPoint(LhandScr.offsetPos.localPosition))) {
                        return -OVRInput.GetLocalControllerVelocity(OVRInput.Controller.LTouch).y;
                    }
                    else {
                        return 0f;
                    }
                default:
                    return -OVRInput.GetLocalControllerVelocity(OVRInput.Controller.RTouch).y;
            }
        }
        else {
            return 0f;
        }
    }

    public Vector3 GetAimDir(HandSide hand) {
        switch (hand) {
            case HandSide.Right:
                return RhandScr.offsetPos.forward;
            case HandSide.Left:
                return LhandScr.offsetPos.forward;
            default:
                return RightHand.transform.forward;
        }
    }

    public Vector3 GetAimUp(HandSide hand) {
        switch (hand) {
            case HandSide.Right:
                return RightHand.transform.forward + (RightHand.transform.up*1.5f);
            case HandSide.Left:
                return LeftHand.transform.forward + (LeftHand.transform.up*1.5f);
            default:
                return RightHand.transform.up;
        }
    }

    public Vector2 GetWalkVector(HandSide hand) {
        OVRInput.Controller source = (hand == HandSide.Left ? OVRInput.Controller.RTouch : OVRInput.Controller.LTouch);
        Vector3 rawAxis =  new Vector3(OVRInput.Get(Locomotion,source).x, 0, OVRInput.Get(Locomotion, source).y);
        rawAxis = Head.transform.localRotation * rawAxis;
        return new Vector2(rawAxis.x,rawAxis.z);
    }

    public bool GetRotateLeft(HandSide hand) {
        return OVRInput.GetDown(RotateLeft,(hand == HandSide.Left? OVRInput.Controller.LTouch : OVRInput.Controller.RTouch)) && OVRInput.GetDown(JumpButton, (hand == HandSide.Right ? OVRInput.Controller.LTouch : OVRInput.Controller.RTouch));
    }

    public bool GetRotateRight(HandSide hand) {
        return OVRInput.GetDown(RotateRight, (hand == HandSide.Left ? OVRInput.Controller.LTouch : OVRInput.Controller.RTouch)) && OVRInput.GetDown(JumpButton, (hand == HandSide.Right ? OVRInput.Controller.LTouch : OVRInput.Controller.RTouch));
    }

    public bool JumpPress(HandSide hand) {
        return OVRInput.Get(JumpButton, (hand == HandSide.Left ? OVRInput.Controller.RTouch : OVRInput.Controller.LTouch));
    }

    public bool CollectPress(HandSide hand) {
        return OVRInput.Get(CollectButton, (hand == HandSide.Left ? OVRInput.Controller.LTouch : OVRInput.Controller.RTouch));
    }

    public bool ActionPress(HandSide hand) {
        return OVRInput.Get(ActionButton, (hand == HandSide.Left ? OVRInput.Controller.LTouch : OVRInput.Controller.RTouch));
    }

    public bool GunInteract(HandSide hand) {
        return OVRInput.Get(GunInteract1Btn, (hand == HandSide.Left ? OVRInput.Controller.LTouch : OVRInput.Controller.RTouch));
    }

    public bool GunInteractLongPress(HandSide hand) {
        return OVRInput.Get(GunInteractLongBtn, (hand == HandSide.Left ? OVRInput.Controller.LTouch : OVRInput.Controller.RTouch));
    }

    public bool GunInteract2(HandSide hand) {
        return OVRInput.Get(GunInteract2Btn, (hand == HandSide.Left ? OVRInput.Controller.LTouch : OVRInput.Controller.RTouch));
    }

    public bool GunInteract3(HandSide hand) {
        return OVRInput.Get(GunInteract3Btn, (hand == HandSide.Left ? OVRInput.Controller.LTouch : OVRInput.Controller.RTouch));
    }

    public bool ActionPressDown(HandSide hand) {
        return OVRInput.GetDown(ActionButton, (hand == HandSide.Left ? OVRInput.Controller.LTouch : OVRInput.Controller.RTouch));
    }

    public bool GunInteractDown(HandSide hand) {
        return OVRInput.GetDown(GunInteract1Btn, (hand == HandSide.Left ? OVRInput.Controller.LTouch : OVRInput.Controller.RTouch));
    }

    public bool GunInteractLongPressDown(HandSide hand) {
        return OVRInput.GetDown(GunInteractLongBtn, (hand == HandSide.Left ? OVRInput.Controller.LTouch : OVRInput.Controller.RTouch));
    }

    public bool GunInteract2Down(HandSide hand) {
        return OVRInput.GetDown(GunInteract2Btn, (hand == HandSide.Left ? OVRInput.Controller.LTouch : OVRInput.Controller.RTouch));
    }

    public bool GunInteract3Down(HandSide hand) {
        return OVRInput.GetDown(GunInteract3Btn, (hand == HandSide.Left ? OVRInput.Controller.LTouch : OVRInput.Controller.RTouch));
    }

    public bool ActionPressUp(HandSide hand) {
        return OVRInput.GetUp(ActionButton, (hand == HandSide.Left ? OVRInput.Controller.LTouch : OVRInput.Controller.RTouch));
    }

    public bool GunInteractUp(HandSide hand) {
        return OVRInput.GetUp(GunInteract1Btn, (hand == HandSide.Left ? OVRInput.Controller.LTouch : OVRInput.Controller.RTouch));
    }

    public bool GunInteractLongPressUp(HandSide hand) {
        return OVRInput.GetUp(GunInteractLongBtn, (hand == HandSide.Left ? OVRInput.Controller.LTouch : OVRInput.Controller.RTouch));
    }

    public bool GunInteract2Up(HandSide hand) {
        return OVRInput.GetUp(GunInteract2Btn, (hand == HandSide.Left ? OVRInput.Controller.LTouch : OVRInput.Controller.RTouch));
    }

    public bool GunInteract3Up(HandSide hand) {
        return OVRInput.GetUp(GunInteract3Btn, (hand == HandSide.Left ? OVRInput.Controller.LTouch : OVRInput.Controller.RTouch));
    }

    void Update()
    {
        if(TallestHead < Head.transform.localPosition.y) {
            TallestHead = Head.transform.localPosition.y+0.1f;
        }
        InventoryPos.transform.localPosition = Head.transform.localPosition - (Vector3.up * TallestHead / 3f);
        InventoryPos.transform.rotation = transform.rotation;

        if (OVRInput.GetDown(ChangeHandedness)) {
            if(VRInputBridge.instance.aimScript_ref.primaryHand == HandSide.Right) {
                VRInputBridge.instance.aimScript_ref.primaryHand = HandSide.Left;
                VRInputBridge.instance.aimScript_ref.secondaryHand = HandSide.Right;
                LHandSphere.SetActive(false);
                RHandSphere.SetActive(true);
            }
            else {
                VRInputBridge.instance.aimScript_ref.primaryHand = HandSide.Right;
                VRInputBridge.instance.aimScript_ref.secondaryHand = HandSide.Left;
                LHandSphere.SetActive(true);
                RHandSphere.SetActive(false);
            }
        }
    }

    private void FixedUpdate() {
        LeftHand.transform.localPosition = OVRInput.GetLocalControllerPosition(OVRInput.Controller.LTouch);
        RightHand.transform.localPosition = OVRInput.GetLocalControllerPosition(OVRInput.Controller.RTouch);

        LeftHand.transform.localRotation = OVRInput.GetLocalControllerRotation(OVRInput.Controller.LTouch);
        RightHand.transform.localRotation = OVRInput.GetLocalControllerRotation(OVRInput.Controller.RTouch);
    }
}
