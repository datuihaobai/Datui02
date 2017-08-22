using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

/* modify 2017-03-18 */
public class CdConfigAsset : ConfigAssetBase
{
	[System.Serializable]
	public class CdConfig : ConfigAssetBase.ConfigBase
	{
		
		public int queueLableID;
		public int cdTime;
		public int isAccelerateJewel;
		public int clearGold;

		public override void parse(ArrayList cells)
		{
			id = Int32.Parse(cells[0] as string);
			cdTime = Int32.Parse(GameUtility.toNumber(cells[2] as string));
			isAccelerateJewel = Int32.Parse(GameUtility.toNumber(cells[3] as string));
			clearGold = Int32.Parse(GameUtility.toNumber(cells[4] as string));
		}
	}

	public List<CdConfig> configs = new List<CdConfig> ();

	public Dictionary<int,CdConfig> configDictionary = new Dictionary<int,CdConfig> ();

	public override string GetConfigName()
	{ return "CdConfig";}

	public override ConfigBase GetNewConfig ()
	{
		return new CdConfig ();

	}
	public override void add(ConfigBase config){
		configs.Add (config as CdConfig);
	}

	public override void readList(){
		foreach(CdConfig config in configs){
			if(configDictionary.ContainsKey(config.id))
				Debug.LogError("CdConfig has contain " + config.id);
			configDictionary.Add(config.id, config);
		}
	}

	public CdConfig GetById(int id)
	{
		if (this.configDictionary.ContainsKey(id)){
			return configDictionary[id];
		}
		return null;
	}
}