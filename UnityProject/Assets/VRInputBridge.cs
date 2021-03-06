﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VRInputBridge : MonoBehaviour
{
    public static VRInputBridge instance;

    public AimScript aimScript_ref;

    Renderer SlideObject, FiremodeObject, ThumbsafetyObject, SlidelockObject, HammerObject, CylinderObject, ExtractorObject;
    Bounds SlideBounds;

    bool RotatingCylinder, MeleeWeapon;

    Vector3 closeDirection;

    void Awake()
    {
        instance = this;
    }

    private IEnumerator Start() {
        yield return null;
        yield return null;
        yield return null;
        yield return null;
        if (aimScript_ref.gun_script.HasGunComponent(GunAspect.YOKE_VISUAL)) {
            closeDirection = WhichOffsetDirectionForFlipClose(aimScript_ref.gun_script.GetComponent<YokeVisualComponent>().yoke_pivot_rel_rot, aimScript_ref.gun_script.GetComponent<YokeVisualComponent>().yoke_pivot, aimScript_ref.gun_script.GetComponent<YokeVisualComponent>().point_yoke_pivot_open);
        }
        yield return new WaitForSeconds(0.5f);

        if (aimScript_ref.gun_script.HasGunComponent(GunAspect.SLIDE)){
            SlideObject = aimScript_ref.gun_script.GetComponent<SlideVisualComponent>().slide.GetComponent<Renderer>();
            if(SlideObject == null) {
                SlideObject = aimScript_ref.gun_script.GetComponent<SlideVisualComponent>().slide.GetComponentInChildren<Renderer>();
            }
        }

        if (aimScript_ref.gun_script.HasGunComponent(GunAspect.FIRE_MODE_VISUAL)) {
            FiremodeObject = aimScript_ref.gun_script.GetComponent<FireModeVisualComponent>().fire_mode_toggle.GetComponent<Renderer>();
            if (FiremodeObject == null) {
                FiremodeObject = aimScript_ref.gun_script.GetComponent<FireModeVisualComponent>().fire_mode_toggle.GetComponentInChildren<Renderer>();
            }
            if (FiremodeObject == null) {
                Debug.Log("FiremodeObject Component Doesn't exist!");
            }
        }

        if (aimScript_ref.gun_script.HasGunComponent(GunAspect.THUMB_SAFETY_VISUAL)) {
            ThumbsafetyObject = aimScript_ref.gun_script.GetComponent<ThumbSafetyVisualComponent>().safety.GetComponent<Renderer>();
            if (ThumbsafetyObject == null) {
                ThumbsafetyObject = aimScript_ref.gun_script.GetComponent<ThumbSafetyVisualComponent>().safety.GetComponentInChildren<Renderer>();
            }
            if (ThumbsafetyObject == null) {
                Debug.Log("ThumbsafetyObject Component Doesn't exist!");
            }
        }

        if (aimScript_ref.gun_script.HasGunComponent(GunAspect.SLIDE_LOCK_VISUAL)) {
            SlidelockObject = aimScript_ref.gun_script.GetComponent<SlideLockVisualComponent>().slide_lock.GetComponent<Renderer>();
            if (SlidelockObject == null) {
                SlidelockObject = aimScript_ref.gun_script.GetComponent<SlideLockVisualComponent>().slide_lock.GetComponentInChildren<Renderer>();
            }
            if (SlidelockObject == null) {
                Debug.Log("SlidelockObject Component Doesn't exist!");
            }
        }

        if (aimScript_ref.gun_script.HasGunComponent(GunAspect.LOCKABLE_BOLT)) {
            //SlidelockObject = aimScript_ref.gun_script.GetComponent<LockableBoltComponent>().bolt.GetComponent<Renderer>();
            if (SlidelockObject == null) {
               // SlidelockObject = aimScript_ref.gun_script.GetComponent<LockableBoltComponent>().bolt.GetComponentInChildren<Renderer>();
            }
            if (SlidelockObject == null) {
                //Debug.Log("LOCKABLE_BOLT Component Doesn't exist!");
            }
        }

        if (aimScript_ref.gun_script.HasGunComponent(GunAspect.CYLINDER_VISUAL)) {
            CylinderObject = aimScript_ref.gun_script.GetComponent<CylinderVisualComponent>().cylinder_assembly.GetComponent<Renderer>();
            if (CylinderObject == null) {
                CylinderObject = aimScript_ref.gun_script.GetComponent<CylinderVisualComponent>().cylinder_assembly.GetComponentInChildren<Renderer>();
            }
            if (CylinderObject == null) {
                Debug.Log("CylinderObject Component Doesn't exist!");
            }
        }

        if (aimScript_ref.gun_script.HasGunComponent(GunAspect.REVOLVER_CYLINDER)) {
            ExtractorObject = aimScript_ref.gun_script.GetComponent<RevolverCylinderComponent>().chamber_parent.GetComponent<Renderer>();
            if (ExtractorObject == null) {
                ExtractorObject = aimScript_ref.gun_script.GetComponent<RevolverCylinderComponent>().chamber_parent.GetComponentInChildren<Renderer>();
            }
            if (ExtractorObject == null) {
                Debug.Log("ExtractorObject Component Doesn't exist!");
            }

            RotatingCylinder = aimScript_ref.gun_script.GetComponent<RevolverCylinderComponent>().rotateable;
        }

        if (aimScript_ref.gun_script.HasGunComponent(GunAspect.HAMMER_VISUAL)) {
            HammerObject = aimScript_ref.gun_script.GetComponent<HammerVisualComponent>().hammer.GetComponent<Renderer>();
            if (HammerObject == null) {
                HammerObject = aimScript_ref.gun_script.GetComponent<HammerVisualComponent>().hammer.GetComponentInChildren<Renderer>();
            }
            if (HammerObject == null) {
                Debug.Log("HammerObject Component Doesn't exist!");
            }
        }

        if (aimScript_ref.gun_script.HasGunComponent(GunAspect.SLIDE_LOCK) && aimScript_ref.gun_script.HasGunComponent(GunAspect.THUMB_COCKING)) {
            hc = aimScript_ref.gun_script.GetComponent<HammerComponent>();
        }

        Camera.main.nearClipPlane = 0.01f;

        MeleeWeapon = (aimScript_ref.gun_script.GetComponentInChildren<MeleeWeaponInfo>() != null);
    }

    public Vector3 WhichOffsetDirectionForFlipClose(Quaternion YolkBaseRot ,Transform YolkPivot, Transform YolkPivotOpen) {
        float distance = 0f;
        Vector3 returnVec = Vector3.zero;
        if(Vector3.Distance(YolkBaseRot*YolkPivot.up, YolkPivotOpen.up) > distance) {
            returnVec = YolkBaseRot * YolkPivot.up - YolkPivotOpen.up;
            distance = Vector3.Distance(YolkPivot.up, YolkPivotOpen.up);
        }
        if (Vector3.Distance(YolkBaseRot*YolkPivot.right, YolkPivotOpen.right) > distance) {
            returnVec = YolkBaseRot * YolkPivot.right - YolkPivotOpen.right;
            distance = Vector3.Distance(YolkPivot.right, YolkPivotOpen.right);
        }
        if (Vector3.Distance(YolkBaseRot*YolkPivot.forward, YolkPivotOpen.forward) > distance) {
            returnVec = YolkBaseRot * YolkPivot.forward - YolkPivotOpen.forward;
            distance = Vector3.Distance(YolkPivot.forward, YolkPivotOpen.forward);
        }

        return returnVec;
    }

    public bool MagOut;

    public bool CheckBoundsAndClicked(Renderer renderedComponent, HandSide side, bool shrink = false) {
        if (renderedComponent == null) {
            return false;
        }
        Bounds bound = renderedComponent.bounds;

        if (shrink) {
            bound.extents *= 0.5f;
        }

        if (side == HandSide.Left) {
            if (bound.Contains(VRInputController.instance.LHandSphere.transform.position)) {
                return VRInputController.instance.ActionPress(side);
            }
            else {
                return false;
            }
        }
        else {
            if (bound.Contains(VRInputController.instance.RHandSphere.transform.position)) {
                return VRInputController.instance.ActionPress(side);
            }
            else {
                return false;
            }
        }
    }

    public bool CheckBoundsAndClickedDown(Renderer renderedComponent, HandSide side) {
        if(renderedComponent == null) {
            return false;
        }
        Bounds bound = renderedComponent.bounds;

        if (side == HandSide.Left) {
            if (bound.Contains(VRInputController.instance.LHandSphere.transform.position)) {
                
                return VRInputController.instance.ActionPressDown(side);
            }
            else {
                return false;
            }
        }
        else {
            if (bound.Contains(VRInputController.instance.RHandSphere.transform.position)) {
                return VRInputController.instance.ActionPressDown(side);
            }
            else {
                return false;
            }
        }
    }

    public bool CheckBoundsAndClickedUp(Renderer renderedComponent, HandSide side) {
        if (renderedComponent == null) {
            return false;
        }
        Bounds bound = renderedComponent.bounds;

        if (side == HandSide.Left) {
            if (bound.Contains(VRInputController.instance.LHandSphere.transform.position)) {
                return VRInputController.instance.ActionPressUp(side);
            }
            else {
                return false;
            }
        }
        else {
            if (bound.Contains(VRInputController.instance.RHandSphere.transform.position)) {
                return VRInputController.instance.ActionPressUp(side);
            }
            else {
                return false;
            }
        }
    }

    public bool GetButtonDown(string input_str, HandSide hand) {
        switch (input_str) {
            case "Slide Lock":
                return VRInputController.instance.GunInteractDown(hand) || CheckBoundsAndClickedDown(SlidelockObject, aimScript_ref.secondaryHand);
            case "Safety":
                return VRInputController.instance.GunInteract2Down(hand) || CheckBoundsAndClickedDown(ThumbsafetyObject, aimScript_ref.secondaryHand);
            case "Auto Mod Toggle":
                return VRInputController.instance.GunInteract3Down(hand) || CheckBoundsAndClickedDown(FiremodeObject, aimScript_ref.secondaryHand);
            case "Pull Back Slide":
                if(SlideObject == null) {
                    return false;
                }
                return CheckSlidePullDown(hand);

            case "Swing Out Cylinder":
                return VRInputController.instance.GunInteractDown(hand) || CheckBoundsAndClickedDown(CylinderObject, aimScript_ref.secondaryHand);

            case "Close Cylinder":
                bool GunClose = Vector3.Angle(closeDirection,VRInputController.instance.GetControllerVel(hand)) <= 70 && VRInputController.instance.GetControllerVel(hand).magnitude > 1.5f;//aimScript_ref.gun_instance.transform.right.y < -0.75f;

                
                //if (!RotatingCylinder) {
                 //   GunClose = VRInputController.instance.GunInteractUp(hand);
                //}

                return GunClose || CheckBoundsAndClickedUp(CylinderObject, aimScript_ref.secondaryHand);

            case "Insert":

                if (aimScript_ref != null) {
                    
                    if (aimScript_ref.primaryHand == HandSide.Right) {
                        if (hand == HandSide.Left) {
                            if (aimScript_ref.gun_instance != null) {//If gun_instance is null your gun is in its holster, which swaps which hand inserts bullets.
                                if (!aimScript_ref.gun_script.HasGunComponent(GunAspect.EXTERNAL_MAGAZINE) && !MeleeWeapon) {
                                    Vector3 bulletInsertPos = aimScript_ref.gun_script.HasGunComponent(GunAspect.CYLINDER_VISUAL)?aimScript_ref.gun_script.GetComponent<CylinderVisualComponent>().cylinder_assembly.position: aimScript_ref.gun_script.GetComponent<InternalMagazineComponent>().magazine_script.transform.position;
                                    return (Vector3.Distance(VRInputController.instance.LHandSphere.transform.position, bulletInsertPos) < 0.2f) && VRInputController.instance.GunInteractDown(HandSide.Left);//Magazine bullet insert
                                }
                                else {
                                    return VRInputController.instance.GunInteractDown(HandSide.Left);//Magazine bullet insert
                                }
                            }
                            else {
                                return (Vector3.Distance(VRInputController.instance.LHandSphere.transform.position, VRInputController.instance.RHandSphere.transform.position) < 0.2f) && VRInputController.instance.GunInteractDown(HandSide.Right);//Magazine bullet insert
                            }
                        }
                        else if (aimScript_ref.gun_script.HasGunComponent(GunAspect.EXTERNAL_MAGAZINE)){//Magazine into gun insert, have to hold the mag under the gun.
                            Vector3 magInsertPos = aimScript_ref.gun_script.GetComponent<ExternalMagazineComponent>().point_mag_to_insert.position;
                            if (MagOut && Vector3.Distance(VRInputController.instance.LHandSphere.transform.position, magInsertPos) < 0.1f && VRInputController.instance.GetControllerVel(HandSide.Left).magnitude > 0.5f && Vector3.Dot(VRInputController.instance.GetControllerVel(HandSide.Left),aimScript_ref.gun_script.transform.up) < 0.3f ) {
                                return true;
                            }
                            else {
                                return false;
                            }
                        }
                    }
                    else {
                        if (hand == HandSide.Right) {
                            if (aimScript_ref.gun_instance != null) {//If gun_instance is null your gun is in its holster, which swaps which hand inserts bullets.
                                if (!aimScript_ref.gun_script.HasGunComponent(GunAspect.EXTERNAL_MAGAZINE)) {
                                    Vector3 bulletInsertPos = aimScript_ref.gun_script.HasGunComponent(GunAspect.CYLINDER_VISUAL) ? aimScript_ref.gun_script.GetComponent<CylinderVisualComponent>().cylinder_assembly.position : aimScript_ref.gun_script.GetComponent<InternalMagazineComponent>().magazine_script.transform.position;
                                    return (Vector3.Distance(bulletInsertPos, VRInputController.instance.RHandSphere.transform.position) < 0.2f) && VRInputController.instance.GunInteractDown(HandSide.Right);//Lefthanded version
                                }
                                else {
                                    return VRInputController.instance.GunInteractDown(HandSide.Right);//Lefthanded version
                                }
                            }
                            else {
                                return (Vector3.Distance(VRInputController.instance.LHandSphere.transform.position, VRInputController.instance.RHandSphere.transform.position) < 0.2f) && VRInputController.instance.GunInteractDown(HandSide.Left);//Lefthanded version
                            }
                        }
                        else if (aimScript_ref.gun_script.HasGunComponent(GunAspect.EXTERNAL_MAGAZINE)) {
                            Vector3 magInsertPos = aimScript_ref.gun_script.GetComponent<ExternalMagazineComponent>().point_mag_to_insert.position;
                            if (MagOut && Vector3.Distance(magInsertPos, VRInputController.instance.RHandSphere.transform.position) < 0.05f && VRInputController.instance.GetControllerVel(HandSide.Right).magnitude > 0.5f && Vector3.Dot(VRInputController.instance.GetControllerVel(HandSide.Right), aimScript_ref.gun_script.transform.up) < 0.3f) {
                                return true;
                            }
                            else {
                                return false;
                            }
                        }
                    }
                }
                return false;

            case "Eject/Drop":
                return VRInputController.instance.GunInteractLongPressDown(hand);
            case "Flashlight Toggle":
                return VRInputController.instance.GunInteract2Down(hand);
            case "Inventory 1":
                if (VRInventoryManager.instance.ActiveSlot == 0) {
                    return VRInputController.instance.ActionPressDown(hand);
                }
                else {
                    return false;
                }
            case "Inventory 2":
                if (VRInventoryManager.instance.ActiveSlot == 1) {
                    return VRInputController.instance.ActionPressDown(hand);
                }
                else {
                    return false;
                }
            case "Inventory 3":
                if (VRInventoryManager.instance.ActiveSlot == 2) {
                    return VRInputController.instance.ActionPressDown(hand);
                }
                else {
                    return false;
                }
            case "Inventory 4":
                if (VRInventoryManager.instance.ActiveSlot == 3) {
                    return VRInputController.instance.ActionPressDown(hand);
                }
                else {
                    return false;
                }
            case "Inventory 5":
                if (VRInventoryManager.instance.ActiveSlot == 4) {
                    return VRInputController.instance.ActionPressDown(hand);
                }
                else {
                    return false;
                }
            case "Inventory 6":
                if (VRInventoryManager.instance.ActiveSlot == 5) {
                    return VRInputController.instance.ActionPressDown(hand);
                }
                else {
                    return false;
                }
            case "Inventory 7":
                if (VRInventoryManager.instance.ActiveSlot == 6) {
                    return VRInputController.instance.ActionPressDown(hand);
                }
                else {
                    return false;
                }
            case "Inventory 8":
                if (VRInventoryManager.instance.ActiveSlot == 7) {
                    return VRInputController.instance.ActionPressDown(hand);
                }
                else {
                    return false;
                }
            case "Inventory 9":
                if (VRInventoryManager.instance.ActiveSlot == 8) {
                    return VRInputController.instance.ActionPressDown(hand);
                }
                else {
                    return false;
                }
            case "Inventory 10":
                if (VRInventoryManager.instance.ActiveSlot == 9) {
                    return VRInputController.instance.ActionPressDown(hand);
                }
                else {
                    return false;
                }
            case "Tape Player":
                if (VRInventoryManager.instance.TapePlayer) {
                    return VRInputController.instance.ActionPressDown(hand);
                }
                else {
                    return false;
                }
            case "Holster":
                if (VRInventoryManager.instance.Holster) {
                    return VRInputController.instance.CollectPressDown(hand);
                }
                else {
                    return false;
                }
            case "Hammer":
                return VRInputController.instance.GunInteract3Down(hand) || CheckBoundsAndClickedDown(HammerObject, aimScript_ref.secondaryHand);
            default:
                return Input.GetButtonDown(input_str);
        }
    }

    public bool CheckSlidePullDown(HandSide hand) {
        Vector3 SlidePullPos = aimScript_ref.gun_script.GetComponent<SlideVisualComponent>().point_slide_end.position;

        Vector3 HandPos = VRInputController.instance.LHandSphere.transform.position;
        SlideBounds = SlideObject.bounds;
        //SlideBounds.extents *= 0.75f;

        if (hand == HandSide.Right) {
            HandPos = VRInputController.instance.RHandSphere.transform.position;
        }

        float frontPosDistance = Vector3.Distance(HandPos, aimScript_ref.gun_script.GetComponent<SlideVisualComponent>().point_slide_start.position);

        if (SlideBounds.Contains(HandPos) && Vector3.Distance(HandPos, SlidePullPos) < frontPosDistance) {// 
            return VRInputController.instance.ActionPressDown(hand);
        }
        else {
            return false;
        }
    }

    public bool CheckSlidePull(HandSide hand) {
        Vector3 SlidePullPos = aimScript_ref.gun_script.GetComponent<SlideVisualComponent>().point_slide_end.position;

        Vector3 HandPos = VRInputController.instance.LHandSphere.transform.position;
        SlideBounds = SlideObject.bounds;
        //SlideBounds.extents *= 0.75f;

        if (hand == HandSide.Right) {
            HandPos = VRInputController.instance.RHandSphere.transform.position;
        }

        float frontPosDistance = Vector3.Distance(HandPos, aimScript_ref.gun_script.GetComponent<SlideVisualComponent>().point_slide_start.position);

        if (SlideBounds.Contains(HandPos) && Vector3.Distance(HandPos, SlidePullPos) < frontPosDistance) {// && Vector3.Distance(HandPos, SlidePullPos) < endposDistance/2f)
            return VRInputController.instance.ActionPress(hand);
        }
        else {
            return false;
        }
    }

    public bool CheckSlidePressCheck(HandSide hand) {
        Vector3 SlidePressCheckPos = aimScript_ref.gun_script.GetComponent<SlideVisualComponent>().point_slide_start.position;

        Vector3 HandPos = VRInputController.instance.LHandSphere.transform.position;
        SlideBounds = SlideObject.bounds;
        //SlideBounds.extents *= 0.75f;

        if (hand == HandSide.Right) {
            HandPos = VRInputController.instance.RHandSphere.transform.position;
        }

        float presscheckDistance = Vector3.Distance(HandPos, aimScript_ref.gun_script.GetComponent<SlideVisualComponent>().point_slide_end.position);

        if ((SlideBounds.Contains(HandPos) && Vector3.Distance(HandPos, SlidePressCheckPos) < presscheckDistance)) {
            return VRInputController.instance.ActionPress(hand);
        }
        else {
            return false;
        }
    }

    public bool GetButton(string input_str, HandSide hand) {
        switch (input_str) {
            case "Trigger":
                return VRInputController.instance.ActionPress(hand);
            case "Slide Lock":
                return VRInputController.instance.GunInteract(hand) || CheckBoundsAndClicked(SlidelockObject, aimScript_ref.secondaryHand);
            case "Extractor Rod":
                return (VRInputController.instance.GunInteract2(hand) || CheckBoundsAndClicked(ExtractorObject, aimScript_ref.secondaryHand, true)) && aimScript_ref.IsAiming();
            case "Hammer":
                return VRInputController.instance.GunInteract3(hand) || CheckBoundsAndClicked(HammerObject, aimScript_ref.secondaryHand);
            case "Get":
                return VRInputController.instance.CollectPress(hand);
            case "Pull Back Slide":
                if (SlideObject == null) {
                    return false;
                }
                return CheckSlidePull(hand);
            case "Pull Back Slide Press Check":
                if (SlideObject == null) {
                    return false;
                }
                return CheckSlidePressCheck(hand);
            default:
                return Input.GetButton(input_str);
        }
        
    }

    HammerComponent hc;

    private void Update() {
        MagOut = (aimScript_ref.magazine_instance_in_hand != null);

        if (hc != null && !GetButton("Hammer", aimScript_ref.primaryHand)) {
            hc.thumb_on_hammer = Thumb.OFF_HAMMER;
        }

        //Debug.DrawLine(VRInputController.instance.RightHand.transform.position, VRInputController.instance.RightHand.transform.position + VRInputController.instance.RightHand.transform.rotation*closeDirection, Color.red);
    }

    public bool GetButtonUp(string input_str, HandSide hand) {
        switch (input_str) {
            case "Slide Lock":
                return VRInputController.instance.GunInteractUp(hand) || CheckBoundsAndClickedUp(SlidelockObject, aimScript_ref.secondaryHand);
            case "Pull Back Slide":
                return VRInputController.instance.ActionPressUp(hand);
            case "Hammer":
                return VRInputController.instance.GunInteract3Up(hand) || CheckBoundsAndClickedUp(HammerObject, aimScript_ref.secondaryHand);
            default:
                return Input.GetButtonUp(input_str);
        }
        
    }
    public float GetAxis(string input_str, HandSide hand) {
        switch (input_str) {
            case "Mouse ScrollWheel":
                return VRInputController.instance.GetSpinSpeed(hand) * 10f;
            default:
                return Input.GetAxis(input_str);
        }
        
    }
}
