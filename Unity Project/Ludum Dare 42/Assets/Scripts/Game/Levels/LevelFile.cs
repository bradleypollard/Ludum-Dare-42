using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Xml; //Needed for XML functionality
using System.Xml.Serialization; //Needed for XML Functionality
using System.IO;

public class LevelFile
{
	public LevelFile( List<InputCell> _inputs, List<OutputCell> outputs, int _numStartingOutputs, int _dimensionX, int _dimensionY, Color _BGColour )
	{
		Inputs = _inputs;
		Outputs = outputs;
		NumStartingOutputs = _numStartingOutputs;
		DimensionX = _dimensionX;
		DimensionY = _dimensionY;
		BGColour = _BGColour;
	}

	public List<InputCell> Inputs;
	public List<OutputCell> Outputs;
	public int NumStartingOutputs;
	public int DimensionX, DimensionY;
	public Color BGColour;
}