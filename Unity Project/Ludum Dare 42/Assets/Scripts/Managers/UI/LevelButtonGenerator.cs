using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelButtonGenerator : MonoBehaviour
{
    public GameObject scrollView;
    public List<GameObject> iconPrefabs;
    private List<GameObject> levelObjects;

    // Update is called once per frame
    public void RegenerateButtons(List<GateType> _gates, List<int> _incDecValues)
    {
        //Delete old GameObjects
        if (levelObjects != null)
        {
            foreach (GameObject menuObject in levelObjects)
            {
                Destroy(menuObject);
            }
        }

        levelObjects = new List<GameObject>();

        float gateSize = 50;
        foreach (GateType gateType in _gates)
        {
            RectTransform rect = iconPrefabs[(int)gateType].GetComponent<RectTransform>();
            gateSize += rect.sizeDelta.y + 40;
        }

        if(_gates.Contains(GateType.IncrementDecrement))
        {
            gateSize += ((_incDecValues.Count - 1) * 120) + 40;
        }

        scrollView.GetComponent<RectTransform>().sizeDelta = new Vector2(scrollView.GetComponent<RectTransform>().sizeDelta.x, gateSize);

        //Make Buttons
        int buttonCount = 0;
        float gateHeight = gateSize - 100.0f;

        foreach (GateType gateType in _gates)
        {
			int numToCreate = 1;
			if ( gateType == GateType.IncrementDecrement )
			{
				numToCreate = _incDecValues.Count;
			}

			for ( int i = 0; i < numToCreate; ++i  )
			{
				GameObject gateButton = Instantiate( iconPrefabs[(int)gateType], scrollView.transform );
				levelObjects.Add( gateButton );

				RectTransform rect = gateButton.GetComponent<RectTransform>();
				rect.anchoredPosition = new Vector2( 94.0f, gateHeight );
				gateHeight -= rect.sizeDelta.y + 50.0f;

				if ( gateType == GateType.IncrementDecrement )
				{
					int value = _incDecValues[i];
					VisualGate incDec = gateButton.GetComponent<VisualGate>();
					incDec.value = value;

					gateButton.transform.Find( "Text" ).GetComponent<Text>().text = ( value <= 0 ? "" : "+" ) + value;
				}

				buttonCount++;
			}
			
        }
	}
}
