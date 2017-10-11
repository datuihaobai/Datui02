using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

/* modify 2017-03-18 */
public class TerrainCommonConfigAsset : ConfigAssetBase
{
	[System.Serializable]
	public class TerrainCommonConfig : ConfigAssetBase.ConfigBase
	{
		
		public int key;
		public int value;

		public override void parse(ArrayList cells)
		{
			id = Int32.Parse(cells[0] as string);
			key = Int32.Parse(GameUtility.toNumber(cells[1] as string));
			value = Int32.Parse(GameUtility.toNumber(cells[2] as string));
		}
	}

	public List<TerrainCommonConfig> configs = new List<TerrainCommonConfig> ();

	public Dictionary<int,TerrainCommonConfig> configDictionary = new Dictionary<int,TerrainCommonConfig> ();

	public override string GetConfigName()
	{ return "TerrainCommonConfig";}

	public override ConfigBase GetNewConfig ()
	{
		return new TerrainCommonConfig ();

	}
	public override void add(ConfigBase config){
		configs.Add (config as TerrainCommonConfig);
	}

	public override void readList(){
		foreach(TerrainCommonConfig config in configs){
			if(configDictionary.ContainsKey(config.id))
				Debug.LogError("TerrainCommonConfig has contain " + config.id);
			configDictionary.Add(config.id, config);
		}
	}

	public TerrainCommonConfig GetById(int id)
	{
		if (this.configDictionary.ContainsKey(id)){
			return configDictionary[id];
		}
		return null;
	}
}