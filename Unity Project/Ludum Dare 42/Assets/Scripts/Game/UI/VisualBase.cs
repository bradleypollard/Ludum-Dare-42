using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VisualBase : MonoBehaviour
{
    [HideInInspector]
	public ObjectOrientation objectOrientation;

	protected Vector2 spawnLocation;
	public Vector2 GetSpawnLocation() { return spawnLocation; }

    [HideInInspector]
    public float localScale = 1.0f;

    public void Start()
	{
		spawnLocation = GetComponent<RectTransform>().anchoredPosition;
        GetComponent<RectTransform>().localScale = Vector2.one * localScale;
    }

	public void Rotate( bool _clockwise )
	{
		// Update orientation
		int or = (int)objectOrientation;
		or = _clockwise ? or + 1 : or - 1;
		or = or % (int)ObjectOrientation.SIZE;
		objectOrientation = (ObjectOrientation)or;

		// Do visual rotation
		RectTransform trans = GetComponent<RectTransform>();
		if ( _clockwise)
		{
			trans.Rotate( new Vector3( 0, 0, -90 ) );
		}
		else
		{
			trans.Rotate( new Vector3( 0, 0, 90 ) );
		}
	}

    public void ResetBase()
    {
        objectOrientation = ObjectOrientation.Or0;

        RectTransform trans = GetComponent<RectTransform>();
        trans.rotation = Quaternion.identity;

        localScale = 1.0f;
        GetComponent<RectTransform>().localScale = Vector2.one * localScale;

        for(int i = 1; i <= 2; i++)
        {
            if(transform.Find("InConnector_" + i) != null)
            {
                transform.Find("InConnector_" + i).gameObject.SetActive(true);
            }

            if (transform.Find("OutConnector_" + i) != null)
            {
                transform.Find("OutConnector_" + i).gameObject.SetActive(true);
            }
        }
    }

    public void Update()
    {
        if (GetComponent<RectTransform>().localScale.x != localScale)
        {
            float newLocalScale = Mathf.Lerp(GetComponent<RectTransform>().localScale.x, localScale, Time.deltaTime * 5.0f);
            GetComponent<RectTransform>().localScale = Vector2.one * newLocalScale;
        }
    }
}
