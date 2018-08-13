using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Xml; //Needed for XML functionality
using System.Xml.Serialization; //Needed for XML Functionality
using System.IO;

public class LevelFile
{
	public LevelFile( string _name, List<InputCell> _inputs, List<OutputCell> _outputs, List<GateType> _buttons, List<int> _incDecValues, int _numStartingOutputs, int _dimensionX, int _dimensionY, Color _BGColour )
	{
		Name = _name;
		Inputs = _inputs;
		Outputs = _outputs;
		Buttons = _buttons;
		IncDecValues = _incDecValues;
		NumStartingOutputs = _numStartingOutputs;
		DimensionX = _dimensionX;
		DimensionY = _dimensionY;
		BGColour = _BGColour;
	}

	public string Name;
	public List<InputCell> Inputs;
	public List<OutputCell> Outputs;
	public List<GateType> Buttons;
	public List<int> IncDecValues;
	public int NumStartingOutputs;
	public int DimensionX, DimensionY;
	public Color BGColour;
}