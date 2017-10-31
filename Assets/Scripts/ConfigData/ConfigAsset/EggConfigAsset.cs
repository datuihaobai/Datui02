using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

/* modify 2017-03-18 */
public class EggConfigAsset : ConfigAssetBase
{
	[System.Serializable]
	public class EggConfig : ConfigAssetBase.ConfigBase
	{
		
		public int elementType;
		public int time;
		public string prefab;

		public override void parse(ArrayList cells)
		{
			id = Int32.Parse(cells[0] as string);
			elementType = Int32.Parse(GameUtility.toNumber(cells[1] as string));
			time = Int32.Parse(GameUtility.toNumber(cells[2] as string));
			prefab = cells[3] as string;
		}
	}

	public List<EggConfig> configs = new List<EggConfig> ();

	public Dictionary<int,EggConfig> configDictionary = new Dictionary<int,EggConfig> ();

	public override string GetConfigName()
	{ return "EggConfig";}

	public override ConfigBase GetNewConfig ()
	{
		return new EggConfig ();

	}
	public override void add(ConfigBase config){
		configs.Add (config as EggConfig);
	}

	public override void readList(){
		foreach(EggConfig config in configs){
			if(configDictionary.ContainsKey(config.id))
				Debug.LogError("EggConfig has contain " + config.id);
			configDictionary.Add(config.id, config);
		}
	}

	public EggConfig GetById(int id)
	{
		if (this.configDictionary.ContainsKey(id)){
			return configDictionary[id];
		}
		return null;
	}
}