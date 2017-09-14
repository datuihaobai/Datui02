using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

/* modify 2017-03-18 */
public class DecalConfigAsset : ConfigAssetBase
{
	[System.Serializable]
	public class DecalConfig : ConfigAssetBase.ConfigBase
	{
		
		public int decalType;
		public int elementType;
		public List<int> elementValue;
		public List<int> tileSetIndex;
		public int rotateType;
		public int maxRate;
		public int atten;

		public override void parse(ArrayList cells)
		{
			id = Int32.Parse(cells[0] as string);
			decalType = Int32.Parse(GameUtility.toNumber(cells[1] as string));
			elementType = Int32.Parse(GameUtility.toNumber(cells[2] as string));
			string[] elementValue_strArgs = (cells[3] as string).Split (',');
			int[] elementValue_args = new int[elementValue_strArgs.Length];
			for (int i =0; i< elementValue_strArgs.Length;i++){
				elementValue_args[i] = Int32.Parse(GameUtility.toNumber(elementValue_strArgs[i]));
			}
				elementValue = new List<int> (elementValue_args);
			string[] tileSetIndex_strArgs = (cells[4] as string).Split (',');
			int[] tileSetIndex_args = new int[tileSetIndex_strArgs.Length];
			for (int i =0; i< tileSetIndex_strArgs.Length;i++){
				tileSetIndex_args[i] = Int32.Parse(GameUtility.toNumber(tileSetIndex_strArgs[i]));
			}
				tileSetIndex = new List<int> (tileSetIndex_args);
			rotateType = Int32.Parse(GameUtility.toNumber(cells[5] as string));
			maxRate = Int32.Parse(GameUtility.toNumber(cells[6] as string));
			atten = Int32.Parse(GameUtility.toNumber(cells[7] as string));
		}
	}

	public List<DecalConfig> configs = new List<DecalConfig> ();

	public Dictionary<int,DecalConfig> configDictionary = new Dictionary<int,DecalConfig> ();

	public override string GetConfigName()
	{ return "DecalConfig";}

	public override ConfigBase GetNewConfig ()
	{
		return new DecalConfig ();

	}
	public override void add(ConfigBase config){
		configs.Add (config as DecalConfig);
	}

	public override void readList(){
		foreach(DecalConfig config in configs){
			if(configDictionary.ContainsKey(config.id))
				Debug.LogError("DecalConfig has contain " + config.id);
			configDictionary.Add(config.id, config);
		}
	}

	public DecalConfig GetById(int id)
	{
		if (this.configDictionary.ContainsKey(id)){
			return configDictionary[id];
		}
		return null;
	}
}