using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RadialInteractButton : MonoBehaviour
{
    public static RadialInteractButton instance;

    public GameObject[] ButtonsToShow;
    public List<GameObject> ButtonsToShowList = new List<GameObject>();

    public List<string> ButtonsToEnable = new List<string>();

    public float anglemult = 1f;

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
            float angle = ((i - 2) * Mathf.PI / (ButtonsToShow.Length)) * anglemult;
            Vector3 pos = new Vector3(-Mathf.Cos(angle), Mathf.Sin(angle), 0f) * 100f;
            ButtonsToShowList[i].transform.localScale = Vector3.zero;
            ButtonsToShowList[i].transform.localPosition = pos;
            ButtonsToShowList[i].SetActive(true);
        }
        StopAllCoroutines();
        StartCoroutine(MenuActivate(true));

        
    }

    IEnumerator MenuActivate(bool value) {
        int N = 0;
        while (N < 120) {
            for (int i = 0; i < ButtonsToShowList.Count; i++) {
                ButtonsToShowList[i].SetActive(true);
                float angle = ((i - 2) * Mathf.PI / (ButtonsToShow.Length)) * anglemult;
                Vector3 pos = new Vector3(-Mathf.Cos(angle), Mathf.Sin(angle), 0f) * 200f;
                ButtonsToShowList[i].transform.localPosition = Vector3.Lerp(ButtonsToShowList[i].transform.localPosition, value ? pos : pos/2f, 0.1f);
                ButtonsToShowList[i].transform.localScale = Vector3.Lerp(ButtonsToShowList[i].transform.localScale, value ? Vector3.one : Vector3.zero,0.1f);
                if (!value && N > 30) {
                    ButtonsToShowList[i].SetActive(false);
                }
            }
            yield return null;
            N++;
        }
    }

    public void OnReleased() {
        StopAllCoroutines();
        StartCoroutine(MenuActivate(false));
        /*for (int i = 0; i < ButtonsToShowList.Count; i++) {
            ButtonsToShowList[i].SetActive(false);
        }*/
    }

    void Update() {
        
    }
}
