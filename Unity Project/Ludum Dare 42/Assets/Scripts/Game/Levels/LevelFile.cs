using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Xml; //Needed for XML functionality
using System.Xml.Serialization; //Needed for XML Functionality
using System.IO;

public class LevelFile
{
	public LevelFile( List<InputCell> _inputs, List<OutputCell> outputs, int _numStartingOutputs )
	{
		Inputs = _inputs;
		Outputs = outputs;
		NumStartingOutputs = _numStartingOutputs;
	}

	public List<InputCell> Inputs;
	public List<OutputCell> Outputs;
	public int NumStartingOutputs;
}