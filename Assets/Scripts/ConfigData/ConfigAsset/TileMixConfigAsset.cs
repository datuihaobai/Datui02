using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

/* modify 2017-03-18 */
public class TileMixConfigAsset : ConfigAssetBase
{
	[System.Serializable]
	public class TileMixConfig : ConfigAssetBase.ConfigBase
	{
		
		public int qtrTile0;
		public int qtrTile1;
		public int qtrTile2;
		public int qtrTile3;
		public int tilesetIndex;
		public int rotateType;

		public override void parse(ArrayList cells)
		{
			id = Int32.Parse(cells[0] as string);
			qtrTile0 = Int32.Parse(GameUtility.toNumber(cells[1] as string));
			qtrTile1 = Int32.Parse(GameUtility.toNumber(cells[2] as string));
			qtrTile2 = Int32.Parse(GameUtility.toNumber(cells[3] as string));
			qtrTile3 = Int32.Parse(GameUtility.toNumber(cells[4] as string));
			tilesetIndex = Int32.Parse(GameUtility.toNumber(cells[5] as string));
			rotateType = Int32.Parse(GameUtility.toNumber(cells[6] as string));
		}
	}

	public List<TileMixConfig> configs = new List<TileMixConfig> ();

	public Dictionary<int,TileMixConfig> configDictionary = new Dictionary<int,TileMixConfig> ();

	public override string GetConfigName()
	{ return "TileMixConfig";}

	public override ConfigBase GetNewConfig ()
	{
		return new TileMixConfig ();

	}
	public override void add(ConfigBase config){
		configs.Add (config as TileMixConfig);
	}

	public override void readList(){
		foreach(TileMixConfig config in configs){
			if(configDictionary.ContainsKey(config.id))
				Debug.LogError("TileMixConfig has contain " + config.id);
			configDictionary.Add(config.id, config);
		}
	}

	public TileMixConfig GetById(int id)
	{
		if (this.configDictionary.ContainsKey(id)){
			return configDictionary[id];
		}
		return null;
	}
}