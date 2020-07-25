using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RadialInteractButton : MonoBehaviour
{
    public static RadialInteractButton instance;

    public GameObject[] ButtonsToShow;
    public List<GameObject> ButtonsToShowList = new List<GameObject>();

    public List<string> ButtonsToEnable = new List<string>();

    void Awake() {
        instance = this;
    }

    public void OnClicked() {
        for (int i = 0; i < ButtonsToShow.Length; i++) {
            if (ButtonsToEnable.Contains(ButtonsToShow[i].name) && !ButtonsToShowList.Contains(ButtonsToShow[i])) {
                ButtonsToShowList.Add(ButtonsToShow[i]);
            }
        }

        for (int i = 0; i < ButtonsToShowList.Count; i++) {
            ButtonsToShowList[i].SetActive(true);
            float angle = i * Mathf.PI * 2 / ButtonsToShow.Length;
            Vector3 pos = new Vector3(-Mathf.Cos(angle), Mathf.Sin(angle), 0f) * 275f;
            ButtonsToShowList[i].transform.localPosition = pos;
        }

    }

    public void OnReleased() {
        for (int i = 0; i < ButtonsToShowList.Count; i++) {
            ButtonsToShowList[i].SetActive(false);
        }
    }

    void Update() {
        
    }
}
