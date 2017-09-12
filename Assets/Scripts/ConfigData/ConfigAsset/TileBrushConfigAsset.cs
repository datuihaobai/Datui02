using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

/* modify 2017-03-18 */
public class TileBrushConfigAsset : ConfigAssetBase
{
	[System.Serializable]
	public class TileBrushConfig : ConfigAssetBase.ConfigBase
	{
		
		public int elementType;
		public int level;
		public int brush;

		public override void parse(ArrayList cells)
		{
			id = Int32.Parse(cells[0] as string);
			elementType = Int32.Parse(GameUtility.toNumber(cells[1] as string));
			level = Int32.Parse(GameUtility.toNumber(cells[2] as string));
			brush = Int32.Parse(GameUtility.toNumber(cells[3] as string));
		}
	}

	public List<TileBrushConfig> configs = new List<TileBrushConfig> ();

	public Dictionary<int,TileBrushConfig> configDictionary = new Dictionary<int,TileBrushConfig> ();

	public override string GetConfigName()
	{ return "TileBrushConfig";}

	public override ConfigBase GetNewConfig ()
	{
		return new TileBrushConfig ();

	}
	public override void add(ConfigBase config){
		configs.Add (config as TileBrushConfig);
	}

	public override void readList(){
		foreach(TileBrushConfig config in configs){
			if(configDictionary.ContainsKey(config.id))
				Debug.LogError("TileBrushConfig has contain " + config.id);
			configDictionary.Add(config.id, config);
		}
	}

	public TileBrushConfig GetById(int id)
	{
		if (this.configDictionary.ContainsKey(id)){
			return configDictionary[id];
		}
		return null;
	}
}