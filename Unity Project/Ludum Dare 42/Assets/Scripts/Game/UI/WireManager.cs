using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WireManager : MonoBehaviour
{
	private bool m_inWireEditMode;

	// Use this for initialization
	void Start()
	{

	}

	// Update is called once per frame
	void Update()
	{
		if ( Input.GetMouseButtonUp( 1 ) )
		{
			EndMode();
		}
	}

	///////////////////////// API /////////////////////////
	public bool IsInWireEditMode()
	{
		return m_inWireEditMode;
	}

	public void StartMode()
	{
		m_inWireEditMode = true;
	}

	public void EndMode()
	{
		m_inWireEditMode = false;
	}

	///////////////////////// Helpers /////////////////////////

}
