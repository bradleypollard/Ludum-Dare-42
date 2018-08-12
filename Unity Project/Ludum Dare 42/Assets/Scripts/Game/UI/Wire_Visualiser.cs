using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wire_Visualiser : MonoBehaviour
{
    private const float pulseWait = 2.0f;
    private float pulseTimer;

    public GameObject pulsePrefab;
    public float pulseSpeed = 5.0f;

    private List<GameObject> pulses;
    private RectTransform m_rectTransform;


    private void Start()
    {
        pulses = new List<GameObject>();
        pulseTimer = 0.001f;
        m_rectTransform = GetComponent<RectTransform>();
    }

    // Update is called once per frame
    void Update ()
    {
        //Slide Pulses along Wire
		foreach(GameObject pulse in pulses.ToArray())
        {
            RectTransform rectTransform = pulse.GetComponent<RectTransform>();
            rectTransform.anchoredPosition += new Vector2(pulseSpeed * Time.deltaTime, 0.0f);

            bool grow = rectTransform.anchoredPosition.x < m_rectTransform.sizeDelta.x - 50;

            rectTransform.sizeDelta = Vector2.Lerp(rectTransform.sizeDelta, Vector2.one * (grow ? 30.0f : 9.5f), Time.deltaTime * 5.0f);

            if (rectTransform.anchoredPosition.x > m_rectTransform.sizeDelta.x)
            {
                pulses.Remove(pulse);
                Destroy(pulse);
            }
        }

        //Generate New Pulses
        if(pulses.Count > 0 && pulseTimer <= 0.0f)
        {
            pulseTimer = pulseWait;
        }

        if(pulseTimer > 0.0)
        {
            pulseTimer -= Time.deltaTime;

            if(pulseTimer <= 0.0f)
            {
                //Spawn Pulses
                pulses.Add(Instantiate(pulsePrefab, pulsePrefab.transform.position, pulsePrefab.transform.rotation, pulsePrefab.transform.parent));
            }
        }
	}
}
