using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class ButtonBehavior : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] TextMeshProUGUI selector;
    void Start()
    {
        selector=GetComponent<TextMeshProUGUI>();
        selector.enabled=false;
        Debug.Log(selector.text);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void MouseOver() {
        selector.enabled=true;
        Debug.Log(selector.enabled.ToString());
    }
    public void MouseExit() {
        selector.enabled=false;
    }
}
