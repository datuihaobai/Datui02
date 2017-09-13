using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

/* modify 2017-03-18 */
public class CrystalRangeConfigAsset : ConfigAssetBase
{
	[System.Serializable]
	public class CrystalRangeConfig : ConfigAssetBase.ConfigBase
	{
		
		public int level;
		public int centerValue;
		public int atten;

		public override void parse(ArrayList cells)
		{
			id = Int32.Parse(cells[0] as string);
			level = Int32.Parse(GameUtility.toNumber(cells[1] as string));
			centerValue = Int32.Parse(GameUtility.toNumber(cells[2] as string));
			atten = Int32.Parse(GameUtility.toNumber(cells[3] as string));
		}
	}

	public List<CrystalRangeConfig> configs = new List<CrystalRangeConfig> ();

	public Dictionary<int,CrystalRangeConfig> configDictionary = new Dictionary<int,CrystalRangeConfig> ();

	public override string GetConfigName()
	{ return "CrystalRangeConfig";}

	public override ConfigBase GetNewConfig ()
	{
		return new CrystalRangeConfig ();

	}
	public override void add(ConfigBase config){
		configs.Add (config as CrystalRangeConfig);
	}

	public override void readList(){
		foreach(CrystalRangeConfig config in configs){
			if(configDictionary.ContainsKey(config.id))
				Debug.LogError("CrystalRangeConfig has contain " + config.id);
			configDictionary.Add(config.id, config);
		}
	}

	public CrystalRangeConfig GetById(int id)
	{
		if (this.configDictionary.ContainsKey(id)){
			return configDictionary[id];
		}
		return null;
	}
}